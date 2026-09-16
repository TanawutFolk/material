using Microsoft.Office.Core;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace RawMat.Utilities
{
    /// <summary>
    /// แปลงไฟล์ Excel เป็น PDF เพื่อใช้ preview ในโปรแกรม
    /// ผลลัพธ์ถูก cache ตามวันเวลาแก้ไขล่าสุดของไฟล์ต้นทาง ถ้าไฟล์ต้นทางไม่เปลี่ยนจะไม่เปิด Excel ซ้ำ
    /// </summary>
    public static class ExcelPreviewCls
    {
        // Excel interop เรียกพร้อมกันหลาย thread ไม่ได้ ต้องแปลงทีละไฟล์
        private static readonly object convertLock = new object();

        private static bool cacheCleaned;

        // ใช้หา process id ของ Excel ที่เราเปิดเอง เพื่อปิดทิ้งให้แน่ใจ
        // (วิธีเดียวกับ CommonClassLibrary.Controllers.ExcelController.GetExcelProcess)
        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(int hWnd, out int processId);

        /// <summary>
        /// แปลงไฟล์ Excel เป็น PDF แล้วคืน path ของ PDF ที่ได้ ถ้าเคยแปลงไว้แล้วจะคืนไฟล์ใน cache ทันที
        /// </summary>
        public static string ToPdf(string excelPath)
        {
            if (string.IsNullOrWhiteSpace(excelPath) || !File.Exists(excelPath))
            {
                throw new FileNotFoundException("ไม่พบไฟล์ Excel ที่ต้องการแปลง", excelPath ?? string.Empty);
            }

            string pdfPath = GetCachePath(excelPath);

            lock (convertLock)
            {
                CleanupCacheOnce();

                if (File.Exists(pdfPath))
                {
                    return pdfPath;
                }

                Convert(excelPath, pdfPath);
                RemoveOtherVersions(excelPath, pdfPath);
            }

            return pdfPath;
        }

        public static string GetCacheDirectory()
        {
            string configured = ConfigurationManager.AppSettings["ExcelPreviewCachePath"];

            string dir = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(Path.GetTempPath(), "RawMat", "ExcelPreview")
                : Environment.ExpandEnvironmentVariables(configured);

            Directory.CreateDirectory(dir);
            return dir;
        }

        private static void Convert(string excelPath, string pdfPath)
        {
            Excel.Application app;

            try
            {
                app = new Excel.Application();
            }
            catch (COMException ex)
            {
                throw new InvalidOperationException("เครื่องนี้ยังไม่ได้ติดตั้ง Microsoft Excel จึงเปิด preview ไฟล์ Excel ไม่ได้", ex);
            }

            Process excelProcess = FindExcelProcess(app);
            Excel.Workbooks books = null;
            Excel.Workbook workbook = null;

            try
            {
                app.Visible = false;
                app.DisplayAlerts = false;
                app.ScreenUpdating = false;
                app.EnableEvents = false;
                app.AskToUpdateLinks = false;

                // ไฟล์อยู่บน network share กัน Protected View บล็อกการเปิดผ่าน automation
                app.AutomationSecurity = MsoAutomationSecurity.msoAutomationSecurityForceDisable;

                books = app.Workbooks;

                // ReadOnly + Notify:false กัน dialog "File in use" เด้งค้างตอนคนอื่นเปิดไฟล์นี้อยู่
                workbook = books.Open(excelPath, UpdateLinks: 0, ReadOnly: true, Notify: false);

                FitColumnsToPageWidth(workbook);

                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                }

                workbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdfPath);
            }
            finally
            {
                if (workbook != null)
                {
                    try { workbook.Close(false); } catch (COMException) { }
                    ReleaseCom(workbook);
                }

                ReleaseCom(books);

                try { app.Quit(); } catch (COMException) { }
                ReleaseCom(app);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Excel มักไม่ยอมดับเองแม้ปล่อย COM ครบแล้ว ถ้ายังค้างต้องปิด process ที่เราเปิดเองทิ้ง
                // ไม่งั้น EXCEL.EXE จะสะสมทุกครั้งที่ preview
                KillIfStillAlive(excelProcess);
            }
        }

        /// <summary>
        /// บังคับให้ทุกคอลัมน์อยู่ในความกว้างหน้าเดียว
        /// </summary>
        /// <remarks>
        /// ถ้า PageSetup.Zoom ถูกตั้งเป็นตัวเลข Excel จะไม่สนใจ FitToPagesWide เลย
        /// ผลคือชีตที่กว้างเกินหน้ากระดาษจะถูกตัดคอลัมน์ท้าย ๆ ไปพิมพ์เป็นชุดหน้าแยกต่อท้าย
        /// เช่นเอกสารตรวจของ vendor ที่ Zoom=74 คอลัมน์สุดท้ายไปโผล่หน้า 18-34 แทนที่จะอยู่ในตาราง
        /// เอกสาร QA ห้ามมีคอลัมน์หายจากสายตา จึงปิด Zoom แล้วให้ fit ความกว้างแทน
        /// ส่วนความสูงปล่อยเป็นอัตโนมัติ ไม่งั้นข้อมูลนับพันแถวจะถูกยัดลงหน้าเดียวจนอ่านไม่ออก
        ///
        /// อย่าใส่ Application.PrintCommunication = false ครอบ วัดแล้วช้ากว่าเดิม (458ms เทียบกับ 11ms)
        /// เพราะตอนเปิดกลับเป็น true Excel จะ repaginate ทั้งชีตรวดเดียว
        /// </remarks>
        private static void FitColumnsToPageWidth(Excel.Workbook workbook)
        {
            Excel.Sheets sheets = null;

            try
            {
                sheets = workbook.Worksheets;

                foreach (Excel.Worksheet sheet in sheets)
                {
                    Excel.PageSetup pageSetup = null;

                    try
                    {
                        pageSetup = sheet.PageSetup;
                        pageSetup.Zoom = false;
                        pageSetup.FitToPagesWide = 1;
                        pageSetup.FitToPagesTall = false;
                    }
                    catch (COMException)
                    {
                        // ชีตที่ถูกป้องกันไว้หรือไม่มีเครื่องพิมพ์ ปล่อยให้ใช้ค่าเดิมของไฟล์
                    }
                    finally
                    {
                        ReleaseCom(pageSetup);
                        ReleaseCom(sheet);
                    }
                }
            }
            catch (COMException) { }
            finally
            {
                ReleaseCom(sheets);
            }
        }

        private static Process FindExcelProcess(Excel.Application app)
        {
            try
            {
                int processId;
                GetWindowThreadProcessId(app.Hwnd, out processId);
                return Process.GetProcessById(processId);
            }
            catch (COMException) { return null; }
            catch (ArgumentException) { return null; }
            catch (InvalidOperationException) { return null; }
        }

        private static void KillIfStillAlive(Process process)
        {
            if (process == null)
            {
                return;
            }

            try
            {
                // ให้โอกาสปิดตัวเองก่อน
                if (process.WaitForExit(1000))
                {
                    return;
                }

                process.Refresh();

                // ปิดเฉพาะ instance ที่ไม่มีหน้าต่าง คือตัวที่เราเปิดเอง จะได้ไม่ไปฆ่า Excel ที่ผู้ใช้เปิดค้างไว้
                if (!process.HasExited && process.MainWindowHandle == IntPtr.Zero)
                {
                    process.Kill();
                }
            }
            catch (InvalidOperationException) { }
            catch (Win32Exception) { }
            catch (NotSupportedException) { }
            finally
            {
                process.Dispose();
            }
        }

        private static string GetCachePath(string excelPath)
        {
            // ผูกชื่อ cache กับทั้ง path และเวลาแก้ไข ไฟล์ต้นทางเปลี่ยนเมื่อไหร่จึงแปลงใหม่
            string key = Hash(excelPath.ToLowerInvariant())
                + "_" + File.GetLastWriteTimeUtc(excelPath).Ticks;

            string name = MakeSafeFileName(Path.GetFileNameWithoutExtension(excelPath)) + "_" + key + ".pdf";

            return Path.Combine(GetCacheDirectory(), name);
        }

        private static void CleanupCacheOnce()
        {
            if (cacheCleaned)
            {
                return;
            }

            cacheCleaned = true;
            CleanupCache();
        }

        /// <summary>
        /// ล้าง cache ทิ้ง ตามอายุก่อน แล้วตามด้วยเพดานขนาด
        /// </summary>
        /// <remarks>
        /// public เพราะต้องเรียกตอนเปิดโปรแกรมด้วย ดู Program.Main
        /// ถ้าล้างเฉพาะตอนแปลงไฟล์ เครื่องที่นาน ๆ เปิดเอกสาร Excel ทีจะแทบไม่เคยล้างเลย
        /// (บน share มี PDF สองหมื่นกว่าไฟล์ แต่ Excel ทั้งปีมีไม่กี่ไฟล์)
        /// </remarks>
        public static void CleanupCache()
        {
            try
            {
                string dir = GetCacheDirectory();

                // ตามอายุ
                DateTime limit = DateTime.UtcNow.AddDays(-GetSetting("ExcelPreviewCacheDays", 7));

                foreach (FileInfo file in new DirectoryInfo(dir).GetFiles("*.pdf"))
                {
                    if (file.LastWriteTimeUtc < limit)
                    {
                        TryDelete(file);
                    }
                }

                // ตามเพดานขนาด กันกรณีเปิดเอกสารหนัก ๆ หลายใบรวดเดียวจนยังไม่ถึงกำหนดลบ
                FileInfo[] remaining = new DirectoryInfo(dir).GetFiles("*.pdf");

                long total = 0;
                foreach (FileInfo file in remaining)
                {
                    total += file.Length;
                }

                long cap = (long)GetSetting("ExcelPreviewCacheMaxMB", 200) * 1024L * 1024L;

                if (total <= cap)
                {
                    return;
                }

                // เก่าสุดไปก่อน
                Array.Sort(remaining, delegate (FileInfo a, FileInfo b)
                {
                    return a.LastWriteTimeUtc.CompareTo(b.LastWriteTimeUtc);
                });

                foreach (FileInfo file in remaining)
                {
                    if (total <= cap)
                    {
                        break;
                    }

                    long size = file.Length;

                    if (TryDelete(file))
                    {
                        total -= size;
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        /// <summary>
        /// ลบ cache เวอร์ชันเก่าของเอกสารเดียวกัน เหลือไว้เฉพาะไฟล์ที่เพิ่งแปลง
        /// </summary>
        /// <remarks>
        /// ชื่อ cache ผูกกับเวลาแก้ไขของไฟล์ต้นทาง vendor แก้ไฟล์ทีก็ได้ cache เพิ่มอีกไฟล์
        /// ของเก่าไม่มีวันถูกใช้แล้วแต่จะค้างจนกว่าจะครบอายุ จึงเก็บกวาดตั้งแต่ตอนนี้เลย
        /// </remarks>
        private static void RemoveOtherVersions(string excelPath, string keepPdfPath)
        {
            try
            {
                string prefix = MakeSafeFileName(Path.GetFileNameWithoutExtension(excelPath))
                    + "_" + Hash(excelPath.ToLowerInvariant()) + "_";

                foreach (FileInfo file in new DirectoryInfo(GetCacheDirectory()).GetFiles(prefix + "*.pdf"))
                {
                    if (!string.Equals(file.FullName, keepPdfPath, StringComparison.OrdinalIgnoreCase))
                    {
                        TryDelete(file);
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        private static bool TryDelete(FileInfo file)
        {
            // ไฟล์ที่กำลังเปิดค้างใน viewer จะถูก Adobe ล็อกไว้ ลบไม่ได้ก็ข้าม ไว้รอบหน้า
            try
            {
                file.Delete();
                return true;
            }
            catch (IOException) { return false; }
            catch (UnauthorizedAccessException) { return false; }
        }

        private static int GetSetting(string key, int fallback)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[key], out value) && value > 0
                ? value
                : fallback;
        }

        private static string MakeSafeFileName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name;
        }

        private static string Hash(string value)
        {
            unchecked
            {
                uint hash = 2166136261;

                foreach (char c in value)
                {
                    hash ^= c;
                    hash *= 16777619;
                }

                return hash.ToString("x8");
            }
        }

        private static void ReleaseCom(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                Marshal.ReleaseComObject(comObject);
            }
        }
    }
}
