using System;
using System.Data;

namespace RawMat.ViewsMaterialNCR
{
    /// <summary>
    /// ข้อมูลตัวอย่างชั่วคราว ใช้ระหว่างที่ยังไม่มีตาราง NCR ใน DB
    ///
    /// TODO: ลบไฟล์นี้ทิ้งเมื่อมี query จริง แล้วให้แต่ละหน้าเรียก controller แทน
    /// ตารางที่คืนกลับไปคือ schema ที่ ucNcrTable ต้องการ ของจริงต้องหน้าตาเหมือนกัน
    /// </summary>
    internal static class NcrSampleData
    {
        public static DataTable Create()
        {
            DataTable table = new DataTable("NCR");
            table.Columns.Add("NCR_NO", typeof(string));
            table.Columns.Add("NCR_DATE", typeof(DateTime));
            table.Columns.Add("SUPPLIER", typeof(string));
            table.Columns.Add("PART_NO", typeof(string));
            table.Columns.Add("PROBLEM", typeof(string));
            table.Columns.Add("OWNER", typeof(string));
            table.Columns.Add("STATUS", typeof(string));

            string[,] samples = new string[,]
            {
                { "ABC Components Ltd.", "PN-10045", "Dimensional out of tolerance", "John Doe", "Open" },
                { "Precision Parts Inc.", "PN-20033", "Surface finish not as specified", "Mary Smith", "In Review" },
                { "Global Metals Co.", "PN-30021", "Incorrect material hardness", "Robert Brown", "Open" },
                { "ABC Components Ltd.", "PN-10012", "Missing certification", "John Doe", "In Review" },
                { "Techno Forgings", "PN-40018", "Crack observed after assembly", "Linda Green", "Closed" },
                { "Sunrise Industries", "PN-20011", "Label information incorrect", "Mary Smith", "Closed" },
                { "Precision Parts Inc.", "PN-20033", "Packaging not per requirement", "Robert Brown", "Open" },
                { "Global Metals Co.", "PN-30055", "Weld porosity found", "Linda Green", "In Review" },
            };

            DateTime latest = new DateTime(2024, 5, 20);
            for (int i = 0; i < 37; i++)
            {
                int s = i % 8;
                table.Rows.Add(
                    "NCR-2024-" + (152 - i).ToString("0000"),
                    latest.AddDays(-2 * i),
                    samples[s, 0],
                    samples[s, 1],
                    samples[s, 2],
                    samples[s, 3],
                    samples[s, 4]);
            }

            return table;
        }
    }
}
