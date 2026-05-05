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
    public partial class CustomMsgBoxYesNo : CustomMsgBoxBase
    {
        public bool IsYesClicked { get; private set; } = false;

        // Add this property
        public CustomMsgBoxYesNo()
        {
            InitializeComponent();
        }

        // Example: Accessing inherited controls
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

        // Example: Accessing inherited controls
        public void SetIcon(MessageBoxIconType iconType)
        {
            Icon = iconType; // Uses inherited Icon property
        }

        private void bt_no_Click(object sender, EventArgs e)
        {
            IsYesClicked = false;
            this.Close();
        }

        private void bt_yes_Click(object sender, EventArgs e)
        {
            IsYesClicked = true;
            this.Close();
        }
    }
}
