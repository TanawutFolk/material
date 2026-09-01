-- ============================================================================
-- ชุดทดสอบใหญ่ : เลือก M-CODE ให้ครบทุกฟังก์ชันเช็ค แล้วใส่ค่าเหมือนตรวจจริง
-- ============================================================================
-- เลือกจากการไล่ชุดค่าผสมของ info_mat_inspection_list ทั้งหมด 19 แบบ
-- ใช้ 6 ใบก็ครอบคลุมได้ครบ
--
--  ใบ        M-CODE          k p r f d a   จุดที่ต้องการทดสอบ
--  QA26-9401 R179S622-YTC    1 1 1 1 1 1   ใบเดียวที่เปิดครบทั้ง 6 ฟังก์ชัน
--  QA26-9402 CAM008          1 1 0 1 1 1   Function Pc/Cavity + Dimension มี Cavity
--  QA26-9403 CAM010          1 1 1 0 1 1   Regular แบบ Pc/Cavity
--  QA26-9404 RFLCA072-JIN    0 1 0 0 1 1   Dimension All + Appearance All
--  QA26-9405 RCOMM001-SHI    1 1 0 0 1 1   Dimension Strictness Normal (LevelDown)
--  QA26-9406 D16             0 2 0 0 0 0   Packing mode 2 (เทียบรูป)
--
-- ตั้งใจใส่ NG ไว้ 3 จุดเพื่อดูว่าตัดสินถูก
--   QA26-9401 Dimension ตัวอย่างที่ 4 จุดที่ 2
--   QA26-9402 Function  cavity P
--   QA26-9404 Appearance batch 2
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

SET @caliper    = 135;   -- B16272968
SET @microscope = 131;   -- 1G19405
SET @heightgg   = 137;   -- 1517230
SET @today      = '2026-08-20';


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, COUNT(*) AS reports
FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-94%';


-- ----------------------------------------------------------------------------
-- 0) ล้างของเดิมทั้งชุด
-- ----------------------------------------------------------------------------
DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_dimension_data`     WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_function_data`      WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_inspection_data`    WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_packing_check`      WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_packing_size`       WHERE `Report_No` LIKE 'QA26-94%';
DELETE FROM `db_report_lot_no`      WHERE `REPORT_NO` LIKE 'QA26-94%';
DELETE FROM `db_report_status`      WHERE `Report_No` LIKE 'QA26-94%';
DELETE FROM `db_receive_mat`        WHERE `Report_No` LIKE 'QA26-94%';
DELETE FROM `db_regular_data`       WHERE `REGULAR_NO` LIKE 'RG26-94%';


-- ----------------------------------------------------------------------------
-- 1) หัวใบรับของ  (Material_Name / Vendor ดึงจาก MES ให้ตรงของจริง)
-- ----------------------------------------------------------------------------
INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
SELECT r.`Report_No`, @today, 1, r.`Regular_No`, r.`M_Code`,
       COALESCE(c.`ITEM_EXTERNAL_SHORT_NAME`, r.`M_Code`),
       r.`Invoice_No`, COALESCE(v.`VENDOR_NAME`, 'TEST VENDOR'),
       r.`Lot_Size`, r.`Lot_Size`, 'TEST', NOW()
FROM (
    SELECT 'QA26-9401' Report_No, 'RG26-9401' Regular_No, 'R179S622-YTC' M_Code, 'TEST-9401' Invoice_No,    50 Lot_Size
    UNION ALL SELECT 'QA26-9402', '',          'CAM008',       'TEST-9402',  4000
    UNION ALL SELECT 'QA26-9403', 'RG26-9403', 'CAM010',       'TEST-9403',  2000
    UNION ALL SELECT 'QA26-9404', '',          'RFLCA072-JIN', 'TEST-9404',    20
    UNION ALL SELECT 'QA26-9405', '',          'RCOMM001-SHI', 'TEST-9405', 45000
    UNION ALL SELECT 'QA26-9406', '',          'D16',          'TEST-9406',   500
) r
LEFT JOIN `mes`.`item_manufacturing` c ON c.`ITEM_CODE_FOR_SUPPORT_MES` = r.`M_Code`
LEFT JOIN `mes`.`vendor` v ON v.`VENDOR_ID` = c.`VENDOR_ID`;


-- ----------------------------------------------------------------------------
-- 2) สถานะใบ : ขั้นที่ M-CODE ไม่ต้องเช็ค ตั้งเป็น 3 (Skip) ที่ต้องเช็คตั้งเป็น 1 (OK)
--    ตรงกับที่โปรแกรมเขียนเองเมื่อทำจนจบ flow
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_status`
    (`Report_No`, `Keep_Data`, `Receive_WH`, `Emp_Receive_WH`, `Receive_WH_Date`,
     `Packing_Check`, `Regular_Check`, `Regular_Check_Lot_No`, `Inspection_Data_Check`,
     `Function_Check`, `Dimension_Check`, `Appearance_Check`, `Report_Status`)
