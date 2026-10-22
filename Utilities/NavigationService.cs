using RawMat.ViewsMaterial.PackingCheck;
using RawMat.ViewsMaterial.ReceiveWH;
using RawMat.ViewsMaterial.RegularCheck;
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

        // หน้าตรวจที่เปิดค้างอยู่ (อยู่ข้างในหน้าเลือกใบอีกชั้น) ต้องออกให้เรียบร้อยก่อนถูก Dispose
        // ไม่งั้นใบค้างสถานะ Working และล็อกค้าง เหมือนไม่มีใครกดย้อนกลับ ดู IInspectionScreen
        private static void LeaveInspectionScreens(Control root)
        {
            foreach (Control child in root.Controls)
            {
                if (child is IInspectionScreen screen)
                {
                    try
                    {
                        screen.LeaveScreen();
                    }
                    catch (Exception ex)
                    {
                        // ห้ามทำให้กดเมนูไม่ไป แจ้งแล้วให้ไปต่อ ใบที่ค้าง Admin ปลดล็อกเองได้จากหน้าเลือกใบ
                        MessageBox.Show("ปลดสถานะใบที่เปิดค้างไว้ไม่สำเร็จ : " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    continue;
                }

                LeaveInspectionScreens(child);
            }
        }

        public bool ConfirmLeaveInspectionScreens()
        {
            return ConfirmLeaveInspectionScreens(_panel);
        }

        private static bool ConfirmLeaveInspectionScreens(Control root)
        {
            foreach (Control child in root.Controls)
            {
                if (child is IInspectionScreen screen)
                {
                    if (!screen.ConfirmLeave()) return false;
                    continue;
                }

                if (!ConfirmLeaveInspectionScreens(child)) return false;
            }

            return true;
        }

        public void NavigateTo(UserControl userControl)
        {
            if (userControl == null) throw new ArgumentNullException(nameof(userControl));

            LeaveInspectionScreens(_panel);

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
            LeaveInspectionScreens(_panel);

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
