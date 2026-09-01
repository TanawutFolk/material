-- ============================================================================
-- สร้างข้อมูลทดสอบหน้า Pending ของ Admin ครบทุกเมนู
--   Packing / Regular / Ins.Data / Function / Dimension / Appearance
-- ============================================================================
-- แยก report คนละใบต่อ 1 เมนู จะได้กดทดสอบทีละอันไม่ปนกัน
--
--   QA26-9201  ->  Packing Check Pending
--   QA26-9202  ->  Regular Check Pending
--   QA26-9203  ->  Insp. Data Check Pending
--   QA26-9204  ->  Function Check Pending
--   QA26-9205  ->  Dimension Check Pending
--   QA26-9206  ->  Appearance Check Pending
--
-- เงื่อนไขที่ค้นเจอจากโค้ด
--   list ของ Packing/Regular/Data/Function/Dimension = db_report_status.<process> = 6
--   หน้า detail ต้องมีข้อมูลจริงรองรับด้วย มิฉะนั้นจอจะว่าง :
--     Regular    -> db_regular_data    INUSE=1 JUDGE=0  (join ด้วย REGULAR_NO)
--     Function   -> db_function_data   INUSE=1 JUDGE=0
--     Dimension  -> db_dimension_data  INUSE=1 JUDGE=0
--     Ins.Data   -> db_inspection_data INUSE=1 JUDGE IN (0,6)
--     Appearance -> ไม่ได้ดูสถานะ 6 แต่ดู db_appearance_pending.RESULT IS NULL
--                   และ COALESCE(QTY_NG,0) > 0
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

SET @MC   = 'R179S622-YTC';
SET @NAME = 'Plate-CMOS-33A2361B';
SET @VEN  = 'YTC Co., Ltd.';


-- ----------------------------------------------------------------------------
-- 1) ล้างของเดิม (รันซ้ำได้)
-- ----------------------------------------------------------------------------
DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` LIKE 'QA26-92%';
DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` LIKE 'QA26-92%';
DELETE FROM `db_dimension_data`     WHERE `REPORT_NO` LIKE 'QA26-92%';
DELETE FROM `db_function_data`      WHERE `REPORT_NO` LIKE 'QA26-92%';
DELETE FROM `db_inspection_data`    WHERE `REPORT_NO` LIKE 'QA26-92%';
DELETE FROM `db_regular_data`       WHERE `REGULAR_NO` LIKE 'RI2608-92%';
DELETE FROM `db_packing_size`       WHERE `Report_No` LIKE 'QA26-92%';
DELETE FROM `db_report_lot_no`      WHERE `REPORT_NO` LIKE 'QA26-92%';
DELETE FROM `db_report_status`      WHERE `Report_No` LIKE 'QA26-92%';
DELETE FROM `db_receive_mat`        WHERE `Report_No` LIKE 'QA26-92%';


-- ----------------------------------------------------------------------------
-- 2) report ทั้ง 6 ใบ
-- ----------------------------------------------------------------------------
INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
VALUES
    ('QA26-9201', CURDATE(), 1, 'RI2608-9201', @MC, @NAME, 'TEST-PEND-PACK', @VEN, 50, 8, 'TEST', NOW()),
    ('QA26-9202', CURDATE(), 1, 'RI2608-9202', @MC, @NAME, 'TEST-PEND-REG',  @VEN, 50, 8, 'TEST', NOW()),
    ('QA26-9203', CURDATE(), 1, 'RI2608-9203', @MC, @NAME, 'TEST-PEND-DATA', @VEN, 50, 8, 'TEST', NOW()),
    ('QA26-9204', CURDATE(), 1, 'RI2608-9204', @MC, @NAME, 'TEST-PEND-FUNC', @VEN, 50, 8, 'TEST', NOW()),
    ('QA26-9205', CURDATE(), 1, 'RI2608-9205', @MC, @NAME, 'TEST-PEND-DIM',  @VEN, 50, 8, 'TEST', NOW()),
    ('QA26-9206', CURDATE(), 1, 'RI2608-9206', @MC, @NAME, 'TEST-PEND-APP',  @VEN, 50, 8, 'TEST', NOW());

