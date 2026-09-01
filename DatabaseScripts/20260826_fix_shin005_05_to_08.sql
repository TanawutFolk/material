-- ============================================================================
-- SHIN005-05 ถึง -08 ตั้งค่าแบบเดียวกับ -02/-03/-04
-- ============================================================================
-- เป้าหมาย : Keep Data ทุกตัว , ตรวจแค่ Packing กับ Appearance
--   Keep_Data_Need        1
--   Packing_Check_Mode    1
--   Regular_Check_Need    0
--   Function_Check_Need   0
--   Dimension_Check_Need  0
--   Appearance_Check_Need 1   ->  S-2 Normal Cavity >=2 Pcs.
--
-- เทียบกับ DB แล้วต่างจากเป้าหมายแค่จุดเดียว
--   ทั้ง 4 ตัวเปิด Dimension_Check_Need = 1 อยู่   ->  ต้องเป็น 0
--
-- ส่วนที่ตรงอยู่แล้ว
--   Keep_Data_Need = 1 ครบทั้ง 4 ตัว จึงไม่ได้เปลี่ยนอะไร (เขียนไว้ให้ชัดเจน)
--   Appearance sampling : Sampling Table / Normal / S-2 / qty 2 / cavity 4 (E,F,G,H)
--                         ตรงกันหมดทั้ง 8 ตัวในตระกูล ไม่ต้องแตะ
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
WHERE `M_CODE` IN ('SHIN005-05', 'SHIN005-06', 'SHIN005-07', 'SHIN005-08')
ORDER BY `M_CODE`;


-- ----------------------------------------------------------------------------
-- ตั้งให้ตรงเป้าหมาย
-- ----------------------------------------------------------------------------
UPDATE `info_mat_inspection_list`
   SET `Keep_Data_Need`        = 1,
       `Regular_Check_Need`    = 0,
       `Function_Check_Need`   = 0,
       `Dimension_Check_Need`  = 0,
       `Appearance_Check_Need` = 1
 WHERE `M_CODE` IN ('SHIN005-05', 'SHIN005-06', 'SHIN005-07', 'SHIN005-08');


-- ----------------------------------------------------------------------------
-- AFTER : ดูทั้งตระกูลเทียบกัน
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
WHERE l.`M_CODE` LIKE 'SHIN005%'
ORDER BY l.`M_CODE`;