SELECT m.`Report_No`,
       l.`Keep_Data_Need`, 1, 'TEST', NOW(),
       1,
       IF(l.`Regular_Check_Need`    = 1, 1, 3), m.`Regular_No`,
       IF(l.`Keep_Data_Need`        = 1, 1, 3),
       IF(l.`Function_Check_Need`   = 1, 1, 3),
       IF(l.`Dimension_Check_Need`  = 1, 1, 3),
       IF(l.`Appearance_Check_Need` = 1, 1, 3),
       1
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code`
WHERE m.`Report_No` LIKE 'QA26-94%';


-- ----------------------------------------------------------------------------
-- 3) Lot No. ของผู้ผลิต
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_lot_no` (`REPORT_NO`, `LOT_NO`)
SELECT `Report_No`, CONCAT('LOT-', RIGHT(`Report_No`, 4), '-A')
FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-94%';


-- ----------------------------------------------------------------------------
-- 4) Packing Check : 3 ข้อ ผ่านหมด (ทุกใบต้องผ่านขั้นนี้)
-- ----------------------------------------------------------------------------
INSERT INTO `db_packing_check`
    (`REPORT_NO`, `METHOD_ID`, `COUNT`, `DETAIL_JUDGE`, `JUDGMENT`, `EMP_PACKING_CHECK`)
SELECT m.`Report_No`, t.`ID`, 1, '', 1, 'TEST'
FROM `db_receive_mat` m
CROSS JOIN (SELECT 1 ID UNION SELECT 2 UNION SELECT 3) t
WHERE m.`Report_No` LIKE 'QA26-94%';

INSERT INTO `db_packing_size` (`Report_No`, `BATCH`, `VALUE`, `PACK_COUNT`, `PACKING_SIZE`)
SELECT `Report_No`, 1, `Lot_Size`, 1, `Lot_Size`
FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-94%';


-- ----------------------------------------------------------------------------
-- 5) Inspection Data Check : เฉพาะใบที่ Keep_Data_Need = 1
-- ----------------------------------------------------------------------------
INSERT INTO `db_inspection_data`
    (`REPORT_NO`, `COUNT`, `JUDGE`, `REMARK`, `EMP_ID`, `INSPECTION_DATA_DATE`, `INUSE`)
