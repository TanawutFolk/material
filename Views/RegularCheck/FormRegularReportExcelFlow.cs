using RawMat.Property;
using RawMat.Controllers;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace RawMat.Views.RegularCheck
{
    public class FormRegularReportExcelFlow : Form
    {
        private const string DefaultReportTitle = "FM-QA-B13-A Material Regular Inspection Record Sheet";
        private const bool ShowExcelDebugMessage = false;
        private const int AutoReportStartRow = 2;
        private const int AutoReportStartColumn = 2;
        private const int AutoReportLastColumn = 31;
        private const int AutoReportPageRows = 42;
        private const int AutoReportTableBodyRows = 15;

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
            return CreateWaitApprovedExcel(dataItem, regularData, dataItem?.FORMAT_REPORT_NAME, null);
        }

        public static string CreateWaitApprovedExcel(QAdataProperty dataItem, DataTable regularData, string reportTitle)
        {
            return CreateWaitApprovedExcel(dataItem, regularData, reportTitle, null);
        }

        public static string CreateWaitApprovedExcel(QAdataProperty dataItem, DataTable regularData, string reportTitle, DataTable formatMap)
        {
            if (!string.IsNullOrWhiteSpace(reportTitle) && dataItem != null)
            {
                dataItem.FORMAT_REPORT_NAME = reportTitle.Trim();
            }

            string filePath = Path.Combine(GetWaitApprovedPathStatic(dataItem), BuildExcelFileName(dataItem));
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            if (IsExcelDebugMessageEnabled())
            {
                ShowExcelDataDebugMessage(dataItem, regularData, filePath);
            }

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

                string templatePath = GetRegularTemplatePath(dataItem);
                workbook = string.IsNullOrWhiteSpace(templatePath)
                    ? excelApp.Workbooks.Add()
                    : excelApp.Workbooks.Open(templatePath);
                sheet = (Excel.Worksheet)workbook.Worksheets[1];

                if (HasFormatMap(formatMap))
                {
                    ApplyFormatReportMapping(sheet, dataItem, regularData, formatMap);
                }
                else
                {
                    sheet.Name = "master";
                    BuildReportSheet(sheet, dataItem, regularData);
                }

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
            return Path.Combine(GetWaitApprovedPathStatic(dataItem), BuildExcelFileName(dataItem));
        }

        public static string GetApprovedFilePath(QAdataProperty dataItem)
        {
            return Path.Combine(GetApprovedPathStatic(dataItem), BuildExcelFileName(dataItem));
        }

        public static string GetPdfFilePath(QAdataProperty dataItem)
        {
            string pdfName = $"{BuildReportFileBaseName(dataItem)}.pdf";
            return Path.Combine(GetPdfSavePathStatic(dataItem), pdfName);
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
            return GetWaitApprovedPathStatic(propQA);
        }

        private string GetApprovedPath()
        {
            return GetApprovedPathStatic(propQA);
        }

        private string GetPdfSavePath()
        {
            return GetPdfSavePathStatic(propQA);
        }

        private static string GetWaitApprovedPathStatic(QAdataProperty dataItem)
        {
            // return ConfigurationManager.AppSettings["RegularReportWaitAppTest"]  // Test path
            string configuredPath = ConfigurationManager.AppSettings["RegularReportWaitApp"]
                ?? @"C:\192.168.2.100\12_qa\01_Material\Z2_Receipt_Inspection\04_Regular check\2026\Wait Approved";
            return BuildYearFolderPath(configuredPath, GetReportYear(dataItem), "Wait Approved");
        }

        private static string GetApprovedPathStatic(QAdataProperty dataItem)
        {
            // return ConfigurationManager.AppSettings["RegularReportAppTest"];  // Test path
            string configuredPath = ConfigurationManager.AppSettings["RegularReportApp"]
                ?? @"C:\192.168.2.100\12_qa\01_Material\Z2_Receipt_Inspection\04_Regular check\2026\Approved";
            return BuildYearFolderPath(configuredPath, GetReportYear(dataItem), "Approved");
        }

        private static string GetPdfSavePathStatic(QAdataProperty dataItem)
        {
            // string scanRoot = ConfigurationManager.AppSettings["RegularReportScanTest"];  // Test path
            string scanRoot = ConfigurationManager.AppSettings["RegularReportScan"]
                ?? @"C:\192.168.2.100\15_Document_Scan\DOCUMENT QA";
            return BuildYearFolderPath(scanRoot, GetReportYear(dataItem), GetReportTitle(dataItem));
        }

        private static int GetReportYear(QAdataProperty dataItem)
        {
            if (dataItem != null && dataItem.dtReceiveDate != DateTime.MinValue)
            {
                return dataItem.dtReceiveDate.Year;
            }

            return DateTime.Now.Year;
        }

        private static string BuildYearFolderPath(string configuredPath, int year, string leafFolderName)
        {
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                return Path.Combine(year.ToString(), leafFolderName);
            }

            DirectoryInfo configuredDirectory = new DirectoryInfo(configuredPath);
            if (IsYearLeafPath(configuredDirectory, year, leafFolderName))
            {
                return configuredPath;
            }

            if (IsTestPathUnderYearLeaf(configuredDirectory))
            {
                DirectoryInfo configuredYearDirectory = configuredDirectory.Parent.Parent;
                if (string.Equals(configuredYearDirectory.Name, year.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return configuredPath;
                }

                string basePath = configuredYearDirectory.Parent?.FullName ?? configuredPath;
                return Path.Combine(basePath, year.ToString(), leafFolderName, configuredDirectory.Name);
            }

            if (string.Equals(configuredDirectory.Name, leafFolderName, StringComparison.OrdinalIgnoreCase))
            {
                DirectoryInfo yearDirectory = configuredDirectory.Parent;
                if (yearDirectory != null && int.TryParse(yearDirectory.Name, out _))
                {
                    string basePath = yearDirectory.Parent?.FullName ?? configuredPath;
                    return Path.Combine(basePath, year.ToString(), leafFolderName);
                }

                return Path.Combine(yearDirectory?.FullName ?? configuredPath, year.ToString(), leafFolderName);
            }

            return Path.Combine(configuredPath, year.ToString(), leafFolderName);
        }

        private static bool IsYearLeafPath(DirectoryInfo directory, int year, string leafFolderName)
        {
            return directory != null
                && directory.Parent != null
                && string.Equals(directory.Name, leafFolderName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(directory.Parent.Name, year.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTestPathUnderYearLeaf(DirectoryInfo directory)
        {
            return directory != null
                && directory.Parent != null
                && directory.Parent.Parent != null
                && string.Equals(directory.Name, "Test", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(directory.Parent.Parent.Name, out _);
        }

        private static string BuildExcelFileName(QAdataProperty dataItem)
        {
            return $"{BuildReportFileBaseName(dataItem)}.xlsx";
        }

        private static string BuildReportFileBaseName(QAdataProperty dataItem)
        {
            string reportName = dataItem?.Regular_No ?? dataItem?.Report_No ?? "Regular_Report";
            string mCode = dataItem?.M_CODE;
            if (!string.IsNullOrWhiteSpace(mCode))
            {
                reportName = $"{reportName}_{mCode.Trim()}";
            }

            return SanitizeFileName(reportName);
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
            sheet.Cells.Font.Name = "Tahoma";
            sheet.Cells.Font.Size = 10;
            sheet.Cells.Font.Bold = false;
            ApplyAutoReportColumnLayout(sheet);

            DataTable data = regularData ?? new DataTable();
            string referenceText = GetReferenceText(dataItem);
            var regularImagePaths = GetReportImagePaths("RegularPath", dataItem?.M_CODE);
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
                int topRow = AutoReportStartRow + ((page - 1) * AutoReportPageRows);
                var pageSamples = sampleNos.Skip(start).Take(5).ToList();
                string imagePath = GetImagePathForPage(regularImagePaths, page);
                if (page > 1)
                {
                    AddHorizontalPageBreak(sheet, topRow);
                }

                BuildReportPage(sheet, dataItem, data, pageSamples, topRow, page, referenceText, imagePath);
            }

            ApplyPageSetup(
                sheet,
                AutoReportLastColumn,
                AutoReportStartRow,
                AutoReportStartColumn);
        }

        private static void ApplyPageSetup(
            Excel.Worksheet sheet,
            int printColumns = 0,
            int printStartRow = 1,
            int printStartColumn = 1)
        {
            Excel.PageSetup pageSetup = sheet.PageSetup;
            Excel.Range lastCell = null;
            Excel.Range printRange = null;
            pageSetup.Orientation = Excel.XlPageOrientation.xlPortrait;
            pageSetup.PaperSize = Excel.XlPaperSize.xlPaperA4;
            pageSetup.Zoom = false;
            pageSetup.FitToPagesWide = 1;
            pageSetup.FitToPagesTall = false;
            pageSetup.LeftMargin = 18;
            pageSetup.RightMargin = 18;
            pageSetup.TopMargin = 18;
            pageSetup.BottomMargin = 18;
            pageSetup.CenterHorizontally = true;
            try
            {
                lastCell = sheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
                int lastRow = Math.Max(1, Convert.ToInt32(lastCell.Row));
                int lastColumn = printColumns > 0
                    ? printColumns
                    : Math.Max(1, Convert.ToInt32(lastCell.Column));
                printRange = sheet.Range[
                    sheet.Cells[printStartRow, printStartColumn],
                    sheet.Cells[lastRow, lastColumn]];
                pageSetup.PrintArea = printRange.Address;
            }
            finally
            {
                ReleaseCom(printRange);
                ReleaseCom(lastCell);
            }

            ReleaseCom(pageSetup);
        }

        private static void ApplyAutoReportColumnLayout(Excel.Worksheet sheet)
        {
            for (int col = AutoReportStartColumn; col <= AutoReportLastColumn; col++)
            {
                Excel.Range column = null;
                try
                {
                    column = (Excel.Range)sheet.Columns[col];
                    column.ColumnWidth = 3.2;
                }
                finally
                {
                    ReleaseCom(column);
                }
            }

            SetColumnWidth(sheet, 2, 4.7);
            SetColumnWidth(sheet, 3, 4.7);
            SetColumnWidth(sheet, 4, 4.7);
            SetColumnWidth(sheet, 5, 4.2);
            SetColumnWidth(sheet, 6, 4.2);
            SetColumnWidth(sheet, 7, 4.2);
            SetColumnWidth(sheet, 8, 4.2);
            SetColumnWidth(sheet, 9, 4.8);
            SetColumnWidth(sheet, 10, 4.8);
            SetColumnWidth(sheet, 30, 4.2);
            SetColumnWidth(sheet, 31, 8.0);
        }

        private static void SetColumnWidth(Excel.Worksheet sheet, int columnIndex, double width)
        {
            Excel.Range column = null;
            try
            {
                column = (Excel.Range)sheet.Columns[columnIndex];
                column.ColumnWidth = width;
            }
            finally
            {
                ReleaseCom(column);
            }
        }

        private static void ApplyAutoReportRowLayout(Excel.Worksheet sheet, int topRow)
        {
            for (int row = topRow; row < topRow + AutoReportPageRows; row++)
            {
                SetRowHeight(sheet, row, 20.4);
            }

            for (int row = topRow; row <= topRow + 2; row++)
            {
                SetRowHeight(sheet, row, 22.8);
            }

            for (int row = topRow + 3; row <= topRow + 7; row++)
            {
                SetRowHeight(sheet, row, 21.6);
            }

            SetRowHeight(sheet, topRow + 6, 25.92);
            SetRowHeight(sheet, topRow + 7, 25.92);
            SetRowHeight(sheet, topRow + 8, 20.4);

            for (int row = topRow + 9; row <= topRow + 25; row++)
            {
                SetRowHeight(sheet, row, 26.4);
            }

            SetRowHeight(sheet, topRow + 26, 36);
        }

        private static void SetRowHeight(Excel.Worksheet sheet, int rowIndex, double height)
        {
            Excel.Range row = null;
            try
            {
                row = (Excel.Range)sheet.Rows[rowIndex];
                row.RowHeight = height;
            }
            finally
            {
                ReleaseCom(row);
            }
        }

        private static void AddHorizontalPageBreak(Excel.Worksheet sheet, int topRow)
        {
            Excel.Range breakCell = null;
            try
            {
                breakCell = (Excel.Range)sheet.Cells[topRow, AutoReportStartColumn];
                sheet.HPageBreaks.Add(breakCell);
            }
            catch
            {
                // Excel can still paginate from the A4 page setup if a manual break cannot be added.
            }
            finally
            {
                ReleaseCom(breakCell);
            }
        }

        private static void BuildReportPage(Excel.Worksheet sheet, QAdataProperty dataItem, DataTable data, System.Collections.Generic.List<string> sampleNos, int topRow, int page, string referenceText, string imagePath)
        {
            int imageTop = topRow + 8;
            int tableTop = topRow + 26;

            ApplyAutoReportRowLayout(sheet, topRow);

            StylePageFont(sheet, topRow, topRow + AutoReportPageRows - 1);

            string reportHeader = $"{GetReportTitle(dataItem)}\n{BuildMaterialHeaderText(dataItem)}";
            Merge(sheet, topRow, 2, topRow + 2, 22, reportHeader);
            StyleFont(sheet, topRow, 2, topRow + 2, 22, 13, true);

            SetLabel(sheet, topRow, 23, topRow, 27, "Report No.");
            SetLabel(sheet, topRow, 28, topRow, 31, "Approve");
            Merge(sheet, topRow + 1, 23, topRow + 2, 27, dataItem?.Report_No);
            Merge(sheet, topRow + 1, 28, topRow + 2, 31, string.Empty);

            SetLabel(sheet, topRow + 3, 2, topRow + 3, 6, "Vender");
            MergeLeft(sheet, topRow + 3, 7, topRow + 3, 31, dataItem?.Vendor_Name);
            SetLabel(sheet, topRow + 4, 2, topRow + 4, 6, "Receipt Date");
            Merge(sheet, topRow + 4, 7, topRow + 4, 17, FormatDate(dataItem?.dtReceiveDate));
            SetLabel(sheet, topRow + 4, 18, topRow + 4, 22, "INV. No.");
            Merge(sheet, topRow + 4, 23, topRow + 4, 31, dataItem?.Invoice_No);
            SetLabel(sheet, topRow + 5, 2, topRow + 5, 6, "Lot Size");
            Merge(sheet, topRow + 5, 7, topRow + 5, 17, dataItem?.Qty);
            SetLabel(sheet, topRow + 5, 18, topRow + 5, 22, "Lot No.");
            Merge(sheet, topRow + 5, 23, topRow + 5, 31, dataItem?.Lot_No);
            SetLabel(sheet, topRow + 6, 2, topRow + 6, 6, "Inspection Size");
            Merge(sheet, topRow + 6, 7, topRow + 6, 17, dataItem?.SAMPLING_QTY);
            SetLabel(sheet, topRow + 6, 18, topRow + 6, 22, "Reference");
            Merge(sheet, topRow + 6, 23, topRow + 6, 31, referenceText);
            SetLabel(sheet, topRow + 7, 2, topRow + 7, 6, "Inspection Date");
            Merge(sheet, topRow + 7, 7, topRow + 7, 17, DateTime.Now.ToString("dd-MMM-yyyy"));
            SetLabel(sheet, topRow + 7, 18, topRow + 7, 22, "Inspector");
            Merge(sheet, topRow + 7, 23, topRow + 7, 31, dataItem?.EMP_ID);

            MergeLeft(sheet, imageTop, 2, imageTop, 31, "Check Point");
            StyleCheckPointHeader(sheet, imageTop, 2, 31);
            Merge(sheet, imageTop + 1, 2, tableTop - 1, 31, string.Empty);
            AddReportImage(sheet, imagePath, imageTop + 1, 2, tableTop - 1, 31);

            BuildAutoReportTable(sheet, data, sampleNos, tableTop);

            Excel.Range pageBorder = sheet.Range[
                sheet.Cells[topRow, AutoReportStartColumn],
                sheet.Cells[topRow + AutoReportPageRows - 1, AutoReportLastColumn]];
            pageBorder.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            pageBorder.Borders.Weight = Excel.XlBorderWeight.xlThin;
            pageBorder.BorderAround(Excel.XlLineStyle.xlContinuous, Excel.XlBorderWeight.xlMedium);
            ReleaseCom(pageBorder);
        }

        private static System.Collections.Generic.List<string> GetReportImagePaths(string appSettingKey, string fileName)
        {
            var imagePaths = new System.Collections.Generic.List<string>();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return imagePaths;
            }

            string folderPath = ConfigurationManager.AppSettings[appSettingKey];
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
            {
                return imagePaths;
            }

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            var files = Directory.GetFiles(folderPath, fileName + "*.*")
                .Where(path => allowedExtensions.Contains(Path.GetExtension(path).ToLower()))
                .OrderBy(path => GetImageSortKey(path, fileName))
                .ToList();

            if (files.Count > 0)
            {
                return files;
            }

            foreach (string extension in allowedExtensions)
            {
                string fullPath = Path.Combine(folderPath, fileName + extension);
                if (File.Exists(fullPath))
                {
                    imagePaths.Add(fullPath);
                    break;
                }
            }

            return imagePaths;
        }

        private static string GetImageSortKey(string path, string fileName)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (name.StartsWith(fileName))
            {
                string suffix = name.Substring(fileName.Length);
                if (int.TryParse(suffix, out int number))
                {
                    return number.ToString("D10");
                }

                return suffix;
            }

            return name;
        }

        private static string GetImagePathForPage(System.Collections.Generic.List<string> imagePaths, int page)
        {
            if (imagePaths == null || imagePaths.Count == 0)
            {
                return string.Empty;
            }

            int index = Math.Min(page - 1, imagePaths.Count - 1);
            return imagePaths[index];
        }

        private static void AddReportImage(Excel.Worksheet sheet, string imagePath, int row1, int col1, int row2, int col2)
        {
            if (string.IsNullOrWhiteSpace(imagePath) || !File.Exists(imagePath))
            {
                return;
            }

            Excel.Range imageRange = sheet.Range[sheet.Cells[row1, col1], sheet.Cells[row2, col2]];
            float left = Convert.ToSingle(imageRange.Left);
            float top = Convert.ToSingle(imageRange.Top);
            float width = Convert.ToSingle(imageRange.Width);
            float height = Convert.ToSingle(imageRange.Height);

            sheet.Shapes.AddPicture(
                imagePath,
                Microsoft.Office.Core.MsoTriState.msoFalse,
                Microsoft.Office.Core.MsoTriState.msoTrue,
                left + 6,
                top + 4,
                width - 12,
                height - 8);

            ReleaseCom(imageRange);
        }

        private static string GetReferenceText(QAdataProperty dataItem)
        {
            if (dataItem == null || string.IsNullOrWhiteSpace(dataItem.M_CODE))
            {
                return string.Empty;
            }

            return new QAdataControllers().SearchReferenceByMCode(dataItem);
        }

        private static string BuildMaterialHeaderText(QAdataProperty dataItem)
        {
            if (dataItem == null)
            {
                return string.Empty;
            }

            string mCode = dataItem.M_CODE?.Trim() ?? string.Empty;
            string materialName = dataItem.Material_Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(mCode))
            {
                return materialName;
            }

            if (string.IsNullOrWhiteSpace(materialName))
            {
                return mCode;
            }

            return $"{mCode} : {materialName}";
        }

        private static string GetReportTitle(QAdataProperty dataItem)
        {
            return DefaultReportTitle;
        }

        private static bool IsExcelDebugMessageEnabled()
        {
            string configuredValue = ConfigurationManager.AppSettings["RegularReportExcelDebug"];
            return bool.TryParse(configuredValue, out bool isEnabled)
                ? isEnabled
                : ShowExcelDebugMessage;
        }

        private static bool HasFormatMap(DataTable formatMap)
        {
            return formatMap != null
                && formatMap.Rows.Count > 0
                && FindColumn(formatMap, "CELL") != null
                && FindColumn(formatMap, "CELL_NAME") != null;
        }

        private static string GetRegularTemplatePath(QAdataProperty dataItem)
        {
            string configuredTemplate = ConfigurationManager.AppSettings["RegularReportTemplateFile"];
            if (!string.IsNullOrWhiteSpace(configuredTemplate) && File.Exists(configuredTemplate))
            {
                return configuredTemplate;
            }

            string templateRoot = ConfigurationManager.AppSettings["ReportFormatFile"];
            if (string.IsNullOrWhiteSpace(templateRoot))
            {
                return string.Empty;
            }

            string configuredTitle = GetReportTitle(dataItem);
            string byTitle = FindExcelFile(Path.Combine(templateRoot, configuredTitle));
            if (!string.IsNullOrWhiteSpace(byTitle))
            {
                return byTitle;
            }

            string byMCode = FindExcelFile(Path.Combine(templateRoot, dataItem?.M_CODE ?? string.Empty));
            return byMCode ?? string.Empty;
        }

        private static string FindExcelFile(string basePath)
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return null;
            }

            if (File.Exists(basePath))
            {
                return basePath;
            }

            if (File.Exists(basePath + ".xlsx")) return basePath + ".xlsx";
            if (File.Exists(basePath + ".xls")) return basePath + ".xls";
            return null;
        }

        private static void ApplyFormatReportMapping(Excel.Worksheet sheet, QAdataProperty dataItem, DataTable regularData, DataTable formatMap)
        {
            DataTable data = regularData ?? new DataTable();
            foreach (DataRow row in formatMap.Rows)
            {
                string cell = GetMapValue(row, "CELL");
                string cellName = GetMapValue(row, "CELL_NAME");
                if (string.IsNullOrWhiteSpace(cell) || string.IsNullOrWhiteSpace(cellName))
                {
                    continue;
                }

                string normalizedCellName = NormalizeCellName(cellName);
                if (normalizedCellName == "REGULARTABLESTART")
                {
                    if (TryGetCellPosition(sheet, cell, out int tableRow, out int tableColumn))
                    {
                        BuildTable(sheet, data, GetSampleNos(data), tableRow, tableColumn);
                    }
                    continue;
                }

                if (normalizedCellName == "CHECKPOINTIMAGE")
                {
                    if (TryGetCellPosition(sheet, cell, out int imageRow, out int imageColumn))
                    {
                        string imagePath = GetImagePathForPage(GetReportImagePaths("RegularPath", dataItem?.M_CODE), 1);
                        AddReportImage(sheet, imagePath, imageRow, imageColumn, imageRow + 18, imageColumn + 15);
                    }
                    continue;
                }

                Excel.Range range = null;
                try
                {
                    range = sheet.Range[cell];
                    range.Value2 = GetValueForCellName(cellName, dataItem);
                }
                finally
                {
                    ReleaseCom(range);
                }
            }

            ApplyPageSetup(sheet);
        }

        private static string GetValueForCellName(string cellName, QAdataProperty dataItem)
        {
            if (string.IsNullOrWhiteSpace(cellName))
            {
                return string.Empty;
            }

            if (cellName.StartsWith("Text:", StringComparison.OrdinalIgnoreCase))
            {
                return cellName.Substring("Text:".Length);
            }

            switch (NormalizeCellName(cellName))
            {
                case "REPORTTITLE":
                case "FORMATREPORTNAME":
                    return GetReportTitle(dataItem);
                case "MATERIALHEADER":
                case "MCODEMATERIALNAME":
                    return BuildMaterialHeaderText(dataItem);
                case "REPORTNO":
                    return dataItem?.Report_No ?? string.Empty;
                case "REGULARNO":
                    return dataItem?.Regular_No ?? string.Empty;
                case "MCODE":
                    return dataItem?.M_CODE ?? string.Empty;
                case "MATERIALNAME":
                    return dataItem?.Material_Name ?? string.Empty;
                case "VENDOR":
                case "VENDER":
                case "VENDORNAME":
                    return dataItem?.Vendor_Name ?? string.Empty;
                case "RECEIVEDATE":
                    return FormatDate(dataItem?.dtReceiveDate);
                case "INVNO":
                case "INVOICENO":
                    return dataItem?.Invoice_No ?? string.Empty;
                case "LOTSIZE":
                case "QTY":
                    return dataItem?.Qty ?? string.Empty;
                case "LOTNO":
                    return dataItem?.Lot_No ?? string.Empty;
                case "INSPECTIONSIZE":
                case "SAMPLINGQTY":
                    return dataItem?.SAMPLING_QTY ?? string.Empty;
                case "REFERENCE":
                    return GetReferenceText(dataItem);
                case "INSPECTIONDATE":
                case "TODAY":
                    return DateTime.Now.ToString("dd-MMM-yyyy");
                case "INSPECTOR":
                case "EMPID":
                case "ISSUEEMPID":
                    return dataItem?.EMP_ID ?? string.Empty;
                case "ISSUEEMPNAME":
                    return dataItem?.EMP_NAME ?? string.Empty;
                case "CHECKPOINT":
                    return "Check Point";
                default:
                    return string.Empty;
            }
        }

        private static System.Collections.Generic.List<string> GetSampleNos(DataTable data)
        {
            var sampleNos = data.AsEnumerable()
                .Select(row => GetString(row, "SAMPLING_NO"))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct()
                .OrderBy(value => int.TryParse(value, out int n) ? n : int.MaxValue)
                .Take(5)
                .ToList();

            if (sampleNos.Count == 0)
            {
                sampleNos.Add("1");
            }

            return sampleNos;
        }

        private static bool TryGetCellPosition(Excel.Worksheet sheet, string cell, out int row, out int column)
        {
            row = 0;
            column = 0;
            Excel.Range range = null;
            try
            {
                range = sheet.Range[cell];
                row = range.Row;
                column = range.Column;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                ReleaseCom(range);
            }
        }

        private static string GetMapValue(DataRow row, string columnName)
        {
            DataColumn column = FindColumn(row.Table, columnName);
            return column != null && row[column] != DBNull.Value
                ? row[column]?.ToString()
                : string.Empty;
        }

        private static DataColumn FindColumn(DataTable table, string columnName)
        {
            if (table == null || string.IsNullOrWhiteSpace(columnName))
            {
                return null;
            }

            foreach (DataColumn column in table.Columns)
            {
                if (string.Equals(column.ColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            return null;
        }

        private static string NormalizeCellName(string cellName)
        {
            return new string((cellName ?? string.Empty)
                .Where(char.IsLetterOrDigit)
                .Select(char.ToUpperInvariant)
                .ToArray());
        }

        private static void ShowExcelDataDebugMessage(QAdataProperty dataItem, DataTable regularData, string filePath)
        {
            string referenceText = GetReferenceText(dataItem);
            var regularImagePaths = GetReportImagePaths("RegularPath", dataItem?.M_CODE);

            var message = new StringBuilder();
            message.AppendLine("Regular Report Excel Debug");
            message.AppendLine();
            message.AppendLine("Output file:");
            message.AppendLine(filePath);
            message.AppendLine();
            message.AppendLine("Header data:");
            message.AppendLine($"Report No. <- propQA.Report_No = {dataItem?.Report_No}");
            message.AppendLine($"Vender <- propQA.Vendor_Name = {dataItem?.Vendor_Name}");
            message.AppendLine($"Receive Date <- propQA.dtReceiveDate = {FormatDate(dataItem?.dtReceiveDate)}");
            message.AppendLine($"INV. No. <- propQA.Invoice_No = {dataItem?.Invoice_No}");
            message.AppendLine($"Lot Size <- propQA.Qty = {dataItem?.Qty}");
            message.AppendLine($"Lot No. <- propQA.Lot_No = {dataItem?.Lot_No}");
            message.AppendLine($"Inspection Size <- propQA.SAMPLING_QTY = {dataItem?.SAMPLING_QTY}");
            message.AppendLine($"Reference <- info_reference.reference where mcode = {dataItem?.M_CODE} = {referenceText}");
            message.AppendLine($"Inspection Date <- DateTime.Now = {DateTime.Now:dd-MMM-yyyy}");
            message.AppendLine($"Inspector <- propQA.EMP_ID = {dataItem?.EMP_ID}");
            message.AppendLine();
            message.AppendLine("Check Point image:");
            message.AppendLine($"Source <- App.config RegularPath + M_CODE = {dataItem?.M_CODE}");
            message.AppendLine($"Found image count = {regularImagePaths.Count}");
            message.AppendLine($"First image -> Check Point area = {(regularImagePaths.Count > 0 ? regularImagePaths[0] : "not found")}");
            message.AppendLine();
            message.AppendLine("Bottom table:");
            message.AppendLine($"Source <- originalDataTable / regularData rows = {regularData?.Rows.Count ?? 0}");
            message.AppendLine("Point <- POINT_NAME");
            message.AppendLine("Min <- CRITERIA_MIN");
            message.AppendLine("Max <- CRITERIA_MAX");
            message.AppendLine("Equipment <- EQUIPMENT_NAME");
            message.AppendLine("Cavity No <- CAVITY_NAME");
            message.AppendLine("Actual <- VALUE");
            message.AppendLine("Judg <- POINT_JUDGE summary");

            MessageBox.Show(message.ToString(), "Excel Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void BuildAutoReportTable(Excel.Worksheet sheet, DataTable data, System.Collections.Generic.List<string> sampleNos, int tableTop)
        {
            SetMergedHeader(sheet, tableTop, 2, 4, "Point", Color.LightBlue, Color.Black);
            SetMergedHeader(sheet, tableTop, 5, 6, "Min", Color.LightBlue, Color.Black);
            SetMergedHeader(sheet, tableTop, 7, 8, "Max", Color.LightBlue, Color.Black);
            SetMergedHeader(sheet, tableTop, 9, 10, "Equipment", Color.LightYellow, Color.Black);

            for (int i = 0; i < 5; i++)
            {
                int col = 11 + (i * 4);
                SetMergedHeader(sheet, tableTop, col, col + 1, "Cavity\nNo", Color.White, Color.Green);
                SetMergedHeader(sheet, tableTop, col + 2, col + 3, "Actual", Color.White, Color.Black);
            }

            SetMergedHeader(sheet, tableTop, 31, 31, "Judgment", Color.White, Color.Blue);

            var points = data.AsEnumerable()
                .GroupBy(row => GetString(row, "POINT_ORDER"))
                .OrderBy(group => int.TryParse(group.Key, out int n) ? n : int.MaxValue)
                .Take(AutoReportTableBodyRows)
                .ToList();

            for (int rowOffset = 0; rowOffset < AutoReportTableBodyRows; rowOffset++)
            {
                int rowIndex = tableTop + 1 + rowOffset;
                var pointGroup = rowOffset < points.Count ? points[rowOffset] : null;
                DataRow point = pointGroup?.First();

                Merge(sheet, rowIndex, 2, rowIndex, 4, GetString(point, "POINT_NAME"));
                Merge(sheet, rowIndex, 5, rowIndex, 6, GetString(point, "CRITERIA_MIN"));
                Merge(sheet, rowIndex, 7, rowIndex, 8, GetString(point, "CRITERIA_MAX"));
                Merge(sheet, rowIndex, 9, rowIndex, 10, GetEquipmentText(pointGroup));

                string totalJudge = string.Empty;
                for (int i = 0; i < 5; i++)
                {
                    int col = 11 + (i * 4);
                    DataRow sampleRow = null;
                    if (pointGroup != null && i < sampleNos.Count)
                    {
                        sampleRow = pointGroup.FirstOrDefault(row => GetString(row, "SAMPLING_NO") == sampleNos[i]);
                    }

                    Merge(sheet, rowIndex, col, rowIndex, col + 1, GetString(sampleRow, "CAVITY_NAME"));
                    Merge(sheet, rowIndex, col + 2, rowIndex, col + 3, GetString(sampleRow, "VALUE"));

                    string judge = GetString(sampleRow, "POINT_JUDGE");
                    if (judge == "0")
                    {
                        totalJudge = "NG";
                    }
                    else if (judge == "1" && string.IsNullOrWhiteSpace(totalJudge))
                    {
                        totalJudge = "OK";
                    }
                }

                Merge(sheet, rowIndex, 31, rowIndex, 31, totalJudge);
            }

            Excel.Range tableRange = sheet.Range[
                sheet.Cells[tableTop, AutoReportStartColumn],
                sheet.Cells[tableTop + AutoReportTableBodyRows, AutoReportLastColumn]];
            tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            tableRange.Borders.Weight = Excel.XlBorderWeight.xlThin;
            tableRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            tableRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            tableRange.WrapText = true;
            tableRange.Font.Name = "Tahoma";
            tableRange.Font.Size = 9;
            ReleaseCom(tableRange);

            // Apply this after the table-wide font because that formatting would otherwise reset it to 9 pt.
            StyleFont(sheet, tableTop, 31, tableTop, 31, 8, true);

            Excel.Range firstColumns = sheet.Range[sheet.Cells[tableTop, 2], sheet.Cells[tableTop + AutoReportTableBodyRows, 8]];
            firstColumns.Interior.Color = ColorTranslator.ToOle(Color.LightBlue);
            ReleaseCom(firstColumns);

            Excel.Range equipmentColumn = sheet.Range[sheet.Cells[tableTop, 9], sheet.Cells[tableTop + AutoReportTableBodyRows, 10]];
            equipmentColumn.Interior.Color = ColorTranslator.ToOle(Color.LightYellow);
            ReleaseCom(equipmentColumn);
        }

        private static void BuildTable(Excel.Worksheet sheet, DataTable data, System.Collections.Generic.List<string> sampleNos, int tableTop, int startCol = 1)
        {
            SetHeader(sheet, tableTop, startCol, "Point");
            SetHeader(sheet, tableTop, startCol + 1, "Min");
            SetHeader(sheet, tableTop, startCol + 2, "Max");
            SetHeader(sheet, tableTop, startCol + 3, "Equipment");

            for (int i = 0; i < 5; i++)
            {
                int col = startCol + 4 + (i * 2);
                SetHeader(sheet, tableTop, col, "Cavity\nNo");
                SetHeader(sheet, tableTop, col + 1, "Actual");
            }

            SetHeader(sheet, tableTop, startCol + 14, "Judgment");

            var points = data.AsEnumerable()
                .GroupBy(row => GetString(row, "POINT_ORDER"))
                .OrderBy(group => int.TryParse(group.Key, out int n) ? n : int.MaxValue)
                .Take(12)
                .ToList();

            int rowIndex = tableTop + 1;
            foreach (var pointGroup in points)
            {
                DataRow point = pointGroup.First();
                sheet.Cells[rowIndex, startCol] = GetString(point, "POINT_NAME");
                sheet.Cells[rowIndex, startCol + 1] = GetString(point, "CRITERIA_MIN");
                sheet.Cells[rowIndex, startCol + 2] = GetString(point, "CRITERIA_MAX");
                sheet.Cells[rowIndex, startCol + 3] = GetEquipmentText(pointGroup);

                string totalJudge = "1";
                for (int i = 0; i < sampleNos.Count; i++)
                {
                    DataRow sampleRow = pointGroup.FirstOrDefault(row => GetString(row, "SAMPLING_NO") == sampleNos[i]);
                    if (sampleRow == null)
                    {
                        continue;
                    }

                    int col = startCol + 4 + (i * 2);
                    sheet.Cells[rowIndex, col] = GetString(sampleRow, "CAVITY_NAME");
                    sheet.Cells[rowIndex, col + 1] = GetString(sampleRow, "VALUE");
                    string judge = GetString(sampleRow, "POINT_JUDGE");
                    if (judge == "0")
                    {
                        totalJudge = "0";
                    }
                }

                sheet.Cells[rowIndex, startCol + 14] = totalJudge == "0" ? "NG" : "OK";
                rowIndex++;
            }

            Excel.Range tableRange = sheet.Range[sheet.Cells[tableTop, startCol], sheet.Cells[tableTop + 13, startCol + 14]];
            tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            tableRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            tableRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            tableRange.WrapText = true;
            ReleaseCom(tableRange);

            StyleFont(sheet, tableTop, startCol + 14, tableTop, startCol + 14, 8, true);

            Excel.Range firstColumns = sheet.Range[sheet.Cells[tableTop, startCol], sheet.Cells[tableTop + 13, startCol + 2]];
            firstColumns.Interior.Color = ColorTranslator.ToOle(Color.LightBlue);
            ReleaseCom(firstColumns);

            Excel.Range equipmentColumn = sheet.Range[sheet.Cells[tableTop, startCol + 3], sheet.Cells[tableTop + 13, startCol + 3]];
            equipmentColumn.Interior.Color = ColorTranslator.ToOle(Color.LightYellow);
            ReleaseCom(equipmentColumn);
        }

        private static string GetEquipmentText(System.Collections.Generic.IEnumerable<DataRow> rows)
        {
            if (rows == null)
            {
                return string.Empty;
            }

            return string.Join(", ", rows
                .Select(row => GetString(row, "EQUIPMENT_NAME"))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static void AddStampImage(Excel.Worksheet sheet, string stampImagePath)
        {
            if (string.IsNullOrWhiteSpace(stampImagePath) || !File.Exists(stampImagePath))
            {
                return;
            }

            string approveStartCell;
            string approveEndCell;
            if (IsCellText(sheet, "AB2", "Approve"))
            {
                approveStartCell = "AB3";
                approveEndCell = "AE4";
            }
            else if (IsCellText(sheet, "AA1", "Approve"))
            {
                approveStartCell = "AA2";
                approveEndCell = "AE3";
            }
            else
            {
                approveStartCell = "N2";
                approveEndCell = "P3";
            }

            Excel.Range approveCell = sheet.Range[approveStartCell, approveEndCell];
            float left = Convert.ToSingle(approveCell.Left);
            float top = Convert.ToSingle(approveCell.Top);
            float width = Convert.ToSingle(approveCell.Width);
            float height = Convert.ToSingle(approveCell.Height);
            sheet.Shapes.AddPicture(stampImagePath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, left + 4, top + 2, width - 8, height - 4);
            ReleaseCom(approveCell);
        }

        private static bool IsCellText(Excel.Worksheet sheet, string cellAddress, string expectedText)
        {
            Excel.Range cell = null;
            try
            {
                cell = sheet.Range[cellAddress];
                string value = cell.Value2?.ToString() ?? string.Empty;
                return string.Equals(value.Trim(), expectedText, StringComparison.OrdinalIgnoreCase);
            }
            finally
            {
                ReleaseCom(cell);
            }
        }

        private static void SetLabel(Excel.Worksheet sheet, int row1, int col1, int row2, int col2, string text)
        {
            Merge(sheet, row1, col1, row2, col2, text);
            Excel.Range range = sheet.Range[sheet.Cells[row1, col1], sheet.Cells[row2, col2]];
            range.Interior.Color = ColorTranslator.ToOle(Color.FromArgb(204, 255, 204));
            range.Font.Name = "Tahoma";
            range.Font.Size = 10;
            range.Font.Bold = true;
            ReleaseCom(range);
        }

        private static void SetMergedHeader(Excel.Worksheet sheet, int row, int col1, int col2, string text, Color backgroundColor, Color fontColor)
        {
            Merge(sheet, row, col1, row, col2, text);
            Excel.Range range = sheet.Range[sheet.Cells[row, col1], sheet.Cells[row, col2]];
            range.Font.Name = "Tahoma";
            range.Font.Size = 9;
            range.Font.Bold = true;
            range.Interior.Color = ColorTranslator.ToOle(backgroundColor);
            range.Font.Color = ColorTranslator.ToOle(fontColor);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            range.WrapText = true;
            ReleaseCom(range);
        }

        private static void StyleCheckPointHeader(Excel.Worksheet sheet, int row, int col1, int col2)
        {
            Excel.Range range = sheet.Range[sheet.Cells[row, col1], sheet.Cells[row, col2]];
            range.Font.Name = "Tahoma";
            range.Font.Size = 10;
            range.Font.Bold = false;
            range.Font.Color = ColorTranslator.ToOle(Color.Blue);
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            ReleaseCom(range);
        }

        private static void StylePageFont(Excel.Worksheet sheet, int topRow, int bottomRow)
        {
            Excel.Range range = sheet.Range[
                sheet.Cells[topRow, AutoReportStartColumn],
                sheet.Cells[bottomRow, AutoReportLastColumn]];
            range.Font.Name = "Tahoma";
            range.Font.Size = 10;
            range.Font.Bold = false;
            ReleaseCom(range);
        }

        private static void StyleFont(
            Excel.Worksheet sheet,
            int row1,
            int col1,
            int row2,
            int col2,
            double size,
            bool bold)
        {
            Excel.Range range = sheet.Range[sheet.Cells[row1, col1], sheet.Cells[row2, col2]];
            range.Font.Name = "Tahoma";
            range.Font.Size = size;
            range.Font.Bold = bold;
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

        private static void MergeLeft(Excel.Worksheet sheet, int row1, int col1, int row2, int col2, string text)
        {
            Excel.Range range = sheet.Range[sheet.Cells[row1, col1], sheet.Cells[row2, col2]];
            range.Merge();
            range.Value2 = text ?? string.Empty;
            range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
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
#pragma warning disable CA1416
                Marshal.ReleaseComObject(comObject);
#pragma warning restore CA1416
            }
        }
    }
}
