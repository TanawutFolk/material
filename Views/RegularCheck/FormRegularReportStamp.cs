using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.RegularCheck
{
    public partial class FormRegularReportStamp : Form
    {
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        QAdataControllers conQA = new QAdataControllers();
        QAdataProperty propQA = new QAdataProperty();
        private Image stamp_pic;
        private string selectedStampPath = string.Empty;

        public FormRegularReportStamp(QAdataProperty dataItem)
        {
            InitializeComponent();
            propQA = dataItem ?? new QAdataProperty();
            this.lb_reportNo.Text = propQA.Regular_No;
            if (employee.EMP_LEVEL != "1")
            {
                this.Close();
                return;
            }

        }

        private void bt_copy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lb_reportNo.Text))
            {
                Clipboard.SetText(lb_reportNo.Text);
                MessageBox.Show("คัดลอกหมายเลข Regular No: " + lb_reportNo.Text + " เรียบร้อยแล้ว",
                    "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("ไม่มีข้อมูลที่จะคัดลอก", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FormRegularReportStamp_Load(object sender, EventArgs e)
        {
            // ตั้งค่า Watermark ให้กับ tb_programStamp
            SetWatermark(tb_programStamp, "กรุณาเลือก path E-Stamp");
        }

        // เมธอดสำหรับตั้งค่า Watermark
        private void SetWatermark(TextBox textBox, string watermarkText)
        {
            // เก็บข้อความวอเตอร์มาร์ค
            textBox.Tag = watermarkText;

            // ถ้า TextBox ว่างให้แสดงข้อความวอเตอร์มาร์ค
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = watermarkText;
                textBox.ForeColor = Color.Gray;
            }

            // Event เมื่อ TextBox ได้รับโฟกัส
            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == watermarkText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            // Event เมื่อ TextBox เสียโฟกัส
            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = watermarkText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void bt_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TIFF files (*.tiff;*.tif)|*.tiff;*.tif|All files (*.*)|*.*";
            openFileDialog.Title = "เลือกไฟล์รูปภาพ Stamp (.tiff)";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // โหลดรูปภาพจากไฟล์
                    stamp_pic = Image.FromFile(openFileDialog.FileName);
                    selectedStampPath = openFileDialog.FileName;
                    tb_programStamp.Text = selectedStampPath;
                    tb_programStamp.ForeColor = Color.Black;
                    // แสดงรูปภาพใน PictureBox (ถ้ามีในฟอร์ม)
                    if (pb_stamp != null)
                    {
                        pb_stamp.Image = stamp_pic;
                        pb_stamp.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    MessageBox.Show("โหลดไฟล์ Stamp เรียบร้อยแล้ว", "สำเร็จ",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ไม่สามารถโหลดไฟล์ได้: " + ex.Message, "ข้อผิดพลาด",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Property สำหรับเข้าถึงข้อมูล stamp_pic
        public Image GetStampImage()
        {
            return stamp_pic;
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            //PDF After OK
            string stampPath = GetSelectedStampPath();
            if (string.IsNullOrWhiteSpace(stampPath) || !System.IO.File.Exists(stampPath))
            {
                MessageBox.Show("กรุณาเลือกไฟล์ E-Stamp ก่อนกด OK", "แจ้งเตือน",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ExportExcell.ApproveExcelReport(propQA, stampPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถ Save PDF หรือย้ายไฟล์ Regular Report ได้: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            propQA.process = "Regular_Check";
            propQA.inProcStatus = ((int)QAdataProperty.ProcStatus.OK).ToString();
            propQA.reportStatus = ((int)QAdataProperty.ProcStatus.OK).ToString();

            if (!conQA.UpdateStatus(propQA))
            {
                MessageBox.Show("ไม่สามารถเปลี่ยนสถานะ Regular Report เป็น OK ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetSelectedStampPath()
        {
            if (!string.IsNullOrWhiteSpace(selectedStampPath))
            {
                return selectedStampPath;
            }

            if (tb_programStamp.ForeColor == Color.Gray)
            {
                return string.Empty;
            }

            return tb_programStamp.Text?.Trim() ?? string.Empty;
        }

        private void bt_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();

        }
    }
}
