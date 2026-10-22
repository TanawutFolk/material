using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    /// <summary>
    /// ทำให้ช่อง EQ_SN ในตารางตรวจเป็น Dropdown เลือก S/N ที่มีอยู่แล้ว
    ///
    /// เดิมช่องนี้เป็น TextBox ให้พิมพ์เอง แล้วตอน Save จะเอาสิ่งที่พิมพ์ไปสร้างแถวใหม่
    /// ใน info_equipment_serial ถ้ายังไม่มี — พิมพ์ผิดครั้งเดียวก็กลายเป็น master ถาวร
    ///
    /// ใช้ร่วมกันทั้ง Regular / Dimension / Function และหน้า Pending ของ Regular / Dimension
    /// เพราะทุกหน้าใช้ชื่อคอลัมน์ชุดเดียวกัน (EQUIPMENT_SERIAL / EQUIPMENT_TYPE)
    /// </summary>
    internal static class EquipmentSerialColumn
    {
        private const string SerialColumn = "EQUIPMENT_SERIAL";
        private const string TypeColumn = "EQUIPMENT_TYPE";

        /// <summary>
        /// ค่าที่ใส่ให้เองในช่อง EQ_SN ของเครื่องมือที่ไม่ได้ผูก S/N ไว้ เช่น Pin Gauge GO / NO GO
        ///
        /// ใช้ "-" ไม่ใช้ช่องว่าง เพราะ
        ///   1) info_equipment_serial ใช้ "-" แทน "ไม่มี S/N" อยู่แล้วหลายชนิด
        ///   2) ตอนบันทึก "-" จะได้ ID จริงเหมือน S/N ตัวอื่น
        ///      ถ้าปล่อยว่าง Regular / Dimension จะส่ง '' ลง EQUIPMENT_SERIAL_ID (smallint) แล้วโดน STRICT mode
        ///      และหัวบล็อก Equipment ของใบ B08 (B08DimensionEquipment) JOIN แบบ inner แถวที่ไม่มี S/N จะหายไปจากเอกสาร
        /// </summary>
        public const string NoSerialText = "-";

        // เก็บรายการไว้ให้ตอนคลิกแก้ไขหยิบไปกรองตามชนิดเครื่องมือของแถวนั้น
        private static DataTable _serials;

        /// <summary>
        /// เครื่องมือชนิดนี้มี S/N จริงให้เลือกไหม ("-" ไม่นับ เพราะแปลว่าไม่มี S/N)
        /// ไม่รู้ชนิด หรือยังไม่ได้โหลดรายการ ถือว่ามีไว้ก่อน จะได้ไม่ล็อกช่องผิด ๆ
        /// </summary>
        public static bool HasSerial(string equipmentType)
        {
            if (_serials == null || string.IsNullOrWhiteSpace(equipmentType)) return true;

            return _serials.Rows.Cast<DataRow>().Any(serial =>
                serial["EQUIPMENT_TYPE_ID"].ToString() == equipmentType.Trim() &&
                serial[SerialColumn].ToString().Trim() != NoSerialText);
        }

        // ช่องที่ไม่ต้องกรอก : เครื่องมือไม่มี S/N และช่องยังเป็น "-" หรือว่าง
        // ค่าอื่นที่บันทึกไว้แล้ว (เช่นใบเก่าในหน้า Pending) ยังแก้ได้ ไม่งั้นค่าที่ผิดจะแก้คืนไม่ได้เลย
        private static bool IsNoSerialCell(DataGridView grid, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count || !grid.Columns.Contains(TypeColumn)) return false;

            DataGridViewRow row = grid.Rows[rowIndex];
            if (row.IsNewRow || HasSerial(row.Cells[TypeColumn].Value?.ToString())) return false;

            string current = row.Cells[SerialColumn].Value?.ToString().Trim();
            return string.IsNullOrEmpty(current) || current == NoSerialText;
        }

        private static DataTable GetBoundTable(DataGridView grid)
        {
            object source = grid.DataSource;

            while (source is BindingSource bindingSource)
            {
                source = bindingSource.DataSource;
            }

            return source is DataView view ? view.Table : source as DataTable;
        }

        // เติม "-" ลงทุกแถวของตาราง ไม่ใช่แค่แถวที่เห็น เพราะ Regular / Dimension กรองโชว์ทีละหน้า
        private static void FillNoSerialRows(DataGridView grid)
        {
            DataTable table = GetBoundTable(grid);
            if (table == null || !table.Columns.Contains(SerialColumn) || !table.Columns.Contains(TypeColumn)) return;

            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                if (HasSerial(row[TypeColumn]?.ToString())) continue;
                if (!string.IsNullOrWhiteSpace(row[SerialColumn]?.ToString())) continue;

                row[SerialColumn] = NoSerialText;
            }
        }

        /// <summary>
        /// ค่าที่เติมกลับมาจากร่าง อาจเป็น S/N ที่ผู้ตรวจพิมพ์เพิ่มเองซึ่งยังไม่อยู่ใน master
        /// ถ้าไม่เติมเข้ารายการของคอลัมน์ ช่องจะโชว์ว่างทั้งที่มีค่า
        /// </summary>
        public static void AddMissingItems(DataGridView grid)
        {
            if (grid == null || !(grid.Columns[SerialColumn] is DataGridViewComboBoxColumn serialColumn)) return;

            DataTable table = GetBoundTable(grid);
            if (table == null || !table.Columns.Contains(SerialColumn)) return;

            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;

                string serial = row[SerialColumn]?.ToString();
                if (!string.IsNullOrWhiteSpace(serial) && !serialColumn.Items.Contains(serial))
                {
                    serialColumn.Items.Add(serial);
                }
            }
        }

        private static void BlockNoSerialEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex].Name != SerialColumn) return;

            if (IsNoSerialCell(grid, e.RowIndex))
            {
                e.Cancel = true;
            }
        }

        // ช่องที่ไม่ต้องกรอกให้เป็นสีเทา ไม่มีปุ่ม Dropdown ผู้ตรวจจะได้เห็นว่าข้ามได้
        private static void FormatNoSerialCell(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || grid.Columns[e.ColumnIndex].Name != SerialColumn) return;

            bool noSerial = IsNoSerialCell(grid, e.RowIndex);

            if (noSerial)
            {
                e.CellStyle.BackColor = Color.Gainsboro;
                e.CellStyle.SelectionBackColor = Color.Gainsboro;
                e.CellStyle.SelectionForeColor = Color.Black;
            }

            if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell comboCell)
            {
                DataGridViewComboBoxDisplayStyle style = noSerial
                    ? DataGridViewComboBoxDisplayStyle.Nothing
                    : DataGridViewComboBoxDisplayStyle.DropDownButton;

                // ตั้งเฉพาะตอนเปลี่ยน การตั้งค่าทำให้เซลล์วาดใหม่ ถ้าตั้งทุกครั้งจะวนไม่จบ
                if (comboCell.DisplayStyle != style)
                {
                    comboCell.DisplayStyle = style;
                }
            }
        }

        /// <summary>
        /// เปลี่ยนคอลัมน์ EQ_SN เป็น ComboBox
        ///
        /// ใส่ S/N ทั้งหมดไว้ที่ระดับคอลัมน์ก่อน เพื่อให้ค่าที่บันทึกไว้แล้วทุกค่าถือว่าถูกต้อง
        /// ไม่งั้น DataGridView จะฟ้อง "ค่าไม่อยู่ในรายการ" ตั้งแต่เปิดหน้าจอ
        /// ส่วนการกรองให้เหลือเฉพาะชนิดของแถวนั้นไปทำตอนคลิกแก้ไข (HandleEditingControlShowing)
        /// เพราะแถวในตารางเดียวกันใช้เครื่องมือคนละชนิดได้
        /// เช่น CAM008 จุด 1-3 ใช้ Caliper จุด 4-6 ใช้ Microscope
        /// </summary>
        public static void Apply(DataGridView grid, DataTable serials)
        {
            if (grid == null || !grid.Columns.Contains(SerialColumn)) return;
            if (serials == null || serials.Rows.Count == 0) return;

            _serials = serials;

            // เครื่องมือที่ไม่ได้ผูก S/N ไม่ต้องกรอก : เติม "-" ให้เองแล้วล็อกช่องไว้
            // ช่องมีค่าแล้ว ตัวเช็คช่องว่างก่อนบันทึกของทุกหน้าจึงผ่านเองโดยไม่ต้องแก้
            FillNoSerialRows(grid);
            grid.CellBeginEdit -= BlockNoSerialEdit;
            grid.CellBeginEdit += BlockNoSerialEdit;
            grid.CellFormatting -= FormatNoSerialCell;
            grid.CellFormatting += FormatNoSerialCell;

            // เรียกซ้ำได้ ถ้าเปลี่ยนเป็น ComboBox ไปแล้วก็ไม่ต้องทำใหม่
            if (grid.Columns[SerialColumn] is DataGridViewComboBoxColumn) return;

            int index = grid.Columns[SerialColumn].Index;
            bool readOnly = grid.Columns[SerialColumn].ReadOnly;

            var comboColumn = new DataGridViewComboBoxColumn
            {
                Name = SerialColumn,
                DataPropertyName = SerialColumn,
                HeaderText = "EQ_SN",
                MinimumWidth = 120,
                ReadOnly = readOnly,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            };

            foreach (DataRow serial in serials.Rows)
            {
                string text = serial[SerialColumn].ToString();

                if (!comboColumn.Items.Contains(text))
                {
                    comboColumn.Items.Add(text);
                }
            }

            // DB ที่ยังไม่มีแถว "-" เลย ค่าที่เติมให้เองจะโชว์เป็นช่องว่าง
            if (!comboColumn.Items.Contains(NoSerialText))
            {
                comboColumn.Items.Add(NoSerialText);
            }

            grid.Columns.RemoveAt(index);
            grid.Columns.Insert(index, comboColumn);
        }

        /// <summary>
        /// ตอนคลิกแก้ไข ค่อยกรองรายการให้เหลือเฉพาะ S/N ของเครื่องมือชนิดที่แถวนั้นใช้
        /// ทำตรงนี้เพราะได้ตัว ComboBox จริงมาแล้ว ไม่ต้องเดาว่า binding เสร็จหรือยัง
        ///
        /// เปิดให้พิมพ์ S/N ใหม่ได้ด้วย ไม่ใช่เลือกได้อย่างเดียว
        /// เผื่อเครื่องมือที่เพิ่งสอบเทียบมาและยังไม่เคยบันทึก ตัว Save จะ insert ให้เอง
        /// </summary>
        public static void HandleEditingControlShowing(DataGridView grid, DataGridViewEditingControlShowingEventArgs e)
        {
            if (grid?.CurrentCell == null) return;
            if (grid.Columns[grid.CurrentCell.ColumnIndex].Name != SerialColumn) return;

            ComboBox combo = e.Control as ComboBox;
            if (combo == null) return;

            combo.DropDownStyle = ComboBoxStyle.DropDown;
            combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            combo.AutoCompleteSource = AutoCompleteSource.ListItems;

            // ถ้าเซลล์นี้ถูกตั้ง DataSource มาจากที่อื่น แตะ Items ไม่ได้
            // .NET จะโยน "Items collection cannot be modified when the DataSource property is set"
            // เป็น unhandled exception เด้งใส่หน้าผู้ใช้ทันทีที่กด Dropdown
            // กรณีนี้หน้าจอนั้นจัดการรายการเองอยู่แล้ว ปล่อยไว้ไม่ต้องกรองซ้ำ
            if (combo.DataSource != null) return;

            if (_serials == null || _serials.Rows.Count == 0) return;

            DataGridViewRow gridRow = grid.Rows[grid.CurrentCell.RowIndex];

            string equipmentType = grid.Columns.Contains(TypeColumn)
                ? gridRow.Cells[TypeColumn].Value?.ToString()
                : null;

            string current = gridRow.Cells[SerialColumn].Value?.ToString();

            combo.Items.Clear();

            // เดิมถ้าชนิดนี้ไม่มี S/N เลย จะโชว์ S/N ของทุกชนิดแทน
            // ผู้ตรวจเลยเลือก S/N ของ Caliper ไปใส่ให้ Pin Gauge ได้ และ "-" ของแต่ละชนิดโผล่ซ้ำเป็นสิบตัว
            // ตอนนี้ชนิดที่ไม่มี S/N ถูกล็อกเป็น "-" ไปแล้ว (BlockNoSerialEdit) จึงกรองตามชนิดอย่างเดียว
            // ให้เห็นทั้งหมดเฉพาะตารางที่ไม่รู้ชนิดของแถว
            bool knownType = !string.IsNullOrWhiteSpace(equipmentType);

            foreach (DataRow serial in _serials.Rows)
            {
                string text = serial[SerialColumn].ToString();

                if ((!knownType || serial["EQUIPMENT_TYPE_ID"].ToString() == equipmentType)
                    && !combo.Items.Contains(text))
                {
                    combo.Items.Add(text);
                }
            }

            if (!string.IsNullOrWhiteSpace(current) && !combo.Items.Contains(current))
            {
                combo.Items.Add(current);
            }

            combo.Text = current;
        }

        /// <summary>พิมพ์ค่านอกรายการแล้วอย่าเด้ง error ให้รับไว้เหมือนเดิม</summary>
        public static void HandleDataError(DataGridView grid, DataGridViewDataErrorEventArgs e)
        {
            if (grid == null || e.ColumnIndex < 0 || e.ColumnIndex >= grid.Columns.Count) return;

            if (grid.Columns[e.ColumnIndex].Name == SerialColumn)
            {
                e.ThrowException = false;
                e.Cancel = false;
            }
        }
    }
}
