using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsMaterial.Setting
{
    public partial class frmConfirm : Form
    {
        public frmConfirm(string message) : this(message, null, null)
        {
        }

        public frmConfirm(string message, Image icon) : this(message, icon, null)
        {
        }

        /// <summary>
        /// ส่งรูปกับข้อความบนปุ่มยืนยันเข้ามาแทนค่าเริ่มต้นได้
        /// เช่นหน้า Check Sheet ใช้รูปรถเข็นและปุ่มว่า Issue check sheet
        /// </summary>
        public frmConfirm(string message, Image icon, string confirmText)
        {
            InitializeComponent();

            lblMessage.Text = message;

            if (icon != null)
            {
                pictureBox1.Image = icon;
            }

            if (!string.IsNullOrWhiteSpace(confirmText))
            {
                btnSave.ButtonText = confirmText;
                btnSave.Text = confirmText;
                ShrinkFontToFit(btnSave, confirmText);
            }

            FitToMessage();
        }

        /// <summary>
        /// ปุ่มกว้างตายตัว 183px ข้อความยาวกว่านั้นจะโดนตัด
        /// ลดขนาดตัวอักษรลงจนพอดีแทนที่จะขยายปุ่มไปชนปุ่ม Cancel
        /// </summary>
        private void ShrinkFontToFit(Bunifu.Framework.UI.BunifuFlatButton button, string text)
        {
            const int Padding = 12;

            using (Graphics g = button.CreateGraphics())
            {
                Font font = button.TextFont;

                while (font.Size > 7f &&
                       g.MeasureString(text, font).Width > button.Width - Padding)
                {
                    Font smaller = new Font(font.FontFamily, font.Size - 0.5f, font.Style);
                    if (!ReferenceEquals(font, button.TextFont))
                    {
                        font.Dispose();
                    }
                    font = smaller;
                }

                button.TextFont = font;
            }
        }

        /// <summary>
        /// ผังเดิมวางปุ่มไว้ตายตัวที่ y=158 ส่วน lblMessage เป็น AutoSize
        /// ข้อความหลายบรรทัดจึงยืดลงไปทับปุ่ม ต้องดันปุ่มกับความสูงฟอร์มตามข้อความ
        /// </summary>
        private void FitToMessage()
        {
            // จำกัดความกว้างก่อน ข้อความยาวจะได้ตัดบรรทัดแทนที่จะล้นออกนอกฟอร์ม
            lblMessage.MaximumSize = new Size(panel1.Width - (lblMessage.Left * 2), 0);

            const int GapBelowMessage = 18;

            int wantedButtonTop = lblMessage.Bottom + GapBelowMessage;
            int shift = wantedButtonTop - btnSave.Top;

            // ข้อความสั้นกว่าผังเดิม ใช้ผังเดิมไปเลย ไม่ต้องหดฟอร์ม
            if (shift <= 0) return;

            btnSave.Top += shift;
            btnCancle.Top += shift;
            panel1.Height += shift;
            this.Height += shift;
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
