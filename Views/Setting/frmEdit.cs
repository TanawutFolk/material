using BunifuAnimatorNS;
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

namespace RawMat.Views.Setting
{
    public partial class frmEdit : Form
    {
        SettingController _controller = new SettingController();
        // ประกาศตัวแปรส่วนตัว (Private) ไว้เก็บค่าที่รับมา
        private string _category, _mCode, _cavityQty, _samplingType, _samplingQty, _strictnessType, _strictnessLevel, _cavityName;

        private void btnSave_Click(object sender, EventArgs e)
        {
            short cavityQty = 0;
            short samplingQty = 0;

            if (cavityQty < 0 || samplingQty < 0)
            {
                MessageBox.Show("จำนวนตัวเลขห้ามติดลบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. เอาข้อความมาหั่นแยกชิ้นด้วยลูกน้ำ (,) และตัดช่องว่างที่อาจเผลอพิมพ์เกินมาออก
            string[] nameArray = txtCavityName.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            int nameCount = nameArray.Length;

            // 2. ถ้าตัวเลข Cavity Qty มากกว่า 0 แต่จำนวนชื่อที่หั่นมาได้ ไม่เท่ากับตัวเลข Qty
            // (อนุโลมให้กรณี Qty = 0 เผื่อบาง Part ไม่มีการใช้ Cavity)
            if (cavityQty > 0 && nameCount != cavityQty)
            {
                MessageBox.Show($"จำนวนชื่อ Cavity ไม่สอดคล้องกับ Cavity Qty!\n\n" +
                                $"คุณตั้ง Cavity Qty ไว้ที่: {cavityQty}\n" +
                                $"แต่ระบุชื่อมา: {nameCount} ชื่อ ({txtCavityName.Text})\n\n" +
                                $"*กรุณาระบุชื่อให้ครบและคั่นด้วยลูกน้ำ (,) เช่น A,B,C,D",
                                "แจ้งเตือนตรรกะข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // เด้งออก ไม่ให้เซฟ
            }
            try
            {
                // 1. แพ็คข้อมูลจากหน้าจอ ใส่กล่อง SamplingSettingModel
                SettingProperty.SamplingSettingModel updateData = new SettingProperty.SamplingSettingModel();

                updateData.M_Code = txtMCode.Text;
                updateData.Cavity_Name = txtCavityName.Text;

                // แปลงข้อความจากช่อง TextBox เป็นตัวเลข (short)
                updateData.Cavity_Qty = Convert.ToInt16(txtCavityQty.Text);
                updateData.Sampling_Qty = Convert.ToInt16(txtSamplingQty.Text);

                // ดึงตัวเลข ID จาก Dropdown (ที่ซ่อนไว้ใน ValueMember)
                updateData.Sampling_Type = Convert.ToInt16(cboSamplingType.SelectedValue);
                updateData.Strictness_Type = Convert.ToInt16(cboStrictnessType.SelectedValue);
                updateData.Strictness_Level = Convert.ToInt16(cboStrictnessLevel.SelectedValue);

                // 2. ส่งกล่องข้อมูล และประเภทตาราง ไปให้ Controller จัดการ
                bool isSuccess = _controller.UpdateSamplingData(updateData, _category);

                // 3. ถ้าอัปเดตสำเร็จ แจ้งเตือนแล้วปิดหน้าต่าง
                if (isSuccess)
                {
                    MessageBox.Show("อัปเดตข้อมูลสำเร็จ!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close(); // ปิดฟอร์ม Edit เพื่อกลับไปหน้าหลัก
                }
            }
            catch (Exception ex)
            {
                // ดัก Error กรณี User พิมพ์ตัวอักษรใส่ช่องตัวเลข
                MessageBox.Show("กรุณาตรวจสอบความถูกต้องของข้อมูล\n" + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCavityQty_TextChanged(object sender, EventArgs e)
        {

        }

        // 1. แก้ไข Constructor ให้รับค่าจากหน้าหลัก
        public frmEdit(string category, string mCode, string cavityQty, string samplingType, string samplingQty, string strictnessType, string strictnessLevel, string cavityName)
        {
            InitializeComponent();

            // รับค่ามาแล้วเก็บไว้ในตัวแปร
            _category = category;
            _mCode = mCode;
            _cavityQty = cavityQty;
            _samplingType = samplingType;
            _samplingQty = samplingQty;
            _strictnessType = strictnessType;
            _strictnessLevel = strictnessLevel;
            _cavityName = cavityName;
        }

        private void frmEdit_Load(object sender, EventArgs e)
        {
            this.Text = "Edit " + _category;

            // ==========================================
            // 1: โหลด "ตัวเลือกทั้งหมด" ใส่ Dropdown ก่อนนะครับพี่น้อง
            // ==========================================
            LoadDropdownOptions();

            // ==========================================
            // 2: ค่อยเอา "ค่าเดิม" มาหยอดใส่ช่องต่างๆ
            // ==========================================
            txtMCode.Text = _mCode;
            txtMCode.ReadOnly = true;

            txtCavityQty.Text = _cavityQty;
            txtSamplingQty.Text = _samplingQty;
            txtCavityName.Text = _cavityName;

            cboSamplingType.Text = _samplingType;
            cboStrictnessType.Text = _strictnessType;
            cboStrictnessLevel.Text = _strictnessLevel;
        }

        // สร้างฟังก์ชันแยกออกมา เพื่อความสะอาดของโค้ด
        private void LoadDropdownOptions()
        {
            // 1. โหลด Sampling Type
            DataTable dtSampling = _controller.GetMasterSamplingType(); // เรียกผ่าน Controller ของคุณ
            cboSamplingType.DataSource = dtSampling;
            cboSamplingType.DisplayMember = "Sampling_Type_Name"; // สิ่งที่ให้ User เห็น (ภาษาคน)
            cboSamplingType.ValueMember = "sampling_type";        // สิ่งที่ซ่อนไว้เซฟลง DB (ตัวเลข ID)

            // 2. โหลด Strictness Type
            DataTable dtStrictType = _controller.GetMasterStrictnessType();
            cboStrictnessType.DataSource = dtStrictType;
            cboStrictnessType.DisplayMember = "Strictness_Name";
            cboStrictnessType.ValueMember = "Strictness_Type";

            // 3. โหลด Strictness Level
            DataTable dtStrictLevel = _controller.GetMasterStrictnessLevel();
            cboStrictnessLevel.DataSource = dtStrictLevel;
            cboStrictnessLevel.DisplayMember = "Strictness_Level_Name";
            cboStrictnessLevel.ValueMember = "Strictness_Level";
        }
    }
}
