-- ============================================================================
-- แก้ค่า inspection list ของ SHIN005-02 / -03 / -04 ให้ตรงกับเอกสาร
-- ============================================================================
-- เอกสารระบุทั้ง 3 ตัวเหมือนกัน
--   KEEP DATA    KEEP
--   Q'ty Cavity  4   Cavity name  #E #F #G #H
--   REGULAR      -                        (ไม่เช็ค)
--   FUNCTION     -                        (ไม่เช็ค)
--   DIMENSION    -                        (ไม่เช็ค)
--   APPEARANCE   S-2 Normal Cavity >=2 Pcs.
--
-- เทียบกับ DB แล้วไม่ตรง 5 จุด
--   SHIN005-02   Regular_Check_Need = 1   ควรเป็น 0
--                Dimension_Check_Need = 1 ควรเป็น 0
--   SHIN005-03   Regular_Check_Need = 1   ควรเป็น 0
--                Dimension_Check_Need = 1 ควรเป็น 0
--   SHIN005-04   Dimension_Check_Need = 1 ควรเป็น 0
--
-- ส่วนที่ตรงอยู่แล้วไม่แตะ
--   Keep_Data_Need = 1 , Packing_Check_Mode = 1 , Function_Check_Need = 0
--   Appearance : Sampling Table / Normal / S-2 / qty 2 / cavity 4 (E,F,G,H)
--
-- แถวใน info_*_sampling ของขั้นที่ปิดไปไม่ได้ลบทิ้ง
-- เพราะ Need = 0 กั้นไว้อยู่แล้ว และถ้าวันหลังเปิดใช้อีกจะได้ไม่ต้องตั้งค่าใหม่
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `M_CODE`,
       `Keep_Data_Need` AS k, `Packing_Check_Mode` AS p, `Regular_Check_Need` AS r,
       `Function_Check_Need` AS f, `Dimension_Check_Need` AS d, `Appearance_Check_Need` AS a
FROM `info_mat_inspection_list`
WHERE `M_CODE` IN ('SHIN005-02', 'SHIN005-03', 'SHIN005-04')
ORDER BY `M_CODE`;


-- ----------------------------------------------------------------------------
-- ปิด Regular และ Dimension ตามเอกสาร
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Regular_Check_Need`   = 0,
       `Dimension_Check_Need` = 0
 WHERE `M_CODE` IN ('SHIN005-02', 'SHIN005-03', 'SHIN005-04');


-- ----------------------------------------------------------------------------
-- AFTER : ต้องอ่านออกมาตรงกับเอกสาร
-- ----------------------------------------------------------------------------
SELECT l.`M_CODE`,
       IF(l.`Keep_Data_Need` = 1, 'KEEP', '-')        AS `keep_data`,
       IF(l.`Regular_Check_Need` = 1, 'check', '-')   AS `regular`,
       IF(l.`Function_Check_Need` = 1, 'check', '-')  AS `function_`,
       IF(l.`Dimension_Check_Need` = 1, 'check', '-') AS `dimension_`,
       CASE
           WHEN l.`Appearance_Check_Need` = 0 THEN '-'
           WHEN a.`Sampling_Type` = 3 THEN CONCAT(sl.`Strictness_Level_Name`, ' ', st.`Strictness_Name`,
                                                  ' Cavity >=', a.`Sampling_Qty`,
                                                  IF(a.`Sampling_Qty` = 1, ' Pc.', ' Pcs.'))
           ELSE t.`Sampling_Type_Name`
       END AS `appearance`,
       a.`Cavity_Qty` AS `cavity`, a.`Cavity_Name`
FROM `info_mat_inspection_list` l
LEFT JOIN `info_appearance_sampling` a  ON a.`M_Code` = l.`M_CODE`
LEFT JOIN `info_sampling_type` t        ON t.`Sampling_Type` = a.`Sampling_Type`
LEFT JOIN `info_strictness_type` st     ON st.`Strictness_Type` = a.`Strictness_Type`
LEFT JOIN `info_strictness_level` sl    ON sl.`Strictness_Level` = a.`Strictness_Level`
WHERE l.`M_CODE` IN ('SHIN005-02', 'SHIN005-03', 'SHIN005-04')
ORDER BY l.`M_CODE`;
