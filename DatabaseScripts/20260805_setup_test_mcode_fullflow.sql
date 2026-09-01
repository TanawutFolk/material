-- ============================================================================
-- ตั้งค่า M-Code + สร้าง Report สำหรับทดสอบครบทุกเมนู (Packing -> Appearance)
-- ============================================================================
-- M-Code ที่เลือก : R179S622-YTC
--   เหตุผล - มีอยู่ใน mes.item_manufacturing แล้ว (JOIN ในคิวงานผ่าน)
--            Plate-CMOS-33A2361B / vendor YTC Co., Ltd.
--          - มี info_*_sampling ครบทั้ง 4 ชนิด
--          - มีจุดวัด Regular 4 จุด และ Dimension 4 จุด
--          - เปิด check ไว้แล้ว 4 จาก 5 ขาดแค่ Function
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `M_CODE`, `Keep_Data_Need` k, `Packing_Check_Mode` p, `Regular_Check_Need` r,
       `Function_Check_Need` f, `Dimension_Check_Need` d, `Appearance_Check_Need` a, `INUSE`
FROM `info_mat_inspection_list` WHERE `M_CODE` = 'R179S622-YTC';


-- ----------------------------------------------------------------------------
-- 1) เปิด check ให้ครบทุกขั้นตอน
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Keep_Data_Need`        = 1,   -- เปิด Inspection Data Check ด้วย (ธงเดียวคุม 2 อย่าง)
       `Packing_Check_Mode`    = 1,
       `Regular_Check_Need`    = 1,
       `Function_Check_Need`   = 1,   -- <-- เดิมเป็น 0
       `Dimension_Check_Need`  = 1,
       `Appearance_Check_Need` = 1,
       `INUSE`                 = 1
 WHERE `M_CODE` = 'R179S622-YTC';


-- ----------------------------------------------------------------------------
-- 2) ลบ report ทดสอบเดิม (ถ้ารันซ้ำ) เพื่อให้เริ่มใหม่ได้สะอาด
-- ----------------------------------------------------------------------------
DELETE FROM `db_appearance_pending` WHERE `REPORT_NO`  IN ('QA26-9101','QA26-9102','QA26-9103');
DELETE FROM `db_appearance_data`    WHERE `REPORT_NO`  IN ('QA26-9101','QA26-9102','QA26-9103');
DELETE FROM `db_inspection_data`    WHERE `REPORT_NO`  IN ('QA26-9101','QA26-9102','QA26-9103');
DELETE FROM `db_packing_size`       WHERE `Report_No`  IN ('QA26-9101','QA26-9102','QA26-9103');
DELETE FROM `db_report_status`      WHERE `Report_No`  IN ('QA26-9101','QA26-9102','QA26-9103');
DELETE FROM `db_receive_mat`        WHERE `Report_No`  IN ('QA26-9101','QA26-9102','QA26-9103');


-- ----------------------------------------------------------------------------
-- 3) สร้าง report ทดสอบ 3 ใบ (จะได้ทดสอบซ้ำได้หลายรอบ)
-- ----------------------------------------------------------------------------
-- Lot_Size = 50 เลือกให้จำนวนสุ่มตรวจน้อย ทดสอบเร็ว
--   Regular   : Fix 5 ชิ้น x 4 จุด
--   Function  : Fix 5 ชิ้น
--   Dimension : Fix 5 ชิ้น x 4 จุด
--   Appearance: Sampling Table Normal/II -> lot 26-50 = 8 ชิ้น
-- Inspection_Qty ปล่อย NULL - หน้า Packing Check จะเป็นคนเขียนให้เอง
INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
VALUES
    ('QA26-9101', CURDATE(), 1, NULL, 'R179S622-YTC', 'Plate-CMOS-33A2361B', 'TEST-INV-9101', 'YTC Co., Ltd.', 50, NULL, 'TEST', NOW()),
    ('QA26-9102', CURDATE(), 1, NULL, 'R179S622-YTC', 'Plate-CMOS-33A2361B', 'TEST-INV-9102', 'YTC Co., Ltd.', 50, NULL, 'TEST', NOW()),
    ('QA26-9103', CURDATE(), 1, NULL, 'R179S622-YTC', 'Plate-CMOS-33A2361B', 'TEST-INV-9103', 'YTC Co., Ltd.', 50, NULL, 'TEST', NOW());


-- ----------------------------------------------------------------------------
-- 4) ตั้งสถานะให้เริ่มที่คิว Packing Check
-- ----------------------------------------------------------------------------
-- เงื่อนไขคิว Packing (SearchForOpPackingCheck) :
--     Receive_WH = 1  AND  Packing_Check IS NULL / 2 / 8
INSERT INTO `db_report_status`
    (`Report_No`, `Keep_Data`, `Receive_WH`, `Packing_Check`, `Regular_Check`,
     `Inspection_Data_Check`, `Function_Check`, `Dimension_Check`, `Appearance_Check`, `Report_Status`)
VALUES
    ('QA26-9101', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, 8),
    ('QA26-9102', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, 8),
    ('QA26-9103', 1, 1, NULL, NULL, NULL, NULL, NULL, NULL, 8);


-- ----------------------------------------------------------------------------
-- ตรวจสอบ
-- ----------------------------------------------------------------------------
SELECT 'AFTER - การตั้งค่า M-Code' AS phase, `M_CODE`, `Keep_Data_Need` k, `Packing_Check_Mode` p,
       `Regular_Check_Need` r, `Function_Check_Need` f, `Dimension_Check_Need` d,
       `Appearance_Check_Need` a, `INUSE`
FROM `info_mat_inspection_list` WHERE `M_CODE` = 'R179S622-YTC';

SELECT 'report ทดสอบที่สร้าง' AS phase, m.`Report_No`, m.`M_Code`, m.`Lot_Size`, m.`Inspection_Qty`,
       s.`Keep_Data`, s.`Receive_WH`, s.`Packing_Check`, s.`Regular_Check`,
       s.`Inspection_Data_Check`, s.`Function_Check`, s.`Dimension_Check`, s.`Appearance_Check`
FROM `db_receive_mat` m JOIN `db_report_status` s ON m.`Report_No` = s.`Report_No`
WHERE m.`Report_No` LIKE 'QA26-91%' ORDER BY m.`Report_No`;

-- ต้องขึ้นในคิว Packing Check ทั้ง 3 ใบ
SELECT 'CHECK คิว Packing Check (ต้องเจอ 3 ใบ)' AS check_name, a.`Report_No`, b.`M_Code`, b.`Lot_Size`
FROM `db_report_status` a
JOIN `db_receive_mat` b ON a.`Report_No` = b.`Report_No`
JOIN mes.item_manufacturing c ON b.`M_Code` = c.`ITEM_CODE_FOR_SUPPORT_MES`
JOIN mes.vendor d ON c.`VENDOR_ID` = d.`VENDOR_ID`
WHERE a.`Receive_WH` = 1
  AND (a.`Packing_Check` IS NULL OR a.`Packing_Check` = 2 OR a.`Packing_Check` = 8)
  AND a.`Report_No` LIKE 'QA26-91%';


-- ============================================================================
-- ROLLBACK - ลบ report ทดสอบและคืนค่า Function_Check_Need
-- ============================================================================
-- DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` IN ('QA26-9101','QA26-9102','QA26-9103');
-- DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` IN ('QA26-9101','QA26-9102','QA26-9103');
-- DELETE FROM `db_inspection_data`    WHERE `REPORT_NO` IN ('QA26-9101','QA26-9102','QA26-9103');
-- DELETE FROM `db_packing_size`       WHERE `Report_No` IN ('QA26-9101','QA26-9102','QA26-9103');
-- DELETE FROM `db_report_status`      WHERE `Report_No` IN ('QA26-9101','QA26-9102','QA26-9103');
-- DELETE FROM `db_receive_mat`        WHERE `Report_No` IN ('QA26-9101','QA26-9102','QA26-9103');
-- UPDATE `info_mat_inspection_list` SET `Function_Check_Need` = 0 WHERE `M_CODE` = 'R179S622-YTC';
