-- ============================================================================
-- ดัน report ทดสอบให้ไปยืนรอที่คิว Regular Check เลย
-- จะได้ทดสอบ Regular ได้ทันทีโดยไม่ต้องทำ Packing Check ก่อน
-- ============================================================================
-- เงื่อนไขคิว Regular (SearchForOpRegular - QAdataSQL.cs:449) :
--     a.Packing_Check = 1  AND  Packing_Check IS NOT NULL
--     a.Regular_Check IS NULL / 2 / 8
--     b.Regular_No IS NOT NULL          <-- ข้อนี้สำคัญ ถ้าไม่มีจะไม่ขึ้นในคิวเลย
--
-- จำลองสภาพ "Packing Check เสร็จแล้ว" ให้ครบตามที่หน้า Packing เขียนจริง :
--     db_receive_mat.Regular_No       <- UpdateRegularNo()
--     db_receive_mat.Inspection_Qty   <- UpdateInspQtyAppear()
--     db_packing_size                 <- InsertPackingSize()
--     db_report_lot_no                <- Regular อ่านผ่าน ReportLot()
--
-- การคำนวณ Inspection_Qty (userControlPackingCheck.cs:844-887, SAMPLING_TYPE=3)
--     VALUE = 50 -> info_strictness Normal/II ช่วง 26-50 = 8
--     cavityCal = CAVITY_QTY(0) x SAMPLING_QTY(0) = 0
--     intSelect = MAX(8, 0) = 8   ->   PACKING_SIZE = 8 x PACK_COUNT(1) = 8
--
-- QA26-9101 : ปล่อยไว้ที่ Packing Check (ทดสอบ flow เต็มตั้งแต่ต้น)
-- QA26-9102 : ดันมาที่ Regular
-- QA26-9103 : ดันมาที่ Regular (เผื่อทดสอบซ้ำ)
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, m.`Report_No`, m.`Regular_No`, m.`Lot_Size`, m.`Inspection_Qty`,
       s.`Receive_WH`, s.`Packing_Check`, s.`Regular_Check`
FROM `db_receive_mat` m JOIN `db_report_status` s ON m.`Report_No` = s.`Report_No`
WHERE m.`Report_No` LIKE 'QA26-91%' ORDER BY m.`Report_No`;


-- ----------------------------------------------------------------------------
-- 1) เคลียร์ของเดิม (รันซ้ำได้)
-- ----------------------------------------------------------------------------
DELETE FROM `db_packing_size`  WHERE `Report_No` IN ('QA26-9102','QA26-9103');
DELETE FROM `db_report_lot_no` WHERE `REPORT_NO` IN ('QA26-9102','QA26-9103');


-- ----------------------------------------------------------------------------
-- 2) ผลลัพธ์ของ Packing Check
-- ----------------------------------------------------------------------------
UPDATE `db_receive_mat`
   SET `Regular_No`     = CONCAT('RI2608-', RIGHT(`Report_No`, 4)),
       `Inspection_Qty` = 8
 WHERE `Report_No` IN ('QA26-9102','QA26-9103');

INSERT INTO `db_packing_size` (`Report_No`, `BATCH`, `VALUE`, `PACK_COUNT`, `PACKING_SIZE`)
VALUES ('QA26-9102', 1, 50, 1, 8),
       ('QA26-9103', 1, 50, 1, 8);

INSERT INTO `db_report_lot_no` (`REPORT_NO`, `LOT_NO`)
VALUES ('QA26-9102', 'LOT-TEST-9102'),
       ('QA26-9103', 'LOT-TEST-9103');


-- ----------------------------------------------------------------------------
-- 3) สถานะ : Packing เสร็จ (=1) รอ Regular (=NULL)
-- ----------------------------------------------------------------------------
UPDATE `db_report_status`
   SET `Packing_Check` = 1,
       `Regular_Check` = NULL,
       `Report_Status` = 8
 WHERE `Report_No` IN ('QA26-9102','QA26-9103');


-- ----------------------------------------------------------------------------
-- ตรวจสอบ
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase, m.`Report_No`, m.`Regular_No`, m.`Lot_Size`, m.`Inspection_Qty`,
       s.`Receive_WH`, s.`Packing_Check`, s.`Regular_Check`,
       (SELECT COUNT(*) FROM `db_packing_size`  p WHERE p.`Report_No` = m.`Report_No`) AS packing_rows,
       (SELECT COUNT(*) FROM `db_report_lot_no` l WHERE l.`REPORT_NO` = m.`Report_No`) AS lot_rows
FROM `db_receive_mat` m JOIN `db_report_status` s ON m.`Report_No` = s.`Report_No`
WHERE m.`Report_No` LIKE 'QA26-91%' ORDER BY m.`Report_No`;

-- รันเงื่อนไขคิว Regular จริง - ต้องเจอ QA26-9102 กับ QA26-9103
SELECT 'CHECK คิว Regular (ต้องเจอ 2 ใบ)' AS check_name,
       a.`Report_No`, b.`Regular_No`, b.`M_Code`, b.`Lot_Size`, e.`Regular_Check_Ref`
FROM `db_report_status` a
JOIN `db_receive_mat` b ON a.`Report_No` = b.`Report_No`
JOIN mes.item_manufacturing c ON b.`M_Code` = c.`ITEM_CODE_FOR_SUPPORT_MES`
JOIN mes.vendor d ON c.`VENDOR_ID` = d.`VENDOR_ID`
JOIN `info_mat_inspection_list` e ON b.`M_Code` = e.`M_CODE`
WHERE (a.`Packing_Check` = 1 AND a.`Packing_Check` IS NOT NULL)
  AND (a.`Regular_Check` IS NULL OR a.`Regular_Check` = 2 OR a.`Regular_Check` = 8)
  AND (b.`Regular_No` IS NOT NULL)
  AND a.`Report_No` LIKE 'QA26-91%';

-- QA26-9101 ต้องยังอยู่คิว Packing
SELECT 'CHECK คิว Packing (ต้องเจอ QA26-9101 ใบเดียว)' AS check_name, a.`Report_No`
FROM `db_report_status` a
JOIN `db_receive_mat` b ON a.`Report_No` = b.`Report_No`
WHERE a.`Receive_WH` = 1
  AND (a.`Packing_Check` IS NULL OR a.`Packing_Check` = 2 OR a.`Packing_Check` = 8)
  AND a.`Report_No` LIKE 'QA26-91%';


-- ============================================================================
-- ROLLBACK - ดันกลับไปเริ่มที่ Packing เหมือนเดิม
-- ============================================================================
-- DELETE FROM `db_packing_size`  WHERE `Report_No` IN ('QA26-9102','QA26-9103');
-- DELETE FROM `db_report_lot_no` WHERE `REPORT_NO` IN ('QA26-9102','QA26-9103');
-- UPDATE `db_receive_mat`   SET `Regular_No` = NULL, `Inspection_Qty` = NULL
--  WHERE `Report_No` IN ('QA26-9102','QA26-9103');
-- UPDATE `db_report_status` SET `Packing_Check` = NULL, `Regular_Check` = NULL
--  WHERE `Report_No` IN ('QA26-9102','QA26-9103');
