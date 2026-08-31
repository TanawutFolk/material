using System;
using System.Data;
using System.Globalization;
using System.Windows.Forms;

namespace RawMat.Utilities
{
    /// <summary>
    /// จุดตรวจมีสองแบบ วัดออกมาเป็นตัวเลข กับ ตัดสินผ่าน/ไม่ผ่านด้วย Jig หรือ Gauge
    ///
    /// เดิมโค้ดเดาเอาจาก CRITERIA_MIN = CRITERIA_MAX ซึ่งเป็นข้อตกลงโดยปริยาย
    /// ไม่มีที่ไหนบันทึกไว้ว่าตั้งใจให้เป็นแบบไหน และกฎนี้เขียนกระจายอยู่ 5 ที่แบบไม่ตรงกัน
    /// (บางที่เช็ค == 1 เป๊ะ บางที่เช็ค min != max)
    /// ถ้าวันหลังมีคนแก้เกณฑ์จาก 1/1 เป็นเลขอื่น ความหมายของข้อมูลเก่าจะเปลี่ยนไปเงียบๆ
    ///
    /// ตอนนี้อ่านจากคอลัมน์ JUDGE_TYPE ตรงๆ ดู DatabaseScripts/20260827_add_judge_type.sql
    /// </summary>
    internal static class PointJudgeType
    {
        /// <summary>วัดเป็นตัวเลข เทียบกับช่วง MIN-MAX</summary>
        public const int Numeric = 1;

        /// <summary>ตัดสินผ่าน/ไม่ผ่าน ผู้ตรวจเลือกจาก Dropdown OK/NG</summary>
        public const int PassFail = 2;

        public const string ColumnName = "JUDGE_TYPE";

        /// <summary>
        /// คิวรี่ฝั่งหน้า Pending ยังดึงข้อมูลจาก db_*_data โดยไม่ได้เอาคอลัมน์นี้มาด้วย
        /// ถ้าไม่มีค่าให้ถอยไปใช้กฎเดิม ความหมายของข้อมูลจะได้ไม่เปลี่ยนระหว่างทาง
        /// </summary>
        private static bool FallbackIsPassFail(object min, object max)
        {
            if (!TryParse(min, out decimal minValue)) { return false; }
            if (!TryParse(max, out decimal maxValue)) { return false; }

            return minValue == maxValue;
        }

        public static bool IsPassFail(object judgeType, object min, object max)
        {
            if (TryParse(judgeType, out decimal value))
            {
                return (int)value == PassFail;
            }

            return FallbackIsPassFail(min, max);
        }

        public static bool IsPassFail(DataRow row)
        {
            if (row == null) { return false; }

            object judgeType = row.Table.Columns.Contains(ColumnName) ? row[ColumnName] : null;

            return IsPassFail(judgeType, GetValue(row, "CRITERIA_MIN"), GetValue(row, "CRITERIA_MAX"));
        }

        public static bool IsPassFail(DataGridViewRow row)
        {
            if (row == null) { return false; }

            return IsPassFail(GetValue(row, ColumnName),
                              GetValue(row, "CRITERIA_MIN"),
                              GetValue(row, "CRITERIA_MAX"));
        }

        /// <summary>จุดที่วัดเป็นตัวเลขเท่านั้นที่เอาไปคิดผลต่างภายในชิ้นได้</summary>
        public static bool IsNumeric(DataRow row)
        {
            return !IsPassFail(row);
        }

        private static object GetValue(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) ? row[column] : null;
        }

        private static object GetValue(DataGridViewRow row, string column)
        {
            if (row.DataGridView == null || !row.DataGridView.Columns.Contains(column)) { return null; }

            return row.Cells[column].Value;
        }

        private static bool TryParse(object rawValue, out decimal value)
        {
            value = 0;

            if (rawValue == null || rawValue == DBNull.Value) { return false; }

            string text = rawValue.ToString().Trim();
            if (text.Length == 0) { return false; }

            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                || decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }
    }
}
