using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.AppearCheck
{
    public partial class frmAlert : Form
    {
        public frmAlert()
        {
            InitializeComponent();
            AcceptButton = btnOk;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
