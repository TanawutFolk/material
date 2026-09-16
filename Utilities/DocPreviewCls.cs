using AxAcroPDFLib;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    /// <summary>
    /// หาไฟล์เอกสารของรายการตรวจแล้วเตรียมให้พร้อมโชว์ใน panel preview
    /// เจอ PDF เปิดตรง ๆ เจอ Excel จะถูกแปลงเป็น PDF ก่อนด้วย <see cref="ExcelPreviewCls"/>
    /// </summary>
    public static class DocPreviewCls
    {
        // เรียงตามลำดับที่ต้องการ ถ้ามี PDF ที่สแกนไว้แล้วให้ใช้ตัวนั้นก่อน
        private static readonly string[] previewExtensions = { ".pdf", ".xlsx", ".xlsm", ".xls" };

        private const string messageLabelName = "lb_previewMessage";

        /// <summary>
        /// หาไฟล์เอกสารจากชื่อฐาน (path ที่ยังไม่มีนามสกุล) คืน null ถ้าไม่เจอไฟล์ที่รองรับ
        /// </summary>
        public static string FindDocument(string basePathWithoutExtension)
        {
            if (string.IsNullOrWhiteSpace(basePathWithoutExtension))
            {
                return null;
            }

            foreach (string extension in previewExtensions)
            {
                string candidate = basePathWithoutExtension + extension;

                // share ล่มหรือไม่มีสิทธิ์อ่าน ถือว่าไม่เจอ ไม่ต้องให้ทั้งหน้าจอพัง
                try
                {
                    if (File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            return null;
        }

        /// <summary>
        /// มีเอกสารของชื่อฐานนี้อยู่ไหม รองรับทั้ง PDF และ Excel
        /// ใช้แทน File.Exists(...+".pdf") ที่เดิมมองไม่เห็นไฟล์ Excel
        /// </summary>
        public static bool Exists(string basePathWithoutExtension)
        {
            return FindDocument(basePathWithoutExtension) != null;
        }

        /// <summary>
        /// นามสกุลที่รองรับ ใช้ประกอบข้อความตอนแจ้งผู้ใช้ว่าหาไฟล์ไม่เจอ
        /// </summary>
        public static string SupportedExtensionsText
        {
            get { return string.Join(" / ", previewExtensions); }
        }

        /// <summary>
        /// คืน path ของ PDF ที่พร้อมโชว์ ถ้าต้นทางเป็น Excel จะแปลงให้ก่อน (ใช้เวลา เรียกใน background)
        /// </summary>
        public static string ToPdf(string sourcePath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return null;
            }

            return sourcePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? sourcePath
                : ExcelPreviewCls.ToPdf(sourcePath);
        }

        public static void LoadPdf(Panel panel, AxAcroPDF viewer, string pdfPath)
        {
            if (panel == null || viewer == null)
            {
                return;
            }

            RemoveMessage(panel);

            panel.Visible = true;

            if (!panel.Controls.Contains(viewer))
            {
                panel.Controls.Add(viewer);
            }
            else
            {
                // AxAcroPDF ล็อกไฟล์ที่เปิดค้างไว้ ต้องปล่อยของเดิมก่อนถึงจะโหลดไฟล์ใหม่ทับได้
                try { viewer.src = string.Empty; } catch (Exception) { }
            }

            viewer.Dock = DockStyle.Fill;
            viewer.Visible = true;
            viewer.src = pdfPath;
            viewer.LoadFile(pdfPath);
            viewer.setShowToolbar(false);
            ApplyViewMode(viewer);
            viewer.BringToFront();
            viewer.Show();
            viewer.Focus();
        }

        /// <summary>
        /// ตั้งโหมดแสดงผลให้เอกสารเต็มความกว้างของ panel
        /// </summary>
        /// <remarks>
        /// ถ้าไม่สั่ง viewer จะใช้ค่า default ของ Adobe Reader ในเครื่องนั้น ซึ่งมักเป็น Fit Page
        /// คือย่อทั้งหน้าให้พอดีกรอบ ความสูงจึงเป็นตัวบีบ เอกสารแนวนอนอย่าง check sheet ของ SANWA
        /// (Letter แนวนอน สูง 816 px) ถูกย่อเหลือ 77% ในกรอบสูง 631 px จนอ่านไม่ออก
        /// FitH ใช้ความกว้างเป็นตัวกำหนดแทน แล้วให้เลื่อนลงอ่านส่วนที่เหลือ
        /// OneColumn ทำให้เลื่อนข้ามหน้าต่อกันไปได้ เพราะซ่อน toolbar ไว้ ผู้ใช้จึงไม่มีปุ่มเปลี่ยนหน้า
        /// </remarks>
        private static void ApplyViewMode(AxAcroPDF viewer)
        {
            // เอกสารเสียหรือ Adobe เวอร์ชันเก่าอาจไม่รองรับ ถือว่าไม่เป็นไร ใช้ค่า default แทน
            try
            {
                viewer.setLayoutMode("OneColumn");
                viewer.setView("FitH");
            }
            catch (Exception) { }
        }

        /// <summary>
        /// แสดงข้อความแทนเอกสารใน panel เช่นตอนไม่พบไฟล์ หรือกำลังแปลงไฟล์อยู่
        /// </summary>
        public static void ShowMessage(Panel panel, string message)
        {
            if (panel == null)
            {
                return;
            }

            panel.Visible = true;

            Label label = panel.Controls[messageLabelName] as Label;

            if (label == null)
            {
                label = new Label
                {
                    Name = messageLabelName,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                    ForeColor = Color.DimGray,
                    BackColor = Color.White
                };

                panel.Controls.Add(label);
            }

            label.Text = message;
            label.Visible = true;
            label.BringToFront();
        }

        private static void RemoveMessage(Panel panel)
        {
            Label label = panel.Controls[messageLabelName] as Label;

            if (label == null)
            {
                return;
            }

            panel.Controls.Remove(label);
            label.Dispose();
        }
    }
}
