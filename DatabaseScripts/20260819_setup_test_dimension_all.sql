-- ============================================================================
-- สร้าง Report + ค่าวัด Dimension แบบ All สำหรับดูฟอร์ม FM-QA-B08-F
-- ============================================================================
-- M-Code ที่เลือก : RFLCA072-JIN
--   เหตุผล - info_dimension_sampling.Sampling_Type = 1 (All) 1 ใน 2 ตัวที่มี
--          - มีจุดวัด master 9 จุด เกณฑ์ 6.1468 ~ 6.5532 (6.35 +- 0.2032 mm)
--          - Dimension_Check_Need = 1 , Appearance_Check_Need = 1
--
-- Lot Size 40 -> ฟอร์มต้องออก 3 บล็อก (15 + 15 + 10)
-- ตั้งใจให้ชิ้น No.7 กับ No.23 ผลต่างเกิน 0.2032 เพื่อดูว่า Judgement ขึ้น NG
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

SET @report = 'QA26-9301';
SET @mcode  = 'RFLCA072-JIN';
SET @lot    = 40;
SET @points = 9;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase,
       (SELECT COUNT(*) FROM `db_receive_mat`    WHERE `Report_No` = @report) AS receive_rows,
       (SELECT COUNT(*) FROM `db_dimension_data` WHERE `REPORT_NO` = @report) AS dimension_rows;


-- ----------------------------------------------------------------------------
-- 1) ล้างของเดิม (ให้รันซ้ำได้)
-- ----------------------------------------------------------------------------
DELETE FROM `db_dimension_data` WHERE `REPORT_NO` = @report;
DELETE FROM `db_report_status`  WHERE `Report_No` = @report;
DELETE FROM `db_receive_mat`    WHERE `Report_No` = @report;


-- ----------------------------------------------------------------------------
-- 2) หัวใบรับของ
-- ----------------------------------------------------------------------------
INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
SELECT @report, '2026-08-19', 1, '', @mcode, 'SPIRAL BASE PLATE',
       'JP5102-TEST', 'JIN Co., Ltd.', @lot, @lot, 'TEST', NOW();


-- ----------------------------------------------------------------------------
-- 3) ค่าที่วัดได้ : 9 จุด x 40 ชิ้น = 360 แถว
--    ค่ากลาง 6.35 กระจายแบบ deterministic ด้วย MOD ไม่ใช้ RAND จะได้ผลเหมือนเดิมทุกครั้ง
--    ชิ้น 7 กับ 23 ตั้งจุดที่ 9 เป็น 6.52 เพื่อดันผลต่างให้เกิน 0.2032
-- ----------------------------------------------------------------------------
INSERT INTO `db_dimension_data`
    (`REPORT_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `DIMENSION_DATE`, `INUSE`)
SELECT @report,
       pt.`POINT_ORDER`,
       pc.`n`,
       1,
       '0',
       (SELECT MIN(`ID`) FROM `info_equipment_serial`),
       ROUND(IF(pc.`n` IN (7, 23) AND pt.`POINT_ORDER` = @points,
                6.52,                                                    -- ดันจุดสุดท้ายให้สูงจนผลต่างเกินเกณฑ์
                6.35 + ((pc.`n` * 7 + pt.`POINT_ORDER` * 3) MOD 17 - 8) * 0.01), 4),
       1,
       'TEST',
       NOW(),
       1
FROM (SELECT `POINT_ORDER` FROM `info_dimension_equipment` WHERE `M_CODE` = @mcode) pt
CROSS JOIN (
    SELECT (t.`i` * 10 + u.`i` + 1) AS `n`
    FROM (SELECT 0 i UNION SELECT 1 UNION SELECT 2 UNION SELECT 3) t
    CROSS JOIN (SELECT 0 i UNION SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4
                UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9) u
) pc
WHERE pc.`n` <= @lot;


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase,
       (SELECT COUNT(*) FROM `db_receive_mat`    WHERE `Report_No` = @report) AS receive_rows,
       (SELECT COUNT(*) FROM `db_dimension_data` WHERE `REPORT_NO` = @report) AS dimension_rows;

-- ผลต่างต่อชิ้น : ชิ้นไหนเกิน 0.2032 ฟอร์มต้องขึ้น NG
SELECT `SAMPLING_NO`,
       ROUND(MAX(`VALUE`) - MIN(`VALUE`), 4) AS `difference`,
       IF(MAX(`VALUE`) - MIN(`VALUE`) > 0.2032, 'NG', 'OK') AS `expected`
FROM `db_dimension_data`
WHERE `REPORT_NO` = @report
GROUP BY `SAMPLING_NO`
ORDER BY `SAMPLING_NO`;
