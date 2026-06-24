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

namespace RawMat.Views.AppearCheck
{
    public partial class userControlSelectAppearPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();

        public userControlSelectAppearPending()
        {
            InitializeComponent();
        }

        private void userControlSelectAppearPending_Load(object sender, EventArgs e)
        {
            DataTable dt = conQA.SearchForAppearPending();
            dtg_appearPending.DataSource = dt;

            if (dtg_appearPending.Columns.Contains("process_status_id"))
            {
                dtg_appearPending.Columns["process_status_id"].Visible = false;
            }

            if (dtg_appearPending.Columns.Contains("Issue_Date"))
            {
                dtg_appearPending.Columns["Issue_Date"].Visible = false;
            }
        }
    }
}
