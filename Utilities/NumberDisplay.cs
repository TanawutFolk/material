using System;
using System.Globalization;

namespace RawMat.Utilities
{
    /// <summary>
    /// กติกาการเขียนตัวเลขวัดลงหน้าจอและลงเอกสาร ให้เหมือนกันทุกที่
    ///
    /// ทุกคอลัมน์ที่เก็บค่าวัดเป็น decimal(12,6) พออ่านมาตรงๆ จะได้ 1.150000
    /// ศูนย์ท้ายไม่ได้เพิ่มค่าอะไร แต่ทำให้อ่านยากและไม่ตรงกับเอกสารต้นทาง
    ///
    /// สำคัญ : ตัดแค่ตอนแสดงผล ค่าใน DB ยังเป็น decimal(12,6) เหมือนเดิม
    /// และ 6 ตำแหน่งนี้เท่ากับความละเอียดสูงสุดของคอลัมน์ จึงไม่มีทางปัดค่าทิ้ง
    /// </summary>
    internal static class NumberDisplay
    {
        /// <summary>ใช้กับ DataGridViewColumn.DefaultCellStyle.Format ของคอลัมน์ที่ผูกกับ decimal</summary>
        public const string GridFormat = "0.######";

        /// <summary>
        /// ใช้กับค่าที่ถูกแปลงเป็นข้อความไปแล้ว เช่นตอนประกอบตารางหรือเขียนลง Excel
        /// (คอลัมน์ที่เป็น string ตั้ง Format ไม่มีผล ต้องตัดตั้งแต่ตอนใส่ค่า)
        /// อะไรที่ไม่ใช่ตัวเลขคืนกลับไปเหมือนเดิม ไม่กลืนข้อมูล
        /// </summary>
        public static string Trim(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            string text = value.ToString().Trim();

            if (text.Length == 0)
            {
                return string.Empty;
            }

            if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
            {
                return text;
            }

            return number.ToString(GridFormat, CultureInfo.InvariantCulture);
        }
    }
}
