using RawMat.Views.CustomMsg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RawMat.Utilities
{
    public class docCls
    {
        public void ExportToCSV(DataGridView dgv, string filePath)
        {
            StringBuilder sb = new StringBuilder();

            // 🔹 ดึง Header ของ DataGridView มาใส่เป็นบรรทัดแรก
            string[] columnNames = dgv.Columns.Cast<DataGridViewColumn>()
                                              .Select(column => "\"" + column.HeaderText + "\"") // ใส่ "" เพื่อป้องกัน comma
                                              .ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            // 🔹 ดึงข้อมูลในแต่ละแถวไปใส่ใน CSV
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!row.IsNewRow) // ข้ามแถวสุดท้ายที่ยังไม่ได้ใส่ข้อมูล
                {
                    string[] fields = row.Cells.Cast<DataGridViewCell>()
                                               .Select(cell => "\"" + (cell.Value ?? "").ToString().Replace("\"", "\"\"") + "\"")
                                               .ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }
            }

            // 🔹 บันทึกไฟล์ CSV
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);

            CustomMsgBoxBase.ShowCustomMessageBox("Export CSV สำเร็จ!", "สำเร็จ", CustomMsgBoxBase.MessageBoxIconType.OK);

        }

        public void ExportToExcel(DataGridView dgv, string filePath)
        {
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];

            // 🔹 ดึง Header ของ DataGridView ไปใส่ใน Excel
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dgv.Columns[i].HeaderText;
            }

            // 🔹 ดึงข้อมูลไปใส่ใน Excel
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    worksheet.Cells[i + 2, j + 1] = dgv.Rows[i].Cells[j].Value?.ToString();
                }
            }

            // 🔹 บันทึกไฟล์ Excel
            workbook.SaveAs(filePath);
            workbook.Close();
            excelApp.Quit();

            CustomMsgBoxBase.ShowCustomMessageBox("Export Excel สำเร็จ!", "สำเร็จ", CustomMsgBoxBase.MessageBoxIconType.OK);
        }

    }
}
