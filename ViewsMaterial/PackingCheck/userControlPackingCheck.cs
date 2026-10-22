using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.PackingCheck;
using RawMat.ViewsMaterial.Main;
using RawMat.ViewsMaterial.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using RawMat.ViewsMaterial.CustomMsg;
using static RawMat.Property.QAdataProperty;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace RawMat.ViewsMaterial.PackingCheck
{
    public partial class userControlPackingCheck : UserControl, IInspectionScreen
    {
        public event Action<UserControl> AddUserControlRequested;
        public event Action OnReleaseMutex;
        public event EventHandler BackToARequested;
        public event Action<string> RequestReleaseMutex;
        public event Action OnClose;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private BindingSource bindingSource = new BindingSource();
        private IParent parent;
        private readonly Dictionary<int, string> originalMethodJudgments = new Dictionary<int, string>();

        // ร่าง : ปุ่ม bt_save = Record Data (เก็บร่าง) / ปุ่ม bt_saveContinue = Save and Continue (ส่งผล) แยกกัน (Utilities/InspectionDraft.cs)
        // หน้าอื่นใช้ปุ่มเดียวสลับตามความครบ แต่ Packing กรอก Lot No. ได้เรื่อย ๆ จึงรู้ไม่ได้ว่าครบเมื่อไหร่
        private const string DraftPackingSizeTable = "PACKING_SIZE";
        private const string DraftLotNoTable = "LOT_NO";
        private bool _draftEnabled;     // Admin เปิดมาตัดสินงาน NG ปุ่มนี้คือปุ่มอนุมัติ จึงไม่ใช้ร่าง
        private bool _isRestoringDraft;
        // ค่าบนจอตอนที่ถือว่าเก็บแล้ว (เปิดหน้าเสร็จ / กด Record Data) ใช้เทียบตอนออกว่ามีค่าที่ยังไม่ได้ Record ไหม
        private string _recordedDraft;

        public userControlPackingCheck(IParent parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void userControlPackingCheck_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");

            pb_packing1.Image = imgCls.LoadPackingImage("Packing1");
            pb_packing2.Image = imgCls.LoadPackingImage("Packing2");
            pb_packing3.Image = imgCls.LoadPackingImage("Packing3");

            propQA.METHOD_ID = "1";
            lb_method1.Text = conQA.DetailMethod(propQA);

            propQA.METHOD_ID = "2";
            lb_method2.Text = conQA.DetailMethod(propQA);

            propQA.METHOD_ID = "3";
            lb_method3.Text = conQA.DetailMethod(propQA);

            // ตั้งค่าเริ่มต้น groupbox

            LoadPackingCheckData();
            LoadReportLotNoData();

            // Admin เปิดมาตัดสินงาน NG ปุ่ม SAVE คือปุ่มอนุมัติ (SaveAdminPartialApproval) ไม่ใช่ปุ่มกรอกผล
            _draftEnabled = !IsAdminPendingReview();
            if (_draftEnabled)
            {
                // ค่าที่เคย Save ไว้ตอนยังกรอกไม่ครบ ดึงกลับมาเติมให้ทำต่อได้
                LoadDraft();
                HookDraftTracking();
                UpdateRecordButtonMode();
                _recordedDraft = InspectionDraft.Serialize(BuildDraft());
            }
            else
            {
                bt_saveContinue.Visible = false;
            }
        }

        private void HookDraftTracking()
        {
            foreach (RadioButton radio in new[] { rb_ok_method1, rb_ng_method1, rb_ok_method2, rb_ng_method2, rb_ok_method3, rb_ng_method3 })
            {
                radio.CheckedChanged += Input_Changed;
            }

            tb_detail_method1.TextChanged += Input_Changed;
            tb_detail_method2.TextChanged += Input_Changed;
            tb_detail_method3.TextChanged += Input_Changed;

            // ผูกหลัง handler เดิมใน Designer จึงทำงานหลังคิด CALVALUE / ตัวพิมพ์ใหญ่ Lot No. เสร็จแล้ว
            dtg_packing_size.CellEndEdit += Input_Changed;
            dtg_packing_size.RowsRemoved += Input_Changed;
            dtg_lot_no.CellEndEdit += Input_Changed;

            // พิมพ์ในตารางเมื่อไหร่ เปลี่ยนปุ่มทันที ไม่ต้องรอกด Enter
            dtg_packing_size.EditingControlShowing += WatchEditingValue;
            dtg_lot_no.EditingControlShowing += WatchEditingValue;
        }

        private void WatchEditingValue(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            InspectionDraft.WatchEditingControl(e, EditingValue_Changed);
        }

        private void EditingValue_Changed(object sender, EventArgs e)
        {
            UpdateRecordButtonMode();
        }

        // ติ๊ก / พิมพ์เสร็จ / ลบแถว : คิดใหม่ว่าปุ่มเป็น Record Data หรือ Save and Continue
        private void Input_Changed(object sender, EventArgs e)
        {
            if (_isRestoringDraft)
            {
                return;
            }

            UpdateRecordButtonMode();
        }

        private void LoadDraft()
        {
            DataSet draft = InspectionDraft.Load(conQA, propQA);
            if (draft == null)
            {
                return;
            }

            _isRestoringDraft = true;

            try
            {
                // Packing Size ต้องมาก่อน เพราะติ๊ก OK ข้อ 3 ได้ต่อเมื่อผลรวมเท่ากับ Lot Size แล้ว (rb_ok_method3_CheckedChanged)
                // ข้อ 3 ที่บันทึก OK ไปแล้วถูกล็อก และ Packing Size โหลดจาก DB ไปแล้ว (LoadPackingSizeData)
                if (gb_method3.Enabled && draft.Tables.Contains(DraftPackingSizeTable) && draft.Tables[DraftPackingSizeTable].Rows.Count > 0)
                {
                    dtg_packing_size.Rows.Clear();

                    foreach (DataRow row in draft.Tables[DraftPackingSizeTable].Rows)
                    {
                        dtg_packing_size.Rows.Add(row["VALUE"], row["PACK_COUNT"], null);
                    }

                    UpdateCalculatedValues();
                }

                for (int methodId = 1; methodId <= 3; methodId++)
                {
                    // ข้อที่บันทึก OK ลง DB แล้วถูกล็อกไว้ (LoadPackingCheckData) ไม่เอาร่างไปทับ
                    if (!GetGroupBox(methodId).Enabled)
                    {
                        continue;
                    }

                    var (rbOk, rbNg, tbDetail, _) = GetMethodControls(methodId);
                    string judge = InspectionDraft.GetField(draft, "METHOD" + methodId);
                    string detail = InspectionDraft.GetField(draft, "DETAIL" + methodId);

                    if (detail != null)
                    {
                        tbDetail.Text = detail;
                    }

                    if (judge == "1")
                    {
                        rbOk.Checked = true;
                    }
                    else if (judge == "0")
                    {
                        rbNg.Checked = true;
                    }
                }

                if (draft.Tables.Contains(DraftLotNoTable) && draft.Tables[DraftLotNoTable].Rows.Count > 0)
                {
                    ShowLotNumbers(draft.Tables[DraftLotNoTable].Rows.Cast<DataRow>()
                        .Select(row => row["LOT_NO"].ToString())
                        .ToList());
                }
            }
            finally
            {
                _isRestoringDraft = false;
            }

        }

        private DataSet BuildDraft()
        {
            var draft = new DataSet("DRAFT");

            for (int methodId = 1; methodId <= 3; methodId++)
            {
                var (rbOk, rbNg, tbDetail, _) = GetMethodControls(methodId);
                InspectionDraft.SetField(draft, "METHOD" + methodId, rbOk.Checked ? "1" : rbNg.Checked ? "0" : null);
                InspectionDraft.SetField(draft, "DETAIL" + methodId, tbDetail.Text);
            }

            DataTable packingSize = draft.Tables.Add(DraftPackingSizeTable);
            packingSize.Columns.Add("VALUE", typeof(string));
            packingSize.Columns.Add("PACK_COUNT", typeof(string));
            foreach (DataGridViewRow row in GetEnteredPackingSizeRows())
            {
                packingSize.Rows.Add(row.Cells["VALUE"].Value?.ToString(), row.Cells["PACK_COUNT"].Value?.ToString());
            }

            DataTable lotNumbers = draft.Tables.Add(DraftLotNoTable);
            lotNumbers.Columns.Add("LOT_NO", typeof(string));
            for (int i = 0; dtg_lot_no.Rows.Count > 0 && i < dtg_lot_no.Columns.Count; i++)
            {
                string lotNo = dtg_lot_no[i, 0].Value?.ToString();
                if (!string.IsNullOrWhiteSpace(lotNo))
                {
                    lotNumbers.Rows.Add(lotNo);
                }
            }

            return draft;
        }

        private bool SaveDraft(bool showMessage)
        {
            DataSet draft = BuildDraft();

            if (!InspectionDraft.Save(conQA, propQA, draft, employee.EMP_CODE))
            {
                return false;
            }

            _recordedDraft = propQA.DRAFT_DATA;
            UpdateRecordButtonMode();

            if (showMessage)
            {
                string problem = FindPackingProblem();

                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Save ข้อมูลที่กรอกไว้แล้ว กลับมาทำต่อได้" + (problem == null
                        ? "\nข้อมูลครบแล้ว กด " + InspectionDraft.ContinueText + " เมื่อพร้อมส่งผล"
                        : "\n" + problem),
                    "Save",
                    CustomMsgBoxBase.MessageBoxIconType.OK);
            }

            return true;
        }

        // กดเมนูอื่น / Logout โดยไม่ผ่านปุ่มย้อนกลับ : NavigationService เรียกก่อน Dispose หน้านี้ (IInspectionScreen)
        // ทำแบบเดียวกับ bt_back_Click คือตั้ง Not Finished yet + ปลดล็อกใบ
        // ไม่เก็บร่างให้ ค่าที่ยังไม่ได้กด Record Data จะทิ้งไป (user เลือกให้เก็บเฉพาะตอนกด Record Data)
        public void LeaveScreen()
        {
            // Admin ด้วย : ReleaseReport แตะเฉพาะใบที่ยัง Working และล็อกเป็นของเครื่องนี้ ใบที่ Admin เปิดมาตัดสิน NG (Pending) ไม่โดน
            InspectionDraft.ReleaseReport(conQA, propQA);
        }

        // ออกจากหน้า (ย้อนกลับ / Home / Logout) : ค่าบนจอไม่ตรงกับตอนเปิดหน้าหรือตอนกด Record Data ครั้งล่าสุด = ถามก่อน
        // false = ผู้ตรวจกด Cancel อยู่หน้าเดิม ค่าที่กรอกยังอยู่บนจอ
        // Admin ที่เปิดมาตัดสินงาน NG ไม่ได้ใช้ร่าง (_recordedDraft ว่าง) ออกได้เลยเหมือนเดิม
        public bool ConfirmLeave()
        {
            if (!_draftEnabled || _recordedDraft == null) return true;

            // ช่องที่กำลังพิมพ์อยู่ให้ลงตารางก่อน handler เดิมจะได้คิด CALVALUE / ตัวพิมพ์ใหญ่ Lot No. ให้
            dtg_packing_size.EndEdit();
            dtg_lot_no.EndEdit();

            // ช่องที่ยัง commit ไม่ได้ (ค่าไม่ผ่าน CellValidating) ก็คือพิมพ์ค้างไว้
            bool unrecorded = dtg_packing_size.IsCurrentCellDirty || dtg_lot_no.IsCurrentCellDirty
                || InspectionDraft.Serialize(BuildDraft()) != _recordedDraft;

            return !unrecorded || InspectionDraft.ConfirmLeaveUnrecorded(this, "Packing");
        }

        private void UpdateRecordButtonMode()
        {
            InspectionDraft.ApplySplitButtons(bt_save, bt_saveContinue, FindPackingProblem() == null);
        }

        /// <summary>
        /// เงื่อนไขเดียวกับ ValidateAllData + UpdateStatusBasedOnJudgments แต่ไม่เด้งข้อความ
        /// ใช้ตัดสินว่าปุ่มเป็น Record Data หรือ Save and Continue และบอกผู้ตรวจหลัง Save ว่ายังขาดอะไร
        /// ตอนกด Save and Continue ตัวตรวจเดิมยังทำงานครบทุกข้อ ถ้าตรงนี้หลุดอะไรไป ตัวเดิมยังกันไว้
        /// คืน null ถ้าพร้อมบันทึกผลแล้ว
        /// </summary>
        private string FindPackingProblem()
        {
            bool allOk = rb_ok_method1.Checked && rb_ok_method2.Checked && rb_ok_method3.Checked;
            bool someNg = rb_ng_method1.Checked || rb_ng_method2.Checked || rb_ng_method3.Checked;

            // บันทึกได้ 2 แบบ : OK ครบ 3 ข้อ หรือมีข้อที่ NG (ข้อถัดจาก NG ไม่ต้องตรวจต่อ)
            if (!allOk && !someNg)
            {
                return "ยังเลือก OK / NG ไม่ครบ";
            }

            for (int methodId = 1; methodId <= 3; methodId++)
            {
                var (_, rbNg, tbDetail, _) = GetMethodControls(methodId);
                if (rbNg.Checked && string.IsNullOrWhiteSpace(tbDetail.Text))
                {
                    return $"ข้อ {methodId} เลือก NG แต่ยังไม่ได้กรอกรายละเอียด";
                }
            }

            if (rb_ng_method1.Checked && (rb_ok_method2.Checked || rb_ok_method3.Checked))
            {
                return "ข้อ 1 เป็น NG แล้ว ข้อ 2-3 เลือก OK ไม่ได้";
            }

            if (rb_ng_method2.Checked && rb_ok_method3.Checked)
            {
                return "ข้อ 2 เป็น NG แล้ว ข้อ 3 เลือก OK ไม่ได้";
            }

            if (rb_ok_method3.Checked)
            {
                if (!HasCompletePackingSizeRows())
                {
                    return "ยังกรอก Packing Size ไม่ครบ";
                }

                int totalValue = GetPackingSizeTotal();
                if (!int.TryParse(propQA.Qty, out int lotSize) || totalValue != lotSize)
                {
                    return $"ผลรวม Packing Size {totalValue} ยังไม่เท่ากับ Lot Size ({propQA.Qty})";
                }
            }

            if (rb_ok_method3.Checked || rb_ng_method3.Checked)
            {
                return FindLotNoProblem();
            }

            return null;
        }

        // ตาราง LOT NO Check เพิ่มช่องใหม่ให้ทุกครั้งที่กรอกช่องสุดท้าย และไม่มีที่ไหนบอกว่าใบนี้ต้องมีกี่ Lot
        // โปรแกรมจึงนับว่าครบตั้งแต่มี Lot แรก ให้ผู้ตรวจทวนรายการ Lot ในกล่องยืนยันของปุ่ม Save and Continue เองก่อนส่งผล
        // ยังไม่ครบก็กด Cancel แล้วกรอกต่อได้ (Cancel = อยู่หน้าเดิม ไม่บันทึก)
        private string BuildContinueQuestion()
        {
            const string Question = "ต้องการบันทึกผลและไปขั้นตอนถัดไปหรือไม่?";
            const int MaxShown = 20;

            List<string> lotNumbers = new List<string>();
            for (int i = 0; dtg_lot_no.Rows.Count > 0 && i < dtg_lot_no.Columns.Count; i++)
            {
                string lotNo = dtg_lot_no[i, 0].Value?.ToString()?.Trim();
                if (!string.IsNullOrWhiteSpace(lotNo))
                {
                    lotNumbers.Add(lotNo);
                }
            }

            // ข้อ 3 ไม่ได้ตรวจ (NG ตั้งแต่ข้อ 1-2) จะไม่มี Lot ให้ทวน
            if (lotNumbers.Count == 0)
            {
                return Question;
            }

            string shown = string.Join(", ", lotNumbers.Take(MaxShown));
            if (lotNumbers.Count > MaxShown)
            {
                shown += $" และอีก {lotNumbers.Count - MaxShown} Lot";
            }

            return $"LOT NO ที่จะบันทึก {lotNumbers.Count} Lot : {shown}{Environment.NewLine}{Question}";
        }

        // เงื่อนไข Lot No. เดียวกับ ValidateAllData : ช่องแรกต้องมีค่า และห้ามเว้นช่องว่างคั่นกลาง
        private string FindLotNoProblem()
        {
            if (dtg_lot_no.Rows.Count == 0 || dtg_lot_no.Columns.Count == 0
                || string.IsNullOrWhiteSpace(GetCellValueAfterEdit(dtg_lot_no, dtg_lot_no[0, 0])))
            {
                return "ยังไม่ได้กรอก Lot No.";
            }

            bool foundNonEmpty = false;
            for (int i = 0; i < dtg_lot_no.Columns.Count - 1; i++)
            {
                string cellValue = GetCellValueAfterEdit(dtg_lot_no, dtg_lot_no[i, 0]);
                if (string.IsNullOrWhiteSpace(cellValue) && foundNonEmpty)
                {
                    return $"Lot No. คอลัมน์ {dtg_lot_no.Columns[i].HeaderText} ว่างอยู่ แต่มีข้อมูลในคอลัมน์ถัดไป";
                }

                if (!string.IsNullOrWhiteSpace(cellValue))
                {
                    foundNonEmpty = true;
                }
            }

            return null;
        }

        private void LoadPackingCheckData()
        {
            if (conQA.CountMaxPackingCheck(propQA) > 0)
            {
                DataTable dt = conQA.PackingCheck(propQA);
                foreach (DataRow row in dt.Rows)
                {
                    string methodIdValue = row["Method_ID"].ToString();
                    string judgment = row["JUDGMENT"].ToString();
                    if (int.TryParse(methodIdValue, out int methodId))
                    {
                        originalMethodJudgments[methodId] = judgment;
                    }

                    switch (methodIdValue)
                    {
                        case "1":
                            UpdateMethodUI(1, judgment, row["DETAIL_JUDGE"]);
                            if (judgment == "1")
                            {
                                gb_method1.Enabled = false;
                            }
                            break;
                        case "2":
                            UpdateMethodUI(2, judgment, row["DETAIL_JUDGE"]);
                            if (judgment == "1")
                            {
                                gb_method2.Enabled = false;
                            }
                            break;
                        case "3":
                            rb_ok_method3.CheckedChanged -= rb_ok_method3_CheckedChanged;
                            UpdateMethodUI(3, judgment, row["DETAIL_JUDGE"]);
                            if (judgment == "1")
                            {
                                gb_method3.Enabled = false;
                                dtg_packing_size.RowValidating -= dtg_packing_size_RowValidating;
                                LoadPackingSizeData();
                                dtg_packing_size.RowValidating += dtg_packing_size_RowValidating;
                                dtg_packing_size.AllowUserToAddRows = false;
                            }
                            rb_ok_method3.CheckedChanged += rb_ok_method3_CheckedChanged;
                            break;
                    }
                }
            }
        }

        private void LoadReportLotNoData()
        {
            if (conQA.CountReportLotNo(propQA) > 0)
            {
                DataTable dt = conQA.ReportLot(propQA);
                List<string> lotNumbers = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string lotNo = row["LOT_NO"] != DBNull.Value ? row["LOT_NO"].ToString() : string.Empty;
                    if (!string.IsNullOrWhiteSpace(lotNo))
                    {
                        lotNumbers.Add(lotNo);
                    }
                }
                if (lotNumbers.Any())
                {
                    ShowLotNumbers(lotNumbers);
                }
                else
                {
                    dtg_lot_no.ColumnCount = 1;
                    dtg_lot_no.Columns[0].Name = "LOT_NO1";
                    dtg_lot_no.Rows.Add();
                }
            }
            else
            {
                dtg_lot_no.ColumnCount = 1;
                dtg_lot_no.Columns[0].Name = "LOT_NO1";
                dtg_lot_no.Rows.Add();
            }
        }

        // 1 คอลัมน์ต่อ 1 Lot แล้วเผื่อคอลัมน์ว่างท้ายสุดไว้ให้กรอก Lot ถัดไป
        private void ShowLotNumbers(List<string> lotNumbers)
        {
            dtg_lot_no.Columns.Clear();
            for (int i = 0; i < lotNumbers.Count; i++)
            {
                dtg_lot_no.Columns.Add("LOT_NO" + (i + 1), "LOT_NO" + (i + 1));
            }
            if (dtg_lot_no.Rows.Count == 0)
            {
                dtg_lot_no.Rows.Add();
            }
            for (int i = 0; i < lotNumbers.Count && i < dtg_lot_no.Columns.Count; i++)
            {
                dtg_lot_no[i, 0].Value = lotNumbers[i].ToUpper();
            }
            int newColumnIndex = dtg_lot_no.Columns.Count;
            dtg_lot_no.Columns.Add("LOT_NO" + (newColumnIndex + 1), "LOT_NO" + (newColumnIndex + 1));
        }

        private void UpdateMethodUI(int methodId, string judgment, object detailJudge)
        {
            var (rbOk, rbNg, tbDetail, lbLength) = GetMethodControls(methodId);
            tbDetail.Text = detailJudge != DBNull.Value ? detailJudge.ToString() : string.Empty;
            switch (judgment)
            {
                case "0":
                case "6":
                    rbNg.Checked = true;
                    break;
                case "1":
                    rbOk.Checked = true;
                    break;
            }
        }

        private (RadioButton rbOk, RadioButton rbNg, TextBox tbDetail, Label lbLength) GetMethodControls(int methodId)
        {
            switch (methodId)
            {
                case 1: return (rb_ok_method1, rb_ng_method1, tb_detail_method1, lb_length_detail_method1);
                case 2: return (rb_ok_method2, rb_ng_method2, tb_detail_method2, lb_length_detail_method2);
                case 3: return (rb_ok_method3, rb_ng_method3, tb_detail_method3, lb_length_detail_method3);
                default: throw new ArgumentException("Invalid method ID");
            }
        }

        private void LoadPackingSizeData()
        {
            DataTable dtPackingSize = conQA.PackingSize(propQA);
            dtg_packing_size.DataSource = null;
            dtg_packing_size.AutoGenerateColumns = false;
            foreach (DataRow rowPackingSize in dtPackingSize.Rows)
            {
                int value = Convert.ToInt32(rowPackingSize["VALUE"].ToString());
                int packCount = Convert.ToInt32(rowPackingSize["PACK_COUNT"].ToString());
                int calValue = value * packCount;
                dtg_packing_size.Rows.Add(rowPackingSize["VALUE"].ToString(), rowPackingSize["PACK_COUNT"].ToString(), calValue);
            }
        }

        private List<DataGridViewRow> GetEnteredPackingSizeRows()
        {
            return dtg_packing_size.Rows
                .Cast<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .ToList();
        }

        private bool HasCompletePackingSizeRows()
        {
            List<DataGridViewRow> rows = GetEnteredPackingSizeRows();
            return rows.Any() && rows.All(row =>
                HasPackingSizeCellValue(row, "VALUE") &&
                HasPackingSizeCellValue(row, "PACK_COUNT"));
        }

        private int GetPackingSizeTotal()
        {
            int totalValue = 0;
            foreach (DataGridViewRow row in GetEnteredPackingSizeRows())
            {
                int.TryParse(GetCellValueAfterEdit(dtg_packing_size, row.Cells["VALUE"]), out int value);
                int.TryParse(GetCellValueAfterEdit(dtg_packing_size, row.Cells["PACK_COUNT"]), out int pack);
                totalValue += value * pack;
            }

            return totalValue;
        }

        private bool ValidatePackingSizeInput()
        {
            if (!HasCompletePackingSizeRows())
            {
                dtg_packing_size.AllowUserToAddRows = true;
                MessageBox.Show("กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก OK กรุณากรอก Packing Size", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            int totalValue = GetPackingSizeTotal();
            if (!int.TryParse(propQA.Qty, out int lotSize))
            {
                MessageBox.Show($"Lot Size ไม่ถูกต้อง ({propQA.Qty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (totalValue != lotSize)
            {
                MessageBox.Show($"ผลรวม {totalValue} ไม่เท่ากับ LotSize ({lotSize})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool HasPackingSizeCellValue(DataGridViewRow row, string columnName)
        {
            return !string.IsNullOrWhiteSpace(GetCellValueAfterEdit(dtg_packing_size, row.Cells[columnName]));
        }

        // ค่าที่ช่องนี้จะมีหลังกด Enter : ช่องที่กำลังพิมพ์อยู่ใช้ค่าที่พิมพ์ ปุ่ม Save / Save and Continue จะได้เปลี่ยนทันที
        // ตอนกดปุ่มบันทึก ตารางออกจากโหมดแก้ไขไปแล้ว จึงได้ค่าในเซลล์ตามเดิม
        private static string GetCellValueAfterEdit(DataGridView grid, DataGridViewCell cell)
        {
            return InspectionDraft.TryGetPendingEdit(grid, out DataGridViewCell editingCell, out string editingValue) && cell == editingCell
                ? editingValue
                : cell.Value?.ToString();
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            var parentForm = this.FindForm() as frmMain;
            tb_detail_method1.Text = tb_detail_method1.Text.Trim();
            tb_detail_method2.Text = tb_detail_method2.Text.Trim();
            tb_detail_method3.Text = tb_detail_method3.Text.Trim();
            propQA.process = "Packing_Check";
            propQA.EMP_ID = employee.EMP_CODE;

            if (IsAdminPendingReview())
            {
                List<int> approvedMethodIds = GetAdminApprovedMethodIds();
                if (approvedMethodIds.Count == 0)
                {
                    MessageBox.Show(
                        "กรุณาเลือกกล่องที่ต้องการเปลี่ยนสถานะจาก NG เป็น OK อย่างน้อย 1 กล่อง",
                        "ข้อมูล",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (!SaveAdminPartialApproval(approvedMethodIds))
                {
                    return;
                }

                parentForm?.LoadStatus();
                NavigateBasedOnStatus();
                return;
            }

            // Record Data : เก็บร่างอย่างเดียว ไม่ตัดสินผล ไม่เปลี่ยนสถานะใบ อยู่หน้าเดิม
            // ส่งผลใช้ปุ่ม Save and Continue ข้าง ๆ (bt_saveContinue_Click)
            SaveDraft(true);
        }

        // Save and Continue : แยกเป็นอีกปุ่มจาก Record Data (user ขอ 2026-09-30)
        // Lot No. เพิ่มได้เรื่อย ๆ โปรแกรมรู้ไม่ได้ว่าผู้ตรวจกรอก Lot ครบหรือยัง
        // เดิมใช้ปุ่มเดียวที่สลับเองตามความครบ พอข้ออื่นครบปุ่มก็กลายเป็น Save and Continue ทั้งที่ยังพิมพ์ Lot ไม่เสร็จ
        private void bt_saveContinue_Click(object sender, EventArgs e)
        {
            var parentForm = this.FindForm() as frmMain;
            tb_detail_method1.Text = tb_detail_method1.Text.Trim();
            tb_detail_method2.Text = tb_detail_method2.Text.Trim();
            tb_detail_method3.Text = tb_detail_method3.Text.Trim();
            propQA.process = "Packing_Check";
            propQA.EMP_ID = employee.EMP_CODE;

            // ยังไม่ครบ (ปุ่มเป็นสีเทา) : ไม่บันทึกอะไร บอกว่ายังขาดอะไร
            string problem = FindPackingProblem();
            if (problem != null)
            {
                InspectionDraft.ShowNotComplete(problem);
                return;
            }

            // ถามก่อนส่งผล พร้อมรายการ Lot No. ที่จะบันทึก ให้ผู้ตรวจทวนว่ากรอก Lot ครบแล้วจริง
            // Cancel = อยู่หน้าเดิม ไม่บันทึกอะไร (จะเก็บไว้ทำต่อกด Record Data)
            if (!InspectionDraft.ConfirmContinue(this, "Packing", BuildContinueQuestion()))
            {
                return;
            }

            // เก็บร่างล่าสุดไว้ก่อนส่งผล ถ้าบันทึกพังกลางทางค่าที่กรอกยังอยู่
            SaveDraft(false);

            // Validation ข้อมูลทั้งหมดก่อน
            if (!ValidateAllData())
            {
                return; // หยุดการทำงาน ถ้า validation ไม่ผ่าน
            }

            if (!UpdateStatusBasedOnJudgments())
            {
                // ถ้า validation ใน UpdateStatusBasedOnJudgments ไม่ผ่าน
                // จะไม่มีการเปลี่ยนหน้า และไม่มีการบันทึก
                return;
            }

            // ผลจริงลง db_packing_check แล้ว ร่างไม่ต้องใช้อีก
            conQA.DeleteInspectionDraft(propQA);

            //if (rb_ng_method1.Checked)
            //{
            //    if (rb_ok_method2.Checked || rb_ok_method3.Checked)
            //    {
            //        // ล้างการเลือกของ Method 2 และ 3
            //        rb_ok_method2.Checked = false;
            //        rb_ng_method2.Checked = false;
            //        tb_detail_method2.Text = string.Empty;

            //        rb_ok_method3.Checked = false;
            //        rb_ng_method3.Checked = false;
            //        tb_detail_method3.Text = string.Empty;
            //        dtg_packing_size.Rows.Clear();
            //        dtg_packing_size.AllowUserToAddRows = true;

            //    }

            //    if (rb_ng_method2.Checked || rb_ng_method3.Checked)
            //    {
            //        // ล้างการเลือกของ Method 2 และ 3
            //        rb_ok_method2.Checked = false;
            //        rb_ng_method2.Checked = false;
            //        tb_detail_method2.Text = string.Empty;

            //        rb_ok_method3.Checked = false;
            //        rb_ng_method3.Checked = false;
            //        tb_detail_method3.Text = string.Empty;
            //        dtg_packing_size.Rows.Clear();
            //        dtg_packing_size.AllowUserToAddRows = true;

            //    }

            //}
            //if (rb_ng_method2.Checked && (rb_ok_method3.Checked || rb_ng_method3.Checked))
            //{
            //    // ล้างการเลือกของ Method 3

            //    rb_ok_method3.Checked = false;
            //    rb_ng_method3.Checked = false;
            //    tb_detail_method3.Text = string.Empty;
            //    dtg_packing_size.Rows.Clear();
            //    dtg_packing_size.AllowUserToAddRows = true;
            //}

            //if(rb_ok_method3.Checked == true)
            //{
            //    if (employee.EMP_LEVEL == "1")
            //    {
            //        dtg_packing_size.AllowUserToAddRows = false;

            //    }

            //    if (dtg_packing_size.Rows.Count == 0 || dtg_packing_size.Rows.Cast<DataGridViewRow>().Any(r => r.Cells.Cast<DataGridViewCell>().Any(c => c.Value == null || string.IsNullOrWhiteSpace(c.Value.ToString()))))
            //    {
            //        MessageBox.Show("กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก OK กรุณากรอก Packing Size", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }
            //}

            //if(rb_ok_method3.Checked == true || rb_ng_method3.Checked == true)
            //{

            //    string firstCellValue = dtg_lot_no[0, 0].Value?.ToString();

            //    List<string> lotNumbers = new List<string>();
            //    for (int i = 0; i < dtg_lot_no.Columns.Count; i++)
            //    {
            //        string cellValue = dtg_lot_no[i, 0].Value?.ToString();
            //        if (!string.IsNullOrWhiteSpace(cellValue))
            //        {
            //            lotNumbers.Add(cellValue);
            //        }
            //    }

            //    if (!lotNumbers.Any() || string.IsNullOrWhiteSpace(firstCellValue))
            //    {
            //        MessageBox.Show("กรุณากรอก Lot No. ให้ครบถ้วนก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return ;
            //    }
            //    bool foundNonEmpty = false;
            //    for (int i = 0; i < dtg_lot_no.Columns.Count - 1; i++)
            //    {
            //        string cellValue = dtg_lot_no[i, 0].Value?.ToString();
            //        if (string.IsNullOrWhiteSpace(cellValue) && foundNonEmpty)
            //        {
            //            MessageBox.Show($"คอลัมน์ {dtg_lot_no.Columns[i].HeaderText} ว่างอยู่ แต่มีข้อมูลในคอลัมน์ถัดไป!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //            return;
            //        }
            //        else if (!string.IsNullOrWhiteSpace(cellValue))
            //        {
            //            foundNonEmpty = true;
            //        }
            //    }
            //    //check dtg_packing_size check appearance_check_need = 1 
            //    //

            //}

            // เป็น false มา จะไม่มีการ update insert จะกลับไปสู่หน้า menu ไป operate , pending และ return โดยไม่มี msg ใน NavigateBasedOnStatus ขึ้นมา
            // เป็น true จะ ok ng อย่างถูกต้อง มี msg ใน NavigateBasedOnStatus ขึ้นมา 

            //if (!UpdateStatusBasedOnJudgments())
            //{
            //    //NavigateToPendingOrSelect();
            //    return;
            //}

            parentForm?.LoadStatus();
            NavigateBasedOnStatus();
        }

        private bool IsAdminPendingReview()
        {
            if (employee.EMP_LEVEL != "1")
            {
                return false;
            }

            return originalMethodJudgments.Any(item => item.Value == "0" || item.Value == "6");
        }

        private List<int> GetAdminApprovedMethodIds()
        {
            return originalMethodJudgments
                .Where(item =>
                    (item.Value == "0" || item.Value == "6") &&
                    GetMethodControls(item.Key).rbOk.Checked)
                .Select(item => item.Key)
                .ToList();
        }

        private bool SaveAdminPartialApproval(List<int> approvedMethodIds)
        {
            foreach (int methodId in approvedMethodIds)
            {
                var (_, _, tbDetail, _) = GetMethodControls(methodId);
                propQA.METHOD_ID = methodId.ToString();
                propQA.detail_Method = tbDetail.Text;
                propQA.judge = ((int)ProcStatus.OK).ToString();
                propQA.EMP_ID = employee.EMP_CODE;

                if (!conQA.InsertPackingCheck(propQA))
                {
                    MessageBox.Show(
                        $"เกิดข้อผิดพลาดในการบันทึกผล Packing Check Method {methodId}",
                        "ข้อผิดพลาด",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

            if (!conQA.UpdateStatus(propQA))
            {
                MessageBox.Show(
                    "เกิดข้อผิดพลาดในการส่งงานกลับให้พนักงานดำเนินการต่อ",
                    "ข้อผิดพลาด",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }

            conQA.DeleteReportActive(propQA);
            loadstatus();
            return true;
        }

        // เพิ่มฟังก์ชันสำหรับ validation ข้อมูลทั้งหมด
        private bool ValidateAllData()
        {
            // Validation สำหรับ Method 3 เมื่อเลือก OK
            if (rb_ok_method3.Checked == true)
            {
                if (!ValidatePackingSizeInput())
                {
                    return false;
                }
            }

            // Validation สำหรับ Lot No. (Method 3)
            if (rb_ok_method3.Checked == true || rb_ng_method3.Checked == true)
            {
                string firstCellValue = dtg_lot_no[0, 0].Value?.ToString();

                List<string> lotNumbers = new List<string>();
                for (int i = 0; i < dtg_lot_no.Columns.Count; i++)
                {
                    string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        lotNumbers.Add(cellValue);
                    }
                }

                if (!lotNumbers.Any() || string.IsNullOrWhiteSpace(firstCellValue))
                {
                    MessageBox.Show("กรุณากรอก Lot No. ให้ครบถ้วนก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                bool foundNonEmpty = false;
                for (int i = 0; i < dtg_lot_no.Columns.Count - 1; i++)
                {
                    string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(cellValue) && foundNonEmpty)
                    {
                        MessageBox.Show($"คอลัมน์ {dtg_lot_no.Columns[i].HeaderText} ว่างอยู่ แต่มีข้อมูลในคอลัมน์ถัดไป!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        foundNonEmpty = true;
                    }
                }
            }

            return true;
        }

        private bool SaveMethodData(int methodId)
        {
            var (rbOk, rbNg, tbDetail, _) = GetMethodControls(methodId);
            var gbMethod = GetGroupBox(methodId);
            propQA.METHOD_ID = methodId.ToString();
            propQA.detail_Method = tbDetail.Text;
            propQA.EMP_ID = employee.EMP_CODE;

            //if (!gbMethod.Enabled)
            //{
            //    return true; // ถ้า method ไม่ enable ให้ผ่านไป
            //}

            if (rbNg.Checked)
            {
                if (string.IsNullOrWhiteSpace(tbDetail.Text))
                {
                    MessageBox.Show($"กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก NG", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                propQA.judge = "6";
                if (employee.EMP_LEVEL == "1") propQA.judge = "0";
            }
            else if (rbOk.Checked)
            {
                propQA.judge = "1";
            }
            else
            {
                //MessageBox.Show($"กรุณาเลือก OK หรือ NG สำหรับ Method {methodId} ก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (propQA.judge != ((int)ProcStatus.Unfinished).ToString() && conQA.CountPackingCheck(propQA) == 0)
            {
                conQA.InsertPackingCheck(propQA);
            }
            return true;
        }

        private bool UpdateStatusBasedOnJudgments()
        {
            bool allOk = rb_ok_method1.Checked && rb_ok_method2.Checked && rb_ok_method3.Checked;
            bool someNg = rb_ng_method1.Checked || rb_ng_method2.Checked || rb_ng_method3.Checked;

            // Validation: ต้องเลือก OK หรือ NG อย่างน้อย 1 Method
            if (!allOk && !someNg)
            {
                MessageBox.Show("กรุณาเลือก OK หรือ NG อย่างน้อย 1 กล่อง", "ข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validation สำหรับ NG: ต้องกรอก detail
            if (someNg)
            {
                for (int methodId = 1; methodId <= 3; methodId++)
                {
                    var (rbOk, rbNg, tbDetail, _) = GetMethodControls(methodId);

                    if (rbNg.Checked && string.IsNullOrWhiteSpace(tbDetail.Text))
                    {
                        MessageBox.Show($"กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก NG",
                                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            // ถ้าเลือก NG บาง Method และ OK บาง Method ให้ตรวจสอบเงื่อนไขพิเศษ
            if (someNg && !allOk)
            {
                // ตรวจสอบว่า Method ที่เป็น NG มีผลต่อ Method ถัดไปหรือไม่
                if (rb_ng_method1.Checked && (rb_ok_method2.Checked || rb_ok_method3.Checked))
                {
                    //"กล่อง 1 เลือก NG ไม่สามารถเลือก OK ใน กล่อง 2 และ 3 ได้"
                    MessageBox.Show("เลือก NG ไม่สามารถเลือก OK",
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (rb_ng_method2.Checked && (rb_ok_method3.Checked))
                {
                    //กล่อง 2 เลือก NG ไม่สามารถเลือก OK ใน กล่อง 3 ได้
                    MessageBox.Show("เลือก NG ไม่สามารถเลือก OK",
                                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // Validation สำหรับ OK (ทุก Method ต้อง OK)
            if (allOk)
            {
                // Validation Packing Size
                if (!ValidatePackingSizeInput())
                {
                    return false;
                }

                // Validation Lot No.
                string firstCellValue = dtg_lot_no[0, 0].Value?.ToString();
                List<string> lotNumbers = new List<string>();
                for (int i = 0; i < dtg_lot_no.Columns.Count; i++)
                {
                    string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        lotNumbers.Add(cellValue);
                    }
                }

                if (!lotNumbers.Any() || string.IsNullOrWhiteSpace(firstCellValue))
                {
                    MessageBox.Show("กรุณากรอก Lot No. ให้ครบถ้วนก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                bool foundNonEmpty = false;
                for (int i = 0; i < dtg_lot_no.Columns.Count - 1; i++)
                {
                    string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(cellValue) && foundNonEmpty)
                    {
                        MessageBox.Show($"คอลัมน์ {dtg_lot_no.Columns[i].HeaderText} ว่างอยู่ แต่มีข้อมูลในคอลัมน์ถัดไป!",
                                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        foundNonEmpty = true;
                    }
                }
            }

            // ถ้าผ่าน Validation ทั้งหมดแล้ว จึงดำเนินการบันทึกข้อมูล
            return PerformSaveOperations(allOk, someNg);
        }

        // แยกฟังก์ชันสำหรับการบันทึกข้อมูล
        private bool PerformSaveOperations(bool allOk, bool someNg)
        {
            try
            {
                if (someNg)
                {
                    // บันทึกข้อมูลสำหรับกรณี NG
                    for (int methodId = 1; methodId <= 3; methodId++)
                    {
                        var (rbOk, rbNg, tbDetail, _) = GetMethodControls(methodId);
                        propQA.METHOD_ID = methodId.ToString();
                        propQA.detail_Method = tbDetail.Text;
                        propQA.EMP_ID = employee.EMP_CODE;

                        if (rbNg.Checked)
                        {
                            propQA.judge = "0";
                            propQA.inProcStatus = (employee.EMP_LEVEL == "1") ? "0" : "6";
                            propQA.reportStatus = (employee.EMP_LEVEL == "1") ? "0" : "6";
                        }
                        else if (rbOk.Checked)
                        {
                            propQA.judge = "1";
                            propQA.inProcStatus = "8";
                            propQA.reportStatus = "8";
                        }
                        else
                        {
                            continue;
                        }

                        if (conQA.CountPackingCheck(propQA) == 0)
                        {
                            if (!conQA.InsertPackingCheck(propQA))
                            {
                                MessageBox.Show("เกิดข้อผิดพลาดในการเพิ่มข้อมูล packing check", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }

                        if (methodId == 3)
                        {
                            // บันทึก Lot No.
                            DataTable dtLotNo = new DataTable();
                            dtLotNo.Columns.Add("LOT_NO", typeof(string));
                            for (int i = 0; i < dtg_lot_no.Columns.Count; i++)
                            {
                                string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                                if (!string.IsNullOrWhiteSpace(cellValue))
                                {
                                    dtLotNo.Rows.Add(cellValue);
                                }
                            }

                            propQA.dtLotNo = dtLotNo;

                            if (propQA.dtLotNo.Rows.Count >= 1)
                            {
                                if (!conQA.InsertReportLotNoList(propQA))
                                {
                                    MessageBox.Show("เกิดข้อผิดพลาดในการเพิ่มข้อมูล Lot No", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                            }
                        }
                    }
                }
                else if (allOk)
                {
                    string firstCellValue = dtg_lot_no[0, 0].Value?.ToString();

                    List<string> lotNumbers = new List<string>();
                    for (int i = 0; i < dtg_lot_no.Columns.Count; i++)
                    {
                        string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            lotNumbers.Add(cellValue);
                        }
                    }

                    if (!lotNumbers.Any() || string.IsNullOrWhiteSpace(firstCellValue))
                    {
                        MessageBox.Show("กรุณากรอก Lot No. ให้ครบถ้วนก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    bool foundNonEmpty = false;
                    for (int i = 0; i < dtg_lot_no.Columns.Count - 1; i++)
                    {
                        string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                        if (string.IsNullOrWhiteSpace(cellValue) && foundNonEmpty)
                        {
                            MessageBox.Show($"คอลัมน์ {dtg_lot_no.Columns[i].HeaderText} ว่างอยู่ แต่มีข้อมูลในคอลัมน์ถัดไป!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                        else if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            foundNonEmpty = true;
                        }
                    }

                    DataTable dtLotNo = new DataTable();
                    dtLotNo.Columns.Add("LOT_NO", typeof(string));
                    for (int i = 0; i < dtg_lot_no.Columns.Count; i++)
                    {
                        string cellValue = dtg_lot_no[i, 0].Value?.ToString();
                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            dtLotNo.Rows.Add(cellValue);
                        }
                    }

                    propQA.dtLotNo = dtLotNo;

                    if (!ValidatePackingSizeInput())
                    {
                        return false;
                    }

                    propQA.dtgPackingSize = dtg_packing_size;

                    int makeApp = conQA.NeedAppearCheck(propQA);

                    if (makeApp == 1)
                    {
                        // 1 . จำนวนงานเข้า AppearSampQtyLotSize

                        //propQA.dtDimSamp = conQA.DimensionSampling(propQA);
                        propQA.dtAppSamp = conQA.AppearSampling(propQA);

                        if (propQA.dtAppSamp == null)
                        {
                            MessageBox.Show("เกิดข้อผิดพลาดไม่พบ Appearance Sampling เพื่อจำนำไป คำนวณ packing size", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        else
                        {
                            propQA.SAMPLING_TYPE = propQA.dtAppSamp.Rows[0]["sampling_type"].ToString();
                            propQA.SAMPLING_NAME = propQA.dtAppSamp.Rows[0]["sampling_type_name"].ToString().Trim();
                            propQA.CAVITY_QTY = propQA.dtAppSamp.Rows[0]["Cavity_Qty"].ToString();
                            propQA.SAMPLING_QTY = propQA.dtAppSamp.Rows[0]["Sampling_Qty"].ToString();

                            propQA.Packing_Size_Cal_List = new List<string>();

                            int intSelect = 0;
                            int intPackingSize = 0;
                            int intPackingSizeAll = 0;

                            List<DataGridViewRow> packingSizeRows = GetEnteredPackingSizeRows();
                            int rowCount = packingSizeRows.Count;

                            if (rowCount == 0)
                            {
                                MessageBox.Show("ไม่มี แถว ใน ตาราง Packing Size", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            if (propQA.SAMPLING_TYPE == "1")
                            {

                                // Copy VALUE จากแต่ละ row ใน grid (user input ไว้ล่วงหน้า)
                                foreach (DataGridViewRow row in packingSizeRows)
                                {
                                    intPackingSize = Convert.ToInt32(row.Cells["VALUE"].Value?.ToString() ?? "0");
                                    propQA.Packing_Size_Cal_List.Add(intPackingSize.ToString());
                                }

                                propQA.inspQty = propQA.Qty;
                            }
                            else if (propQA.SAMPLING_TYPE == "3")
                            {

                                //dtg_packing_size.Rows.Count
                                foreach (DataGridViewRow row in packingSizeRows)
                                {
                                    propQA.VALUE = row.Cells["VALUE"].Value.ToString();

                                    DataTable dtSampLot = new DataTable();
                                    dtSampLot = conQA.AppearSampQtyLotSize(propQA);

                                    // ตรวจสอบว่า DataTable มีข้อมูล
                                    if (dtSampLot.Rows.Count == 0)
                                    {
                                        MessageBox.Show("No data returned from Appearance Sampling Qty Lot Size");
                                        return false;
                                    }

                                    int strictCal = Convert.ToInt32(dtSampLot.Rows[0]["Sampling_Qty"].ToString());
                                    int cavityCal = Convert.ToInt32(propQA.CAVITY_QTY) * Convert.ToInt32(propQA.SAMPLING_QTY);

                                    if (strictCal > cavityCal)
                                    {
                                        intSelect = strictCal;
                                    }
                                    else
                                    {
                                        intSelect = cavityCal;
                                    }

                                    //packing_size intSelect * Convert.ToInt32(row.Cells["PACK_COUNT"].Value.ToString());
                                    //
                                    intPackingSize = intSelect * Convert.ToInt32(row.Cells["PACK_COUNT"].Value.ToString());

                                    propQA.Packing_Size_Cal_List.Add(intPackingSize.ToString());


                                    //packing_size_all 
                                    intPackingSizeAll += intPackingSize;

                                }

                                //วนบวกSampling_Qty
                                propQA.inspQty = intPackingSizeAll.ToString();
                            }
                            else if (propQA.SAMPLING_TYPE == "5")
                            {
                                if (!string.IsNullOrWhiteSpace(propQA.SAMPLING_QTY) || propQA.SAMPLING_QTY == "0")
                                {
                                    MessageBox.Show("เกิดข้อผิดพลาดไม่พบ Appearance Sampling เพื่อจำนำไป คำนวณ เป็น % ต้องมีค่ามากกว่า 0 ", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return false;
                                }
                                else
                                {
                                    double result = Convert.ToInt32(propQA.Qty) * (Convert.ToInt32(propQA.SAMPLING_QTY) / 100.0);
                                    int totalInspQty = (int)Math.Ceiling(result);  // ceil เป็น integer แล้ว (ปัดขึ้นโดยรวม)
                                    propQA.inspQty = totalInspQty.ToString();

                                    // แบ่ง totalInspQty เท่า ๆ กัน + distribute เศษ (ให้ sum == totalInspQty เสมอ, เป็นจำนวนเต็มทั้งหมด)
                                    int mean = totalInspQty / rowCount;
                                    int remainder = totalInspQty % rowCount;

                                    int current = 0;
                                    foreach (DataGridViewRow row in packingSizeRows)
                                    {
                                        // ไม่ต้อง set VALUE ถ้าไม่ใช้
                                        intPackingSize = mean + (current < remainder ? 1 : 0);  // row แรก ๆ ได้ +1 ถ้ามีเศษ (ปัดขึ้นให้ครบ)
                                        current++;
                                        propQA.Packing_Size_Cal_List.Add(intPackingSize.ToString());
                                    }
                                }

                            }
                            else
                            {
                                MessageBox.Show("เกิดข้อผิดพลาดไม่พบ SAMPLING_TYPE เพื่อนำไปคำนวณ packing size", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }

                    }


                    propQA.inProcStatus = "1";
                    propQA.reportStatus = "1";

                    // ลูปทีละ method (1, 2, 3)
                    for (int methodId = 1; methodId <= 3; methodId++)
                    {
                        var (rbOk, rbNg, tbDetail, _) = GetMethodControls(methodId);
                        propQA.METHOD_ID = methodId.ToString();
                        propQA.detail_Method = tbDetail.Text;
                        propQA.EMP_ID = employee.EMP_CODE;

                        // เช็คว่าเลือก NG
                        if (rbNg.Checked)
                        {
                            propQA.judge = "0";
                            propQA.inProcStatus = (employee.EMP_LEVEL == "1") ? "0" : "6";
                            propQA.reportStatus = (employee.EMP_LEVEL == "1") ? "0" : "6";

                        }
                        // เช็คว่าเลือก OK
                        else if (rbOk.Checked)
                        {
                            propQA.judge = "1";
                            propQA.inProcStatus = "1";
                            propQA.reportStatus = "1";
                        }
                        else
                        {
                            // ยังไม่เลือกอะไร
                            continue;
                        }

                        // Insert ถ้ายังไม่มี
                        if (conQA.CountPackingCheck(propQA) == 0)
                        {
                            if (!conQA.InsertPackingCheck(propQA))
                            {
                                MessageBox.Show("เกิดข้อผิดพลาดในการเพิ่มข้อมูล packing check", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }
                        }
                    }



                    //insert packingSize ย่อย
                    if (conQA.CountPackingSize(propQA) == 0)
                    {

                        if (!conQA.InsertPackingSize(propQA))
                        {
                            MessageBox.Show("เกิดข้อผิดพลาดในการเพิ่มข้อมูล packing size", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                    }

                    if (makeApp == 1)
                    {
                        if (!conQA.UpdateInspQtyAppear(propQA))
                        {
                            MessageBox.Show("เกิดข้อผิดพลาดในการเพิ่มข้อมูล packing size สำหรับ appearance ", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }


                    if (propQA.dtLotNo.Rows.Count >= 1)
                    {
                        if (!conQA.InsertReportLotNoList(propQA))
                        {
                            MessageBox.Show("เกิดข้อผิดพลาดในการเพิ่มข้อมูล Lot No", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }


                }

                // อัปเดตสถานะ
                if (!conQA.UpdateStatus(propQA))
                {
                    MessageBox.Show("เกิดข้อผิดพลาดในการอัปเดตสถานะ!", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!conQA.DeleteReportActive(propQA))
                {
                    MessageBox.Show("ไม่สามารถคืนสถานะ report no ด้วย ip เครื่องนี้ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                loadstatus();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการบันทึกข้อมูล: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void NavigateBasedOnStatus()
        {
            ProcStatus status = Enum.TryParse(propQA.inProcStatus, out ProcStatus parsedStatus) ? parsedStatus : ProcStatus.NG;
            switch (status)
            {
                case ProcStatus.NG:
                    CustomMsgBoxBase.ShowCustomMessageBox("งาน Packing Check มีข้อผิดพลาด NG", "ข้อมูล", CustomMsgBoxBase.MessageBoxIconType.NG);
                    NavigateToPendingOrSelect();
                    break;
                case ProcStatus.Pending:
                    CustomMsgBoxBase.ShowCustomMessageBox("งานอยู่ในสถานะรอตรวจสอบ PENDING", "ข้อมูล", CustomMsgBoxBase.MessageBoxIconType.Pending);
                    NavigateToPendingOrSelect();
                    break;
                case ProcStatus.OK:
                    CustomMsgBoxBase.ShowCustomMessageBox("งาน Packing Check สำเร็จเรียบร้อย", "สำเร็จ", CustomMsgBoxBase.MessageBoxIconType.OK);
                    NavigateToPendingOrSelect();
                    break;
                case ProcStatus.Unfinished:
                    CustomMsgBoxBase.ShowCustomMessageBox("งาน Packing Check ยังไม่เสร็จ ให้พนักงานดำเนินการต่อ", "สำเร็จ", CustomMsgBoxBase.MessageBoxIconType.OK);
                    NavigateToPendingOrSelect();
                    break;
                case ProcStatus.Working:
                    //NavigateToPendingOrSelect();
                    break;
                default:
                    //CustomMsgBoxBase.ShowCustomMessageBox("สถานะไม่รู้จัก กรุณาตรวจสอบข้อมูล", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.Question);
                    CustomMsgBoxBase.ShowCustomMessageBox("สถานะไม่รู้จัก กรุณาตรวจสอบข้อมูล", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.Question);
                    break;
            }
        }

        private void NavigateToPendingOrSelect()
        {
            if (employee.EMP_LEVEL == "1")
            {
                GoToSelectPackingCheckPending();
            }
            else
            {
                GoToSelectPackingCheck();
            }
        }

        private void GoToSelectPackingCheck()
        {
            var selectPackingCheck = new userControlSelectPackingCheck()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty
                {
                    labelProcess = "Select Report for : Packing Check",
                    process = "Packing_Check",
                    prevProcess = "Receive_WH"
                }
            };
            DataTable dt = conQA.SearchForOpPackingCheck(selectPackingCheck.propQA);
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }
            selectPackingCheck.propQA.dtgRawMat = new DataGridView { DataSource = dt };
            var parentForm = this.FindForm() as frmMain;
            parentForm?.ControlBackLevel(employee);
            if (parentForm != null)
            {
                Control[] foundPanels = parentForm.Controls.Find("panelMain", true);
                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(selectPackingCheck);
                    selectPackingCheck.BringToFront();
                }
            }
            AddUserControlRequested?.Invoke(selectPackingCheck);
        }

        private void GoToSelectPackingCheckPending()
        {
            userControlPackingCheckPending usrPackCheckPend = new userControlPackingCheckPending();
            usrPackCheckPend.Dock = DockStyle.Fill;
            usrPackCheckPend.propQA = propQA;

            var parentForm = this.FindForm() as frmMain;
            parentForm?.ControlBackLevel(employee);

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrPackCheckPend);
                    usrPackCheckPend.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            AddUserControlRequested?.Invoke(usrPackCheckPend);
        }


        //private void rb_ok_method1_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rb_ok_method1.Checked)
        //    {
        //        gb_method2.Enabled = true;
        //    }
        //    else
        //    {
        //        gb_method2.Enabled = false;
        //        ClearMethod2Data();
        //        gb_method3.Enabled = false;
        //        ClearMethod3Data();
        //    }
        //}

        //private void rb_ng_method1_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rb_ng_method1.Checked)
        //    {
        //        gb_method2.Enabled = false;
        //        ClearMethod2Data();
        //        gb_method3.Enabled = false;
        //        ClearMethod3Data();
        //    }
        //}

        //private void rb_ok_method2_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rb_ok_method2.Checked)
        //    {
        //        gb_method3.Enabled = true;
        //    }
        //    else
        //    {
        //        gb_method3.Enabled = false;
        //        ClearMethod3Data();
        //    }
        //}

        //private void rb_ng_method2_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (rb_ng_method2.Checked)
        //    {
        //        gb_method3.Enabled = false;
        //        ClearMethod3Data();
        //    }
        //}

        //private void ClearMethod2Data()
        //{
        //    rb_ok_method2.Checked = false;
        //    rb_ng_method2.Checked = false;
        //    tb_detail_method2.Text = string.Empty;
        //}

        //private void ClearMethod3Data()
        //{
        //    rb_ok_method3.Checked = false;
        //    rb_ng_method3.Checked = false;
        //    tb_detail_method3.Text = string.Empty;
        //    dtg_packing_size.Rows.Clear();
        //    dtg_packing_size.AllowUserToAddRows = true;
        //}

        private GroupBox GetGroupBox(int methodId)
        {
            switch (methodId)
            {
                case 1: return gb_method1;
                case 2: return gb_method2;
                case 3: return gb_method3;
                default: throw new ArgumentException("Invalid method ID");
            }
        }

        private void tb_detail_method1_TextChanged(object sender, EventArgs e)
        {
            lb_length_detail_method1.Text = $"{tb_detail_method1.Text.Length} /255";
            if (tb_detail_method1.Text.Length > 255)
            {
                MessageBox.Show("ข้อความห้ามเกิน 255 อักขระ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_detail_method1.Text = tb_detail_method1.Text.Substring(0, 255);
                tb_detail_method1.SelectionStart = tb_detail_method1.Text.Length;
            }
        }

        private void tb_detail_method2_TextChanged(object sender, EventArgs e)
        {
            lb_length_detail_method2.Text = $"{tb_detail_method2.Text.Length} /255";
            if (tb_detail_method2.Text.Length > 255)
            {
                MessageBox.Show("ข้อความห้ามเกิน 255 อักขระ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_detail_method2.Text = tb_detail_method2.Text.Substring(0, 255);
                tb_detail_method2.SelectionStart = tb_detail_method2.Text.Length;
            }
        }

        private void tb_detail_method3_TextChanged(object sender, EventArgs e)
        {
            lb_length_detail_method3.Text = $"{tb_detail_method3.Text.Length} /255";
            if (tb_detail_method3.Text.Length > 255)
            {
                MessageBox.Show("ข้อความห้ามเกิน 255 อักขระ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_detail_method3.Text = tb_detail_method3.Text.Substring(0, 255);
                tb_detail_method3.SelectionStart = tb_detail_method3.Text.Length;
            }
        }

        private void dtg_packing_size_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ((dtg_packing_size.Columns["VALUE"]?.Index == e.ColumnIndex) ||
                (dtg_packing_size.Columns["PACK_COUNT"]?.Index == e.ColumnIndex))
            {
                UpdateCalculatedValues();
            }
        }

        private void UpdateCalculatedValues()
        {
            int totalValue = 0;
            foreach (DataGridViewRow row in dtg_packing_size.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["VALUE"].Value != null && Regex.IsMatch(row.Cells["VALUE"].Value.ToString(), "[^0-9]"))
                {
                    MessageBox.Show("กรุณากรอกเฉพาะตัวเลข", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["VALUE"].Value = Regex.Replace(row.Cells["VALUE"].Value.ToString(), "[^0-9]", "");
                    return;
                }
                if (row.Cells["PACK_COUNT"].Value != null && Regex.IsMatch(row.Cells["PACK_COUNT"].Value.ToString(), "[^0-9]"))
                {
                    MessageBox.Show("กรุณากรอกเฉพาะตัวเลข", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    row.Cells["PACK_COUNT"].Value = Regex.Replace(row.Cells["PACK_COUNT"].Value.ToString(), "[^0-9]", "");
                    return;
                }
                if (int.TryParse(row.Cells["VALUE"].Value?.ToString(), out int value) &&
                    int.TryParse(row.Cells["PACK_COUNT"].Value?.ToString(), out int pack))
                {
                    int calValue = value * pack;
                    row.Cells["CALVALUE"].Value = calValue.ToString();
                    totalValue += calValue;
                }
                else
                {
                    row.Cells["CALVALUE"].Value = "0";
                }
            }
            if (totalValue > UInt16.Parse(propQA.Qty))
            {
                MessageBox.Show($"ผลรวม {totalValue} เกิน LotSize ที่กำหนด ({propQA.Qty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtg_packing_size.BeginInvoke(new Action(() =>
                {
                    while (dtg_packing_size.Rows.Count > 1)
                    {
                        if (!dtg_packing_size.Rows[dtg_packing_size.Rows.Count - 1].IsNewRow)
                        {
                            dtg_packing_size.Rows.RemoveAt(dtg_packing_size.Rows.Count - 1);
                            dtg_packing_size.AllowUserToAddRows = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }));
            }
            else if (totalValue == UInt16.Parse(propQA.Qty))
            {
                dtg_packing_size.AllowUserToAddRows = false;
            }
            else
            {
                dtg_packing_size.AllowUserToAddRows = true;
            }
        }

        bool isHandlingCheckedChanged = false;

        private void rb_ok_method3_CheckedChanged(object sender, EventArgs e)
        {
            if (isHandlingCheckedChanged) return;
            isHandlingCheckedChanged = true;
            try
            {
                int totalValue = 0;
                foreach (DataGridViewRow row in dtg_packing_size.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (int.TryParse(row.Cells["CALVALUE"].Value?.ToString(), out int calValue))
                    {
                        totalValue += calValue;
                    }
                }
                // ต้องกรอก Packing Size ให้ผลรวมเท่ากับ Lot Size ก่อนถึงจะติ๊ก OK ได้ ทุกคนรวม Admin
                // ยกเว้น Admin ที่เปิดมาตัดสินงาน NG (อนุมัติข้อที่ผู้ตรวจให้ NG ไว้ ซึ่งอาจไม่ได้กรอกจำนวน)
                // เดิมยกเว้น Admin ทุกกรณี Admin ที่ตรวจงานเองเลยติ๊ก OK ได้ทั้งที่ตารางว่าง
                if (!IsAdminPendingReview() && totalValue != UInt16.Parse(propQA.Qty))
                {
                    rb_ok_method3.Checked = false;
                    MessageBox.Show($"ผลรวม {totalValue} ไม่เท่ากับ LotSize ({propQA.Qty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                isHandlingCheckedChanged = false;
            }
        }

        private void dtg_packing_size_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = dtg_packing_size.Rows[e.RowIndex];
            if (row.IsNewRow) return;
            var valueCell = row.Cells["VALUE"];
            var packCell = row.Cells["PACK_COUNT"];
            if (valueCell.Value == null || packCell.Value == null || string.IsNullOrWhiteSpace(valueCell.Value.ToString()) || string.IsNullOrWhiteSpace(packCell.Value.ToString()))
            {
                MessageBox.Show("กรุณากรอกข้อมูลให้ครบถ้วนใน Value และ Pack", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }
            if (!int.TryParse(valueCell.Value.ToString(), out int value) || !int.TryParse(packCell.Value.ToString(), out int pack))
            {
                MessageBox.Show("กรุณากรอกเฉพาะตัวเลขใน Value และ Pack", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }
            if (value == 0 || pack == 0)
            {
                MessageBox.Show("Value และ Pack ต้องไม่เป็น 0", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        public void bt_rec_pack_Click()
        {
            var usrControlSelectPackingCheck = new userControlSelectPackingCheck()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty
                {
                    labelProcess = "Select Report for : Packing Check",
                    process = "Packing_Check",
                    prevProcess = "Receive_WH"
                }
            };
            DataTable dt = conQA.SearchForOpPackingCheck(usrControlSelectPackingCheck.propQA);
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }
            usrControlSelectPackingCheck.propQA.dtgRawMat = new DataGridView { DataSource = dt };
            var parentForm = this.FindForm() as frmMain;
            parentForm?.ControlBackLevel(employee);
            if (parentForm != null)
            {
                Control[] foundPanels = parentForm.Controls.Find("panelMain", true);
                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrControlSelectPackingCheck);
                    usrControlSelectPackingCheck.BringToFront();
                }
            }
        }

        private void dtg_lot_no_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
            {
                textBox.KeyPress -= TextBox_KeyPress;
                textBox.KeyPress += TextBox_KeyPress;
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Back) return;
            if (!Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9\-_]+$"))
            {
                e.Handled = true;
                MessageBox.Show("กรุณากรอกเฉพาะตัวอักษรภาษาอังกฤษ, ตัวเลข, และเครื่องหมาย - หรือ _ เท่านั้น!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dtg_lot_no_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string cellValue = dtg_lot_no[e.ColumnIndex, e.RowIndex].Value?.ToString();
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                if (cellValue.Length > 50)
                {
                    MessageBox.Show("LOT_NO ห้ามยาวเกิน 50 ตัวอักษร!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_lot_no[e.ColumnIndex, e.RowIndex].Value = null;
                    return;
                }
                if (!Regex.IsMatch(cellValue, @"^[a-zA-Z0-9\-_]+$"))
                {
                    MessageBox.Show($"ข้อมูล '{cellValue}' มีอักขระที่ไม่ถูกต้อง!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_lot_no[e.ColumnIndex, e.RowIndex].Value = null;
                    return;
                }
                cellValue = cellValue.ToUpper();
                dtg_lot_no[e.ColumnIndex, e.RowIndex].Value = cellValue;
                for (int col = 0; col < dtg_lot_no.Columns.Count; col++)
                {
                    if (col != e.ColumnIndex)
                    {
                        string otherValue = dtg_lot_no[col, e.RowIndex].Value?.ToString();
                        if (otherValue != null && cellValue == otherValue.ToUpper())
                        {
                            MessageBox.Show($"ข้อมูล '{cellValue}' ซ้ำกับข้อมูลในคอลัมน์ {dtg_lot_no.Columns[col].HeaderText}!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            dtg_lot_no[e.ColumnIndex, e.RowIndex].Value = null;
                            return;
                        }
                    }
                }
                if (e.ColumnIndex == dtg_lot_no.Columns.Count - 1)
                {
                    int newColumnIndex = dtg_lot_no.Columns.Count;
                    dtg_lot_no.Columns.Add("LOT_NO" + (newColumnIndex + 1), "LOT_NO" + (newColumnIndex + 1));
                    BeginInvoke((MethodInvoker)delegate
                    {
                        dtg_lot_no.CurrentCell = dtg_lot_no[newColumnIndex, 0];
                    });
                }
            }
        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            // ไม่เก็บร่างให้เอง (เก็บเฉพาะตอนกด Record Data) แต่ถามก่อนถ้ามีค่าที่ยังไม่ได้ Record
            if (!ConfirmLeave()) return;

            if (employee.EMP_LEVEL != "1")
            {
                propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
                propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();
                if (!conQA.UpdateStatus(propQA))
                {
                    MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Unfinished ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!conQA.DeleteReportActive(propQA))
                {
                    MessageBox.Show("ไม่สามารถคืนสถานะ report no ด้วย ip เครื่องนี้ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else
            {
                // Admin ที่เข้าจากหน้าเลือกใบปกติก็ถูกตั้ง Working + ล็อกเหมือนพนักงาน (userControlSelectPackingCheck)
                // เดิมปุ่มนี้ข้าม Admin ไป ใบเลยค้าง Working ทั้งที่ไม่มีใครทำ (QA26-0005)
                // คืนเฉพาะใบที่ยัง Working และล็อกเป็นของเครื่องนี้ ใบที่เปิดมาตัดสิน NG จากหน้า Pending ไม่ได้ถูกตั้ง Working จึงไม่โดน
                InspectionDraft.ReleaseReport(conQA, propQA);
            }
            bt_rec_pack_Click();
        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter &&
                dtg_packing_size.ContainsFocus &&
                dtg_packing_size.CurrentCell != null &&
                !dtg_packing_size.CurrentCell.ReadOnly)
            {
                string columnName = dtg_packing_size.CurrentCell.OwningColumn.Name;
                if (columnName == "VALUE" || columnName == "PACK_COUNT")
                {
                    int currentRowIndex = dtg_packing_size.CurrentCell.RowIndex;

                    if (!dtg_packing_size.EndEdit())
                    {
                        return true;
                    }

                    BeginInvoke(new Action(() => MoveToNextPackingInput(currentRowIndex, columnName)));
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveToNextPackingInput(int currentRowIndex, string currentColumnName)
        {
            if (IsDisposed || dtg_packing_size.IsDisposed || !dtg_packing_size.IsHandleCreated)
            {
                return;
            }

            string targetColumnName = currentColumnName == "VALUE" ? "PACK_COUNT" : "VALUE";
            int targetRowIndex = currentColumnName == "VALUE" ? currentRowIndex : currentRowIndex + 1;

            if (!dtg_packing_size.Columns.Contains(targetColumnName) ||
                targetRowIndex < 0 ||
                targetRowIndex >= dtg_packing_size.Rows.Count)
            {
                return;
            }

            DataGridViewCell targetCell = dtg_packing_size.Rows[targetRowIndex].Cells[targetColumnName];
            if (!targetCell.Visible || targetCell.ReadOnly)
            {
                return;
            }

            dtg_packing_size.CurrentCell = targetCell;
            dtg_packing_size.Focus();
            dtg_packing_size.BeginEdit(true);
        }

    }
}


