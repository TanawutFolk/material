-- ============================================================================
-- ชุดทดสอบหน้า Appearance : เทียบ M-CODE แบบ All กับไม่ All
-- ============================================================================
-- userControlAppear.cs แตกทางที่ IsAllAppearanceMode() (บรรทัด 1921)
--   All (Sampling_Type = 1)        -> เป้าหมายเช็ค = Lot Size ทั้งก้อน , แก้ค่าที่คอลัมน์ JUDGE
--   ไม่ All (Sampling_Type = 3/5)  -> เป้าหมายเช็ค = Inspection Qty ต่อแพ็ค , แก้ที่ JUDGE_LOT_SIZE
--
--  ใบ        M-CODE         Sampling_Type   Lot   แพ็ค      ที่ต้องดู
--  QA26-9501 RFLCA002-JIN   1  All          300   3 x 100   ต้องเช็คครบ 300
--  QA26-9502 TNM100         1  All          300   1 x 300   All แบบแพ็คเดียว + มี Keep Data
--  QA26-9503 B016           3  Table        300   3 x 100   ตาราง 1/6 lot 281-500 -> 50 ตัว
--  QA26-9504 TNM102         3  Table         40   2 x 20    lot 26-50 -> 8 ตัว + มี Keep Data
--  QA26-9505 RFLBC014-MAC   5  % Lot Size   300   3 x 100   5% ของที่รับเข้า (UI เดินทางเดียวกับ All)
--
-- ตั้งใจไม่ใส่ db_appearance_data ไว้เลย เพื่อให้กรอกเองผ่านโปรแกรม
-- ขั้นก่อนหน้าตั้งให้ผ่านหมดแล้ว ใบจะโผล่ในคิว Appearance ทันที
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

SET @today = '2026-08-20';


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, COUNT(*) AS reports FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 0) ล้างของเดิม
-- ----------------------------------------------------------------------------
DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` LIKE 'QA26-95%';
DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` LIKE 'QA26-95%';
DELETE FROM `db_inspection_data`    WHERE `REPORT_NO` LIKE 'QA26-95%';
DELETE FROM `db_packing_check`      WHERE `REPORT_NO` LIKE 'QA26-95%';
DELETE FROM `db_packing_size`       WHERE `Report_No` LIKE 'QA26-95%';
DELETE FROM `db_report_lot_no`      WHERE `REPORT_NO` LIKE 'QA26-95%';
DELETE FROM `db_report_status`      WHERE `Report_No` LIKE 'QA26-95%';
DELETE FROM `db_receive_mat`        WHERE `Report_No` LIKE 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 1) หัวใบรับของ
-- ----------------------------------------------------------------------------
INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
SELECT r.`Report_No`, @today, 1, '', r.`M_Code`,
       COALESCE(c.`ITEM_EXTERNAL_SHORT_NAME`, r.`M_Code`),
       CONCAT('TEST-', RIGHT(r.`Report_No`, 4)),
       COALESCE(v.`VENDOR_NAME`, 'TEST VENDOR'),
       r.`Lot_Size`, r.`Lot_Size`, 'TEST', NOW()
FROM (
    SELECT 'QA26-9501' Report_No, 'RFLCA002-JIN' M_Code, 300 Lot_Size
    UNION ALL SELECT 'QA26-9502', 'TNM100',       300
    UNION ALL SELECT 'QA26-9503', 'B016',         300
    UNION ALL SELECT 'QA26-9504', 'TNM102',        40
    UNION ALL SELECT 'QA26-9505', 'RFLBC014-MAC', 300
) r
LEFT JOIN `mes`.`item_manufacturing` c ON c.`ITEM_CODE_FOR_SUPPORT_MES` = r.`M_Code`
LEFT JOIN `mes`.`vendor` v ON v.`VENDOR_ID` = c.`VENDOR_ID`;


