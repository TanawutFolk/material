-- ============================================================================
-- ตั้งค่า M-Code ที่ยังไม่เคย set (ธงเป็น NULL ทั้งหมด)
-- เฉพาะ 7 ตัวที่ถอดรหัสจาก Index.xlsx ได้แน่นอน
-- ============================================================================
-- ที่มา : Index.xlsx sheet 'Appendix D1-D5 Update 08Apr 26'
--
-- หลักฐานที่ใช้ยืนยัน
--   * FS399-CAM ตระกูลเดียวกัน Excel เขียนเหมือน FS398-CAM ทุกช่อง
--     DB เก็บ 1,0,1,0,0,0,1 -> ใช้ค่าชุดเดียวกัน
--   * ค่า 'All' ในคอลัมน์ APPEARANCE ถอดรหัสตรง 100% จาก 122 ตัวอย่างใน DB
--     (Cavity_Qty=0, Sampling_Type=1, Sampling_Qty=0, Strictness_Type=0,
--      Strictness_Level=0, Cavity_Name='0')
--
-- ไม่รวมในสคริปต์นี้ (ยังไม่มั่นใจ - รอคำตอบ)
--   FS400-CAM..FS404-CAM  : Excel เว้นว่าง ไม่ได้ใส่ '-'
--   RCOMM002-SAN/SHI , RCOMM003-SHI : มี cavity + S-2 + จุดวัด
-- ============================================================================

USE `_test_qa_system_tanawut`;
-- USE `qa_system`;          -- <<< ปลด comment เมื่อคุณตรวจแล้วว่าถูก


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `M_CODE`, `Keep_Data_Need`, `Regular_Check_Need`, `Packing_Check_Mode`,
       `Function_Check_Need`, `Dimension_Check_Need`, `Appearance_Check_Need`, `INUSE`
FROM `info_mat_inspection_list`
WHERE `M_CODE` IN ('FS328-KMC','FS331-KMC','FS333-KMC','FS390-OHT','FS398-CAM','R179S001-MAC','TNM100')
ORDER BY `M_CODE`;


-- ----------------------------------------------------------------------------
-- 1) KEEP DATA อย่างเดียว ไม่ต้องตรวจอะไรเลย  (Excel: C=KEEP , ที่เหลือ '-')
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Keep_Data_Need`        = 1,
       `Regular_Check_Need`    = 0,
       `Packing_Check_Mode`    = 1,
       `Function_Check_Need`   = 0,
       `Dimension_Check_Need`  = 0,
       `Appearance_Check_Need` = 0,
       `INUSE`                 = 1
 WHERE `M_CODE` IN ('FS328-KMC','FS331-KMC','FS333-KMC','FS390-OHT','FS398-CAM');


-- ----------------------------------------------------------------------------
-- 2) KEEP DATA + Appearance = All   (Excel: C=KEEP , AE=All)
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Keep_Data_Need`        = 1,
       `Regular_Check_Need`    = 0,
       `Packing_Check_Mode`    = 1,
       `Function_Check_Need`   = 0,
       `Dimension_Check_Need`  = 0,
       `Appearance_Check_Need` = 1,
       `INUSE`                 = 1
 WHERE `M_CODE` = 'TNM100';


-- ----------------------------------------------------------------------------
-- 3) ไม่ต้อง KEEP DATA แต่ตรวจ Appearance = All   (Excel: C='-' , AE=All)
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Keep_Data_Need`        = 0,
       `Regular_Check_Need`    = 0,
       `Packing_Check_Mode`    = 1,
       `Function_Check_Need`   = 0,
       `Dimension_Check_Need`  = 0,
       `Appearance_Check_Need` = 1,
       `INUSE`                 = 1
 WHERE `M_CODE` = 'R179S001-MAC';


-- ----------------------------------------------------------------------------
-- 4) แถว sampling ของ Appearance = All  (2 ตัวที่ Appearance_Check_Need = 1)
-- ----------------------------------------------------------------------------
INSERT INTO `info_appearance_sampling`
    (`M_Code`, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`, `Strictness_Type`, `Strictness_Level`, `Cavity_Name`)
VALUES
    ('TNM100',       0, 1, 0, 0, 0, '0'),
    ('R179S001-MAC', 0, 1, 0, 0, 0, '0')
ON DUPLICATE KEY UPDATE
    `Cavity_Qty`=VALUES(`Cavity_Qty`), `Sampling_Type`=VALUES(`Sampling_Type`),
    `Sampling_Qty`=VALUES(`Sampling_Qty`), `Strictness_Type`=VALUES(`Strictness_Type`),
    `Strictness_Level`=VALUES(`Strictness_Level`), `Cavity_Name`=VALUES(`Cavity_Name`);


-- ----------------------------------------------------------------------------
-- ตรวจสอบ
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase, `M_CODE`, `Keep_Data_Need`, `Regular_Check_Need`, `Packing_Check_Mode`,
       `Function_Check_Need`, `Dimension_Check_Need`, `Appearance_Check_Need`, `INUSE`
FROM `info_mat_inspection_list`
WHERE `M_CODE` IN ('FS328-KMC','FS331-KMC','FS333-KMC','FS390-OHT','FS398-CAM','R179S001-MAC','TNM100')
ORDER BY `M_CODE`;

SELECT 'appearance sampling ที่เพิ่ม' AS phase, s.`M_Code`, s.`Cavity_Qty`, s.`Sampling_Type`,
       s.`Sampling_Qty`, s.`Strictness_Type`, s.`Strictness_Level`, s.`Cavity_Name`
FROM `info_appearance_sampling` s
WHERE s.`M_Code` IN ('TNM100','R179S001-MAC');

-- เทียบ FS398-CAM กับ FS399-CAM ที่ set ไว้ก่อนแล้ว - ต้องเหมือนกันทุกช่อง
SELECT 'CHECK FS398 ต้องเหมือน FS399' AS check_name, `M_CODE`, `Keep_Data_Need`, `Regular_Check_Need`,
       `Packing_Check_Mode`, `Function_Check_Need`, `Dimension_Check_Need`, `Appearance_Check_Need`, `INUSE`
FROM `info_mat_inspection_list` WHERE `M_CODE` IN ('FS398-CAM','FS399-CAM') ORDER BY `M_CODE`;

-- ต้องไม่เหลือ M-Code ที่ธงเป็น NULL ในกลุ่มนี้
SELECT 'CHECK ยังเหลือ NULL กี่ตัว (ต้อง = 0)' AS check_name, COUNT(*) AS still_null
FROM `info_mat_inspection_list`
WHERE `M_CODE` IN ('FS328-KMC','FS331-KMC','FS333-KMC','FS390-OHT','FS398-CAM','R179S001-MAC','TNM100')
  AND `Keep_Data_Need` IS NULL;


-- ============================================================================
-- ROLLBACK - คืนกลับเป็น NULL เหมือนเดิม
-- ============================================================================
-- UPDATE `info_mat_inspection_list`
--    SET `Keep_Data_Need`=NULL, `Regular_Check_Need`=NULL, `Packing_Check_Mode`=NULL,
--        `Function_Check_Need`=NULL, `Dimension_Check_Need`=NULL,
--        `Appearance_Check_Need`=NULL, `INUSE`=NULL
--  WHERE `M_CODE` IN ('FS328-KMC','FS331-KMC','FS333-KMC','FS390-OHT','FS398-CAM','R179S001-MAC','TNM100');
-- DELETE FROM `info_appearance_sampling` WHERE `M_Code` IN ('TNM100','R179S001-MAC');
