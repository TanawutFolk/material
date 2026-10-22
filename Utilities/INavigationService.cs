using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    public interface INavigationService
    {
        void NavigateTo(UserControl userControl);
        void Clear();

        /// <summary>หน้าตรวจที่เปิดค้างอยู่ออกได้ไหม (ถามผู้ตรวจก่อนถ้ามีค่าที่ยังไม่ได้กด Record Data) false = ห้ามเปลี่ยนหน้า</summary>
        bool ConfirmLeaveInspectionScreens();
    }
}
