-- ============================================================================
-- ใบทดสอบเส้นทางบันทึกผลตัดสินรายชิ้น (db_dimension_piece_judge)
-- ============================================================================
-- QA26-9601 / RFLCA072-JIN / Lot 3
--   Lot เล็กสุดเท่าที่ยังเห็นทั้งเคสผ่านและไม่ผ่าน  3 ชิ้น x 9 จุด = 27 ช่อง
--   (QA26-9404 เป็น Lot 20 ต้องพิมพ์ 180 ช่อง เยอะเกินไปสำหรับทดสอบกลไก)
--
-- ตั้งใจไม่ใส่ db_dimension_data ไว้เลย ให้กรอกเองผ่านโปรแกรมทั้งหมด
-- ขั้นก่อนหน้าผ่านหมดแล้ว Dimension_Check = NULL ใบจะโผล่ในคิวทันที
--
-- เกณฑ์ของ M-CODE นี้ : ทุกจุด 6.1468 ~ 6.5532 mm
--   tolerance ผลต่าง = (6.5532 - 6.1468) / 2 = 0.2032
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase,
       (SELECT COUNT(*) FROM `db_receive_mat`            WHERE `Report_No` = 'QA26-9601') AS receive_,
       (SELECT COUNT(*) FROM `db_dimension_data`         WHERE `REPORT_NO` = 'QA26-9601') AS dim_data,
       (SELECT COUNT(*) FROM `db_dimension_piece_judge`  WHERE `REPORT_NO` = 'QA26-9601') AS piece_judge;


-- ----------------------------------------------------------------------------
-- 0) ล้างของเดิม
-- ----------------------------------------------------------------------------
DELETE FROM `db_dimension_piece_judge` WHERE `REPORT_NO` = 'QA26-9601';
DELETE FROM `db_dimension_data`        WHERE `REPORT_NO` = 'QA26-9601';
DELETE FROM `db_appearance_data`       WHERE `REPORT_NO` = 'QA26-9601';
DELETE FROM `db_packing_check`         WHERE `REPORT_NO` = 'QA26-9601';
DELETE FROM `db_packing_size`          WHERE `Report_No` = 'QA26-9601';
DELETE FROM `db_report_lot_no`         WHERE `REPORT_NO` = 'QA26-9601';
DELETE FROM `db_report_status`         WHERE `Report_No` = 'QA26-9601';
DELETE FROM `db_receive_mat`           WHERE `Report_No` = 'QA26-9601';


-- ----------------------------------------------------------------------------
-- 1) หัวใบรับของ
-- ----------------------------------------------------------------------------
INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
SELECT 'QA26-9601', CURDATE(), 1, '', 'RFLCA072-JIN',
       COALESCE(c.`ITEM_EXTERNAL_SHORT_NAME`, 'RFLCA072-JIN'),
       'TEST-9601', COALESCE(v.`VENDOR_NAME`, 'TEST VENDOR'),
       3, 3, 'TEST', NOW()
FROM `mes`.`item_manufacturing` c
LEFT JOIN `mes`.`vendor` v ON v.`VENDOR_ID` = c.`VENDOR_ID`
WHERE c.`ITEM_CODE_FOR_SUPPORT_MES` = 'RFLCA072-JIN'
LIMIT 1;


-- ----------------------------------------------------------------------------
-- 2) สถานะ : Dimension เว้น NULL รอทำ , Appearance เว้นไว้ทำต่อได้
--    RFLCA072-JIN มี Keep_Data_Need = 0 -> Inspection_Data_Check ต้องเป็น 3 (Skip)
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_status`
    (`Report_No`, `Keep_Data`, `Receive_WH`, `Emp_Receive_WH`, `Receive_WH_Date`,
     `Packing_Check`, `Regular_Check`, `Regular_Check_Lot_No`, `Inspection_Data_Check`,
     `Function_Check`, `Dimension_Check`, `Appearance_Check`, `Report_Status`)
VALUES
    ('QA26-9601', 0, 1, 'TEST', NOW(), 1, 3, '', 3, 3, NULL, NULL, 1);


-- ----------------------------------------------------------------------------
-- 3) Lot No. + Packing ให้ครบ ใบจะได้ไหลถึง Dimension
-- ----------------------------------------------------------------------------
INSERT INTO `db_report_lot_no` (`REPORT_NO`, `LOT_NO`) VALUES ('QA26-9601', 'LOT-9601-A');

INSERT INTO `db_packing_check`
    (`REPORT_NO`, `METHOD_ID`, `COUNT`, `DETAIL_JUDGE`, `JUDGMENT`, `EMP_PACKING_CHECK`)
SELECT 'QA26-9601', t.`ID`, 1, '', 1, 'TEST'
FROM (SELECT 1 ID UNION SELECT 2 UNION SELECT 3) t;

INSERT INTO `db_packing_size` (`Report_No`, `BATCH`, `VALUE`, `PACK_COUNT`, `PACKING_SIZE`)
VALUES ('QA26-9601', 1, 3, 1, 3);


-- ----------------------------------------------------------------------------
-- AFTER : ต้องเห็นใบนี้ในคิว Dimension
-- ----------------------------------------------------------------------------
SELECT a.`Report_No`, b.`M_Code`, b.`Lot_Size`,
       d.`Sampling_Type` AS `samp_type`,
       IFNULL(a.`Dimension_Check`, 'NULL') AS `dim_status`,
       (SELECT COUNT(*) FROM `db_dimension_data` x WHERE x.`REPORT_NO` = a.`Report_No`) AS `dim_rows`
FROM `db_report_status` a
JOIN `db_receive_mat` b ON a.`Report_No` = b.`Report_No`
JOIN `mes`.`item_manufacturing` c ON b.`M_Code` = c.`ITEM_CODE_FOR_SUPPORT_MES`
JOIN `mes`.`vendor` v ON c.`VENDOR_ID` = v.`VENDOR_ID`
JOIN `info_mat_inspection_list` e ON b.`M_Code` = e.`M_CODE`
LEFT JOIN `info_dimension_sampling` d ON d.`M_Code` = b.`M_Code`
WHERE ((a.`Inspection_Data_Check` = 3 AND a.`Packing_Check` = 1)
    OR (a.`Inspection_Data_Check` = 1 AND e.`Keep_Data_Need` = 1))
  AND (a.`Dimension_Check` IS NULL OR a.`Dimension_Check` = 8 OR a.`Dimension_Check` = 2)
  AND e.`Dimension_Check_Need` = 1
  AND a.`Report_No` = 'QA26-9601';
