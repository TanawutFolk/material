using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsLogin
{
    public partial class ucNCR : UserControl
    {
        public ucNCR()
        {
            InitializeComponent();
            btnNcr.Click += btnNcr_Click;
        }

        /// <summary>
        /// ไทล์ตัวนี้ไม่รู้จักฟอร์มปลายทาง หน้าที่เอาไปวางเป็นคนตัดสินใจเองว่าจะเปิดอะไร
        /// </summary>
        public event EventHandler Clicked;

        private void btnNcr_Click(object sender, EventArgs e)
        {
            EventHandler handler = Clicked;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