SELECT m.`Report_No`, 1, 1, 'ทดสอบชุดใหญ่', 'TEST', NOW(), 1
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code`
WHERE m.`Report_No` LIKE 'QA26-94%' AND l.`Keep_Data_Need` = 1;


-- ----------------------------------------------------------------------------
-- 6) master จุดวัด Dimension ของ RCOMM001-SHI
--    DB ทดสอบไม่มีจุดวัดของ M-CODE นี้เลย แต่ใบจริง QA26-058 มี 9 จุด
--    ใส่ตามใบจริง จุดที่ 1 Caliper ที่เหลือ Microscope
-- ----------------------------------------------------------------------------
DELETE FROM `info_dimension_equipment` WHERE `M_CODE` = 'RCOMM001-SHI';

INSERT INTO `info_dimension_equipment`
    (`M_CODE`, `POINT_ORDER`, `EQUIPMENT_TYPE`, `POINT_NAME`, `POINT_CAL`, `CRITERIA_MIN`, `CRITERIA_MAX`, `UNIT`)
VALUES
    ('RCOMM001-SHI', 1, 4, '1', '0', 12.1,  12.3, 'mm'),
    ('RCOMM001-SHI', 2, 1, '2', '0',    0,   0.1, 'mm'),
    ('RCOMM001-SHI', 3, 1, '3', '0',  8.8,   8.9, 'mm'),
    ('RCOMM001-SHI', 4, 1, '4', '0', 0.75,  0.80, 'mm'),
    ('RCOMM001-SHI', 5, 1, '5', '0', 0.57,  0.59, 'mm'),
    ('RCOMM001-SHI', 6, 1, '6', '0', 0.75,  0.80, 'mm'),
    ('RCOMM001-SHI', 7, 1, '7', '0', 0.63,  0.65, 'mm'),
    ('RCOMM001-SHI', 8, 1, '8', '0', 0.75,  0.80, 'mm'),
    ('RCOMM001-SHI', 9, 1, '9', '0', 0.57,  0.59, 'mm');


-- ----------------------------------------------------------------------------
-- 7) Regular Check : QA26-9401 (Fix 4 จุด) และ QA26-9403 (Pc/Cavity 2 จุด)
--    ค่าเก็บที่ REGULAR_NO ไม่ใช่ REPORT_NO เพราะ Regular ทำเดือนละครั้ง
-- ----------------------------------------------------------------------------
INSERT INTO `db_regular_data`
    (`REGULAR_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `REGULAR_DATE`, `INUSE`)
SELECT m.`Regular_No`,
       pt.`POINT_ORDER`,
       sn.`n`,
       1,
       IFNULL(NULLIF(SUBSTRING_INDEX(SUBSTRING_INDEX(rs.`Cavity_Name`, ',', sn.`n`), ',', -1), '0'), '0'),
       @caliper,
       ROUND(pt.`CRITERIA_MIN` + ((pt.`CRITERIA_MAX` - pt.`CRITERIA_MIN`) * ((sn.`n` + pt.`POINT_ORDER`) MOD 7) / 8), 3),
       1, 'TEST', NOW(), 1
FROM `db_receive_mat` m
JOIN `info_regular_sampling` rs ON rs.`M_Code` = m.`M_Code`
JOIN `info_regular_equipment` pt ON pt.`M_CODE` = m.`M_Code`
CROSS JOIN (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4) sn
WHERE m.`Report_No` LIKE 'QA26-94%' AND m.`Regular_No` <> '';


-- ----------------------------------------------------------------------------
-- 8) Function Check
--    QA26-9401 R179S622-YTC : Fix ไม่มี cavity  5 ตัวอย่าง ผ่านหมด
--    QA26-9402 CAM008       : Pc/Cavity 4 cavity (M,N,O,P) cavity P ให้ NG
-- ----------------------------------------------------------------------------
INSERT INTO `db_function_data`
    (`REPORT_NO`, `COUNT`, `SAMPLING_NO`, `CAVITY_NAME`, `LOT_NO`, `JUDGE`, `REMARK`, `EMP_ID`, `FUNCTION_DATE`, `INUSE`)
VALUES
    ('QA26-9401', 1, 1, '0', 'LOT-9401-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9401', 1, 2, '0', 'LOT-9401-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9401', 1, 3, '0', 'LOT-9401-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9401', 1, 4, '0', 'LOT-9401-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9401', 1, 5, '0', 'LOT-9401-A', 1, '', 'TEST', NOW(), 1),

    ('QA26-9402', 1, 1, 'M', 'LOT-9402-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9402', 1, 2, 'N', 'LOT-9402-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9402', 1, 3, 'O', 'LOT-9402-A', 1, '', 'TEST', NOW(), 1),
    ('QA26-9402', 1, 4, 'P', 'LOT-9402-A', 0, 'ประกอบคอนเนคเตอร์ไม่เข้า', 'TEST', NOW(), 1);


