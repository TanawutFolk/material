namespace RawMat.Utilities
{
    /// <summary>
    /// หน้าตรวจที่เปิดใบค้างไว้ (Packing / Regular / Data Result / Function / Dimension / Appearance)
    ///
    /// ผู้ตรวจออกจากหน้าได้โดยไม่ผ่านปุ่มย้อนกลับ คือกด Home หรือ Logout (แถบเมนูอื่นถูกล็อกไว้ตอนอยู่ในหน้าตรวจ)
    /// ทางนั้น NavigationService จะ Dispose หน้าทิ้งเลย
    ///   ConfirmLeave : frmMain เรียกก่อนเริ่มเปลี่ยนหน้า มีค่าที่ยังไม่ได้กด Record Data ให้ถามก่อน (false = อยู่หน้าเดิม)
    ///   LeaveScreen  : NavigationService เรียกก่อน Dispose ให้ได้ผลแบบเดียวกับปุ่มย้อนกลับ (ตั้ง Not Finished yet + ปลดล็อกใบ)
    /// </summary>
    public interface IInspectionScreen
    {
        bool ConfirmLeave();

        void LeaveScreen();
    }
}