INSERT INTO `db_packing_size` (`Report_No`, `BATCH`, `VALUE`, `PACK_COUNT`, `PACKING_SIZE`)
SELECT `Report_No`, 1, 50, 1, 8 FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-92%';

INSERT INTO `db_report_lot_no` (`REPORT_NO`, `LOT_NO`)
SELECT `Report_No`, CONCAT('LOT-', RIGHT(`Report_No`, 4)) FROM `db_receive_mat` WHERE `Report_No` LIKE 'QA26-92%';


-- ----------------------------------------------------------------------------
-- 3) สถานะ : ตั้ง 6 (PENDING) เฉพาะ process ที่ต้องการทดสอบ
--    ขั้นก่อนหน้าตั้ง 1 (OK) เพื่อให้ flow สมเหตุสมผล
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_status`
    (`Report_No`, `Keep_Data`, `Receive_WH`, `Packing_Check`, `Regular_Check`,
     `Inspection_Data_Check`, `Function_Check`, `Dimension_Check`, `Appearance_Check`, `Report_Status`)
VALUES
    ('QA26-9201', 1, 1,    6, NULL, NULL, NULL, NULL, NULL, 6),   -- Packing pending
    ('QA26-9202', 1, 1,    1,    6, NULL, NULL, NULL, NULL, 6),   -- Regular pending
    ('QA26-9203', 1, 1,    1,    1,    6, NULL, NULL, NULL, 6),   -- Ins.Data pending
    ('QA26-9204', 1, 1,    1,    1,    1,    6, NULL, NULL, 6),   -- Function pending
    ('QA26-9205', 1, 1,    1,    1,    1, NULL,    6, NULL, 6),   -- Dimension pending
    ('QA26-9206', 1, 1,    1,    1,    1, NULL, NULL,    6, 6);   -- Appearance pending


-- ----------------------------------------------------------------------------
-- 4) ข้อมูลรองรับหน้า detail ของแต่ละ Pending
-- ----------------------------------------------------------------------------

-- 4.1 Regular : JUDGE=0 (NG) 4 จุด  - join ด้วย REGULAR_NO
--     serial : Caliper=134 , Micrometer=132 , Microscope=131 , Jig=144
--     ค่าตั้งใจให้หลุด spec จะได้เห็นว่าเป็น NG จริง
INSERT INTO `db_regular_data`
    (`REGULAR_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `REGULAR_DATE`, `INUSE`)
VALUES
    ('RI2608-9202', 1, 1, 1, '0', 134, 6.90000, 0, 'TEST', NOW(), 1),  -- spec 6.14685-6.55321
    ('RI2608-9202', 2, 1, 1, '0', 132,  10.900, 0, 'TEST', NOW(), 1),  -- spec 10.2-10.8
    ('RI2608-9202', 3, 1, 1, '0', 131,   2.100, 0, 'TEST', NOW(), 1),  -- spec 2.5-3.5
    ('RI2608-9202', 4, 1, 1, '0', 144,       0, 0, 'TEST', NOW(), 1);  -- OK/NG -> NG

-- 4.2 Inspection Data : JUDGE=6 (Pending)
INSERT INTO `db_inspection_data`
    (`REPORT_NO`, `COUNT`, `REMARK`, `JUDGE`, `EMP_ID`, `INSPECTION_DATA_DATE`, `INUSE`)
VALUES
    ('QA26-9203', 1, 'ทดสอบ - เอกสาร vendor ไม่ครบ ขอให้ admin ตรวจสอบ', 6, 'TEST', NOW(), 1);

