using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsMaterial.AppearCheck
{
    public partial class frmAlertPending : Form
    {
        public frmAlertPending()
        {
            InitializeComponent();
            AcceptButton = btnOk;
            StartPosition = FormStartPosition.CenterParent;
            btnOk.Click += btnOk_Click;
        }

        public frmAlertPending(string alertText, bool showPrintLabel)
            : this()
        {
            lb_alertText.Text = alertText;
            pcPrintLabel.Visible = showPrintLabel;
            pcMovePrones.Visible = true;
            btnOk.Visible = true;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