-- ----------------------------------------------------------------------------
-- 2) สถานะ : ทุกขั้นก่อน Appearance ผ่านแล้ว , Appearance เว้น NULL รอทำ
--    เงื่อนไขคิว Appearance = (Inspection_Data_Check = 3 และ Packing_Check = 1)
--                             หรือ (Inspection_Data_Check = 1 และ Keep_Data_Need = 1)
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_status`
    (`Report_No`, `Keep_Data`, `Receive_WH`, `Emp_Receive_WH`, `Receive_WH_Date`,
     `Packing_Check`, `Regular_Check`, `Regular_Check_Lot_No`, `Inspection_Data_Check`,
     `Function_Check`, `Dimension_Check`, `Appearance_Check`, `Report_Status`)
SELECT m.`Report_No`,
       l.`Keep_Data_Need`, 1, 'TEST', NOW(),
       1,
       IF(l.`Regular_Check_Need`   = 1, 1, 3), '',
       IF(l.`Keep_Data_Need`       = 1, 1, 3),
       IF(l.`Function_Check_Need`  = 1, 1, 3),
       IF(l.`Dimension_Check_Need` = 1, 1, 3),
       NULL,
       1
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code`
WHERE m.`Report_No` LIKE 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 3) Lot No.
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_lot_no` (`REPORT_NO`, `LOT_NO`)
SELECT `Report_No`, CONCAT('LOT-', RIGHT(`Report_No`, 4), '-A')
FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 4) Packing Check ผ่านครบ 3 ข้อ
-- ----------------------------------------------------------------------------
INSERT INTO `db_packing_check`
    (`REPORT_NO`, `METHOD_ID`, `COUNT`, `DETAIL_JUDGE`, `JUDGMENT`, `EMP_PACKING_CHECK`)
SELECT m.`Report_No`, t.`ID`, 1, '', 1, 'TEST'
FROM `db_receive_mat` m
CROSS JOIN (SELECT 1 ID UNION SELECT 2 UNION SELECT 3) t
WHERE m.`Report_No` LIKE 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 5) ขนาดบรรจุ : หน้า Appearance แบบไม่ All แบ่งงานตาม PACKING_SIZE ของแต่ละแพ็ค
--    ใบที่ตั้งไว้หลายแพ็คจะได้เห็นว่าแบ่งงานถูกไหม
-- ----------------------------------------------------------------------------
-- PACKING_SIZE คือ "จำนวนที่ต้องเช็คของแพ็คนั้น" คำนวณตาม userControlPackingCheck.cs:833-905
--   type 1 All        -> เท่ากับ VALUE ของแพ็ค (เช็คทุกชิ้น)
--   type 3 Table      -> เปิดตาราง AQL ด้วย VALUE ของแพ็ค
--   type 5 % Lot Size -> เฉลี่ย (Lot x %) ลงทุกแพ็ค
INSERT INTO `db_packing_size` (`Report_No`, `BATCH`, `VALUE`, `PACK_COUNT`, `PACKING_SIZE`)
SELECT p.`Report_No`, p.`BATCH`, p.`VALUE`, 1,
       CASE aps.`Sampling_Type`
           WHEN 1 THEN p.`VALUE`
           WHEN 3 THEN GREATEST(
                           COALESCE((SELECT st.`Sampling_Qty` FROM `info_strictness` st
                                     WHERE st.`Strictness_Type`  = aps.`Strictness_Type`
                                       AND st.`Strictness_Level` = aps.`Strictness_Level`
                                       AND p.`VALUE` BETWEEN st.`Min_Qty` AND st.`Max_Qty`), 0),
                           COALESCE(aps.`Cavity_Qty`, 0) * COALESCE(aps.`Sampling_Qty`, 0))
           WHEN 5 THEN CEIL(m.`Lot_Size` * (aps.`Sampling_Qty` / 100.0)
                            / (SELECT COUNT(*) FROM (
                                   SELECT 'QA26-9501' Report_No, 1 BATCH, 100 VALUE
                                   UNION ALL SELECT 'QA26-9501', 2, 100
                                   UNION ALL SELECT 'QA26-9501', 3, 100
                                   UNION ALL SELECT 'QA26-9502', 1, 300
                                   UNION ALL SELECT 'QA26-9503', 1, 100
                                   UNION ALL SELECT 'QA26-9503', 2, 100
                                   UNION ALL SELECT 'QA26-9503', 3, 100
                                   UNION ALL SELECT 'QA26-9504', 1, 20
                                   UNION ALL SELECT 'QA26-9504', 2, 20
                                   UNION ALL SELECT 'QA26-9505', 1, 100
                                   UNION ALL SELECT 'QA26-9505', 2, 100
                                   UNION ALL SELECT 'QA26-9505', 3, 100) z
                               WHERE z.`Report_No` = p.`Report_No`))
           ELSE p.`VALUE`
       END
FROM (
    SELECT 'QA26-9501' Report_No, 1 BATCH, 100 VALUE
    UNION ALL SELECT 'QA26-9501', 2, 100
    UNION ALL SELECT 'QA26-9501', 3, 100
    UNION ALL SELECT 'QA26-9502', 1, 300
    UNION ALL SELECT 'QA26-9503', 1, 100
    UNION ALL SELECT 'QA26-9503', 2, 100
    UNION ALL SELECT 'QA26-9503', 3, 100
    UNION ALL SELECT 'QA26-9504', 1, 20
    UNION ALL SELECT 'QA26-9504', 2, 20
    UNION ALL SELECT 'QA26-9505', 1, 100
    UNION ALL SELECT 'QA26-9505', 2, 100
    UNION ALL SELECT 'QA26-9505', 3, 100
) p
JOIN `db_receive_mat` m ON m.`Report_No` = p.`Report_No`
LEFT JOIN `info_appearance_sampling` aps ON aps.`M_Code` = m.`M_Code`;


-- ----------------------------------------------------------------------------
-- 5b) Inspection_Qty = ผลรวมของ PACKING_SIZE ทุกแพ็ค
--     ค่านี้สำคัญ : หน้า Appearance แบบไม่ All ใช้ตัวนี้เป็นเป้าหมายจำนวนที่ต้องเช็ค
--     ถ้าปล่อยให้เท่ากับ Lot Size จะแยกไม่ออกว่า All กับไม่ All ต่างกันตรงไหน
-- ----------------------------------------------------------------------------
UPDATE `db_receive_mat` m
SET m.`Inspection_Qty` = (SELECT SUM(x.`PACKING_SIZE`) FROM `db_packing_size` x
                          WHERE x.`Report_No` = m.`Report_No`)
WHERE m.`Report_No` LIKE 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 6) Inspection Data Check เฉพาะใบที่ Keep_Data_Need = 1 (QA26-9502, QA26-9504)
-- ----------------------------------------------------------------------------
INSERT INTO `db_inspection_data`
    (`REPORT_NO`, `COUNT`, `JUDGE`, `REMARK`, `EMP_ID`, `INSPECTION_DATA_DATE`, `INUSE`)
SELECT m.`Report_No`, 1, 1, 'ทดสอบ Appearance All กับไม่ All', 'TEST', NOW(), 1
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code`
WHERE m.`Report_No` LIKE 'QA26-95%' AND l.`Keep_Data_Need` = 1;


