using Bunifu.Framework.UI;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Views.ReceiveMat;
using RawMat.Views.ReceiveWH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.Menu
{
    public partial class userControlWH : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        private BunifuTileButton activeButton;
        QAdataControllers conQA = new QAdataControllers();
        public userControlWH()
        {
            InitializeComponent();
        }

        private void bt_rec_replace_Click(object sender, EventArgs e)
        {
            userControlReplacement usrReplace = new userControlReplacement()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()

            };

            //DataTable dt = new DataTable();
            //dt = conQA.SearchReplacement();
            //usrReplace.propQA.dtgRawMat = new DataGridView();

            //usrReplace.propQA.dtgRawMat.DataSource = dt;

            // Raise the event
            AddUserControlRequested?.Invoke(usrReplace);
        }

        private void bt_rec_issue_Click(object sender, EventArgs e)
        {
            userControlCheckSheet usrCheckSheet= new userControlCheckSheet();
            usrCheckSheet.Dock = DockStyle.Fill;

            // Raise the event
            AddUserControlRequested?.Invoke(usrCheckSheet);
        }
    }
}
