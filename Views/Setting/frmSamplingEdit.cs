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
    public partial class frmSamplingEdit : Form
    {
        SettingController _controller = new SettingController();

        private string _category;
        private string _mCode;
        private string _cavityQty;
        private string _samplingType;
        private string _samplingQty;
        private string _strictnessType;
        private string _strictnessLevel;
        private string _cavityName;

        private const string STRICTNESS_TABLE_TEXT = "Strictness Table";
        private const string NONE_TEXT = "-";
        private const string ALL_TEXT = "All";

        public frmSamplingEdit(
            string category,
            string mCode,
            string cavityQty,
            string samplingType,
            string samplingQty,
            string strictnessType,
            string strictnessLevel,
            string cavityName
        )
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

            cboSamplingType.SelectedIndexChanged += cboSamplingType_SelectedIndexChanged;

            ApplySamplingTypeCondition();
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

        private void cboSamplingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplySamplingTypeCondition();
        }

        private void ApplySamplingTypeCondition()
        {
            bool isStrictnessTable = IsStrictnessTableSelected();
            bool isAllSamplingType = IsAllSamplingTypeSelected();

            txtSamplingQty.ReadOnly = isAllSamplingType;
            txtSamplingQty.Enabled = true;

            cboStrictnessType.Enabled = isStrictnessTable;
            cboStrictnessLevel.Enabled = isStrictnessTable;

            if (isAllSamplingType)
            {
                txtSamplingQty.Text = "0";
            }

            if (isStrictnessTable)
            {
                SetComboBoxTextIfExists(cboStrictnessType, _strictnessType);
                SetComboBoxTextIfExists(cboStrictnessLevel, _strictnessLevel);
                return;
            }

            SetComboBoxTextIfExists(cboStrictnessType, NONE_TEXT);
            SetComboBoxTextIfExists(cboStrictnessLevel, NONE_TEXT);
        }

        private bool IsStrictnessTableSelected()
        {
            if (cboSamplingType.Text == null)
            {
                return false;
            }

            return cboSamplingType.Text.Trim().Equals(
                STRICTNESS_TABLE_TEXT,
                StringComparison.OrdinalIgnoreCase
            );
        }

        private bool IsAllSamplingTypeSelected()
        {
            if (cboSamplingType.SelectedValue != null)
            {
                short samplingType;

                if (short.TryParse(cboSamplingType.SelectedValue.ToString(), out samplingType))
                {
                    return samplingType == 0;
                }
            }

            if (cboSamplingType.Text == null)
            {
                return false;
            }

            return cboSamplingType.Text.Trim().Equals(
                ALL_TEXT,
                StringComparison.OrdinalIgnoreCase
            );
        }

        private void SetComboBoxTextIfExists(ComboBox comboBox, string text)
        {
            if (comboBox == null || comboBox.Items.Count == 0)
            {
                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                comboBox.SelectedIndex = i;

                if (comboBox.Text.Trim().Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            comboBox.SelectedIndex = -1;
            comboBox.Text = text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                short cavityQty;

                if (!short.TryParse(txtCavityQty.Text.Trim(), out cavityQty))
                {
                    MessageBox.Show(
                        "กรุณาระบุ Cavity Qty เป็นตัวเลข",
                        "แจ้งเตือน",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (cboSamplingType.SelectedValue == null)
                {
                    MessageBox.Show(
                        "กรุณาเลือก Sampling Type",
                        "แจ้งเตือน",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                short samplingType;

                if (!short.TryParse(cboSamplingType.SelectedValue.ToString(), out samplingType))
                {
                    MessageBox.Show(
                        "กรุณาเลือก Sampling Type",
                        "แจ้งเตือน",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                short samplingQty;

                if (samplingType == 0)
                {
                    samplingQty = 0;
                    txtSamplingQty.Text = "0";
                }
                else
                {
                    if (!short.TryParse(txtSamplingQty.Text.Trim(), out samplingQty))
                    {
                        MessageBox.Show(
                            "กรุณาระบุ Sampling Qty เป็นตัวเลข",
                            "แจ้งเตือน",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }

                bool isStrictnessTable = IsStrictnessTableSelected();

                short strictnessType = 0;
                short strictnessLevel = 0;

                if (isStrictnessTable)
                {
                    if (cboStrictnessType.SelectedValue == null)
                    {
                        MessageBox.Show(
                            "กรุณาเลือก Strictness Type",
                            "แจ้งเตือน",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    if (cboStrictnessLevel.SelectedValue == null)
                    {
                        MessageBox.Show(
                            "กรุณาเลือก Strictness Level",
                            "แจ้งเตือน",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    strictnessType = Convert.ToInt16(cboStrictnessType.SelectedValue);
                    strictnessLevel = Convert.ToInt16(cboStrictnessLevel.SelectedValue);
                }

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

                updateData.Sampling_Type = samplingType;
                updateData.Strictness_Type = strictnessType;
                updateData.Strictness_Level = strictnessLevel;

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
    }
}