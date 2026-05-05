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
            DataTable dt = new DataTable();
            //dt = conQA.SearchForAppearPending();

            //dtg_appearPending.DataSource = dt;

            //dtg_appearPending.Columns["process_id"].Visible = false;
            //dtg_appearPending.Columns["Issue_Date"].Visible = false;
        }
    }
}
