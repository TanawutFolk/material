using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RawMat.ViewsMaterial.ReceiveWH
{
    // ฟอร์ม B08 มี 4 แบบ ต่างกันเฉพาะโซนกลาง (Function / Dimension / Appearance)
    // ส่วนหัว บรรจุภัณฑ์ Lot Regular Final Refer Pending ใช้โครงเดียวกันหมด
    internal enum B08Template
    {
        Standard = 0,   // Reduce S-1 : พอดีฟอร์มเปล่า
        LevelDown = 1,  // Strictness = Normal : Dimension ซ้ำเป็นบล็อกลงล่าง
        WideCavity = 2, // Function = Pc/Cavity : เพิ่มช่อง cavity ออกทางขวา
        DimensionAll = 3 // Sampling_Type = All : ตรวจทุกชิ้น ยาวลงตาม Lot size
    }

    // 1 แถวในตาราง Appearance (Date / Ope. / Check Q'ty / OK / Pending)
    internal class B08AppearanceRow
    {
        public string Date { get; set; }
        public string Operator { get; set; }
        public string CheckQty { get; set; }
        public string OkQty { get; set; }
        public string PendingQty { get; set; }
    }

    // 1 บรรทัดในตาราง Pending detail ท้ายเอกสาร
    internal class B08PendingRow
    {
        public string Detail { get; set; }
        public string Qty { get; set; }
        public string OkQty { get; set; }
        public string NgQty { get; set; }
    }

    // 1 จุดวัดของ Dimension (master ต่อ M-CODE)
    internal class B08DimensionPoint
    {
        public int PointOrder { get; set; }
        public string PointName { get; set; }
        public string Criteria { get; set; }
        public string CriteriaMin { get; set; }
        public string CriteriaMax { get; set; }
        public string Unit { get; set; }

        /// <summary>1 = วัดเป็นตัวเลข , 2 = ตัดสินผ่าน/ไม่ผ่าน ดู Utilities/PointJudgeType.cs</summary>
        public string JudgeType { get; set; }

        public string EquipmentName { get; set; }
    }

    // 1 ตัวอย่างที่ตรวจ : ค่าที่วัดได้ของทุกจุด + ผลตัดสินรวม
    internal class B08DimensionSample
    {
        // ผลตัดสินรายชิ้นของแบบ All ที่บันทึกไว้ตอนตรวจ ไม่ได้คำนวณใหม่ตอน export
        public string Difference { get; set; }
        public string Tolerance { get; set; }
        public string PieceJudge { get; set; }

        public int SamplingNo { get; set; }
        public string CavityName { get; set; }
        public string Judge { get; set; }
        public Dictionary<int, string> ValueByPoint { get; set; } = new Dictionary<int, string>();
    }

    // 1 ตัวอย่างของ Function : db_function_data ไม่มีคอลัมน์ VALUE จึงมีแค่ผลตัดสิน
    internal class B08FunctionSample
    {
        public int SamplingNo { get; set; }
        public string CavityName { get; set; }
        public string Judge { get; set; }
        public string Remark { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentSerial { get; set; }
    }

    // ข้อมูลทั้งหมดที่ engine ต้องใช้วาด 1 ใบ
    internal class B08CheckSheetContent
    {
        // แบบของฟอร์ม ตัดสินจาก sampling type + strictness ของ M-CODE
        public B08Template Template { get; set; } = B08Template.Standard;

        // เปิด/ปิดบล็อก มาจาก info_mat_inspection_list.*_Need -> ตัดสินว่าเทาหรือขาว
        public bool RegularEnabled { get; set; }
        public bool FunctionEnabled { get; set; }
        public bool DimensionEnabled { get; set; }
        public bool AppearanceEnabled { get; set; }

        // หัวเอกสารส่วนที่ไม่ได้อยู่ใน QAdataProperty
        public string IssueBy { get; set; }
        public string IssueTime { get; set; }
        public string InspectionDate { get; set; }
        public string InspectorName { get; set; }
        public string ReferenceText { get; set; }

        // บรรจุภัณฑ์ : ช่อง Judgement 3 บรรทัด (บรรทัดที่ 3 เป็นขนาดบรรจุ)
        public string[] PackingJudgement { get; set; } = new string[2];

        // บรรทัดที่ 3 ของบรรจุภัณฑ์ แยกเป็นผลตัดสิน (T14) กับตัวเลขคำนวณขนาดบรรจุ (T15)
        public string PackingSizeJudgement { get; set; }
        public string PackingSizeText { get; set; }
        public string LotNoText { get; set; }

        // Appearance : เก็บเฉพาะตัวเลข ส่วนป้าย "Inspection Q'ty :" อยู่ในฟอร์มอยู่แล้ว
        public string InspectionQtyValue { get; set; }
        public List<B08AppearanceRow> AppearanceRows { get; set; } = new List<B08AppearanceRow>();

        // Pending detail ท้ายเอกสาร (ฟอร์มมี 6 ช่อง)
        public List<B08PendingRow> PendingRows { get; set; } = new List<B08PendingRow>();

        // จำนวนบล็อกย่อยของ Function : 1 บล็อก = หัว Cavity + แถว OK.NG
        public int FunctionGroups { get; set; } = 1;

        // จำนวนช่อง cavity ต่อแถว ฟอร์มเปล่ามี 5 ช่อง (Q:S T:V W:Y Z:AB AC:AE)
        // แบบ WideCavity จะมากกว่านี้
        public int CavitySlots { get; set; } = 5;

        // Dimension : 1 บล็อก = หัว cavity 1 แถว + จุดวัด N แถว
        // ฟอร์มเปล่ามีจุดวัด 4 แถว (R24-R27)
        public int DimensionGroups { get; set; } = 1;
        public int DimensionPointRows { get; set; } = 4;

        // แบบ DimensionAll : จำนวนชิ้นงานที่ต้องตรวจ = Lot Size
        public int PieceCount { get; set; } = 1;

        // ข้อมูลจริงของ Function / Dimension ที่ดึงมาจาก DB
        public List<B08FunctionSample> FunctionSamples { get; set; } = new List<B08FunctionSample>();
        public string FunctionMethodText { get; set; }
        public string FunctionEquipmentText { get; set; }
        public List<B08DimensionPoint> DimensionPoints { get; set; } = new List<B08DimensionPoint>();
        public List<B08DimensionSample> DimensionSamples { get; set; } = new List<B08DimensionSample>();
        public string DimensionEquipmentText { get; set; }
        public string DimensionEquipmentSerial { get; set; }

        // สรุปผลรวมของแต่ละส่วน : เจอ NG ตัวเดียวก็ Reject ทั้งส่วน
        public string FunctionJudge
        {
            get { return OverallJudge(FunctionSamples.Select(s => s.Judge)); }
        }

        public string DimensionJudge
        {
            get { return OverallJudge(DimensionSamples.Select(s => s.Judge)); }
        }

        private static string OverallJudge(IEnumerable<string> judges)
        {
            var known = judges.Where(j => !string.IsNullOrEmpty(j)).ToList();

            if (known.Count == 0)
            {
                return string.Empty;
            }

            return known.Any(j => j == "NG") ? "Reject" : "Accept";
        }

        // ฟอร์มเปล่ามีช่อง Appearance 4 แถว (R32-R35) ถ้าข้อมูลมากกว่านั้นก็ยืดลงตามจริง
        public int AppearanceRowCount
        {
            get { return Math.Max(AppearanceRows.Count, 4); }
        }
    }

    // ประกอบ B08CheckSheetContent จากฐานข้อมูล
    internal static class B08ContentBuilder
    {
        public static B08CheckSheetContent Build(QAdataControllers conQA, QAdataProperty dataItem)
        {
            var content = new B08CheckSheetContent();

            ApplyHeader(conQA, dataItem, content);
            ApplyPacking(conQA, dataItem, content);
            ApplyLotNo(conQA, dataItem, content);
            ApplyAppearance(conQA, dataItem, content);
            ApplyPending(conQA, dataItem, content);
            ApplyTemplate(conQA, dataItem, content);
            ApplyFunctionAndDimension(conQA, dataItem, content);

            return content;
        }

        // เลือกแบบฟอร์มและคำนวณว่าโซนกลางต้องยืดแค่ไหน
        // ดึงข้อมูล Function / Dimension จริงมาใส่ แล้วปรับจำนวนแถวที่ต้องยืดตามข้อมูล
        private static void ApplyFunctionAndDimension(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            ApplyFunctionSamples(conQA, dataItem, content);
            ApplyDimensionPoints(conQA, dataItem, content);
            ApplyDimensionSamples(conQA, dataItem, content);
            ApplyDimensionEquipment(conQA, dataItem, content);
        }

        private static void ApplyFunctionSamples(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            var methods = new List<string>();
            var equipment = new List<string>();

            foreach (DataRow row in conQA.B08FunctionData(dataItem).Rows)
            {
                string method = Text(row, "FUNCTION_METHOD");
                string equipmentName = Text(row, "Equipment_Name");
                string equipmentSerial = Text(row, "EQUIPMENT_SERIAL");

                content.FunctionSamples.Add(new B08FunctionSample
                {
                    SamplingNo = Number(row, "SAMPLING_NO"),
                    CavityName = CavityLabel(Text(row, "CAVITY_NAME")),
                    Judge = JudgeText(Text(row, "JUDGE")),
                    Remark = Text(row, "REMARK"),
                    EquipmentName = equipmentName,
                    EquipmentSerial = equipmentSerial
                });

                if (method.Length > 0 && !methods.Contains(method))
                {
                    methods.Add(method);
                }

                string equipmentText = equipmentSerial.Length == 0
                    ? equipmentName
                    : $"{equipmentName} S/N : {equipmentSerial}".Trim();
                if (equipmentText.Length > 0 && !equipment.Contains(equipmentText))
                {
                    equipment.Add(equipmentText);
                }
            }

            content.FunctionMethodText = string.Join(" , ", methods);
            content.FunctionEquipmentText = string.Join(" , ", equipment);

            // 1 บล็อกวางได้ 5 ตัวอย่าง ถ้ามากกว่านั้นต้องเพิ่มบล็อกลงล่าง
            if (content.FunctionSamples.Count > DefaultSlotCount)
            {
                content.FunctionGroups = (int)Math.Ceiling(content.FunctionSamples.Count / (double)DefaultSlotCount);
            }
        }

        private static void ApplyDimensionPoints(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            foreach (DataRow row in conQA.B08DimensionPoints(dataItem).Rows)
            {
                content.DimensionPoints.Add(new B08DimensionPoint
                {
                    PointOrder = Number(row, "POINT_ORDER"),
                    PointName = Text(row, "POINT_NAME"),
                    Criteria = BuildCriteriaText(row),
                    CriteriaMin = Text(row, "CRITERIA_MIN"),
                    CriteriaMax = Text(row, "CRITERIA_MAX"),
                    Unit = Text(row, "UNIT"),
                    JudgeType = Text(row, "JUDGE_TYPE"),
                    EquipmentName = Text(row, "Equipment_Name")
                });
            }

            if (content.DimensionPoints.Count > 0)
            {
                content.DimensionPointRows = content.DimensionPoints.Count;
            }
        }

        private static void ApplyDimensionSamples(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            var samples = new Dictionary<int, B08DimensionSample>();

            foreach (DataRow row in conQA.B08DimensionData(dataItem).Rows)
            {
                int samplingNo = Number(row, "SAMPLING_NO");

                if (!samples.TryGetValue(samplingNo, out B08DimensionSample sample))
                {
                    sample = new B08DimensionSample
                    {
                        SamplingNo = samplingNo,
                        CavityName = CavityLabel(Text(row, "CAVITY_NAME"))
                    };
                    samples[samplingNo] = sample;
                }

                sample.ValueByPoint[Number(row, "POINT_ORDER")] = TrimNumber(Text(row, "VALUE"));

                // ตัวอย่างจะ OK ก็ต่อเมื่อทุกจุดผ่าน เจอ NG จุดเดียวถือว่า NG ทั้งตัวอย่าง
                if (Text(row, "JUDGE") == "0")
                {
                    sample.Judge = "NG";
                }
                else if (string.IsNullOrEmpty(sample.Judge))
                {
                    sample.Judge = "OK";
                }
            }

            ApplyDimensionPieceJudge(conQA, dataItem, samples);

            content.DimensionSamples.AddRange(samples.Values.OrderBy(s => s.SamplingNo));

            if (content.DimensionSamples.Count > DefaultSlotCount)
            {
                content.DimensionGroups = (int)Math.Ceiling(content.DimensionSamples.Count / (double)DefaultSlotCount);
            }
        }

        // แบบ All ตัดสินที่ผลต่างภายในชิ้น ค่านี้โปรแกรมคิดแล้วบันทึกไว้ตอนตรวจ
        // ฟอร์มจึงหยิบมาวางตรงๆ ไม่ใช่ใส่สูตรให้ Excel คิดใหม่ ผลตัดสินจะได้ไม่เปลี่ยนหลังเซ็นแล้ว
        private static void ApplyDimensionPieceJudge(
            QAdataControllers conQA, QAdataProperty dataItem, Dictionary<int, B08DimensionSample> samples)
        {
            foreach (DataRow row in conQA.B08DimensionPieceJudge(dataItem).Rows)
            {
                if (!samples.TryGetValue(Number(row, "SAMPLING_NO"), out B08DimensionSample sample))
                {
                    continue;
                }

                sample.Difference = TrimNumber(Text(row, "DIFFERENCE"));
                sample.Tolerance = TrimNumber(Text(row, "TOLERANCE"));
                sample.PieceJudge = JudgeText(Text(row, "JUDGE"));
            }
        }

        private static void ApplyDimensionEquipment(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            var parts = new List<string>();
            var serials = new List<string>();

            foreach (DataRow row in conQA.B08DimensionEquipment(dataItem).Rows)
            {
                string name = Text(row, "Equipment_Name");
                string serial = Text(row, "EQUIPMENT_SERIAL");

                if (name.Length == 0 && serial.Length == 0)
                {
                    continue;
                }

                parts.Add(serial.Length == 0 ? name : $"{name} S/N : {serial}");

                if (serials.Count == 0 || !serials.Contains(serial))
                {
                    serials.Add(serial);
                }
            }

            content.DimensionEquipmentText = string.Join("    ", parts);
            content.DimensionEquipmentSerial = string.Join(" , ", serials.Where(x => x.Length > 0));
        }

        // "12.1～12.3" หรือ "≤ 0.1" เมื่อไม่มีค่าต่ำสุด
        private static string BuildCriteriaText(DataRow row)
        {
            string min = TrimNumber(Text(row, "CRITERIA_MIN"));
            string max = TrimNumber(Text(row, "CRITERIA_MAX"));
            string unit = Text(row, "UNIT");
            string suffix = unit.Length == 0 ? string.Empty : " " + unit;

            if (max.Length == 0)
            {
                return min.Length == 0 ? string.Empty : min + suffix;
            }

            return (IsZeroOrEmpty(min) ? $"≤ {max}" : $"{min}～{max}") + suffix;
        }

        /// <summary>
        /// ตัดศูนย์ท้ายทิ้ง 6.146800 -> 6.1468
        /// DB เก็บ decimal(12,6) เหมือนเดิม แต่เอกสารจริงเขียนเท่าที่จำเป็น
        /// ใบที่พิมพ์กับหน้าจอต้องอ่านค่าได้เหมือนกัน
        /// แปลงไม่ได้ก็คืนข้อความเดิม ไม่กลืนค่าทิ้ง
        /// </summary>
        private static string TrimNumber(string value)
        {
            return Utilities.NumberDisplay.Trim(value);
        }

        private static bool IsZeroOrEmpty(string value)
        {
            return value.Length == 0 || (decimal.TryParse(value, out decimal parsed) && parsed == 0);
        }

        private static string JudgeText(string judge)
        {
            if (judge == "1") { return "OK"; }
            if (judge == "0") { return "NG"; }
            return string.Empty;
        }

        // CAVITY_NAME = "0" แปลว่าไม่ได้ใช้ cavity จริง
        private static string CavityLabel(string cavityName)
        {
            if (cavityName.Length == 0 || cavityName == "0")
            {
                return string.Empty;
            }

            return cavityName.StartsWith("Cavity") ? cavityName : $"Cavity {cavityName}";
        }

        private static void ApplyTemplate(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            DataRow row = FirstRow(conQA.B08Sampling(dataItem));

            if (row == null)
            {
                return; // ไม่มี master ใช้ฟอร์มเปล่าไปก่อน
            }

            int dimSamplingType = Number(row, "DIM_SAMPLING_TYPE");
            int dimStrictnessType = Number(row, "DIM_STRICTNESS_TYPE");
            int funcSamplingType = Number(row, "FUNC_SAMPLING_TYPE");
            int cavityQty = Math.Max(Number(row, "DIM_CAVITY_QTY"), 1);
            int perCavityQty = Math.Max(Number(row, "DIM_SAMPLING_QTY"), 1);
            int strictnessQty = Number(row, "STRICTNESS_SAMPLING_QTY");
            int pointCount = Number(row, "DIM_POINT_COUNT");

            // จำนวนตัวอย่างที่ต้องตรวจ : เอาค่าที่มากกว่าระหว่างตารางความเข้มงวด กับ cavity x จำนวนต่อ cavity
            int sampleCount = Math.Max(strictnessQty, cavityQty * perCavityQty);

            if (pointCount > 0)
            {
                content.DimensionPointRows = pointCount;
            }

            const int SamplingTypeAll = 1;
            const int SamplingTypePcCavity = 4;
            const int StrictnessNormal = 1;

            if (dimSamplingType == SamplingTypeAll)
            {
                // ตรวจทุกชิ้นใน lot : จุดวัดเป็นแถว ชิ้นงานเป็นคอลัมน์
                content.Template = B08Template.DimensionAll;
                content.PieceCount = Math.Max(Number2(dataItem.Qty), 1);
            }
            else if (funcSamplingType == SamplingTypePcCavity)
            {
                // ช่อง cavity ขยายออกทางขวา
                // ไฟล์ตัวอย่าง QA26-007 วางไว้ 20 ช่องแม้ใช้จริงแค่ 4 จึงยึด 20 เป็นขั้นต่ำไว้ก่อน
                content.Template = B08Template.WideCavity;
                content.CavitySlots = Math.Max(sampleCount, WideCavitySlotCount);
            }
            else if (dimStrictnessType == StrictnessNormal && sampleCount > DefaultSlotCount)
            {
                // ตัวอย่างเยอะกว่าที่วางได้ 1 แถว -> ซ้ำบล็อกลงล่าง
                content.Template = B08Template.LevelDown;
                content.DimensionGroups = (int)Math.Ceiling(sampleCount / (double)DefaultSlotCount);
            }
            else
            {
                content.Template = B08Template.Standard;
            }
        }

        private const int DefaultSlotCount = 5;
        private const int WideCavitySlotCount = 20;

        private static int Number2(string value)
        {
            return int.TryParse(value, out int parsed) ? parsed : 0;
        }

        private static void ApplyHeader(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            DataRow row = FirstRow(conQA.B08Header(dataItem));

            if (row == null)
            {
                return;
            }

            // เติมกลับเข้า QAdataProperty เพื่อให้ engine ใช้ค่าเดียวกันทั้งใบ
            dataItem.M_CODE = Text(row, "M_Code");
            dataItem.Material_Name = Text(row, "Material_Name");
            dataItem.Receive_Date = Text(row, "Receive_Date");
            dataItem.Invoice_No = Text(row, "Invoice_No");
            dataItem.Vendor_Name = Text(row, "Vendor_Name");
            dataItem.Qty = Text(row, "Lot_Size");
            dataItem.inspQty = Text(row, "Inspection_Qty");
            dataItem.Regular_No = Text(row, "Regular_No");

            content.RegularEnabled = IsOn(row, "Regular_Check_Need");
            content.FunctionEnabled = IsOn(row, "Function_Check_Need");
            content.DimensionEnabled = IsOn(row, "Dimension_Check_Need");
            content.AppearanceEnabled = IsOn(row, "Appearance_Check_Need");

            content.IssueBy = Text(row, "Emp_Receive_WH");
            content.IssueTime = FormatDateTime(row, "Receive_WH_Date");
            content.ReferenceText = Text(row, "Reference");

            content.InspectionQtyValue = Text(row, "Inspection_Qty");
        }

        // บรรทัด 1-2 = ผลตัดสิน , บรรทัด 3 แยกเป็นผลตัดสิน (T14) กับตัวเลขคำนวณขนาดบรรจุ (T15)
        private static void ApplyPacking(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            DataTable checks = conQA.B08PackingCheck(dataItem);

            for (int index = 0; index < 2 && index < checks.Rows.Count; index++)
            {
                content.PackingJudgement[index] = DescribeJudgement(checks.Rows[index]);
            }

            if (checks.Rows.Count > 2)
            {
                content.PackingSizeJudgement = DescribeJudgement(checks.Rows[2]);
            }

            content.PackingSizeText = BuildPackingSizeText(conQA.B08PackingSize(dataItem));
        }

        private static string DescribeJudgement(DataRow row)
        {
            string judgement = Text(row, "JUDGMENT");

            if (judgement == "1")
            {
                return "OK";
            }

            if (string.IsNullOrWhiteSpace(judgement))
            {
                return string.Empty; // ยังไม่ได้ตรวจ
            }

            string detail = Text(row, "DETAIL_JUDGE");
            return string.IsNullOrWhiteSpace(detail) ? "NG" : detail;
        }

        // "500 X 8 = 4000" ถ้ามีหลาย batch ต่อกันด้วย " , "
        private static string BuildPackingSizeText(DataTable sizes)
        {
            var parts = new List<string>();

            foreach (DataRow row in sizes.Rows)
            {
                int value = Number(row, "VALUE");
                int packCount = Number(row, "PACK_COUNT");

                if (value <= 0 || packCount <= 0)
                {
                    continue;
                }

                parts.Add($"{value} X {packCount} = {value * packCount}");
            }

            return parts.Count == 0 ? string.Empty : string.Join(" , ", parts);
        }

        private static void ApplyLotNo(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            DataTable lots = conQA.B08LotNo(dataItem);

            content.LotNoText = string.Join(" , ", lots.Rows
                .Cast<DataRow>()
                .Select(row => Text(row, "LOT_NO"))
                .Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static void ApplyAppearance(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            DataTable data = conQA.B08AppearanceData(dataItem);
            string inspectorFallback = null;

            foreach (DataRow row in data.Rows)
            {
                content.AppearanceRows.Add(new B08AppearanceRow
                {
                    Date = FormatDate(row, "APPEARANCE_DATE"),
                    Operator = Text(row, "EMP_ID"),
                    CheckQty = Text(row, "QTY_SELECT"),
                    OkQty = Text(row, "QTY_OK"),
                    PendingQty = Text(row, "QTY_NG")
                });

                if (inspectorFallback == null)
                {
                    inspectorFallback = Text(row, "EMP_NAME");
                    content.InspectionDate = FormatDate(row, "APPEARANCE_DATE");
                }
            }

            // ยังไม่มีที่เก็บ Inspector ระดับ report จึงใช้คนที่ตรวจ Appearance คนแรกไปก่อน
            content.InspectorName = inspectorFallback;
        }

        private static void ApplyPending(QAdataControllers conQA, QAdataProperty dataItem, B08CheckSheetContent content)
        {
            DataTable pending = conQA.B08AppearancePending(dataItem);

            foreach (DataRow row in pending.Rows)
            {
                string detail = Text(row, "NG_DETAIL");

                if (string.IsNullOrWhiteSpace(detail))
                {
                    detail = Text(row, "NG_Mode");
                }

                content.PendingRows.Add(new B08PendingRow
                {
                    Detail = detail,
                    Qty = Text(row, "QTY_NG"),
                    OkQty = Text(row, "REVIEW_OK_QTY"),
                    NgQty = Text(row, "REMAIN_NG")
                });
            }
        }

        // ---------- helper ----------

        private static DataRow FirstRow(DataTable table)
        {
            return table != null && table.Rows.Count > 0 ? table.Rows[0] : null;
        }

        private static bool IsOn(DataRow row, string column)
        {
            return Text(row, column) == "1";
        }

        private static int Number(DataRow row, string column)
        {
            return int.TryParse(Text(row, column), out int parsed) ? parsed : 0;
        }

        private static string FormatDate(DataRow row, string column)
        {
            string raw = Text(row, column);
            return DateTime.TryParse(raw, out DateTime parsed) ? parsed.ToString("dd MMM yy") : raw;
        }

        private static string FormatDateTime(DataRow row, string column)
        {
            string raw = Text(row, column);
            return DateTime.TryParse(raw, out DateTime parsed) ? parsed.ToString("dd MMM yy HH:mm") : raw;
        }

        private static string Text(DataRow row, string column)
        {
            if (row == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[column].ToString().Trim();
        }
    }
}
