using RawMat.Property;
using RawMat.Controllers;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RawMat.Views.RegularCheck
{
    public class FormRegularReportExcelFlow : Form
    {
        private const string PdfFolderName = "FM-QA-B13-A Material Regular Inspection Record Sheet";

        private readonly QAdataProperty propQA;
        private readonly string generatedExcelPath;

        public FormRegularReportExcelFlow(QAdataProperty dataItem, string excelPath = null)
        {
            propQA = dataItem;
            generatedExcelPath = excelPath;
            InitializeComponent();
        }

        public static string CreateWaitApprovedExcel(QAdataProperty dataItem, DataTable regularData)
        {
            string filePath = Path.Combine(GetWaitApprovedPathStatic(), BuildExcelFileName(dataItem));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet sheet = null;

            try
            {
                excelApp = new Excel.Application
                {
                    DisplayAlerts = false,
                    Visible = false
                };

                workbook = excelApp.Workbooks.Add();
                sheet = (Excel.Worksheet)workbook.Worksheets[1];
                sheet.Name = "Regular Report";

                BuildReportSheet(sheet, dataItem, regularData);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                workbook.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook);
                return filePath;
            }
            finally
            {
                workbook?.Close(false);
                excelApp?.Quit();
                ReleaseCom(sheet);
                ReleaseCom(workbook);
                ReleaseCom(excelApp);
            }
        }

        public static string GetWaitApprovedFilePath(QAdataProperty dataItem)
        {
            return Path.Combine(GetWaitApprovedPathStatic(), BuildExcelFileName(dataItem));
        }

        public static string GetApprovedFilePath(QAdataProperty dataItem)
        {
            return Path.Combine(GetApprovedPathStatic(), BuildExcelFileName(dataItem));
        }

        public static string GetPdfFilePath(QAdataProperty dataItem)
        {
            string pdfName = $"{SanitizeFileName(dataItem?.Regular_No ?? dataItem?.Report_No ?? "Regular_Report")}.pdf";
            return Path.Combine(GetPdfSavePathStatic(), pdfName);
        }

        public static void ApproveExcelReport(QAdataProperty dataItem, string stampImagePath)
        {
            string waitPath = GetWaitApprovedFilePath(dataItem);
            string approvedPath = GetApprovedFilePath(dataItem);
            string pdfPath = GetPdfFilePath(dataItem);

            if (!File.Exists(waitPath) && File.Exists(approvedPath))
            {
                waitPath = approvedPath;
            }

            if (!File.Exists(waitPath))
            {
                throw new FileNotFoundException("ไม่พบไฟล์ Regular Report Excel ใน Wait Approved", waitPath);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(pdfPath));
            Directory.CreateDirectory(Path.GetDirectoryName(approvedPath));

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet sheet = null;

            try
            {
                excelApp = new Excel.Application
                {
                    DisplayAlerts = false,
                    Visible = false
                };

                workbook = excelApp.Workbooks.Open(waitPath);
                sheet = (Excel.Worksheet)workbook.Worksheets[1];

                AddStampImage(sheet, stampImagePath);

                workbook.Save();
                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                }

                workbook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdfPath);
                workbook.Close(false);
                workbook = null;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(false);
                }

                excelApp?.Quit();
                ReleaseCom(sheet);
                ReleaseCom(workbook);
                ReleaseCom(excelApp);
            }

            if (!string.Equals(waitPath, approvedPath, StringComparison.OrdinalIgnoreCase))
            {
                if (File.Exists(approvedPath))
                {
                    File.Delete(approvedPath);
                }

                File.Move(waitPath, approvedPath);
            }
        }

        private void InitializeComponent()
        {
            Text = "Regular Report Excel Flow";
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Size = new Size(900, 430);
            MinimumSize = new Size(760, 360);

            var topLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 46,
                Text = "Regular Report Excel Test",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 18F, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            var content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(18),
                ColumnCount = 2,
                RowCount = 7
            };

            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < content.RowCount; i++)
            {
                content.RowStyles.Add(new RowStyle(SizeType.Absolute, i == 6 ? 58F : 40F));
            }

            AddRow(content, 0, "Report No.", propQA.Report_No);
            AddRow(content, 1, "Regular No.", propQA.Regular_No);
            AddRow(content, 2, "M-Code", propQA.M_CODE);
            AddRow(content, 3, "Wait Approved", GetWaitApprovedPath());
            AddRow(content, 4, "Save PDF", GetPdfSavePath());
            AddRow(content, 5, "Approved", GetApprovedPath());

            var note = new Label
            {
                Text = string.IsNullOrWhiteSpace(generatedExcelPath)
                    ? "Regular report Excel file is ready for approval."
                    : $"Created: {generatedExcelPath}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Tahoma", 10F, FontStyle.Italic),
                ForeColor = Color.DarkSlateGray
            };
            content.Controls.Add(note, 1, 6);

            var closeButton = new Button
            {
                Text = "OK",
                Width = 120,
                Height = 34,
                Anchor = AnchorStyles.Right | AnchorStyles.Top,
                Font = new Font("Tahoma", 11F)
            };
            closeButton.Click += (sender, e) => Close();
            content.Controls.Add(closeButton, 0, 6);

            Controls.Add(content);
            Controls.Add(topLabel);
        }

        private void AddRow(TableLayoutPanel panel, int rowIndex, string title, string value)
        {
            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                ForeColor = Color.Black,
                Padding = new Padding(0, 0, 12, 0)
            };

            var valueBox = new TextBox
            {
                Text = value ?? string.Empty,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Tahoma", 10F)
            };

            panel.Controls.Add(titleLabel, 0, rowIndex);
            panel.Controls.Add(valueBox, 1, rowIndex);
        }

        private string GetWaitApprovedPath()
        {
            return GetWaitApprovedPathStatic();
        }

        private string GetApprovedPath()
        {
            return GetApprovedPathStatic();
        }

        private string GetPdfSavePath()
        {
            return GetPdfSavePathStatic();
        }

        private static string GetWaitApprovedPathStatic()
        {
            return ConfigurationManager.AppSettings["RegularReportWaitAppTest"]
                ?? ConfigurationManager.AppSettings["RegularReportWaitApp"]
                ?? @"C:\192.168.2.100\12_qa\01_Material\Z2_Receipt_Inspection\04_Regular check\2026\Wait Approved";
        }

        private static string GetApprovedPathStatic()
        {
            return ConfigurationManager.AppSettings["RegularReportAppTest"]
                ?? ConfigurationManager.AppSettings["RegularReportApp"]
                ?? @"C:\192.168.2.100\12_qa\01_Material\Z2_Receipt_Inspection\04_Regular check\2026\Approved";
        }

        private static string GetPdfSavePathStatic()
        {
            string scanRoot = ConfigurationManager.AppSettings["RegularReportScanTest"]
                ?? ConfigurationManager.AppSettings["RegularReportScan"]
                ?? @"C:\192.168.2.100\15_Document_Scan\DOCUMENT QA";
            return Path.Combine(scanRoot, "2026", PdfFolderName);
        }

        private static string BuildExcelFileName(QAdataProperty dataItem)
        {
            string reportName = dataItem?.Regular_No ?? dataItem?.Report_No ?? "Regular_Report";
            return $"{SanitizeFileName(reportName)}.xlsx";
        }

        private static string SanitizeFileName(string value)
        {
            string result = string.IsNullOrWhiteSpace(value) ? "Regular_Report" : value.Trim();
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                result = result.Replace(invalidChar, '_');
            }

            return result;
        }

        private static void BuildReportSheet(Excel.Worksheet sheet, QAdataProperty dataItem, DataTable regularData)
        {
            sheet.Cells.Font.Name = "Arial";
            sheet.Cells.Font.Size = 8;
            sheet.Columns.ColumnWidth = 8;
            sheet.Columns[1].ColumnWidth = 18;
            sheet.Columns[2].ColumnWidth = 8;
            sheet.Columns[3].ColumnWidth = 8;
            sheet.Columns[16].ColumnWidth = 7;

            DataTable data = regularData ?? new DataTable();
            string referenceText = GetReferenceText(dataItem);
            var sampleNos = data.AsEnumerable()
                .Select(row => GetString(row, "SAMPLING_NO"))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct()
                .OrderBy(value => int.TryParse(value, out int n) ? n : int.MaxValue)
                .ToList();

            if (sampleNos.Count == 0)
            {
                sampleNos.Add("1");
            }

            int page = 0;
            for (int start = 0; start < sampleNos.Count; start += 5)
            {
                page++;
                int topRow = 1 + ((page - 1) * 37);
                var pageSamples = sampleNos.Skip(start).Take(5).ToList();
                BuildReportPage(sheet, dataItem, data, pageSamples, topRow, page, referenceText);
            }

            Excel.PageSetup pageSetup = sheet.PageSetup;
            pageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;
            pageSetup.Zoom = false;
            pageSetup.FitToPagesWide = 1;
            pageSetup.FitToPagesTall = false;
            pageSetup.LeftMargin = 18;
            pageSetup.RightMargin = 18;
            pageSetup.TopMargin = 18;
            pageSetup.BottomMargin = 18;
            ReleaseCom(pageSetup);
        }

        private static void BuildReportPage(Excel.Worksheet sheet, QAdataProperty dataItem, DataTable data, System.Collections.Generic.List<string> sampleNos, int topRow, int page, string referenceText)
        {
            int imageTop = topRow + 8;
            int tableTop = topRow + 28;

            Merge(sheet, topRow, 1, topRow + 2, 10, "FM-QA-B13-A Material Regular Inspection Record Sheet");
            Merge(sheet, topRow, 11, topRow, 13, "Report No.");
            Merge(sheet, topRow, 14, topRow, 16, "Approve");
            Merge(sheet, topRow + 1, 11, topRow + 2, 13, dataItem?.Report_No);
            Merge(sheet, topRow + 1, 14, topRow + 2, 16, string.Empty);

            SetLabel(sheet, topRow + 3, 1, topRow + 3, 3, "Vender");
            Merge(sheet, topRow + 3, 4, topRow + 3, 10, dataItem?.Vendor_Name);
            SetLabel(sheet, topRow + 4, 1, topRow + 4, 3, "Receive Date");
            Merge(sheet, topRow + 4, 4, topRow + 4, 8, FormatDate(dataItem?.dtReceiveDate));
            SetLabel(sheet, topRow + 4, 9, topRow + 4, 10, "INV. No.");
            Merge(sheet, topRow + 4, 11, topRow + 4, 16, dataItem?.Invoice_No);
            SetLabel(sheet, topRow + 5, 1, topRow + 5, 3, "Lot Size");
            Merge(sheet, topRow + 5, 4, topRow + 5, 8, dataItem?.Qty);
            SetLabel(sheet, topRow + 5, 9, topRow + 5, 10, "Lot No.");
            Merge(sheet, topRow + 5, 11, topRow + 5, 16, dataItem?.Lot_No);
            SetLabel(sheet, topRow + 6, 1, topRow + 6, 3, "Inspection Size");
            Merge(sheet, topRow + 6, 4, topRow + 6, 8, dataItem?.SAMPLING_QTY);
            SetLabel(sheet, topRow + 6, 9, topRow + 6, 10, "Reference");
            Merge(sheet, topRow + 6, 11, topRow + 6, 16, referenceText);
            SetLabel(sheet, topRow + 7, 1, topRow + 7, 3, "Inspection Date");
            Merge(sheet, topRow + 7, 4, topRow + 7, 8, DateTime.Now.ToString("dd-MMM-yyyy"));
            SetLabel(sheet, topRow + 7, 9, topRow + 7, 10, "Inspector");
            Merge(sheet, topRow + 7, 11, topRow + 7, 16, dataItem?.EMP_ID);

            Merge(sheet, imageTop, 1, imageTop, 16, "Check Point");
            Merge(sheet, imageTop + 1, 1, imageTop + 19, 16, $"Page {page}");
            Excel.Range pageRange = sheet.Range[sheet.Cells[imageTop + 1, 1], sheet.Cells[imageTop + 19, 16]];
            pageRange.Font.Size = 44;
            pageRange.Font.Color = ColorTranslator.ToOle(Color.Gray);
            pageRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            pageRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            ReleaseCom(pageRange);

            BuildTable(sheet, data, sampleNos, tableTop);

            Excel.Range pageBorder = sheet.Range[sheet.Cells[topRow, 1], sheet.Cells[topRow + 35, 16]];
            pageBorder.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            pageBorder.Borders.Weight = Excel.XlBorderWeight.xlThin;
            pageBorder.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium);
            ReleaseCom(pageBorder);
        }

        private static string GetReferenceText(QAdataProperty dataItem)
        {
            if (dataItem == null || string.IsNullOrWhiteSpace(dataItem.M_CODE))
            {
                return string.Empty;
            }

            return new QAdataControllers().SearchReferenceByMCode(dataItem);
        }

        private static void BuildTable(Excel.Worksheet sheet, DataTable data, System.Collections.Generic.List<string> sampleNos, int tableTop)
        {
            SetHeader(sheet, tableTop, 1, "Point");
            SetHeader(sheet, tableTop, 2, "Min");
            SetHeader(sheet, tableTop, 3, "Max");

            for (int i = 0; i < 5; i++)
            {
                int col = 4 + (i * 2);
                SetHeader(sheet, tableTop, col, "Cavity\nNo");
                SetHeader(sheet, tableTop, col + 1, "Actual");
            }

            SetHeader(sheet, tableTop, 16, "Judg");

            var points = data.AsEnumerable()
                .GroupBy(row => GetString(row, "POINT_ORDER"))
                .OrderBy(group => int.TryParse(group.Key, out int n) ? n : int.MaxValue)
                .Take(12)
                .ToList();

            int rowIndex = tableTop + 1;
            foreach (var pointGroup in points)
            {
                DataRow point = pointGroup.First();
                sheet.Cells[rowIndex, 1] = GetString(point, "POINT_NAME");
                sheet.Cells[rowIndex, 2] = GetString(point, "CRITERIA_MIN");
                sheet.Cells[rowIndex, 3] = GetString(point, "CRITERIA_MAX");

                string totalJudge = "1";
                for (int i = 0; i < sampleNos.Count; i++)
                {
                    DataRow sampleRow = pointGroup.FirstOrDefault(row => GetString(row, "SAMPLING_NO") == sampleNos[i]);
                    if (sampleRow == null)
                    {
                        continue;
                    }

                    int col = 4 + (i * 2);
                    sheet.Cells[rowIndex, col] = GetString(sampleRow, "CAVITY_NAME");
                    sheet.Cells[rowIndex, col + 1] = GetString(sampleRow, "VALUE");
                    string judge = GetString(sampleRow, "POINT_JUDGE");
                    if (judge == "0")
                    {
                        totalJudge = "0";
                    }
                }

                sheet.Cells[rowIndex, 16] = totalJudge == "0" ? "NG" : "OK";
                rowIndex++;
            }

            Excel.Range tableRange = sheet.Range[sheet.Cells[tableTop, 1], sheet.Cells[tableTop + 13, 16]];
            tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            tableRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            tableRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            tableRange.WrapText = true;
            ReleaseCom(tableRange);

            Excel.Range firstColumns = sheet.Range[sheet.Cells[tableTop, 1], sheet.Cells[tableTop + 13, 3]];
            firstColumns.Interior.Color = ColorTranslator.ToOle(Color.LightBlue);
            ReleaseCom(firstColumns);
        }

        private static void AddStampImage(Excel.Worksheet sheet, string stampImagePath)
        {
            if (string.IsNullOrWhiteSpace(stampImagePath) || !File.Exists(stampImagePath))
            {
                return;
            }

            Excel.Range approveCell = sheet.Range["N2", "P3"];
            float left = Convert.ToSingle(approveCell.Left);
            float top = Convert.ToSingle(approveCell.Top);
            float width = Convert.ToSingle(approveCell.Width);
            float height = Convert.ToSingle(approveCell.Height);
            sheet.Shapes.AddPicture(stampImagePath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, left + 4, top + 2, width - 8, height - 4);
            ReleaseCom(approveCell);
        }

        private static void SetLabel(Excel.Worksheet sheet, int row1, int col1, int row2, int col2, string text)
        {
            Merge(sheet, row1, col1, row2, col2, text);
            Excel.Range range = sheet.Range[sheet.Cells[row1, col1], sheet.Cells[row2, col2]];
            range.Interior.Color = ColorTranslator.ToOle(Color.LightGreen);
            range.Font.Bold = true;
            ReleaseCom(range);
        }

        private static void SetHeader(Excel.Worksheet sheet, int row, int col, string text)
        {
            Excel.Range range = (Excel.Range)sheet.Cells[row, col];
            range.Value2 = text;
            range.Font.Bold = true;
            range.Interior.Color = ColorTranslator.ToOle(col <= 3 ? Color.LightBlue : Color.White);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.WrapText = true;
            ReleaseCom(range);
        }

        private static void Merge(Excel.Worksheet sheet, int row1, int col1, int row2, int col2, string text)
        {
            Excel.Range range = sheet.Range[sheet.Cells[row1, col1], sheet.Cells[row2, col2]];
            range.Merge();
            range.Value2 = text ?? string.Empty;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.WrapText = true;
            ReleaseCom(range);
        }

        private static string GetString(DataRow row, string columnName)
        {
            return row?.Table.Columns.Contains(columnName) == true && row[columnName] != DBNull.Value
                ? row[columnName]?.ToString()
                : string.Empty;
        }

        private static string FormatDate(DateTime? date)
        {
            return date.HasValue && date.Value != DateTime.MinValue
                ? date.Value.ToString("dd-MMM-yyyy")
                : string.Empty;
        }

        private static void ReleaseCom(object comObject)
        {
            if (comObject != null && Marshal.IsComObject(comObject))
            {
                Marshal.ReleaseComObject(comObject);
            }
        }
    }
}