-- 4.3 Function : JUDGE=0 (NG)
INSERT INTO `db_function_data`
    (`REPORT_NO`, `COUNT`, `SAMPLING_NO`, `CAVITY_NAME`, `LOT_NO`, `JUDGE`, `REMARK`, `EMP_ID`, `FUNCTION_DATE`, `INUSE`)
VALUES
    ('QA26-9204', 1, 1, '0', 'LOT-9204', 0, 'ทดสอบ - Function NG', 'TEST', NOW(), 1),
    ('QA26-9204', 1, 2, '0', 'LOT-9204', 0, 'ทดสอบ - Function NG', 'TEST', NOW(), 1);

-- 4.4 Dimension : JUDGE=0 (NG) 4 จุด
--     info_dimension_equipment ของ M-Code นี้ยังเป็น Jig/OK-NG ทั้ง 4 จุด
INSERT INTO `db_dimension_data`
    (`REPORT_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `DIMENSION_DATE`, `INUSE`)
VALUES
    ('QA26-9205', 1, 1, 1, '0', 144, 0, 0, 'TEST', NOW(), 1),
    ('QA26-9205', 2, 1, 1, '0', 144, 0, 0, 'TEST', NOW(), 1),
    ('QA26-9205', 3, 1, 1, '0', 144, 0, 0, 'TEST', NOW(), 1),
    ('QA26-9205', 4, 1, 1, '0', 144, 0, 0, 'TEST', NOW(), 1);

-- 4.5 Appearance : ต้องมีทั้ง db_appearance_data (ตัวแม่) และ db_appearance_pending
--     ตรวจ 8 ชิ้น -> OK 5 / NG 3 แล้วแตก NG เป็น 3 อาการ
INSERT INTO `db_appearance_data`
    (`REPORT_NO`, `BATCH`, `COUNT`, `LOT_NO`, `QTY_SELECT`, `QTY_OK`, `QTY_NG`,
     `EMP_ID`, `JUDGE`, `APPEARANCE_DATE`, `UPDATETIME`, `INUSE`)
VALUES
    ('QA26-9206', 1, 1, 'LOT-9206', 8, 5, 3, 'TEST', 0, CURDATE(), NOW(), 1);

INSERT INTO `db_appearance_pending`
    (`APPEARANCE_ID`, `REPORT_NO`, `BATCH`, `COUNT`, `LOT_NO`, `NG_COUNT`, `QTY_NG`,
     `NG_DETAIL`, `NG_MODE_ID`, `APPEARANCE_DATE`, `UPDATETIME`)
SELECT a.`APPEARANCE_ID`, 'QA26-9206', 1, 1, 'LOT-9206', v.n, v.q, v.d, v.m, NOW(), NOW()
FROM `db_appearance_data` a
JOIN (
    SELECT 0 AS n, 1 AS q, 'Bending'     AS d, (SELECT ID FROM info_ngmode WHERE NG_Mode='Bending'     LIMIT 1) AS m
    UNION ALL SELECT 1, 1, 'Black stain', (SELECT ID FROM info_ngmode WHERE NG_Mode='Black stain' LIMIT 1)
    UNION ALL SELECT 2, 1, 'Dirty',       (SELECT ID FROM info_ngmode WHERE NG_Mode='Dirty'       LIMIT 1)
) v
WHERE a.`REPORT_NO` = 'QA26-9206' AND a.`INUSE` = 1;


-- ----------------------------------------------------------------------------
-- ตรวจสอบ
-- ----------------------------------------------------------------------------
SELECT 'สถานะ report ทดสอบ' AS phase, s.`Report_No`, s.`Packing_Check` pack, s.`Regular_Check` reg,
       s.`Inspection_Data_Check` data, s.`Function_Check` func, s.`Dimension_Check` dim,
       s.`Appearance_Check` app, m.`Regular_No`
FROM `db_report_status` s JOIN `db_receive_mat` m ON s.`Report_No` = m.`Report_No`
WHERE s.`Report_No` LIKE 'QA26-92%' ORDER BY s.`Report_No`;

SELECT 'ข้อมูลรองรับหน้า detail' AS phase,
       (SELECT COUNT(*) FROM `db_regular_data`    WHERE `REGULAR_NO`='RI2608-9202' AND INUSE=1 AND JUDGE=0)          AS regular_rows,
       (SELECT COUNT(*) FROM `db_inspection_data` WHERE `REPORT_NO`='QA26-9203' AND INUSE=1 AND JUDGE IN (0,6))      AS insdata_rows,
       (SELECT COUNT(*) FROM `db_function_data`   WHERE `REPORT_NO`='QA26-9204' AND INUSE=1 AND JUDGE=0)             AS function_rows,
       (SELECT COUNT(*) FROM `db_dimension_data`  WHERE `REPORT_NO`='QA26-9205' AND INUSE=1 AND JUDGE=0)             AS dimension_rows,
       (SELECT COUNT(*) FROM `db_appearance_pending` WHERE `REPORT_NO`='QA26-9206' AND `RESULT` IS NULL)             AS appear_pending_rows;

-- นับตามที่ปุ่มบน header นับจริง
SELECT 'ตัวเลขที่จะขึ้นบนปุ่ม header' AS phase,
       (SELECT COUNT(*) FROM `db_report_status` WHERE `Packing_Check`=6)          AS packing_pending,
       (SELECT COUNT(*) FROM `db_report_status` WHERE `Regular_Check`=6)          AS regular_pending,
       (SELECT COUNT(*) FROM `db_report_status` WHERE `Inspection_Data_Check`=6)  AS insdata_pending,
       (SELECT COUNT(*) FROM `db_report_status` WHERE `Function_Check`=6)         AS function_pending,
       (SELECT COUNT(*) FROM `db_report_status` WHERE `Dimension_Check`=6)        AS dimension_pending,
       (SELECT COUNT(DISTINCT COALESCE(CAST(p.APPEARANCE_ID AS CHAR),
               CONCAT(p.REPORT_NO,'|',COALESCE(CAST(p.BATCH AS CHAR),''),'|',
                      COALESCE(CAST(p.COUNT AS CHAR),''),'|',COALESCE(p.LOT_NO,''))))
        FROM `db_appearance_pending` p
        JOIN `db_report_status` a ON p.REPORT_NO=a.Report_No
        JOIN `db_receive_mat`   b ON a.Report_No=b.Report_No
        JOIN `info_mat_inspection_list` e ON b.M_Code=e.M_CODE
        WHERE p.RESULT IS NULL AND COALESCE(p.QTY_NG,0)>0 AND e.Appearance_Check_Need=1) AS appearance_pending;


-- ============================================================================
-- ROLLBACK - ลบข้อมูลทดสอบทั้งหมด
-- ============================================================================
-- DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` LIKE 'QA26-92%';
-- DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` LIKE 'QA26-92%';
-- DELETE FROM `db_dimension_data`     WHERE `REPORT_NO` LIKE 'QA26-92%';
-- DELETE FROM `db_function_data`      WHERE `REPORT_NO` LIKE 'QA26-92%';
-- DELETE FROM `db_inspection_data`    WHERE `REPORT_NO` LIKE 'QA26-92%';
-- DELETE FROM `db_regular_data`       WHERE `REGULAR_NO` LIKE 'RI2608-92%';
-- DELETE FROM `db_packing_size`       WHERE `Report_No` LIKE 'QA26-92%';
-- DELETE FROM `db_report_lot_no`      WHERE `REPORT_NO` LIKE 'QA26-92%';
-- DELETE FROM `db_report_status`      WHERE `Report_No` LIKE 'QA26-92%';
-- DELETE FROM `db_receive_mat`        WHERE `Report_No` LIKE 'QA26-92%';
