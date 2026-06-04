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
        private const string DefaultReportTitle = "Regular Inspection Record Sheet";
        private const bool ShowExcelDebugMessage = false;

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
            string pdfName = $"{SanitizeFileName(dataItem?.Regular_No ?? dataItem?.Report_No ?? "Regular_Report")}.pdf";
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
                int topRow = 1 + ((page - 1) * 37);
                var pageSamples = sampleNos.Skip(start).Take(5).ToList();
                string imagePath = GetImagePathForPage(regularImagePaths, page);
                BuildReportPage(sheet, dataItem, data, pageSamples, topRow, page, referenceText, imagePath);
            }

            ApplyPageSetup(sheet);
        }

        private static void ApplyPageSetup(Excel.Worksheet sheet)
        {
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

        private static void BuildReportPage(Excel.Worksheet sheet, QAdataProperty dataItem, DataTable data, System.Collections.Generic.List<string> sampleNos, int topRow, int page, string referenceText, string imagePath)
        {
            int imageTop = topRow + 8;
            int tableTop = topRow + 28;

            Merge(sheet, topRow, 1, topRow, 10, GetReportTitle(dataItem));
            SetLabel(sheet, topRow + 1, 1, topRow + 1, 3, "M-Code");
            Merge(sheet, topRow + 1, 4, topRow + 1, 10, dataItem?.M_CODE);
            SetLabel(sheet, topRow + 2, 1, topRow + 2, 3, "Material Name");
            Merge(sheet, topRow + 2, 4, topRow + 2, 10, dataItem?.Material_Name);
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
            Merge(sheet, imageTop + 1, 1, imageTop + 19, 16, string.Empty);
            AddReportImage(sheet, imagePath, imageTop + 1, 1, imageTop + 19, 16);

            BuildTable(sheet, data, sampleNos, tableTop);

            Excel.Range pageBorder = sheet.Range[sheet.Cells[topRow, 1], sheet.Cells[topRow + 35, 16]];
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
            if (!string.IsNullOrWhiteSpace(dataItem?.FORMAT_REPORT_NAME))
            {
                return dataItem.FORMAT_REPORT_NAME.Trim();
            }

            string configuredTitle = ConfigurationManager.AppSettings["RegularReportDefaultTitle"];
            return string.IsNullOrWhiteSpace(configuredTitle)
                ? DefaultReportTitle
                : configuredTitle.Trim();
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
            message.AppendLine("Cavity No <- CAVITY_NAME");
            message.AppendLine("Actual <- VALUE");
            message.AppendLine("Judg <- POINT_JUDGE summary");

            MessageBox.Show(message.ToString(), "Excel Debug", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void BuildTable(Excel.Worksheet sheet, DataTable data, System.Collections.Generic.List<string> sampleNos, int tableTop, int startCol = 1)
        {
            SetHeader(sheet, tableTop, startCol, "Point");
            SetHeader(sheet, tableTop, startCol + 1, "Min");
            SetHeader(sheet, tableTop, startCol + 2, "Max");

            for (int i = 0; i < 5; i++)
            {
                int col = startCol + 3 + (i * 2);
                SetHeader(sheet, tableTop, col, "Cavity\nNo");
                SetHeader(sheet, tableTop, col + 1, "Actual");
            }

            SetHeader(sheet, tableTop, startCol + 15, "Judg");

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

                string totalJudge = "1";
                for (int i = 0; i < sampleNos.Count; i++)
                {
                    DataRow sampleRow = pointGroup.FirstOrDefault(row => GetString(row, "SAMPLING_NO") == sampleNos[i]);
                    if (sampleRow == null)
                    {
                        continue;
                    }

                    int col = startCol + 3 + (i * 2);
                    sheet.Cells[rowIndex, col] = GetString(sampleRow, "CAVITY_NAME");
                    sheet.Cells[rowIndex, col + 1] = GetString(sampleRow, "VALUE");
                    string judge = GetString(sampleRow, "POINT_JUDGE");
                    if (judge == "0")
                    {
                        totalJudge = "0";
                    }
                }

                sheet.Cells[rowIndex, startCol + 15] = totalJudge == "0" ? "NG" : "OK";
                rowIndex++;
            }

            Excel.Range tableRange = sheet.Range[sheet.Cells[tableTop, startCol], sheet.Cells[tableTop + 13, startCol + 15]];
            tableRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
            tableRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            tableRange.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            tableRange.WrapText = true;
            ReleaseCom(tableRange);

            Excel.Range firstColumns = sheet.Range[sheet.Cells[tableTop, startCol], sheet.Cells[tableTop + 13, startCol + 2]];
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
#pragma warning disable CA1416
                Marshal.ReleaseComObject(comObject);
#pragma warning restore CA1416
            }
        }
    }
}
