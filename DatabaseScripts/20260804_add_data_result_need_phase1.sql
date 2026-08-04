-- ============================================================================
-- Phase 1 : เพิ่มคอลัมน์ Data_Result_Need แยกออกจาก Keep_Data_Need
-- ============================================================================
-- เหตุผล
--   info_mat_inspection_list มีธง Keep_Data_Need ตัวเดียว แต่ต้องคุม 2 เรื่อง
--     1) ต้องเก็บเอกสาร vendor ไหม   -> db_report_status.Keep_Data
--     2) ต้องให้ QA ตรวจเอกสารไหม    -> db_report_status.Inspection_Data_Check
--   จึงตั้งค่าเคส "เก็บ แต่ไม่ต้องตรวจ" ไม่ได้ (เช่น M-Code SHIN005-08)
--
-- ขอบเขตของสคริปต์นี้ (Phase 1 เท่านั้น)
--   - เพิ่มคอลัมน์ + copy ค่าจากของเดิม
--   - ยังไม่มีโค้ดไหนอ่านคอลัมน์ใหม่ => พฤติกรรมโปรแกรมเหมือนเดิม 100%
--   - ยังไม่เปลี่ยนค่าของ M-Code ใดทั้งสิ้น (รวมถึง SHIN005-08)
--
-- Phase ถัดไป (คนละสคริปต์ อย่ารันรวมกัน)
--   Phase 2 : แก้โค้ดให้อ่าน Data_Result_Need
--   Phase 3 : UPDATE ... SET Data_Result_Need = 0 WHERE M_CODE = 'SHIN005-08'
--
-- วิธีใช้
--   1. แก้บรรทัด USE ให้ตรงกับ DB ที่ต้องการ (เริ่มที่ DB ทดสอบก่อนเสมอ)
--   2. รัน STEP 0 ดูค่าตั้งต้นไว้เทียบ
--   3. รัน STEP 1-2
--   4. รัน STEP 3 ตรวจว่า mismatch = 0 ทุกบรรทัด
-- ============================================================================

USE `_test_qa_system_tanawut`;
-- USE `qa_system`;            -- <<< ปลด comment เมื่อจะรันกับ DB จริง


-- ----------------------------------------------------------------------------
-- STEP 0 : ค่าตั้งต้นก่อนแก้ (เก็บผลไว้เทียบกับ STEP 3)
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase,
       IFNULL(`Keep_Data_Need`, -1) AS Keep_Data_Need,
       COUNT(*)                     AS mcodes
FROM `info_mat_inspection_list`
GROUP BY 2
ORDER BY 2;


-- ----------------------------------------------------------------------------
-- STEP 1 : เพิ่มคอลัมน์
-- ----------------------------------------------------------------------------
-- NOT NULL DEFAULT 1 ตั้งใจเลือกแบบนี้:
--   ถ้าภายหลังมีโค้ดจุดไหนลืมเขียนค่า M-Code ใหม่จะได้ 1 = "ต้องตรวจ"
--   -> งานโผล่ในคิวเกิน (เห็นได้ แก้ได้)
--   ถ้าใช้ NULL หรือ DEFAULT 0 -> งานหายจากคิวเงียบ ๆ ซึ่งอันตรายกว่ามาก
ALTER TABLE `info_mat_inspection_list`
    ADD COLUMN `Data_Result_Need` SMALLINT NOT NULL DEFAULT 1
    AFTER `Keep_Data_Need`;


-- ----------------------------------------------------------------------------
-- STEP 2 : copy ค่าจากธงเดิม
-- ----------------------------------------------------------------------------
-- IFNULL(...,0) สำคัญ: ของเดิมที่เป็น NULL ทำให้เงื่อนไข `Keep_Data_Need = 1`
-- เป็นเท็จ (M-Code นั้นไม่เคยเข้าคิว Data Result) จึงต้อง map เป็น 0
-- ถ้า map เป็น 1 จะกลายเป็นเปลี่ยนพฤติกรรมทันที
UPDATE `info_mat_inspection_list`
   SET `Data_Result_Need` = IFNULL(`Keep_Data_Need`, 0);


-- ----------------------------------------------------------------------------
-- STEP 3 : ตรวจสอบ  ***ทุกบรรทัดต้องได้ 0 ถึงจะถือว่าผ่าน***
-- ----------------------------------------------------------------------------

-- 3.1 ค่าที่ copy มาต้องตรงกับของเดิมทุกแถว
SELECT 'CHECK 3.1 copy mismatch (ต้อง = 0)' AS check_name,
       COUNT(*) AS mismatch
FROM `info_mat_inspection_list`
WHERE `Data_Result_Need` <> IFNULL(`Keep_Data_Need`, 0);

-- 3.2 เงื่อนไขที่โค้ดใช้จริง (`= 1`) ต้องให้ผลเท่ากันทั้ง 2 คอลัมน์
-- ต้องใช้ IS TRUE ครอบ เพราะของเดิม 15 แถวเป็น NULL -> `NULL = 1` ได้ NULL
-- ส่วนคอลัมน์ใหม่ได้ FALSE  ถ้าเทียบดิบ ๆ จะนับว่าต่างทั้งที่ WHERE กรองทิ้งเหมือนกัน
SELECT 'CHECK 3.2 queue-condition mismatch (ต้อง = 0)' AS check_name,
       COUNT(*) AS mismatch
FROM `info_mat_inspection_list`
WHERE ((`Keep_Data_Need` = 1) IS TRUE) <> ((`Data_Result_Need` = 1) IS TRUE);

-- 3.2b จำนวน M-Code ที่ผ่านเงื่อนไขเข้าคิว ต้องเท่ากันเป๊ะ (ปัจจุบัน = 226)
SELECT 'CHECK 3.2b passes count (ต้องเท่ากัน)' AS check_name,
       SUM(`Keep_Data_Need`   = 1) AS old_passes,
       SUM(`Data_Result_Need` = 1) AS new_passes,
       COUNT(*)                    AS total_mcodes
FROM `info_mat_inspection_list`;

-- 3.3 จำนวน M-Code ต้องไม่เปลี่ยน และการกระจายต้องเหมือน STEP 0
SELECT 'AFTER' AS phase,
       IFNULL(`Keep_Data_Need`, -1) AS Keep_Data_Need,
       `Data_Result_Need`,
       COUNT(*) AS mcodes
FROM `info_mat_inspection_list`
GROUP BY 2, 3
ORDER BY 2, 3;

-- 3.4 ยืนยันว่า SHIN005-08 ยังไม่ถูกเปลี่ยน (ต้องได้ 1 ทั้งคู่ - รอ Phase 3)
SELECT 'CHECK 3.4 SHIN005 ยังไม่ถูกแตะ' AS check_name,
       `M_CODE`, `Keep_Data_Need`, `Data_Result_Need`
FROM `info_mat_inspection_list`
WHERE `M_CODE` LIKE 'SHIN005%'
ORDER BY `M_CODE`;


-- ============================================================================
-- ROLLBACK - ถ้าต้องการย้อนกลับ ใช้บรรทัดนี้บรรทัดเดียว
-- ปลอดภัยเต็มที่เพราะยังไม่มีโค้ดไหนอ้างถึงคอลัมน์นี้
-- ============================================================================
-- ALTER TABLE `info_mat_inspection_list` DROP COLUMN `Data_Result_Need`;
