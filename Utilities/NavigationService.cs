using RawMat.Views.PackingCheck;
using RawMat.Views.ReceiveWH;
using RawMat.Views.RegularCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    public class NavigationService : INavigationService
    {
        private readonly Panel _panel;
        //private readonly IParent _parent;

        public NavigationService(Panel panel)
        {
            _panel = panel ?? throw new ArgumentNullException(nameof(panel));
            
        }

        public void NavigateTo(UserControl userControl)
        {
            if (userControl == null) throw new ArgumentNullException(nameof(userControl));

            foreach (Control ctrl in _panel.Controls)
            {
                //if (ctrl is userControlRegular regularControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{regularControl.propQA.Report_No}_{regularControl.propQA.process}";
                //    (ctrl as IParent)?.ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlSelectRegular selectRegularControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{selectRegularControl.propQA.Report_No}_{selectRegularControl.propQA.process}";
                //    (ctrl as IParent)?.ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlPackingCheck packingControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{packingControl.propQA.Report_No}_{packingControl.propQA.process}";
                //    (ctrl as IParent)?.ReleaseReportMutex(mutexKey);
                //}



                ctrl.Dispose();
            }

            _panel.Controls.Clear();
            userControl.Dock = DockStyle.Fill;
            _panel.Controls.Add(userControl);
            userControl.BringToFront();
        }

        public void Clear()
        {
            foreach (Control ctrl in _panel.Controls)
            {
                //if (ctrl is userControlRegular regularControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{regularControl.propQA.Report_No}_{regularControl.propQA.process}";
                //    (ctrl as IParent)?.ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlSelectRegular selectRegularControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{selectRegularControl.propQA.Report_No}_{selectRegularControl.propQA.process}";
                //    (ctrl as IParent)?.ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlPackingCheck packingControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{packingControl.propQA.Report_No}_{packingControl.propQA.process}";
                //    (ctrl as IParent)?.ReleaseReportMutex(mutexKey);
                //}
                ctrl.Dispose();
            }
            _panel.Controls.Clear();
        }
    }
}
