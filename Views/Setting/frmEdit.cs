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
        private string _category, _mCode, _cavityQty, _samplingType, _samplingQty, _strictnessType, _strictnessLevel, _cavityName;

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                short cavityQty = Convert.ToInt16(txtCavityQty.Text);
                short samplingQty = Convert.ToInt16(txtSamplingQty.Text);

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

                string[] nameArray = txtCavityName.Text
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                int nameCount = nameArray.Length;

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

                SettingProperty.SamplingSettingModel updateData =
                    new SettingProperty.SamplingSettingModel();

                updateData.M_Code = txtMCode.Text.Trim();
                updateData.Cavity_Name = txtCavityName.Text.Trim();

                updateData.Cavity_Qty = cavityQty;
                updateData.Sampling_Qty = samplingQty;

                updateData.Sampling_Type = Convert.ToInt16(cboSamplingType.SelectedValue);
                updateData.Strictness_Type = Convert.ToInt16(cboStrictnessType.SelectedValue);
                updateData.Strictness_Level = Convert.ToInt16(cboStrictnessLevel.SelectedValue);

                bool isSuccess = _controller.UpdateSamplingData(updateData, _category);

                if (isSuccess)
                {
                    MessageBox.Show(
                        "อัปเดตข้อมูลสำเร็จ!",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;

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

        public frmEdit(string category, string mCode, string cavityQty, string samplingType, string samplingQty, string strictnessType, string strictnessLevel, string cavityName)
        {
            InitializeComponent();

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

            LoadDropdownOptions();

            txtMCode.Text = _mCode;
            txtMCode.ReadOnly = true;

            txtCavityQty.Text = _cavityQty;
            txtSamplingQty.Text = _samplingQty;
            txtCavityName.Text = _cavityName;

            cboSamplingType.Text = _samplingType;
            cboStrictnessType.Text = _strictnessType;
            cboStrictnessLevel.Text = _strictnessLevel;
        }

        private void LoadDropdownOptions()
        {
            DataTable dtSampling = _controller.GetMasterSamplingType();
            cboSamplingType.DataSource = dtSampling;
            cboSamplingType.DisplayMember = "Sampling_Type_Name";
            cboSamplingType.ValueMember = "sampling_type";

            DataTable dtStrictType = _controller.GetMasterStrictnessType();
            cboStrictnessType.DataSource = dtStrictType;
            cboStrictnessType.DisplayMember = "Strictness_Name";
            cboStrictnessType.ValueMember = "Strictness_Type";

            DataTable dtStrictLevel = _controller.GetMasterStrictnessLevel();
            cboStrictnessLevel.DataSource = dtStrictLevel;
            cboStrictnessLevel.DisplayMember = "Strictness_Level_Name";
            cboStrictnessLevel.ValueMember = "Strictness_Level";
        }
    }
}