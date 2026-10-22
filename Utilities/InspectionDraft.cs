using Bunifu.Framework.UI;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.ViewsMaterial.CustomMsg;
using RawMat.ViewsMaterial.Setting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    /// <summary>
    /// ร่างของหน้าตรวจที่ยังกรอกไม่ครบ เก็บที่ db_inspection_draft (DatabaseScripts/20260928_add_inspection_draft.sql)
    ///
    /// หน้าตรวจใช้ปุ่มเดียวสลับ 2 แบบ
    ///   ยังกรอกไม่ครบ -> Record Data       : เก็บค่าที่กรอกไว้เป็นร่าง ไม่ตัดสินผล ไม่เปลี่ยนสถานะใบ อยู่หน้าเดิม
    ///   กรอกครบแล้ว   -> Save and Continue : บันทึกผลจริงแบบเดิม แล้วลบร่างทิ้ง
    ///
    /// ใช้ร่วมกันทั้ง Packing / Regular / Function / Dimension
    /// </summary>
    internal static class InspectionDraft
    {
        public const string SaveText = "Record Data";
        public const string ContinueText = "Save and Continue";

        // สีต่างกันชัด ๆ ผู้ตรวจจะได้รู้ก่อนกดว่าปุ่มนี้แค่เก็บร่าง หรือส่งผลไปขั้นตอนถัดไป
        private static readonly Color SaveColor = Color.FromArgb(41, 128, 185);
        private static readonly Color SaveHoverColor = Color.FromArgb(31, 97, 141);
        private static readonly Color ContinueColor = Color.FromArgb(39, 174, 96);
        private static readonly Color ContinueHoverColor = Color.FromArgb(30, 132, 73);

        // ความกว้างตาม Designer ของแต่ละปุ่ม ใช้หดกลับตอนข้อความสั้นลง
        private static readonly ConditionalWeakTable<Control, object> DesignWidths = new ConditionalWeakTable<Control, object>();

        // ---------------------------------------------------------------------------------
        // ข้อความที่ผู้ตรวจเห็น : รวมไว้ที่เดียว ทุกหน้าจะได้พูดเหมือนกัน
        // ใช้คำที่เห็นบนหน้าจอ (ชื่อปุ่ม หัวคอลัมน์) ไม่ใช้ชื่อคอลัมน์ในฐานข้อมูล
        // ---------------------------------------------------------------------------------

        /// <summary>หลังกด Save : บอกว่าเก็บแล้ว และยังต้องกรอกอะไร (missing = null แปลว่าครบแล้วแต่ยังไม่ส่งผล)</summary>
        public static void ShowSaved(string missing)
        {
            string message = missing == null
                ? "บันทึกเรียบร้อยแล้ว\nข้อมูลครบแล้ว  \"" + ContinueText + "\""
                : "บันทึกเรียบร้อยแล้ว ข้อมูลยังไม่ครบ\n" + missing;

            ShowMessage(message, "บันทึกแล้ว", CustomMsgBoxBase.MessageBoxIconType.OK, CustomMsgBoxBase.MessageBoxDialogType.OK);
        }

        /// <summary>
        /// ก่อนส่งผลจริงตอนกด Save and Continue คืน true = ส่งผล
        /// ใช้ frmConfirm ไม่ใช้ CustomMsgBoxYesNo เพราะ frmConfirm ยืดความสูงตามข้อความ (FitToMessage)
        /// ส่วนกล่อง YES/NO ใช้ตัวอักษร 36pt ในกรอบคงที่ ข้อความ 2 บรรทัดก็โดนตัดแล้ว
        /// </summary>
        public static bool ConfirmContinue(IWin32Window owner, string inspectionName,
            string question = "ต้องการบันทึกผลและไปขั้นตอนถัดไปหรือไม่?")
        {
            using (var frm = new frmConfirm(
                $"กรอกข้อมูล {inspectionName} ครบแล้ว{Environment.NewLine}{question}",
                null,
                ContinueText))
            {
                return frm.ShowDialog(owner) == DialogResult.Yes;
            }
        }

        /// <summary>
        /// ออกจากหน้าตรวจ (ย้อนกลับ / Home / Logout) ตอนยังมีค่าที่ไม่ได้บันทึก
        /// คืน true = ออกจากหน้านี้ ค่าที่ยังไม่ได้บันทึกจะหาย / Cancel หรือปิดกล่อง = อยู่หน้าเดิม ค่าที่กรอกยังอยู่บนจอ
        /// </summary>
        public static bool ConfirmLeaveWithoutSave(IWin32Window owner, string message)
        {
            using (var frm = new frmConfirm(message, null, "ออกโดยไม่บันทึก"))
            {
                return frm.ShowDialog(owner) == DialogResult.Yes;
            }
        }

        /// <summary>ConfirmLeaveWithoutSave ของหน้าที่มีร่าง (Packing / Regular / Function / Dimension)</summary>
        public static bool ConfirmLeaveUnrecorded(IWin32Window owner, string inspectionName)
        {
            return ConfirmLeaveWithoutSave(owner,
                $"{inspectionName} มีข้อมูลที่กรอกไว้แต่ยังไม่ได้กด {SaveText}{Environment.NewLine}" +
                $"ถ้าออกตอนนี้ ข้อมูลที่กรอกไว้จะหาย{Environment.NewLine}" +
                $"(กด Cancel แล้วกด {SaveText} เพื่อเก็บไว้ทำต่อ)");
        }

        /// <summary>กรอบ Cavity ยังไม่ได้ยืนยัน ตารางกรอกผลเลยยังไม่ขึ้น</summary>
        public const string MissingCavityText = "กรอกจำนวนในกรอบ Cavity ให้ครบ แล้วกดปุ่ม CONFIRM CAVITY";

        // ---------------------------------------------------------------------------------
        // Lot No. ต้องเลือกก่อนเริ่มกรอก (Regular / Dimension / Function)
        // Packing เป็นคนสร้างรายการ Lot เองจึงไม่บังคับ ส่วน Appearance บังคับอยู่แล้วตอนกด Select pack
        // ---------------------------------------------------------------------------------

        public const string MissingLotNoText = "ยังไม่ได้เลือก Lot No. (ต้องเลือกก่อนกรอกข้อมูล)";
        public const string NoLotNoFromPackingText = "ไม่พบ Lot No. จาก Packing สำหรับ Report นี้\nต้องมี Lot No. ก่อนถึงจะกรอกข้อมูลได้";
        private static readonly Color LotNoMissingColor = Color.FromArgb(255, 228, 225);

        /// <summary>ใบละ Lot เดียว (combo บนหัวหน้าจอ) : มีรายการเดียวถือว่าเลือกแล้ว เพราะตอนเปิดหน้าเลือกให้เองอยู่แล้ว</summary>
        public static bool HasLotNo(ComboBox lotNo)
        {
            return lotNo.SelectedItem != null || lotNo.Items.Count == 1;
        }

        /// <summary>
        /// ใช้ตอนผู้ตรวจเริ่มแก้ช่องใด ๆ / กด CONFIRM CAVITY : ยังไม่ได้เลือก Lot = ห้ามเริ่ม เด้งบอกแล้วเปิดรายการ Lot ให้เลือก
        /// คืน true = เลือกแล้ว กรอกต่อได้
        /// </summary>
        public static bool EnsureLotNoSelected(ComboBox lotNo)
        {
            if (HasLotNo(lotNo)) return true;

            if (lotNo.Items.Count == 0)
            {
                MessageBox.Show(NoLotNoFromPackingText, "Lot No.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            MessageBox.Show("กรุณาเลือก Lot No. ก่อนกรอกข้อมูล", "Lot No.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            OpenDropDownLater(lotNo);
            return false;
        }

        /// <summary>ยังไม่ได้เลือก Lot : ระบายสีช่อง Lot ให้เห็นว่าต้องเลือกตรงนี้ก่อน</summary>
        public static void HighlightLotNo(ComboBox lotNo)
        {
            lotNo.BackColor = HasLotNo(lotNo) ? SystemColors.Window : LotNoMissingColor;
        }

        // เปิดรายการหลังกล่องข้อความปิดและ DataGridView จบ event ที่กำลังทำอยู่ก่อน
        private static void OpenDropDownLater(ComboBox lotNo)
        {
            if (!lotNo.IsHandleCreated || lotNo.IsDisposed) return;

            lotNo.BeginInvoke((MethodInvoker)(() =>
            {
                if (lotNo.IsDisposed || !lotNo.Visible || !lotNo.Enabled) return;
                lotNo.Focus();
                lotNo.DroppedDown = true;
            }));
        }

        /// <summary>
        /// บรรยายช่องว่างของตาราง Regular / Dimension ให้ผู้ตรวจหาเจอ เช่น
        /// "ยังว่างอีก 12 ช่อง เริ่มที่ หน้า 3/8 · SAMPLE 5 · CHECKPOINT 2 : ค่าที่วัด"
        /// ทั้งสองหน้าใช้ชื่อคอลัมน์ชุดเดียวกัน และโชว์ทีละ CHECKPOINT ต่อหน้า (POINT_ORDER)
        /// </summary>
        public static string DescribeMissingMeasurement(DataTable table, DataRow row, DataColumn column, int totalPages)
        {
            string where = $"หน้า {row["POINT_ORDER"]}/{totalPages}";

            if (table.Columns.Contains("CAVITY_NAME") && !string.IsNullOrWhiteSpace(row["CAVITY_NAME"]?.ToString()))
            {
                where += $" · CAV. {row["CAVITY_NAME"]}";
            }

            where += $" · SAMPLE {row["SAMPLING_NO"]}";

            if (table.Columns.Contains("POINT_NAME") && !string.IsNullOrWhiteSpace(row["POINT_NAME"]?.ToString()))
            {
                where += $" · CHECKPOINT {row["POINT_NAME"]}";
            }

            int remaining = CountEmpty(table, "VALUE") + CountEmpty(table, "EQUIPMENT_SERIAL");
            string prefix = remaining > 1 ? $"ยังว่างอีก {remaining} ช่อง เริ่มที่\n" : string.Empty;

            return prefix + where + " : " + DescribeMeasurementColumn(row, column.ColumnName);
        }

        private static string DescribeMeasurementColumn(DataRow row, string columnName)
        {
            switch (columnName)
            {
                case "VALUE":
                    return PointJudgeType.IsNumeric(row) ? "ค่าที่วัด" : "ผล OK / NG";
                case "EQUIPMENT_SERIAL":
                    return "EQ_SN (S/N เครื่องมือ)";
                case "POINT_JUDGE":
                case "TOTAL_JUDGE":
                    // ช่องกรอกมีค่าแล้ว แต่ตัดสินผลไม่ได้ = พิมพ์ตัวเลขผิดรูปแบบ
                    return "ค่าที่วัดไม่ใช่ตัวเลขที่ถูกต้อง กรุณากรอกใหม่";
                default:
                    return columnName;
            }
        }

        private static int CountEmpty(DataTable table, string columnName)
        {
            if (!table.Columns.Contains(columnName)) return 0;

            return table.Rows.Cast<DataRow>().Count(row =>
                row[columnName] == DBNull.Value || string.IsNullOrWhiteSpace(row[columnName].ToString()));
        }

        // CustomMsgBox มีกรอบข้อความขนาดคงที่ 485 x 177 (CustomMsgBoxBase.Designer.cs:45) ฟอนต์ 18pt
        // ข้อความ Save ที่บอกช่องที่ยังขาดยาวหลายบรรทัด ถ้าล้นกรอบจะโดนตัด
        // จึงหาขนาดฟอนต์ที่ใหญ่ที่สุดที่ข้อความใส่กรอบได้พอดี
        private static bool ShowMessage(string message, string title,
            CustomMsgBoxBase.MessageBoxIconType icon, CustomMsgBoxBase.MessageBoxDialogType dialogType)
        {
            return CustomMsgBoxBase.ShowCustomMessageBox(message, title, icon, dialogType, fontSize: FitFontSize(message));
        }

        private static float FitFontSize(string message)
        {
            var area = new Size(485, int.MaxValue);

            for (float size = 18f; size > 11f; size -= 1f)
            {
                using (var font = new Font("Tahoma", size))
                {
                    if (TextRenderer.MeasureText(message, font, area, TextFormatFlags.WordBreak).Height <= 170)
                    {
                        return size;
                    }
                }
            }

            return 11f;
        }

        // ---------------------------------------------------------------------------------
        // ให้ปุ่มเปลี่ยนตามค่าที่กำลังพิมพ์ ไม่ต้องรอกด Enter
        // DataGridView ยังไม่เขียนค่าลงเซลล์จนกว่าจะกด Enter / ออกจากช่อง (CellEndEdit)
        // จึงต้องอ่านค่าจากตัวแก้ไข (EditingControl) มาคิดแทนค่าในเซลล์ช่องนั้น
        // ---------------------------------------------------------------------------------

        /// <summary>ผูกใน EditingControlShowing : พิมพ์หรือเลือกค่าเมื่อไหร่ เรียก onChanged ทันที</summary>
        public static void WatchEditingControl(DataGridViewEditingControlShowingEventArgs e, EventHandler onChanged)
        {
            // DataGridView ใช้ตัวแก้ไขตัวเดิมซ้ำทุกช่อง ถอดก่อนผูก จะได้ไม่ถูกเรียกซ้ำหลายรอบ
            e.Control.TextChanged -= onChanged;
            e.Control.TextChanged += onChanged;

            if (e.Control is ComboBox combo)
            {
                combo.SelectedIndexChanged -= onChanged;
                combo.SelectedIndexChanged += onChanged;
            }
        }

        /// <summary>ช่องที่กำลังแก้ไขอยู่ และค่าในตัวแก้ไขตอนนี้ คืน false ถ้าไม่ได้อยู่ในโหมดแก้ไข</summary>
        public static bool TryGetPendingEdit(DataGridView grid, out DataGridViewCell cell, out string value)
        {
            cell = null;
            value = null;

            if (grid == null || !grid.IsCurrentCellInEditMode || grid.EditingControl == null || grid.CurrentCell == null)
            {
                return false;
            }

            cell = grid.CurrentCell;

            if (grid.EditingControl is ComboBox combo)
            {
                // ช่อง OK/NG เก็บค่าเป็น Key ("1"/"0") แต่โชว์ "OK"/"NG" จึงต้องอ่าน SelectedValue
                // ช่อง EQ_SN พิมพ์ S/N ที่ยังไม่อยู่ในรายการได้ ตอนนั้นไม่มีรายการที่เลือก ต้องอ่าน Text
                value = (combo.SelectedValue ?? combo.SelectedItem ?? combo.Text)?.ToString();
            }
            else
            {
                value = grid.EditingControl.Text;
            }

            return true;
        }

        public static void ApplyButtonMode(Control button, bool complete)
        {
            if (button == null) return;

            string text = complete ? ContinueText : SaveText;
            Color normal = complete ? ContinueColor : SaveColor;
            Color hover = complete ? ContinueHoverColor : SaveHoverColor;

            if (button is BunifuFlatButton flatButton)
            {
                // BunifuFlatButton วาดข้อความจาก ButtonText และคืนสีจาก Normalcolor ตอนเมาส์ออก
                flatButton.ButtonText = text;
                flatButton.Normalcolor = normal;
                flatButton.OnHovercolor = hover;
                flatButton.Activecolor = hover;

                // ไอคอนกินที่ด้านซ้ายราวความสูงปุ่ม ข้อความยาวอย่าง "Save and Continue" จะโดนตัด
                // Button ธรรมดาไม่ต้อง เพราะตัดขึ้นบรรทัดใหม่เองได้
                FitWidth(flatButton, text, flatButton.TextFont, flatButton.IconVisible ? flatButton.Height : 0);
            }
            else if (button is Button plainButton)
            {
                // FlatStyle.System (ปุ่ม SAVE ของ Packing) วาดตามธีม Windows ไม่สน BackColor เลย
                plainButton.FlatStyle = FlatStyle.Flat;
                plainButton.FlatAppearance.BorderSize = 0;
                plainButton.FlatAppearance.MouseOverBackColor = hover;
                plainButton.UseVisualStyleBackColor = false;
                plainButton.ForeColor = Color.White;
            }

            button.Text = text;

            // ปุ่มที่ถูกปิดใช้งานอยู่ (เช่น Appearance ตอนยังไม่เลือก pack) ต้องยังเป็นสีเทา
            // BunifuFlatButton เปลี่ยน BackColor ตาม Normalcolor ทันทีแม้ปิดใช้งาน จึงต้องตั้งกลับเอง
            // พอโค้ดเดิมเปิดใช้งานปุ่ม Bunifu จะคืนเป็น Normalcolor ที่ตั้งไว้ข้างบนให้เอง
            // ต้องอ่าน Enabled ผ่านชนิด BunifuFlatButton : มันประกาศ Enabled ของตัวเองทับ (new)
            // อ่านผ่าน Control จะได้ true เสมอ
            button.BackColor = button is BunifuFlatButton bunifuButton && !bunifuButton.Enabled
                ? bunifuButton.DisabledColor
                : normal;
        }

        // หน้าที่แยก 2 ปุ่ม : Save and Continue ที่ยังกรอกไม่ครบเป็นสีเทา แต่ยังกดได้ (กดแล้วบอกว่ายังขาดอะไร)
        private static readonly Color NotReadyColor = Color.FromArgb(149, 165, 166);
        private static readonly Color NotReadyHoverColor = Color.FromArgb(127, 140, 141);

        /// <summary>
        /// หน้าที่แยกปุ่ม Record Data กับ Save and Continue ออกจากกัน (Packing)
        /// Lot No. เพิ่มได้เรื่อย ๆ โปรแกรมรู้ไม่ได้ว่าผู้ตรวจกรอก Lot ครบหรือยัง ถ้าใช้ปุ่มเดียวที่สลับเอง
        /// พอข้ออื่นครบปุ่มจะกลายเป็น Save and Continue ทั้งที่ยังพิมพ์ Lot ไม่เสร็จ
        /// </summary>
        public static void ApplySplitButtons(Button record, Button saveAndContinue, bool complete)
        {
            ApplyButtonMode(record, false);
            ApplyButtonMode(saveAndContinue, true);

            if (!complete)
            {
                saveAndContinue.BackColor = NotReadyColor;
                saveAndContinue.FlatAppearance.MouseOverBackColor = NotReadyHoverColor;
            }
        }

        /// <summary>กด Save and Continue ตอนยังกรอกไม่ครบ (หน้าที่แยก 2 ปุ่ม) : ไม่บันทึกอะไร บอกว่ายังขาดอะไร</summary>
        public static void ShowNotComplete(string missing)
        {
            ShowMessage("ยังส่งผลไม่ได้ ข้อมูลยังไม่ครบ\n\nที่ยังต้องกรอก\n" + missing,
                "ข้อมูลยังไม่ครบ", CustomMsgBoxBase.MessageBoxIconType.Warning, CustomMsgBoxBase.MessageBoxDialogType.OK);
        }

        // ขยาย/หดออกทั้งสองข้าง ให้กึ่งกลางปุ่มอยู่ที่เดิมตาม Designer
        // ไม่หดให้เล็กกว่าขนาดใน Designer ปุ่ม Save จะได้หน้าตาเหมือนเดิม
        private static void FitWidth(Control button, string text, Font font, int iconWidth)
        {
            int designWidth = (int)DesignWidths.GetValue(button, b => b.Width);
            int neededWidth = TextRenderer.MeasureText(text, font ?? button.Font).Width + iconWidth + 24;
            int width = Math.Max(designWidth, neededWidth);

            if (width == button.Width) return;

            int center = button.Left + button.Width / 2;
            button.Width = width;
            button.Left = center - width / 2;
        }

        /// <summary>อ่านร่างของใบ + process ที่เปิดอยู่ (propQA.Report_No / propQA.process) คืน null ถ้ายังไม่เคย Save</summary>
        public static DataSet Load(QAdataControllers conQA, QAdataProperty propQA)
        {
            string draftXml = conQA.SearchInspectionDraft(propQA);
            if (string.IsNullOrWhiteSpace(draftXml)) return null;

            try
            {
                return Deserialize(draftXml);
            }
            catch (Exception ex)
            {
                MessageBox.Show("อ่านข้อมูลที่ Save ไว้ไม่ได้ : " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }

        /// <summary>
        /// ออกจากหน้าโดยไม่ผ่านปุ่มย้อนกลับ (IInspectionScreen.LeaveScreen) : ตั้ง Not Finished yet และปลดล็อกใบ แบบเดียวกับปุ่มย้อนกลับ
        /// ทำเฉพาะตอนใบยังเป็น Working และล็อกเป็นของเครื่องนี้ (หรือไม่มีล็อกแล้ว)
        /// ไม่งั้นจะไปทับใบที่บันทึกผลไปแล้ว ใบที่ถูกตั้ง Pending ระหว่างทำ หรือใบที่ Admin ปลดล็อกให้เครื่องอื่นเข้าไปทำต่อแล้ว
        /// </summary>
        public static void ReleaseReport(QAdataControllers conQA, QAdataProperty propQA)
        {
            string working = ((int)QAdataProperty.ProcStatus.Working).ToString();
            if (conQA.GetProcessStatusId(propQA) != working) return;

            DataTable active = conQA.SearchReportActive(propQA);
            string lockedBy = active != null && active.Rows.Count > 0 ? active.Rows[0]["Active_User"]?.ToString() : null;
            if (!string.IsNullOrWhiteSpace(lockedBy) && lockedBy != propQA.myIPv4) return;

            propQA.inProcStatus = ((int)QAdataProperty.ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)QAdataProperty.ProcStatus.Unfinished).ToString();
            conQA.UpdateStatus(propQA);

            if (!string.IsNullOrWhiteSpace(lockedBy))
            {
                conQA.DeleteReportActive(propQA);
            }
        }

        public static bool Save(QAdataControllers conQA, QAdataProperty propQA, DataSet draft, string empId)
        {
            propQA.EMP_ID = empId;
            propQA.DRAFT_DATA = Serialize(draft);
            return conQA.SaveInspectionDraft(propQA);
        }

        // ค่าเดี่ยวที่ไม่ได้อยู่ในตาราง เช่น Lot No. ที่เลือก, OK/NG ของ Packing แต่ละข้อ
        private const string FieldTable = "FIELDS";

        public static void SetField(DataSet draft, string name, string value)
        {
            DataTable fields = draft.Tables[FieldTable];
            if (fields == null)
            {
                fields = draft.Tables.Add(FieldTable);
                fields.Columns.Add("NAME", typeof(string));
                fields.Columns.Add("VALUE", typeof(string));
            }

            DataRow row = fields.Rows.Cast<DataRow>().FirstOrDefault(r => r["NAME"].ToString() == name);
            object stored = value == null ? (object)DBNull.Value : value;

            if (row == null)
            {
                fields.Rows.Add(name, stored);
            }
            else
            {
                row["VALUE"] = stored;
            }
        }

        public static string GetField(DataSet draft, string name)
        {
            DataTable fields = draft?.Tables[FieldTable];
            DataRow row = fields?.Rows.Cast<DataRow>().FirstOrDefault(r => r["NAME"].ToString() == name);

            return row == null || row["VALUE"] == DBNull.Value ? null : row["VALUE"].ToString();
        }

        public static string Serialize(DataSet draft)
        {
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                // ต้องมี schema ติดไปด้วย ไม่งั้นตอนอ่านกลับทุกคอลัมน์จะกลายเป็น string และแถวที่ค่าว่างทั้งแถวจะหาย
                draft.WriteXml(writer, XmlWriteMode.WriteSchema);
                return writer.ToString();
            }
        }

        public static DataSet Deserialize(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml)) return null;

            var draft = new DataSet();
            using (var reader = new StringReader(xml))
            {
                draft.ReadXml(reader, XmlReadMode.ReadSchema);
            }
            return draft;
        }

        /// <summary>
        /// คัดเฉพาะคอลัมน์ที่ต้องใช้ตอนเติมกลับ (key + ช่องที่ผู้ตรวจกรอก)
        /// เกณฑ์ MIN/MAX ชื่อจุด ฯลฯ ไม่ต้องเก็บ เพราะตอนเปิดหน้าใหม่สร้างจาก master อยู่แล้ว
        /// </summary>
        public static DataTable Snapshot(DataTable source, string tableName, params string[] columns)
        {
            string[] existing = columns.Where(source.Columns.Contains).ToArray();

            // DataView ใหม่ ไม่ใช่ DefaultView เพราะ DefaultView ถูก BindingSource กรองทีละหน้าอยู่
            return new DataView(source).ToTable(tableName, false, existing);
        }

        /// <summary>
        /// เติมค่าจากร่างลงตารางที่เพิ่งสร้างใหม่ โดยจับคู่ทีละแถวด้วย key
        /// ไม่เอาร่างมาแทนทั้งตาราง เพราะ master อาจถูกแก้ระหว่างนั้น (เพิ่ม/ลดจุดวัด จำนวน sample)
        /// แถวที่ไม่มีคู่จะถูกข้ามไป คืนจำนวนช่องที่เติมได้
        /// </summary>
        public static int Restore(DataTable target, DataTable draft, string[] keyColumns, string[] valueColumns)
        {
            if (target == null || draft == null) return 0;

            // ใช้เฉพาะ key ที่ตารางใหม่มี เช่น CAVITY_NAME มีเฉพาะใบที่แบ่งตาม cavity
            string[] keys = keyColumns.Where(target.Columns.Contains).ToArray();
            if (keys.Length == 0 || keys.Any(key => !draft.Columns.Contains(key))) return 0;

            var savedRows = new Dictionary<string, DataRow>();
            foreach (DataRow saved in draft.Rows)
            {
                savedRows[BuildKey(saved, keys)] = saved;
            }

            int restored = 0;
            foreach (DataRow row in target.Rows)
            {
                if (!savedRows.TryGetValue(BuildKey(row, keys), out DataRow saved)) continue;

                foreach (string column in valueColumns)
                {
                    if (!target.Columns.Contains(column) || !draft.Columns.Contains(column)) continue;

                    object value = saved[column];
                    if (value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString())) continue;

                    row[column] = value;
                    restored++;
                }
            }

            return restored;
        }

        /// <summary>
        /// Regular / Dimension : ค่าที่ช่องนี้จะมีหลังกด Enter
        /// ถ้ากำลังพิมพ์ช่องนี้อยู่ใช้ค่าที่พิมพ์ ไม่งั้นใช้ค่าในตาราง
        /// EQ_SN ถูกเติมให้ทุกแถวที่ใช้เครื่องมือชนิดเดียวกันตอนกด Enter (CellEndEdit ของทั้งสองหน้า) จึงนับรวมด้วย
        /// </summary>
        public static object GetValueAfterEdit(DataGridView grid, DataRow row, string columnName)
        {
            object current = row[columnName];

            if (!TryGetPendingEdit(grid, out DataGridViewCell cell, out string pending)) return current;
            if (grid.Columns[cell.ColumnIndex].Name != columnName) return current;

            DataRow editingRow = (cell.OwningRow?.DataBoundItem as DataRowView)?.Row;
            if (editingRow == null) return current;
            if (row == editingRow) return pending;

            bool sameEquipment = columnName == "EQUIPMENT_SERIAL"
                && !string.IsNullOrWhiteSpace(pending)
                && row.Table.Columns.Contains("EQUIPMENT_TYPE")
                && row["EQUIPMENT_TYPE"]?.ToString() == editingRow["EQUIPMENT_TYPE"]?.ToString();

            return sameEquipment ? pending : current;
        }

        private static string BuildKey(DataRow row, string[] keys)
        {
            // SAMPLING_NO เป็น int ในตารางหนึ่งแต่เป็น string ในอีกตารางได้ เทียบเป็นข้อความแทน
            return string.Join("\u001F", keys.Select(key =>
                (Convert.ToString(row[key], CultureInfo.InvariantCulture) ?? string.Empty).Trim().ToUpperInvariant()));
        }
    }
}