-- ----------------------------------------------------------------------------
-- 9) Dimension Check : ไล่ทุกใบที่เปิด Dimension และมี master จุดวัด
--    ค่าเดินอยู่ในเกณฑ์แบบ deterministic  ยกเว้น QA26-9401 ตัวอย่าง 4 จุด 2 ที่จงใจให้หลุด
--    cavity ดึงจาก Cavity_Name ใน setting  ใบที่ไม่มี cavity เก็บเป็น '0'
--    เครื่องมือใช้ตาม EQUIPMENT_TYPE ของแต่ละจุด
-- ----------------------------------------------------------------------------
INSERT INTO `db_dimension_data`
    (`REPORT_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `DIMENSION_DATE`, `INUSE`)
SELECT m.`Report_No`,
       pt.`POINT_ORDER`,
       sn.`n`,
       1,
       IF(ds.`Cavity_Qty` > 0,
          SUBSTRING_INDEX(SUBSTRING_INDEX(ds.`Cavity_Name`, ',', 1 + ((sn.`n` - 1) MOD ds.`Cavity_Qty`)), ',', -1),
          '0'),
       (SELECT MIN(es.`ID`) FROM `info_equipment_serial` es WHERE es.`EQUIPMENT_TYPE_ID` = pt.`EQUIPMENT_TYPE`),
       ROUND(IF(m.`Report_No` = 'QA26-9401' AND sn.`n` = 4 AND pt.`POINT_ORDER` = 2,
                pt.`CRITERIA_MAX` + 0.15,
                pt.`CRITERIA_MIN` + ((pt.`CRITERIA_MAX` - pt.`CRITERIA_MIN`) * ((sn.`n` * 3 + pt.`POINT_ORDER`) MOD 9) / 10)), 4),
       IF(m.`Report_No` = 'QA26-9401' AND sn.`n` = 4 AND pt.`POINT_ORDER` = 2, 0, 1),
       'TEST', NOW(), 1
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l  ON l.`M_CODE`  = m.`M_Code` AND l.`Dimension_Check_Need` = 1
JOIN `info_dimension_sampling` ds  ON ds.`M_Code` = m.`M_Code`
JOIN `info_dimension_equipment` pt ON pt.`M_CODE` = m.`M_Code`
JOIN (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5
      UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9 UNION SELECT 10
      UNION SELECT 11 UNION SELECT 12 UNION SELECT 13 UNION SELECT 14 UNION SELECT 15
      UNION SELECT 16 UNION SELECT 17 UNION SELECT 18 UNION SELECT 19 UNION SELECT 20) sn
  -- All = ทุกชิ้นใน Lot , Sampling Table = เปิดตาราง AQL ตาม Lot Size , ที่เหลือใช้ค่าที่ setting ไว้
  -- ทั้งสองแบบถ้ามี cavity ต้องคูณจำนวน cavity เข้าไปด้วย
  ON sn.`n` <= CASE ds.`Sampling_Type`
                   WHEN 1 THEN m.`Lot_Size`
                   -- ต้องได้อย่างน้อยตัวอย่างละ cavity และอย่างน้อยเท่าที่ตาราง AQL กำหนด
                   WHEN 3 THEN GREATEST(
                                   COALESCE((SELECT st.`Sampling_Qty`
                                             FROM `info_strictness` st
                                             WHERE st.`Strictness_Type`  = ds.`Strictness_Type`
                                               AND st.`Strictness_Level` = ds.`Strictness_Level`
                                               AND m.`Lot_Size` BETWEEN st.`Min_Qty` AND st.`Max_Qty`), 5),
                                   GREATEST(ds.`Cavity_Qty`, 1))
                   ELSE GREATEST(ds.`Sampling_Qty`, 1) * GREATEST(ds.`Cavity_Qty`, 1)
               END
WHERE m.`Report_No` LIKE 'QA26-94%';


-- ----------------------------------------------------------------------------
-- 10) Appearance Check : ทุกใบที่เปิด Appearance
--     แบ่งเป็น 3 batch ให้เห็นหลายแถว  QA26-9404 batch 2 จงใจให้มี NG ค้าง Pending
-- ----------------------------------------------------------------------------
INSERT INTO `db_appearance_data`
    (`REPORT_NO`, `BATCH`, `COUNT`, `QTY_SELECT`, `QTY_OK`, `QTY_NG`, `EMP_ID`, `JUDGE`, `INUSE`, `APPEARANCE_DATE`, `LOT_NO`)
SELECT m.`Report_No`,
       b.`n`,
       1,
       qty.`part`,
       IF(m.`Report_No` = 'QA26-9404' AND b.`n` = 2, qty.`part` - 2, qty.`part`),
       IF(m.`Report_No` = 'QA26-9404' AND b.`n` = 2, 2, 0),
       IF(b.`n` = 2, 'S00822', 'S00823'),
       IF(m.`Report_No` = 'QA26-9404' AND b.`n` = 2, 0, 1),
       1, NOW(), CONCAT('LOT-', RIGHT(m.`Report_No`, 4), '-A')
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code` AND l.`Appearance_Check_Need` = 1
CROSS JOIN (SELECT 1 n UNION SELECT 2 UNION SELECT 3) b
JOIN (SELECT `Report_No`, GREATEST(FLOOR(`Lot_Size` / 3), 1) AS `part`
      FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-94%') qty
  ON qty.`Report_No` = m.`Report_No`
WHERE m.`Report_No` LIKE 'QA26-94%';

-- ของที่ NG ต้องมีรายละเอียดค้างอยู่ในตาราง pending ด้วย
INSERT INTO `db_appearance_pending`
    (`APPEARANCE_ID`, `REPORT_NO`, `BATCH`, `COUNT`, `NG_COUNT`, `QTY_NG`,
     `NG_DETAIL`, `NG_MODE_ID`, `APPEARANCE_DATE`, `RESULT`, `REVIEW_OK_QTY`, `LOT_NO`)
SELECT a.`APPEARANCE_ID`, a.`REPORT_NO`, a.`BATCH`, a.`COUNT`, 1, a.`QTY_NG`,
       'ทดสอบชุดใหญ่ - ผิวมีรอย', (SELECT MIN(`ID`) FROM `info_ngmode`), NOW(), NULL, 0, a.`LOT_NO`
FROM `db_appearance_data` a
WHERE a.`REPORT_NO` = 'QA26-9404' AND a.`BATCH` = 2 AND a.`QTY_NG` > 0;


-- ----------------------------------------------------------------------------
-- AFTER : สรุปว่าแต่ละใบมีข้อมูลอะไรบ้าง
-- ----------------------------------------------------------------------------
SELECT m.`Report_No`, m.`M_Code`, m.`Lot_Size`,
       CONCAT(l.`Keep_Data_Need`, l.`Packing_Check_Mode`, l.`Regular_Check_Need`,
              l.`Function_Check_Need`, l.`Dimension_Check_Need`, l.`Appearance_Check_Need`) AS `kprfda`,
       (SELECT COUNT(*) FROM `db_packing_check`   x WHERE x.`REPORT_NO`  = m.`Report_No`) AS `pack`,
       (SELECT COUNT(*) FROM `db_inspection_data` x WHERE x.`REPORT_NO`  = m.`Report_No`) AS `keep`,
       (SELECT COUNT(*) FROM `db_regular_data`    x WHERE x.`REGULAR_NO` = m.`Regular_No` AND m.`Regular_No` <> '') AS `reg`,
       (SELECT COUNT(*) FROM `db_function_data`   x WHERE x.`REPORT_NO`  = m.`Report_No`) AS `func`,
       (SELECT COUNT(*) FROM `db_dimension_data`  x WHERE x.`REPORT_NO`  = m.`Report_No`) AS `dim`,
       (SELECT COUNT(DISTINCT x.`SAMPLING_NO`) FROM `db_dimension_data` x WHERE x.`REPORT_NO` = m.`Report_No`) AS `dim_samp`,
       (SELECT COUNT(*) FROM `db_appearance_data` x WHERE x.`REPORT_NO`  = m.`Report_No`) AS `app`
FROM `db_receive_mat` m
JOIN `info_mat_inspection_list` l ON l.`M_CODE` = m.`M_Code`
WHERE m.`Report_No` LIKE 'QA26-94%'
ORDER BY m.`Report_No`;
