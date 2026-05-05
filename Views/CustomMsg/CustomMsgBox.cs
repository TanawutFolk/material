using RawMat.Views.CustomMsg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.CustomMsg
{
    public partial class CustomMsgBox : CustomMsgBoxBase
    {
        public CustomMsgBox()
        {
            InitializeComponent();
        }

        // อัปเดตเมธอด SetMessage เพื่อรับ title เพิ่มเติม
        public void SetMessage(
            string message,
            string title = null,
            Color? backColor = null,
            Color? foreColor = null,
            Font font = null,
            float? fontSize = null)
        {
            lblMessage.Text = message;
            if (!string.IsNullOrEmpty(title))
                Title = title;
            if (backColor.HasValue)
                MessageBackColor = backColor.Value;
            if (foreColor.HasValue)
                MessageForeColor = foreColor.Value;
            if (font != null)
                MessageFont = font;
            if (fontSize.HasValue)
                MessageFontSize = fontSize.Value;
        }

        public void SetIcon(MessageBoxIconType iconType)
        {
            Icon = iconType; // ใช้ Icon ที่สืบทอดมา
        }

        private void bt_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
