using System.Data;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    /// <summary>
    /// ทำให้ช่อง EQ_SN ในตารางตรวจเป็น Dropdown เลือก S/N ที่มีอยู่แล้ว
    ///
    /// เดิมช่องนี้เป็น TextBox ให้พิมพ์เอง แล้วตอน Save จะเอาสิ่งที่พิมพ์ไปสร้างแถวใหม่
    /// ใน info_equipment_serial ถ้ายังไม่มี — พิมพ์ผิดครั้งเดียวก็กลายเป็น master ถาวร
    ///
    /// ใช้ร่วมกันทั้ง Regular / Dimension และหน้า Pending ของทั้งสอง
    /// เพราะทุกหน้าใช้ชื่อคอลัมน์ชุดเดียวกัน (EQUIPMENT_SERIAL / EQUIPMENT_TYPE)
    /// </summary>
    internal static class EquipmentSerialColumn
    {
        private const string SerialColumn = "EQUIPMENT_SERIAL";
        private const string TypeColumn = "EQUIPMENT_TYPE";

        // เก็บรายการไว้ให้ตอนคลิกแก้ไขหยิบไปกรองตามชนิดเครื่องมือของแถวนั้น
        private static DataTable _serials;

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

            foreach (DataRow serial in _serials.Rows)
            {
                if (serial["EQUIPMENT_TYPE_ID"].ToString() == equipmentType)
                {
                    combo.Items.Add(serial[SerialColumn].ToString());
                }
            }

            // ไม่มี S/N ของชนิดนี้เลย ให้เห็นทั้งหมดดีกว่าเห็นรายการว่าง
            if (combo.Items.Count == 0)
            {
                foreach (DataRow serial in _serials.Rows)
                {
                    combo.Items.Add(serial[SerialColumn].ToString());
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
