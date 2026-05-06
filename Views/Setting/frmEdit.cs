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
            try
            {
                // แปลงค่าตัวเลขก่อน
                short cavityQty = Convert.ToInt16(txtCavityQty.Text);
                short samplingQty = Convert.ToInt16(txtSamplingQty.Text);

                // เช็คค่าติดลบ
                if (cavityQty < 0 || samplingQty < 0)
                {
                    MessageBox.Show(
                        "จำนวนตัวเลขห้ามติดลบ",
                        "แจ้งเตือน",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // แยกชื่อ Cavity ด้วยลูกน้ำ
                string[] nameArray = txtCavityName.Text
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                int nameCount = nameArray.Length;

                // เช็คจำนวนชื่อ Cavity ให้ตรงกับ Cavity Qty
                if (cavityQty > 0 && nameCount != cavityQty)
                {
                    MessageBox.Show(
                        $"จำนวนชื่อ Cavity ไม่สอดคล้องกับ Cavity Qty!\n\n" +
                        $"คุณตั้ง Cavity Qty ไว้ที่: {cavityQty}\n" +
                        $"แต่ระบุชื่อมา: {nameCount} ชื่อ ({txtCavityName.Text})\n\n" +
                        $"*กรุณาระบุชื่อให้ครบและคั่นด้วยลูกน้ำ (,) เช่น A,B,C,D",
                        "แจ้งเตือนตรรกะข้อมูล",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // แพ็คข้อมูลจากหน้าจอ
                SettingProperty.SamplingSettingModel updateData =
                    new SettingProperty.SamplingSettingModel();

                updateData.M_Code = txtMCode.Text.Trim();
                updateData.Cavity_Name = txtCavityName.Text.Trim();

                updateData.Cavity_Qty = cavityQty;
                updateData.Sampling_Qty = samplingQty;

                updateData.Sampling_Type = Convert.ToInt16(cboSamplingType.SelectedValue);
                updateData.Strictness_Type = Convert.ToInt16(cboStrictnessType.SelectedValue);
                updateData.Strictness_Level = Convert.ToInt16(cboStrictnessLevel.SelectedValue);

                // ส่งข้อมูลไป Update
                bool isSuccess = _controller.UpdateSamplingData(updateData, _category);

                if (isSuccess)
                {
                    MessageBox.Show(
                        "อัปเดตข้อมูลสำเร็จ!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // สำคัญมาก: ส่งค่า OK กลับไปให้ frmSetting
                    this.DialogResult = DialogResult.OK;

                    // ปิด frmEdit
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "ไม่สามารถอัปเดตข้อมูลได้",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "กรุณาตรวจสอบความถูกต้องของข้อมูล\n" + ex.Message,
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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