-- ----------------------------------------------------------------------------
-- AFTER : ต้องขึ้นครบ 5 ใบ และ Appearance_Check ต้องเป็น NULL ทุกใบ
-- ----------------------------------------------------------------------------
SELECT m.`Report_No`, m.`M_Code`, m.`Lot_Size`, m.`Inspection_Qty` AS `insp_qty`,
       aps.`Sampling_Type` AS `samp_type`,
       t.`Sampling_Type_Name` AS `samp_name`,
       (SELECT GROUP_CONCAT(x.`PACKING_SIZE` ORDER BY x.`BATCH`)
        FROM `db_packing_size` x WHERE x.`Report_No` = m.`Report_No`) AS `pack_sizes`,
       s.`Packing_Check` AS `pack_st`,
       s.`Inspection_Data_Check` AS `keep_st`,
       IFNULL(s.`Appearance_Check`, 'NULL') AS `appear_st`,
       (SELECT COUNT(*) FROM `db_packing_size` x WHERE x.`Report_No` = m.`Report_No`) AS `packs`,
       (SELECT COUNT(*) FROM `db_appearance_data` x WHERE x.`REPORT_NO` = m.`Report_No`) AS `appear_rows`
FROM `db_receive_mat` m
JOIN `db_report_status` s ON s.`Report_No` = m.`Report_No`
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code`
LEFT JOIN `info_appearance_sampling` aps ON aps.`M_Code` = m.`M_Code`
LEFT JOIN `info_sampling_type` t ON t.`Sampling_Type` = aps.`Sampling_Type`
WHERE m.`Report_No` LIKE 'QA26-95%'
ORDER BY m.`Report_No`;
