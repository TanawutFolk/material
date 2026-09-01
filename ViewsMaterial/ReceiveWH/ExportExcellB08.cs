using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace RawMat.ViewsMaterial.ReceiveWH
{
    // วาดฟอร์ม FM-QA-B08-F : มุมซ้ายบน B2 มุมขวาล่าง AE66 (ฟอร์มเปล่า)
    // layout ตรงกับ Views\Excell Check Sheet\Generate_FM-QA-B08-F.ps1 ซึ่งใช้เป็นสเปก
    //
    // หลักการลงสี
    //   หัวข้อ          -> เขียว #CCFFCC
    //   ช่องกรอกข้อมูล   -> ขาว
    //   บล็อกตรวจสอบ     -> เทา เป็นค่าเริ่มต้น แล้วเปลี่ยนเป็นขาวเฉพาะบล็อกที่ M-CODE ต้องตรวจ
    //
    // บล็อกที่ยืดได้ : Function , Dimension , Appearance
    internal static class ExportExcellB08
    {
        private const string SheetName = "Master";
        private const string ReportTitle = "FM-QA-B08-F Receiving Inspection Check Sheet";
        private const string FontName = "Tahoma";

        private const int FirstFormRow = 2;

        private static readonly Color ColorWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color ColorBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color ColorBlue = Color.FromArgb(0, 0, 255);
        private static readonly Color ColorRed = Color.FromArgb(255, 0, 0);
        private static readonly Color ColorGreen = Color.FromArgb(204, 255, 204);
        private static readonly Color ColorGray = Color.FromArgb(191, 191, 191);
        private static readonly Color ColorJudgeOk = Color.FromArgb(0, 176, 80);
        private static readonly Color ColorJudgeNg = Color.FromArgb(255, 0, 0);

        private const int XlCenter = -4108;
        private const int XlLeft = -4131;
        private const int XlRight = -4152;
        private const int XlTop = -4160;
        private const int XlContinuous = 1;
        private const int XlThin = 2;
        private const int XlMedium = -4138;

        // ช่อง cavity ของฟอร์มเปล่า : 5 ช่อง ก้าวละ 3 คอลัมน์
        private static readonly string[,] DefaultSlots =
        {
            { "Q", "S" }, { "T", "V" }, { "W", "Y" }, { "Z", "AB" }, { "AC", "AE" }
        };

        private static Excel.Worksheet _sheet;

        // ขอบขวาของฟอร์ม ปกติคือ AE แต่แบบ Cavity ขยายข้างจะไกลกว่านั้น
        private static string _lastColumn = "AE";

        public static string CreateCheckSheet(QAdataProperty dataItem, B08CheckSheetContent content, string filePath)
        {
            if (content == null)
            {
                content = new B08CheckSheetContent();
            }

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet sheet = null;

            try
            {
                excelApp = new Excel.Application
                {
                    DisplayAlerts = false,
                    Visible = false,
                    ScreenUpdating = false
                };

                workbook = excelApp.Workbooks.Add();
                sheet = (Excel.Worksheet)workbook.Worksheets[1];
                sheet.Name = SheetName;
                _sheet = sheet;

                BuildSheet(sheet, dataItem, content);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                workbook.SaveAs(filePath, Excel.XlFileFormat.xlOpenXMLWorkbook);
                return filePath;
            }
            finally
            {
                _sheet = null;
                workbook?.Close(false);
                excelApp?.Quit();
                ReleaseCom(sheet);
                ReleaseCom(workbook);
                ReleaseCom(excelApp);
            }
        }

        private static void BuildSheet(Excel.Worksheet sheet, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            sheet.Cells.Font.Name = FontName;
            sheet.Cells.Font.Size = 9;
            sheet.Cells.VerticalAlignment = XlCenter;
            sheet.Cells.WrapText = false;

            _lastColumn = LastFormColumn(content);
            ApplyColumnWidths(sheet, content);

            int row = FirstFormRow;
            row = RenderHeader(dataItem, content, row);
            row = RenderPacking(content, row);
            row = RenderLotNo(content, row);
            row = RenderRegular(content, row);
            row = RenderFunction(content, row);
            row = RenderDimension(content, row);
            row = RenderAppearance(content, row);
            row = RenderFinalJudgement(row);
            row = RenderReferArea(content, row);
            row = RenderPendingDetail(content, row);

            SetOuterBorder($"B{FirstFormRow}:{LastFormColumn(content)}{row - 1}", ColorBlue, XlMedium);
        }

        // ความกว้าง : A ขอบซ้าย , B..J = 2.86 , K = 4.57 , L..P = 2.86 , Q เป็นต้นไป = 2.71
        // แบบ All ช่องค่าเป็นคอลัมน์เดี่ยวไม่ merge ต้องกว้าง 3.57 ไม่งั้นตัวเลขขึ้น ##
        private static void ApplyColumnWidths(Excel.Worksheet sheet, B08CheckSheetContent content)
        {
            bool wideCavity = content.Template == B08Template.WideCavity;
            double valueWidth = content.Template == B08Template.DimensionAll ? 3.57 : (wideCavity ? 4.43 : 2.71);

            int lastColumn = ColumnIndex(LastFormColumn(content));

            SetColumnWidth(sheet, 1, 2.0);

            for (int column = 2; column <= 60; column++)
            {
                double width;

                if (column == 11) { width = 4.57; }
                else if (column == 3 && wideCavity) { width = 3.86; }
                else if (column <= 16) { width = 2.86; }
                else if (column <= lastColumn) { width = valueWidth; }
                else { width = 2.86; }   // พ้นขอบฟอร์มแล้วกลับไปความกว้างปกติ

                SetColumnWidth(sheet, column, width);
            }
        }

        private static string LastFormColumn(B08CheckSheetContent content)
        {
            int slots = Math.Max(content.CavitySlots, 1);

            // ผังฟอร์มเปล่าใช้ช่องละ 3 คอลัมน์จบที่ AE , เกิน 5 ช่องจะไล่ทีละ 2 จาก Q
            return slots <= DefaultSlots.GetLength(0) ? "AE" : ColumnLetter(16 + (slots * 2));
        }

        // address ในโค้ดอ้าง AE เป็นขอบขวาเสมอ ตอนฟอร์มขยายต้องเลื่อนตามคอลัมน์สุดท้ายจริง
        private static int ColumnIndex(string letters)
        {
            int index = 0;
            foreach (char letter in letters)
            {
                index = (index * 26) + (letter - 'A' + 1);
            }

            return index;
        }

        private static string ExpandAddress(string address)
        {
            if (_lastColumn == "AE")
            {
                return address;
            }

            int colon = address.LastIndexOf(':');
            string tail = colon < 0 ? address : address.Substring(colon + 1);

            if (colon < 0 || !tail.StartsWith("AE") || tail.Length < 3 || !char.IsDigit(tail[2]))
            {
                return address;
            }

            return address.Substring(0, colon + 1) + _lastColumn + tail.Substring(2);
        }

        // ---------- R2-R9 : หัวเอกสาร ----------

        private static int RenderHeader(QAdataProperty dataItem, B08CheckSheetContent content, int row)
        {
            SetRowHeight(row, 18);
            SetBlock($"B{row}:U{row}", ReportTitle, ColorWhite, ColorBlack, 14, true, false, XlLeft, XlCenter, false, false);
            SetBlock($"V{row}:AA{row}", "Report No.", ColorGreen, ColorBlack, 10);
            SetBlock($"AB{row}:AE{row}", "Approve", ColorGreen, ColorBlack, 10);
            row++;

            SetRowHeight(row, 18);
            SetBlock($"B{row}:G{row}", Value(dataItem?.M_CODE), ColorWhite, ColorBlue, 14, false, true, XlLeft, XlCenter, false, false);
            SetBlock($"H{row}:U{row}", Value(dataItem?.Material_Name), ColorWhite, ColorBlue, 14, false, true, XlLeft, XlCenter, false, false, true);
            SetBlock($"V{row}:AA{row + 1}", Value(dataItem?.Report_No), ColorWhite, ColorBlue, 18);
            row++;

            SetRowHeight(row, 12.8);
            SetBlock($"B{row}:U{row}", Barcode(dataItem?.M_CODE), ColorWhite, ColorBlack, 10, false, false, XlLeft, XlCenter, false, false);
            SetBlock($"AB{row - 1}:AE{row}", string.Empty, ColorWhite); // ช่องแปะรูป Stamp คร่อม 2 แถว
            row++;

            SetRowHeight(row, 15);
            SetBlock($"B{row}:D{row}", "Receive Date", ColorGreen, ColorBlack, 8, true);
            SetBlock($"E{row}:K{row}", FormatDate(dataItem?.Receive_Date), ColorWhite, ColorBlue, 11, false, false, XlLeft, XlCenter, true);
            SetBlock($"L{row}:N{row + 1}", "INV. No.", ColorGreen, ColorBlack, 10, true, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"O{row}:U{row}", Value(dataItem?.Invoice_No), ColorWhite, ColorBlue, 11, false, false, XlLeft);
            SetBlock($"V{row}:X{row}", "Lot Size.", ColorGreen, ColorBlack, 10, true);
            SetBlock($"Y{row}:AC{row}", Value(dataItem?.Qty), ColorWhite, ColorBlue, 9);
            SetBlock($"AD{row}:AE{row}", "Pcs", ColorWhite, ColorBlack, 9);
            RemoveVerticalDivider($"Y{row}:AC{row}", $"AD{row}:AE{row}");
            row++;

            SetRowHeight(row, 15);
            SetBlock($"B{row}:D{row}", "Vender", ColorGreen, ColorBlack, 9, true);
            SetBlock($"E{row}:K{row}", Value(dataItem?.Vendor_Name), ColorWhite, ColorBlue, 9, false, false, XlLeft);
            SetBlock($"O{row}:U{row}", Barcode(dataItem?.Invoice_No), ColorWhite, ColorBlack, 9);
            SetBlock($"V{row}:X{row}", "Issue by", ColorGreen, ColorBlack, 9, true);
            SetBlock($"Y{row}:AA{row}", Fallback(content.IssueBy, "O/P WH"), ColorWhite, ColorBlue, 9);
            SetBlock($"AB{row}:AE{row}", Fallback(content.IssueTime, "Issue Time"), ColorWhite, ColorBlue, 8, false, false, XlCenter, XlCenter, true);
            RemoveVerticalDivider($"Y{row}:AA{row}", $"AB{row}:AE{row}");
            row++;

            SetRowHeight(row, 3.8); // แถวคั่น รวมเป็นช่องเดียว ไม่มีเส้น
            SetBlock($"B{row}:AE{row}", string.Empty, ColorWhite, ColorBlack, 9, false, false, XlCenter, XlCenter, false, false);
            row++;

            SetRowHeight(row, 15);
            SetBlock($"B{row}:D{row}", "Ins. Date", ColorGreen, ColorBlack, 9, true);
            SetBlock($"E{row}:M{row}", Value(content.InspectionDate), ColorWhite, ColorBlue, 9, false, false, XlCenter, XlCenter, true);
            SetBlock($"N{row}:P{row}", "Inspector", ColorGreen, ColorBlack, 10, true);
            SetBlock($"Q{row}:AE{row}", Value(content.InspectorName), ColorWhite, ColorBlue, 9, false, false, XlLeft);
            row++;

            SetRowHeight(row, 15);
            SetBlock($"B{row}:D{row}", "Item", ColorGreen, ColorBlack, 10, true);
            SetBlock($"E{row}:M{row}", "Content", ColorGreen, ColorBlack, 10, true);
            SetBlock($"N{row}:P{row}", "Method", ColorGreen, ColorBlack, 10, true);
            SetBlock($"Q{row}:AE{row}", "Judgement", ColorGreen, ColorBlack, 10, true);
            row++;

            return row;
        }

        // ---------- R10-R15 : บรรจุภัณฑ์ ----------

        private static int RenderPacking(B08CheckSheetContent content, int row)
        {
            int firstRow = row;
            int lastRow = row + 5;

            double[] heights = { 12.8, 12.8, 13.5, 12.8, 13.5, 12.8 };
            for (int offset = 0; offset < heights.Length; offset++)
            {
                SetRowHeight(firstRow + offset, heights[offset]);
            }

            SetBlock($"B{firstRow}:D{lastRow}", "บรรจุภัณฑ์", ColorWhite, ColorBlack, 9, false, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"N{firstRow}:P{lastRow}", "ตาเปล่า", ColorWhite, ColorBlack, 9, false, false, XlCenter, XlCenter, false, true, true);

            string[] methods =
            {
                "กล่อง/ถุง อยู่ในสภาพสมบูรณ์ไม่บุบ ยุบหรือฉีกขาด",
                "ชื่อของชิ้นงานที่ได้รับตรงกับชิ้นงานจริงในกล่องและตรงกับป้ายแสดงข้างกล่อง",
                "จำนวนที่ได้รับตรงกับจำนวนที่แสดงในช่อง Lot Size และตรงกับป้ายแสดงข้างกล่อง"
            };

            for (int index = 0; index < 3; index++)
            {
                int topRow = firstRow + (index * 2);
                SetBlock($"E{topRow}:M{topRow + 1}", methods[index], ColorWhite, ColorBlack, 9, false, false, XlLeft, XlCenter, false, true, true);
            }

            SetBlock($"Q{firstRow}:AE{firstRow}", PackingText(content, 0), ColorWhite, ColorBlue, 9);
            SetBlock($"Q{firstRow + 1}:AE{firstRow + 1}", "อาการ NG", ColorWhite, ColorBlack, 8, false, false, XlLeft);
            SetBlock($"Q{firstRow + 2}:AE{firstRow + 2}", PackingText(content, 1), ColorWhite, ColorBlue, 9);
            SetBlock($"Q{firstRow + 3}:AE{firstRow + 3}", "อาการ NG", ColorWhite, ColorBlack, 8, false, false, XlLeft);

            SetBlock($"Q{firstRow + 4}:S{lastRow}", "ขนาดบรรจุ", ColorWhite, ColorBlack, 8, false, false, XlLeft);
            SetBlock($"T{firstRow + 4}:AE{firstRow + 4}", Value(content.PackingSizeJudgement), ColorWhite, ColorBlue, 9);
            SetBlock($"T{lastRow}:AE{lastRow}", Value(content.PackingSizeText), ColorWhite, ColorBlue, 9);

            // ช่องผลตัดสินกับบรรทัด "อาการ NG" ใต้มันเป็นกล่องเดียวกัน ไม่ต้องมีเส้นคั่น
            RemoveHorizontalDivider($"Q{firstRow}:AE{firstRow}", $"Q{firstRow + 1}:AE{firstRow + 1}");
            RemoveHorizontalDivider($"Q{firstRow + 2}:AE{firstRow + 2}", $"Q{firstRow + 3}:AE{firstRow + 3}");
            RemoveHorizontalDivider($"T{firstRow + 4}:AE{firstRow + 4}", $"T{lastRow}:AE{lastRow}");

            return lastRow + 1;
        }

        private static int RenderLotNo(B08CheckSheetContent content, int row)
        {
            SetRowHeight(row, 15);
            SetBlock($"B{row}:D{row}", "Lot No.", ColorWhite, ColorBlack, 9);
            SetBlock($"E{row}:AE{row}", Value(content.LotNoText), ColorWhite, ColorBlue, 9, false, false, XlCenter, XlCenter, true);
            return row + 1;
        }

        // ---------- R17-R18 : Regular (สรุปเท่านั้น รายละเอียดอยู่ใน FM-QA-B13-A) ----------

        private static int RenderRegular(B08CheckSheetContent content, int row)
        {
            Color fill = CheckFill(content.RegularEnabled);
            int firstRow = row;
            int lastRow = row + 1;

            SetRowHeight(firstRow, 12.8);
            SetRowHeight(lastRow, 12.8);

            SetBlock($"B{firstRow}:D{firstRow}", "Regular", fill, ColorBlue, 9, false, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"B{lastRow}:D{lastRow}", "Inspection", fill, ColorBlue, 9);
            SetBlock($"E{firstRow}:P{lastRow}", string.Empty, fill, ColorBlue, 9, false, false, XlLeft, XlTop);
            SetBlock($"Q{firstRow}:T{firstRow}", "Regular check", fill, ColorBlue, 9, false, false, XlLeft);
            SetBlock($"U{firstRow}:AE{firstRow}", string.Empty, fill, ColorBlue, 9, false, false, XlLeft);
            SetBlock($"Q{lastRow}:T{lastRow}", "Scrap Q'ty :", fill, ColorBlue, 9, false, false, XlLeft);
            SetBlock($"U{lastRow}:AE{lastRow}", string.Empty, fill, ColorBlue, 9, false, false, XlLeft);

            return lastRow + 1;
        }

        // ---------- R19-R21 : Function ----------

        private static int RenderFunction(B08CheckSheetContent content, int row)
        {
            Color fill = CheckFill(content.FunctionEnabled);
            int groups = Math.Max(content.FunctionGroups, 1);
            string[,] slots = BuildSlots(content.CavitySlots);
            int slotCount = slots.GetLength(0);

            int firstRow = row;
            int lastRow = row + (groups * 2) - 1;
            int judgementRow = lastRow + 1;

            SetRowHeight(firstRow, 12.8);
            for (int r = firstRow + 1; r <= judgementRow; r++)
            {
                SetRowHeight(r, 15);
            }

            SetBlock($"B{firstRow}:D{firstRow}", "Function", fill, ColorBlack, 9);
            SetBlock($"B{firstRow + 1}:D{judgementRow}", "Inspection Level", fill, ColorBlack, 9, false, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"E{firstRow}:M{lastRow}", Nz(content.FunctionMethodText), fill, ColorBlue, 9, false, false, XlLeft, XlTop);
            SetBlock($"N{firstRow}:P{lastRow}", Nz(content.FunctionEquipmentText), fill, ColorBlack, 8, false, false, XlCenter, XlCenter, false, true, true);

            for (int group = 0; group < groups; group++)
            {
                int headerRow = firstRow + (group * 2);

                for (int index = 0; index < slotCount; index++)
                {
                    B08FunctionSample sample = ItemAt(content.FunctionSamples, (group * slotCount) + index);
                    SetSlot(slots, index, headerRow, SlotText(sample?.CavityName, "Cavity"), fill);
                    SetSlot(slots, index, headerRow + 1, SlotText(sample?.Judge, "OK . NG"), fill);
                }
            }

            SetBlock($"E{judgementRow}:P{judgementRow}", "Function Judgement", fill, ColorBlack, 9);
            SetBlock($"Q{judgementRow}:AE{judgementRow}", JudgementText(content.FunctionJudge), fill, ColorBlack, 9);

            return judgementRow + 1;
        }

        // ---------- R22-R29 : Dimension ----------
        // R22 หัวบล็อก + Equipment SN , R23 หัว Cavity , R24-26 จุดวัด , R27 OK.NG , R28-29 สรุป (เหลือง)

        private static int RenderDimension(B08CheckSheetContent content, int row)
        {
            Color fill = CheckFill(content.DimensionEnabled);

            if (content.Template == B08Template.DimensionAll)
            {
                return RenderDimensionAll(content, fill, row);
            }

            int groups = Math.Max(content.DimensionGroups, 1);
            int pointRows = Math.Max(content.DimensionPointRows, 1);
            string[,] slots = BuildSlots(content.CavitySlots);
            int slotCount = slots.GetLength(0);
            bool hasPoints = content.DimensionPoints.Count > 0;

            SetRowHeight(row, 15.8);
            SetBlock($"B{row}:D{row}", "Dimension", fill, ColorBlack, 10);
            SetBlock($"E{row}:P{row}", Nz(content.DimensionEquipmentText), fill, ColorBlack, 9, false, false, XlLeft);
            SetBlock($"Q{row}:AE{row}", string.Empty, fill);
            row++;

            int bodyFirstRow = row;

            for (int group = 0; group < groups; group++)
            {
                int headerRow = row;
                int firstPointRow = headerRow + 1;
                int okRow = headerRow + pointRows + 1;

                for (int r = headerRow; r <= okRow; r++)
                {
                    SetRowHeight(r, 15.8);
                }

                if (hasPoints)
                {
                    // มี master จุดวัด : แตกเป็นชื่อจุด / เกณฑ์ / คำอธิบาย ทีละแถว
                    bool wideCavity = content.Template == B08Template.WideCavity;

                    SetBlock($"E{headerRow}:M{headerRow}", string.Empty, fill, ColorBlack, 9, false, false, XlCenter);
                    SetBlock($"N{headerRow}:P{headerRow}", "Method", ColorGreen, ColorBlack, 9);

                    for (int point = 0; point < pointRows; point++)
                    {
                        int pointRow = firstPointRow + point;
                        B08DimensionPoint master = ItemAt(content.DimensionPoints, point);

                        if (wideCavity)
                        {
                            // แบบขยายข้าง : ช่องซ้ายแคบลงเหลือ E:F แล้วยกเกณฑ์ไปกิน G:M
                            SetBlock($"E{pointRow}:F{pointRow}", Nz(master?.PointName), fill, ColorBlack, 9);
                            SetBlock($"G{pointRow}:M{pointRow}", Nz(master?.Criteria), fill, ColorBlack, 9);
                        }
                        else
                        {
                            SetBlock($"E{pointRow}", Nz(master?.PointName), fill, ColorBlack, 9, false, false, XlCenter);
                            SetBlock($"F{pointRow}:H{pointRow}", Nz(master?.Criteria), fill, ColorBlack, 9, false, false, XlCenter);
                            SetBlock($"I{pointRow}:M{pointRow}", string.Empty, fill, ColorBlue, 9, false, false, XlLeft);
                        }
                    }

                    RenderEquipmentRuns(content.DimensionPoints, firstPointRow, pointRows, fill);

                    SetBlock($"E{okRow}:M{okRow}", string.Empty, fill, ColorBlack, 9, false, false, XlCenter);
                    SetBlock($"N{okRow}:P{okRow}", string.Empty, fill, ColorBlack, 9);
                }
                else
                {
                    SetBlock($"E{headerRow}:M{okRow}", string.Empty, fill, ColorBlue, 9, false, false, XlLeft, XlTop);
                    SetBlock($"N{headerRow}:P{okRow}", string.Empty, fill, ColorBlack, 9, false, false, XlCenter, XlCenter, false, true, true);
                }

                for (int index = 0; index < slotCount; index++)
                {
                    B08DimensionSample sample = ItemAt(content.DimensionSamples, (group * slotCount) + index);
                    SetSlot(slots, index, headerRow, SlotText(sample?.CavityName, "Cavity"), fill);

                    for (int point = 0; point < pointRows; point++)
                    {
                        B08DimensionPoint master = ItemAt(content.DimensionPoints, point);
                        SetSlot(slots, index, firstPointRow + point, MeasuredValue(sample, master, point), fill);
                    }

                    SetSlot(slots, index, okRow, SlotText(sample?.Judge, "OK . NG"), fill);
                }

                row = okRow + 1;
            }

            int summaryRow = row;
            SetRowHeight(summaryRow, 15.8);
            SetRowHeight(summaryRow + 1, 15.8);

            SetBlock($"B{bodyFirstRow}:D{summaryRow + 1}", "Inspection Level", fill, ColorBlack, 10, false, false, XlCenter, XlCenter, false, true, true);

            SetBlock($"E{summaryRow}:P{summaryRow}", "ผลการวัดที่ได้จากผู้ผลิตต้องผ่านเกณฑ์", fill, ColorBlack, 9);
            SetBlock($"Q{summaryRow}:AE{summaryRow}", "Accept  .  Reject", fill, ColorBlack, 9);
            SetBlock($"E{summaryRow + 1}:P{summaryRow + 1}", "Dimension Judgement", fill, ColorBlack, 9);
            SetBlock($"Q{summaryRow + 1}:AE{summaryRow + 1}", JudgementText(content.DimensionJudge), fill, ColorBlack, 9);

            return summaryRow + 2;
        }

        // แบบ All : จุดวัดเป็นแถว ชิ้นงานเป็นคอลัมน์ บล็อกละ 15 ชิ้น เกินกว่านั้นซ้ำบล็อกลงล่างจนครบ Lot Size
        private const int AllModeColumnsPerPage = 15;

        // กัน Lot Size ที่ผิดปกติไม่ให้ลากบล็อกจนไฟล์บวม ของจริงหลักสิบชิ้น
        private const int AllModeMaxPieces = 1000;

        private const double AllModeFontSize = 7;

        private static readonly string[] CircledNumbers =
        {
            "①", "②", "③", "④", "⑤", "⑥", "⑦", "⑧", "⑨",
            "⑩", "⑪", "⑫", "⑬", "⑭", "⑮", "⑯", "⑰", "⑱", "⑲", "⑳"
        };

        private static int RenderDimensionAll(B08CheckSheetContent content, Color fill, int row)
        {
            int pointRows = Math.Max(content.DimensionPointRows, 1);
            int pieces = Math.Min(Math.Max(content.PieceCount, 1), AllModeMaxPieces);
            int blocks = (int)Math.Ceiling(pieces / (double)AllModeColumnsPerPage);

            for (int block = 0; block < blocks; block++)
            {
                int firstPiece = block * AllModeColumnsPerPage;
                int columns = Math.Min(AllModeColumnsPerPage, pieces - firstPiece);
                row = RenderDimensionAllBlock(content, fill, row, pointRows, firstPiece, columns);
            }

            return row;
        }

        // 1 บล็อก = หัวบล็อก + แถวจุดวัด + Difference + Judgement
        private static int RenderDimensionAllBlock(
            B08CheckSheetContent content, Color fill, int headerRow, int pointRows, int firstPiece, int columns)
        {
            int firstPointRow = headerRow + 1;
            int lastPointRow = headerRow + pointRows;
            int differenceRow = lastPointRow + 1;
            int judgementRow = differenceRow + 1;

            SetRowHeight(headerRow, 15);
            for (int r = firstPointRow; r <= judgementRow; r++)
            {
                SetRowHeight(r, 14.25);
            }

            SetBlock($"B{headerRow}:D{judgementRow}", "Dimension Inspection Level All", fill, ColorBlack, 10, false, false, XlCenter, XlCenter, false, true, true);

            // E ไม่ merge ปล่อยข้อความล้นไปทับ F..I เหมือนต้นฉบับ
            SetBlock($"E{headerRow}", EquipmentSerialLabel(content), fill, ColorBlack, 10, false, false, XlLeft, XlCenter, false, false);
            SetBlock($"J{headerRow}:M{headerRow}", Nz(content.DimensionEquipmentSerial), fill, ColorBlack, 10);
            SetBlock($"N{headerRow}:P{headerRow}", "Method", ColorGreen, ColorBlack, 10);

            SetBlock($"G{firstPointRow}:M{lastPointRow}", DifferenceCriteriaText(content), fill, ColorBlack, 10, false, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"N{firstPointRow}:P{lastPointRow}", DistinctEquipmentText(content.DimensionPoints), fill, ColorBlack, 10);

            for (int point = 0; point < pointRows; point++)
            {
                SetBlock($"E{firstPointRow + point}:F{firstPointRow + point}", CircledNumber(point), fill, ColorBlack, 10);
            }

            SetBlock($"E{differenceRow}:P{differenceRow}", "Difference", fill, ColorBlack, 10);
            SetBlock($"E{judgementRow}:P{judgementRow}", "Judgement", fill, ColorBlack, 10);

            decimal tolerance = DifferenceTolerance(content);

            for (int column = 0; column < columns; column++)
            {
                string letter = ColumnLetter(17 + column);
                B08DimensionSample sample = SampleByNumber(content.DimensionSamples, firstPiece + column + 1);

                SetBlock($"{letter}{headerRow}", $"No.{firstPiece + column + 1}", fill, ColorBlack, AllModeFontSize);

                for (int point = 0; point < pointRows; point++)
                {
                    B08DimensionPoint master = ItemAt(content.DimensionPoints, point);
                    SetBlock($"{letter}{firstPointRow + point}", MeasuredValue(sample, master, point), fill, ColorBlue, AllModeFontSize);
                }

                // ค่าที่บันทึกไว้ตอนตรวจคือผลตัดสินจริงที่ QA เซ็นรับ จึงวางเป็นค่า ไม่ใช่สูตร
                // ถ้าใส่สูตร ใครแก้ตัวเลขในไฟล์ผลจะพลิกทันที เอกสารที่เซ็นแล้วจะไม่ตรงกับของจริง
                // ใบเก่าที่ยังไม่มีค่าบันทึกไว้ ให้ Excel คิดให้เหมือนเดิมไปก่อน
                bool hasSavedJudge = sample != null && !string.IsNullOrEmpty(sample.PieceJudge);

                SetBlock($"{letter}{differenceRow}",
                    hasSavedJudge
                        ? Nz(sample.Difference)
                        : $"=MAX({letter}{firstPointRow}:{letter}{lastPointRow})-MIN({letter}{firstPointRow}:{letter}{lastPointRow})",
                    fill, ColorBlack, AllModeFontSize);

                // ปล่อย General ไว้ ช่องแคบจะย่อ 0.24 เหลือ 0.2 และทศนิยมลอยจาก MAX-MIN จะโผล่มา
                SetNumberFormat($"{letter}{differenceRow}", "0.00");

                SetBlock($"{letter}{judgementRow}",
                    hasSavedJudge
                        ? sample.PieceJudge
                        : DifferenceJudgementFormula(letter, lastPointRow, differenceRow, tolerance),
                    fill, ColorBlack, AllModeFontSize);
            }

            ApplyJudgementColors($"Q{judgementRow}:{ColumnLetter(16 + columns)}{judgementRow}");

            return judgementRow + 1;
        }

        private static string EquipmentSerialLabel(B08CheckSheetContent content)
        {
            string name = DistinctEquipmentText(content.DimensionPoints);
            return (name.Length == 0 ? "Equipment" : name.ToUpperInvariant()) + " S/N :";
        }

        private static string CircledNumber(int index)
        {
            return index < CircledNumbers.Length ? CircledNumbers[index] : (index + 1).ToString();
        }

        // เกณฑ์แบบ All ตัดสินที่ผลต่างของ 9 จุดในชิ้นเดียวกัน ไม่ใช่ค่ารายจุด
        // ครึ่งหนึ่งของช่วง MIN-MAX คือค่า ± รอบค่ากลาง ซึ่งคือเกณฑ์ผลต่างที่ยอมได้
        private static decimal DifferenceTolerance(B08CheckSheetContent content)
        {
            // ต้องข้ามจุดที่ตัดสิน OK/NG ไม่งั้นจะได้ (1-1)/2 = 0 แล้วไม่ตัดสินอะไรเลย
            // อ่านจากค่าที่ตั้งไว้ ใช้ตัวเดียวกับหน้ากรอกค่า ดู Utilities/PointJudgeType.cs
            foreach (B08DimensionPoint point in content.DimensionPoints)
            {
                if (Utilities.PointJudgeType.IsPassFail(point.JudgeType, point.CriteriaMin, point.CriteriaMax)) { continue; }
                if (!decimal.TryParse(point.CriteriaMin, out decimal min)) { continue; }
                if (!decimal.TryParse(point.CriteriaMax, out decimal max)) { continue; }

                return (max - min) / 2;
            }

            return 0;
        }

        private static string DifferenceCriteriaText(B08CheckSheetContent content)
        {
            decimal tolerance = DifferenceTolerance(content);
            if (tolerance <= 0) { return string.Empty; }

            B08DimensionPoint first = ItemAt(content.DimensionPoints, 0);
            string unit = first == null ? string.Empty : Nz(first.Unit);

            // เทียบเป็นนิ้วให้เฉพาะตอนหน่วยเป็น mm เท่านั้น หน่วยอื่นแปลงไม่ได้
            if (unit == "mm")
            {
                return $"Dif (MAX-MIN) ≤ {tolerance:0.######} mm ({tolerance / 25.4m:0.######} in)";
            }

            return $"Dif (MAX-MIN) ≤ {tolerance:0.####}{(unit.Length == 0 ? string.Empty : " " + unit)}";
        }

        private static string DifferenceJudgementFormula(string letter, int lastPointRow, int differenceRow, decimal tolerance)
        {
            if (tolerance <= 0) { return string.Empty; }

            string toleranceText = tolerance.ToString("0.######", CultureInfo.InvariantCulture);
            return $"=IF({letter}{lastPointRow}=\"\",\"\",IF({letter}{differenceRow}>{toleranceText},\"NG\",\"OK\"))";
        }

        private static B08DimensionSample SampleByNumber(List<B08DimensionSample> samples, int samplingNo)
        {
            return samples.FirstOrDefault(s => s.SamplingNo == samplingNo);
        }


        // ---------- R30-R36 : Appearance ----------

        private static int RenderAppearance(B08CheckSheetContent content, int row)
        {
            Color fill = CheckFill(content.AppearanceEnabled);
            int dataRows = Math.Max(content.AppearanceRowCount, 1);

            int qtyRow = row;
            int headerRow = row + 1;
            int dataFirstRow = row + 2;
            int dataLastRow = dataFirstRow + dataRows - 1;
            int judgementRow = dataLastRow + 1;

            for (int r = qtyRow; r <= dataLastRow; r++)
            {
                SetRowHeight(r, 15.8);
            }

            SetBlock($"B{qtyRow}:D{qtyRow}", "Appearance", fill, ColorBlack, 10);
            SetBlock($"B{headerRow}:D{judgementRow}", "Inspection Level", fill, ColorBlack, 10, false, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"E{qtyRow}:M{dataLastRow}", Value(content.ReferenceText), fill, ColorBlue, 9, false, false, XlLeft, XlTop);
            SetBlock($"N{qtyRow}:P{dataLastRow}", string.Empty, fill, ColorBlack, 9, false, false, XlCenter, XlCenter, false, true, true);
            SetBlock($"Q{qtyRow}:AE{qtyRow}", AppearanceQtyText(content), fill, ColorBlack, 10, false, false, XlLeft);

            SetBlock($"Q{headerRow}:S{headerRow}", "Date", fill, ColorBlack, 9);
            SetBlock($"T{headerRow}:V{headerRow}", "Ope.", fill, ColorBlack, 9);
            SetBlock($"W{headerRow}:Y{headerRow}", "Check Q'ty", fill, ColorBlack, 9);
            SetBlock($"Z{headerRow}:AB{headerRow}", "OK", fill, ColorBlack, 9);
            SetBlock($"AC{headerRow}:AE{headerRow}", "Pending", fill, ColorBlack, 9);

            for (int index = 0; index < dataRows; index++)
            {
                int dataRow = dataFirstRow + index;
                B08AppearanceRow source = index < content.AppearanceRows.Count ? content.AppearanceRows[index] : null;

                SetBlock($"Q{dataRow}:S{dataRow}", Value(source?.Date), fill, ColorBlue, 9, false, false, XlCenter, XlCenter, true);
                SetBlock($"T{dataRow}:V{dataRow}", Value(source?.Operator), fill, ColorBlue, 8, false, false, XlLeft);
                SetBlock($"W{dataRow}:Y{dataRow}", Value(source?.CheckQty), fill, ColorBlue, 9);
                SetBlock($"Z{dataRow}:AB{dataRow}", Value(source?.OkQty), fill, ColorBlue, 9, false, false, XlLeft);
                SetBlock($"AC{dataRow}:AE{dataRow}", Value(source?.PendingQty), fill, ColorBlue, 9, false, false, XlLeft);

                // ช่องติ๊กจริง (Form Control) วางชิดขวาของแต่ละช่อง ไม่ใช่ตัวอักษร ☐
                AddCheckBox($"T{dataRow}:V{dataRow}", "OK");
                AddCheckBox($"Z{dataRow}:AB{dataRow}", "P");
                AddCheckBox($"AC{dataRow}:AE{dataRow}", "P");
            }

            SetRowHeight(judgementRow, 14.2);
            SetBlock($"E{judgementRow}:P{judgementRow}", "Appearance Judgement", fill, ColorBlack, 9);
            SetBlock($"Q{judgementRow}:AE{judgementRow}", "Accept  .  Reject", fill, ColorBlack, 9);

            return judgementRow + 1;
        }

        // ---------- R37-R38 ----------

        private static int RenderFinalJudgement(int row)
        {
            SetRowHeight(row, 14.2);
            SetBlock($"B{row}:P{row}", "Final Judgement :", ColorWhite, ColorBlack, 10, false, false, XlLeft);
            SetBlock($"Q{row}:AE{row}", "OK (        Pcs).      Pending (        Pcs).      Scrap (        Pcs).", ColorWhite, ColorBlack, 10, false, false, XlLeft);
            row++;

            SetRowHeight(row, 14.2);
            SetBlock($"B{row}:E{row}", "Check point :", ColorWhite, ColorBlue, 10, false, false, XlLeft);
            SetBlock($"F{row}:AE{row}", "ทำการตรวจสอบชิ้นงานตัวแรก และทำเครื่องหมาย ☑ OK ที่ช่อง Ope. ถ้าชิ้นงานมีลักษณะตรงตามจุดเช็คที่กำหนด", ColorWhite, ColorRed, 9, false, false, XlLeft);

            return row + 1;
        }

        // ---------- R39-R60 : พื้นที่แปะรูป (merge ช่องเดียว ไม่มีเส้นแบ่งข้างใน) ----------

        private static int RenderReferArea(B08CheckSheetContent content, int row)
        {
            int firstRow = row;
            int lastRow = row + 21;

            for (int r = firstRow; r <= lastRow; r++)
            {
                SetRowHeight(r, 14.2);
            }

            SetBlock($"B{firstRow}:AE{lastRow}", Fallback(content.ReferenceText, "Refer ST-QA-B30- Vender Inspection Report List"),
                ColorWhite, ColorBlack, 10);

            return lastRow + 1;
        }

        // ---------- R61-R66 : Pending detail ----------

        private static int RenderPendingDetail(B08CheckSheetContent content, int row)
        {
            int headerRow = row;
            SetRowHeight(headerRow, 12.8);

            SetBlock($"B{headerRow}:I{headerRow}", "Pending detail", ColorGreen, ColorBlack, 9);
            SetBlock($"J{headerRow}:K{headerRow}", "Q'ty", ColorGreen, ColorBlack, 9);
            SetBlock($"L{headerRow}:M{headerRow}", "OK Q'ty", ColorGreen, ColorBlack, 9);
            SetBlock($"N{headerRow}:O{headerRow}", "NG Q'ty", ColorGreen, ColorBlack, 9);
            SetBlock($"P{headerRow}:W{headerRow}", "Pending detail", ColorGreen, ColorBlack, 9);
            SetBlock($"X{headerRow}:Y{headerRow}", "Q'ty", ColorGreen, ColorBlack, 9);
            SetBlock($"Z{headerRow}:AA{headerRow}", "OK Q'ty", ColorGreen, ColorBlack, 9);
            SetBlock($"AB{headerRow}:AC{headerRow}", "NG Q'ty", ColorGreen, ColorBlack, 9);
            SetBlock($"AD{headerRow}:AE{headerRow}", "NCR", ColorGreen, ColorBlack, 9);

            int totalOk = 0;
            int totalNg = 0;

            for (int index = 0; index < 3; index++)
            {
                int detailRow = headerRow + 1 + index;
                SetRowHeight(detailRow, 12.8);

                B08PendingRow left = PendingAt(content, index);
                B08PendingRow right = PendingAt(content, index + 3);

                totalOk += ToInt(left?.OkQty) + ToInt(right?.OkQty);
                totalNg += ToInt(left?.NgQty) + ToInt(right?.NgQty);

                SetBlock($"B{detailRow}", (index + 1).ToString(), ColorWhite, ColorBlack, 9);
                SetBlock($"C{detailRow}:I{detailRow}", Value(left?.Detail), ColorWhite, ColorBlue, 9, false, false, XlLeft);
                SetBlock($"J{detailRow}:K{detailRow}", Value(left?.Qty), ColorWhite, ColorBlue, 9);
                SetBlock($"L{detailRow}:M{detailRow}", Value(left?.OkQty), ColorWhite, ColorBlue, 9);
                SetBlock($"N{detailRow}:O{detailRow}", Value(left?.NgQty), ColorWhite, ColorBlue, 9);

                SetBlock($"P{detailRow}", (index + 4).ToString(), ColorWhite, ColorBlack, 9);
                SetBlock($"Q{detailRow}:W{detailRow}", Value(right?.Detail), ColorWhite, ColorBlue, 9, false, false, XlLeft);
                SetBlock($"X{detailRow}:Y{detailRow}", Value(right?.Qty), ColorWhite, ColorBlue, 9);
                SetBlock($"Z{detailRow}:AA{detailRow}", Value(right?.OkQty), ColorWhite, ColorBlue, 9);
                SetBlock($"AB{detailRow}:AC{detailRow}", Value(right?.NgQty), ColorWhite, ColorBlue, 9);
            }

            // NCR เป็นช่องเดียวคร่อม 3 แถว
            SetBlock($"AD{headerRow + 1}:AE{headerRow + 3}", string.Empty, ColorWhite);

            int totalRow = headerRow + 4;
            bool hasPending = content.PendingRows.Count > 0;
            SetRowHeight(totalRow, 15);
            SetBlock($"B{totalRow}:W{totalRow}", "Total", ColorWhite, ColorBlack, 9, false, false, XlRight);
            SetBlock($"X{totalRow}:Y{totalRow}", string.Empty, ColorWhite, ColorBlue, 9);
            SetBlock($"Z{totalRow}:AA{totalRow}", hasPending ? totalOk.ToString() : "0", ColorWhite, ColorBlue, 9);
            SetBlock($"AB{totalRow}:AC{totalRow}", hasPending ? totalNg.ToString() : "0", ColorWhite, ColorBlue, 9);
            SetBlock($"AD{totalRow}:AE{totalRow}", string.Empty, ColorWhite, ColorBlack, 8);
            AddCheckBox($"AD{totalRow}:AE{totalRow}", "P");

            int footerRow = totalRow + 1;
            SetRowHeight(footerRow, 15);
            SetBlock($"B{footerRow}:T{footerRow}", string.Empty, ColorWhite);
            AddCheckBox($"B{footerRow}:T{footerRow}", "PRONESS Record", true);
            SetBlock($"U{footerRow}:Y{footerRow}", "Judgement by", ColorWhite, ColorBlack, 10);
            SetBlock($"Z{footerRow}:AA{footerRow}", "Date", ColorWhite, ColorBlack, 10);
            SetBlock($"AB{footerRow}:AE{footerRow}", string.Empty, ColorWhite);

            return footerRow + 1;
        }


        // ---------- helper ----------

        // บล็อกตรวจสอบ : เทาเป็นค่าเริ่มต้น เปลี่ยนเป็นขาวเมื่อ M-CODE นั้นต้องตรวจ
        private static Color CheckFill(bool enabled)
        {
            return enabled ? ColorWhite : ColorGray;
        }

        // ผลตัดสินมาจาก formula ค่าเปลี่ยนได้ตอนผู้ตรวจแก้ตัวเลข ต้องใช้ conditional format ไม่ใช่ระบายสีตายตัว
        private static void ApplyJudgementColors(string address)
        {
            Excel.Range range = _sheet.Range[ExpandAddress(address)];

            try
            {
                AddJudgementCondition(range, "OK", ColorJudgeOk);
                AddJudgementCondition(range, "NG", ColorJudgeNg);
            }
            finally
            {
                ReleaseCom(range);
            }
        }

        private static void AddJudgementCondition(Excel.Range range, string text, Color fill)
        {
            Excel.FormatCondition condition = (Excel.FormatCondition)range.FormatConditions.Add(
                Excel.XlFormatConditionType.xlCellValue,
                Excel.XlFormatConditionOperator.xlEqual,
                $"=\"{text}\"");

            try
            {
                condition.Interior.Color = ToOle(fill);
                condition.Font.Color = ToOle(ColorWhite);
                condition.Font.Bold = true;
            }
            finally
            {
                ReleaseCom(condition);
            }
        }

        private static void SetNumberFormat(string address, string format)
        {
            Excel.Range range = _sheet.Range[ExpandAddress(address)];

            try { range.NumberFormat = format; }
            finally { ReleaseCom(range); }
        }

        private static void SetSlot(string[,] slots, int index, int row, string text, Color fill)
        {
            SetBlock($"{slots[index, 0]}{row}:{slots[index, 1]}{row}", text, fill, ColorBlack, 9);
        }

        private static string Nz(string value)
        {
            return value ?? string.Empty;
        }

        // ยังไม่มีข้อมูลก็คงข้อความที่พิมพ์ไว้ในฟอร์มเปล่าเอาไว้เหมือนเดิม
        private static string SlotText(string value, string placeholder)
        {
            return string.IsNullOrEmpty(value) ? placeholder : value;
        }

        private static string JudgementText(string judge)
        {
            return string.IsNullOrEmpty(judge) ? "Accept  .  Reject" : judge;
        }

        private static T ItemAt<T>(List<T> items, int index) where T : class
        {
            return index >= 0 && index < items.Count ? items[index] : null;
        }

        // ต้นฉบับ merge ช่อง Method เฉพาะช่วงจุดที่ใช้เครื่องมือเดียวกันติดกัน
        // เช่น CAM008 จุด 1-3 Caliper , จุด 4-6 Microscope ไม่ใช่รวบเป็นก้อนเดียวแล้วเขียนชื่อต่อกัน
        private static void RenderEquipmentRuns(List<B08DimensionPoint> points, int firstPointRow, int pointRows, Color fill)
        {
            int runStart = 0;

            for (int point = 1; point <= pointRows; point++)
            {
                if (point < pointRows && EquipmentAt(points, point) == EquipmentAt(points, runStart))
                {
                    continue;
                }

                int top = firstPointRow + runStart;
                int bottom = firstPointRow + point - 1;

                SetBlock($"N{top}:P{bottom}", EquipmentAt(points, runStart), fill, ColorBlack, 9, false, false, XlCenter, XlCenter, false, true, true);
                runStart = point;
            }
        }

        private static string EquipmentAt(List<B08DimensionPoint> points, int index)
        {
            B08DimensionPoint point = ItemAt(points, index);
            return point == null ? string.Empty : Nz(point.EquipmentName);
        }

        private static string DistinctEquipmentText(List<B08DimensionPoint> points)
        {
            return string.Join(" , ", points.Select(p => Nz(p.EquipmentName)).Where(name => name.Length > 0).Distinct());
        }

        // จับคู่ค่าที่วัดได้กับจุดวัด ถ้าไม่มี master ก็ไล่ตามลำดับที่บันทึกไว้
        private static string MeasuredValue(B08DimensionSample sample, B08DimensionPoint master, int pointIndex)
        {
            if (sample == null)
            {
                return string.Empty;
            }

            int pointOrder = master != null ? master.PointOrder : pointIndex + 1;
            return sample.ValueByPoint.TryGetValue(pointOrder, out string value) ? Nz(value) : string.Empty;
        }

        // ≤5 ช่องใช้ผังฟอร์มเปล่า (ก้าวละ 3) , มากกว่านั้นไล่ออกขวาก้าวละ 2
        private static string[,] BuildSlots(int slotCount)
        {
            int count = Math.Max(slotCount, 1);

            if (count <= DefaultSlots.GetLength(0))
            {
                string[,] trimmed = new string[count, 2];
                for (int index = 0; index < count; index++)
                {
                    trimmed[index, 0] = DefaultSlots[index, 0];
                    trimmed[index, 1] = DefaultSlots[index, 1];
                }

                return trimmed;
            }

            string[,] slots = new string[count, 2];
            int column = 17; // Q

            for (int index = 0; index < count; index++)
            {
                slots[index, 0] = ColumnLetter(column);
                slots[index, 1] = ColumnLetter(column + 1);
                column += 2;
            }

            return slots;
        }

        private static void SetBlock(
            string address,
            string text,
            Color fillColor,
            Color? fontColor = null,
            double fontSize = 9,
            bool bold = false,
            bool italic = false,
            int horizontalAlignment = XlCenter,
            int verticalAlignment = XlCenter,
            bool asText = false,
            bool border = true,
            bool wrap = false)
        {
            Excel.Range range = _sheet.Range[ExpandAddress(address)];

            try
            {
                if (address.Contains(":"))
                {
                    range.Merge();
                }

                // กัน Excel แปลงค่าที่หน้าตาเหมือนวันที่/ตัวเลขให้กลายเป็น serial
                if (asText)
                {
                    range.NumberFormat = "@";
                }

                range.Value2 = text ?? string.Empty;
                range.Interior.Color = ToOle(fillColor);
                range.Font.Name = FontName;
                range.Font.Size = fontSize;
                range.Font.Bold = bold;
                range.Font.Italic = italic;
                range.Font.Color = ToOle(fontColor ?? ColorBlack);
                range.HorizontalAlignment = horizontalAlignment;
                range.VerticalAlignment = verticalAlignment;

                // ต้นฉบับปิด WrapText เกือบทุกช่อง เปิดเฉพาะช่องข้อความยาว
                // ถ้าเปิดทั่วไป ข้อความจะถูกยัดในคอลัมน์กว้าง 2.86 แล้วเละ
                range.WrapText = wrap;
                range.ShrinkToFit = false;

                if (border)
                {
                    range.Borders.LineStyle = XlContinuous;
                    range.Borders.Weight = XlThin;
                    range.Borders.Color = ToOle(ColorBlack);
                }
            }
            finally
            {
                ReleaseCom(range);
            }
        }

        // เพิ่มช่องติ๊กแบบ Form Control ทับมุมขวาของช่องที่ระบุ
        // ใช้ Form Control ไม่ใช่ ActiveX เพราะบันทึกใน .xlsx ได้และเปิดได้ทุกเครื่องโดยไม่ต้องเปิด macro
        private static void AddCheckBox(string address, string caption, bool alignLeft = false)
        {
            Excel.Range range = _sheet.Range[ExpandAddress(address)];
            Excel.CheckBoxes boxes = null;
            Excel.CheckBox box = null;

            try
            {
                double cellLeft = Convert.ToDouble(range.Left);
                double cellTop = Convert.ToDouble(range.Top);
                double cellWidth = Convert.ToDouble(range.Width);
                double cellHeight = Convert.ToDouble(range.Height);

                double maxWidth = alignLeft ? 140 : 26;
                double boxWidth = Math.Min(cellWidth - 2, maxWidth);
                double boxHeight = Math.Min(cellHeight - 1, 13);
                double left = alignLeft ? (cellLeft + 3) : (cellLeft + cellWidth - boxWidth - 1);
                double top = cellTop + ((cellHeight - boxHeight) / 2);

                boxes = (Excel.CheckBoxes)_sheet.CheckBoxes(Type.Missing);
                box = boxes.Add(left, top, boxWidth, boxHeight);
                box.Caption = caption;
                box.Value = 0;              // ยังไม่ติ๊ก
                box.Display3DShading = false;
            }
            finally
            {
                ReleaseCom(box);
                ReleaseCom(boxes);
                ReleaseCom(range);
            }
        }

        // ลบเส้นคั่นแนวนอนระหว่าง 2 ช่องที่ติดกันบน-ล่าง
        // ต้องลบทั้งขอบล่างของตัวบนและขอบบนของตัวล่าง เพราะ Excel ใช้เส้นร่วมกัน
        private static void RemoveVerticalDivider(string leftAddress, string rightAddress)
        {
            const int XlLineStyleNone = -4142;
            const int XlEdgeLeft = 7;
            const int XlEdgeRight = 10;

            SetEdgeLineStyle(leftAddress, XlEdgeRight, XlLineStyleNone);
            SetEdgeLineStyle(rightAddress, XlEdgeLeft, XlLineStyleNone);
        }

        private static void RemoveHorizontalDivider(string upperAddress, string lowerAddress)
        {
            const int XlLineStyleNone = -4142;
            const int XlEdgeTop = 8;
            const int XlEdgeBottom = 9;

            SetEdgeLineStyle(upperAddress, XlEdgeBottom, XlLineStyleNone);
            SetEdgeLineStyle(lowerAddress, XlEdgeTop, XlLineStyleNone);
        }

        private static void SetEdgeLineStyle(string address, int edgeIndex, int lineStyle)
        {
            Excel.Range range = _sheet.Range[ExpandAddress(address)];
            Excel.Border border = null;

            try
            {
                border = range.Borders[(Excel.XlBordersIndex)edgeIndex];
                border.LineStyle = lineStyle;
            }
            finally
            {
                ReleaseCom(border);
                ReleaseCom(range);
            }
        }

        private static void SetOuterBorder(string address, Color color, int weight)
        {
            Excel.Range range = _sheet.Range[ExpandAddress(address)];

            try
            {
                foreach (int borderIndex in new[] { 7, 8, 9, 10 })
                {
                    Excel.Border border = range.Borders[(Excel.XlBordersIndex)borderIndex];
                    border.LineStyle = XlContinuous;
                    border.Weight = weight;
                    border.Color = ToOle(color);
                    ReleaseCom(border);
                }
            }
            finally
            {
                ReleaseCom(range);
            }
        }

        private static void SetColumnWidth(Excel.Worksheet sheet, int columnIndex, double width)
        {
            Excel.Range column = (Excel.Range)sheet.Columns[columnIndex];
            column.ColumnWidth = width;
            ReleaseCom(column);
        }

        private static void SetRowHeight(int rowIndex, double height)
        {
            Excel.Range row = (Excel.Range)_sheet.Rows[rowIndex];
            row.RowHeight = height;
            ReleaseCom(row);
        }

        private static string ColumnLetter(int columnNumber)
        {
            string letters = string.Empty;

            while (columnNumber > 0)
            {
                int remainder = (columnNumber - 1) % 26;
                letters = (char)('A' + remainder) + letters;
                columnNumber = (columnNumber - 1) / 26;
            }

            return letters;
        }

        // Excel ใช้ BGR ไม่ใช่ RGB
        private static int ToOle(Color color)
        {
            return color.R + (256 * color.G) + (65536 * color.B);
        }

        private static string MaterialHeader(QAdataProperty dataItem)
        {
            if (dataItem == null)
            {
                return string.Empty;
            }

            string mCode = Value(dataItem.M_CODE);
            string material = Value(dataItem.Material_Name);

            if (mCode.Length == 0 && material.Length == 0)
            {
                return string.Empty;
            }

            return $"{mCode}        {material}".Trim();
        }

        private static string AppearanceQtyText(B08CheckSheetContent content)
        {
            string qty = Value(content.InspectionQtyValue);
            return qty.Length == 0 ? "Inspection Q'ty :" : $"Inspection Q'ty : {qty}";
        }

        private static string PackingText(B08CheckSheetContent content, int index)
        {
            if (content.PackingJudgement == null || index >= content.PackingJudgement.Length)
            {
                return string.Empty;
            }

            return Value(content.PackingJudgement[index]);
        }

        private static string Barcode(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : $"*{value.Trim()}*";
        }

        private static string FormatDate(string value)
        {
            return DateTime.TryParse(value, out DateTime parsed)
                ? parsed.ToString("dd-MMM-yy")
                : Value(value);
        }

        private static string Value(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private static string Fallback(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        // "40 ☐ P" ถ้ามีค่า , เหลือแค่ "☐ P" ถ้ายังไม่มี
        private static string Suffixed(string value, string suffix)
        {
            return string.IsNullOrWhiteSpace(value) ? suffix : $"{value.Trim()} {suffix}";
        }

        private static B08PendingRow PendingAt(B08CheckSheetContent content, int index)
        {
            return index < content.PendingRows.Count ? content.PendingRows[index] : null;
        }

        private static int ToInt(string value)
        {
            return int.TryParse(value, out int parsed) ? parsed : 0;
        }

        private static void ReleaseCom(object comObject)
        {
            if (comObject == null)
            {
                return;
            }

            try
            {
                Marshal.ReleaseComObject(comObject);
            }
            catch (ArgumentException)
            {
                // ไม่ใช่ COM object ปล่อยผ่าน
            }
        }
    }
}
