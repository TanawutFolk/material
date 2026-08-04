-- ============================================================================
-- Phase 3 : ตั้งค่า M-Code ที่ "เก็บเอกสาร แต่ไม่ต้องให้ QA ตรวจ"
-- ============================================================================
-- ต้องรัน Phase 1 (20260804_add_data_result_need_phase1.sql) มาก่อน
--
-- รายการที่ยืนยันจากผู้ใช้ (2026-08-04):
--   SHIN005-02 ถึง SHIN005-08  ->  Keep_Data_Need = 1 , Data_Result_Need = 0
--   หมายเหตุ: SHIN005-01 ไม่อยู่ในรายการ คงเป็น 1/1 ตามเดิม
--
-- *** ผลของสคริปต์นี้จะยังไม่มีผลกับโปรแกรมจนกว่าจะทำ Phase 2 (แก้โค้ด) ***
--     ตอนนี้ไม่มีโค้ดบรรทัดไหนอ่าน Data_Result_Need
-- ============================================================================

USE `_test_qa_system_tanawut`;
-- USE `qa_system`;            -- <<< ต้องรัน Phase 1 กับ DB จริงก่อนถึงจะปลด comment ได้


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `M_CODE`, `Keep_Data_Need`, `Data_Result_Need`
FROM `info_mat_inspection_list`
WHERE `M_CODE` LIKE 'SHIN005%'
ORDER BY `M_CODE`;


-- ----------------------------------------------------------------------------
-- UPDATE : เก็บเอกสาร (คงเดิม) แต่ไม่ต้องตรวจ
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Data_Result_Need` = 0
 WHERE `M_CODE` IN (
        'SHIN005-02','SHIN005-03','SHIN005-04',
        'SHIN005-05','SHIN005-06','SHIN005-07','SHIN005-08'
       );


-- ----------------------------------------------------------------------------
-- ตรวจสอบ
-- ----------------------------------------------------------------------------

-- ต้องได้ 7 แถวที่เป็น 1/0 และ SHIN005-01 ต้องยังเป็น 1/1
SELECT 'AFTER' AS phase, `M_CODE`, `Keep_Data_Need`, `Data_Result_Need`,
       CASE WHEN `Keep_Data_Need` = 1 AND `Data_Result_Need` = 0 THEN 'เก็บ ไม่ตรวจ'
            WHEN `Keep_Data_Need` = 1 AND `Data_Result_Need` = 1 THEN 'เก็บ + ตรวจ'
            ELSE 'อื่น ๆ' END AS meaning
FROM `info_mat_inspection_list`
WHERE `M_CODE` LIKE 'SHIN005%'
ORDER BY `M_CODE`;

-- นับรวมทั้งตาราง : ต้องมี "เก็บ ไม่ตรวจ" = 7 พอดี
SELECT 'CHECK นับ M-Code แยกตามความหมาย' AS check_name,
       SUM(`Keep_Data_Need` = 1 AND `Data_Result_Need` = 1) AS keep_and_check,
       SUM(`Keep_Data_Need` = 1 AND `Data_Result_Need` = 0) AS keep_no_check,
       SUM(IFNULL(`Keep_Data_Need`,0) <> 1)                 AS no_keep,
       COUNT(*)                                             AS total
FROM `info_mat_inspection_list`;


-- ============================================================================
-- ROLLBACK
-- ============================================================================
-- UPDATE `info_mat_inspection_list` SET `Data_Result_Need` = 1
--  WHERE `M_CODE` IN ('SHIN005-02','SHIN005-03','SHIN005-04',
--                     'SHIN005-05','SHIN005-06','SHIN005-07','SHIN005-08');
