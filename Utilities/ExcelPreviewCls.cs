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

            try
            {
                int days;
                if (!int.TryParse(ConfigurationManager.AppSettings["ExcelPreviewCacheDays"], out days) || days <= 0)
                {
                    days = 7;
                }

                DateTime limit = DateTime.UtcNow.AddDays(-days);

                foreach (string file in Directory.GetFiles(GetCacheDirectory(), "*.pdf"))
                {
                    // ไฟล์ที่ยังเปิดค้างใน viewer จะลบไม่ได้ ข้ามไปลบรอบหน้า
                    try
                    {
                        if (File.GetLastWriteTimeUtc(file) < limit)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
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
