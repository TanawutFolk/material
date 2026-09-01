-- ============================================================================
-- แก้ค่า sampling ของ SHIN005-01 ให้ตรงกับเอกสารตั้งค่าจริง
-- ============================================================================
-- เอกสารระบุไว้ว่า
--   Q'ty Cavity  4   Cavity name  #E #F #G #H
--   REGULAR      1Pc./Cavity
--   FUNCTION     S-2 Normal Cavity >=2 Pcs.
--   DIMENSION    S-2 Normal Cavity >=2 Pcs.
--   APPEARANCE   S-2 Normal Cavity >=2 Pcs.
--
-- เทียบกับ DB แล้วไม่ตรง 2 จุด
--   1) Regular   Sampling_Qty = 0  ทำให้ 4 cavity x 0 = 0 ชิ้น คือไม่ต้องเก็บอะไรเลย
--                ทั้งที่เปิด Regular_Check_Need = 1 ไว้   ->  ต้องเป็น 1
--   2) Dimension Strictness = 2/1 คือ Reduce / S-1
--                แต่เอกสารเขียน S-2 Normal              ->  ต้องเป็น 1/2
--
--      ผลของข้อ 2 คือเก็บตัวอย่างน้อยกว่าที่ควร เช่น Lot 1201-3200
--      Reduce S-1 = 2 ชิ้น  แต่ Normal S-2 = 8 ชิ้น
--
-- ค่าอ้างอิง : Strictness_Type 1=Normal 2=Reduce , Strictness_Level 1=S-1 2=S-2
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, 'Regular' AS src, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`,
       `Strictness_Type`, `Strictness_Level`, `Cavity_Name`
FROM `info_regular_sampling` WHERE `M_Code` = 'SHIN005-01'
UNION ALL
SELECT 'BEFORE', 'Dimension', `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`,
       `Strictness_Type`, `Strictness_Level`, `Cavity_Name`
FROM `info_dimension_sampling` WHERE `M_Code` = 'SHIN005-01';


-- ----------------------------------------------------------------------------
-- 1) Regular : 1 Pc. / Cavity
-- ----------------------------------------------------------------------------
UPDATE `info_regular_sampling`
   SET `Sampling_Qty` = 1
 WHERE `M_Code` = 'SHIN005-01'
   AND `Sampling_Type` = 4;


-- ----------------------------------------------------------------------------
-- 2) Dimension : S-2 Normal (ไม่ใช่ S-1 Reduce)
-- ----------------------------------------------------------------------------
UPDATE `info_dimension_sampling`
   SET `Strictness_Type`  = 1,   -- Normal
       `Strictness_Level` = 2    -- S-2
 WHERE `M_Code` = 'SHIN005-01';


-- ----------------------------------------------------------------------------
-- AFTER : ต้องอ่านออกมาตรงกับเอกสารทั้ง 4 บรรทัด
-- ----------------------------------------------------------------------------
SELECT x.`src`,
       x.`Cavity_Qty`,
       t.`Sampling_Type_Name`,
       x.`Sampling_Qty`,
       COALESCE(st.`Strictness_Name`, '-')      AS `strictness`,
       COALESCE(sl.`Strictness_Level_Name`, '-') AS `level`,
       x.`Cavity_Name`,
       CASE x.`Sampling_Type`
           WHEN 4 THEN CONCAT(x.`Sampling_Qty`, IF(x.`Sampling_Qty` = 1, 'Pc.', 'Pcs.'), '/Cavity')
           WHEN 3 THEN CONCAT(sl.`Strictness_Level_Name`, ' ', st.`Strictness_Name`,
                              ' Cavity >=', x.`Sampling_Qty`,
                              IF(x.`Sampling_Qty` = 1, ' Pc.', ' Pcs.'))
           ELSE t.`Sampling_Type_Name`
       END AS `อ่านออกมาได้ว่า`
FROM (
    SELECT 'REGULAR' src, 1 ord, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`, `Strictness_Type`, `Strictness_Level`, `Cavity_Name` FROM `info_regular_sampling`    WHERE `M_Code` = 'SHIN005-01'
    UNION ALL SELECT 'FUNCTION',   2, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`, `Strictness_Type`, `Strictness_Level`, `Cavity_Name` FROM `info_function_sampling`   WHERE `M_Code` = 'SHIN005-01'
    UNION ALL SELECT 'DIMENSION',  3, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`, `Strictness_Type`, `Strictness_Level`, `Cavity_Name` FROM `info_dimension_sampling`  WHERE `M_Code` = 'SHIN005-01'
    UNION ALL SELECT 'APPEARANCE', 4, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`, `Strictness_Type`, `Strictness_Level`, `Cavity_Name` FROM `info_appearance_sampling` WHERE `M_Code` = 'SHIN005-01'
) x
LEFT JOIN `info_sampling_type`    t  ON t.`Sampling_Type`   = x.`Sampling_Type`
LEFT JOIN `info_strictness_type`  st ON st.`Strictness_Type`  = x.`Strictness_Type`
LEFT JOIN `info_strictness_level` sl ON sl.`Strictness_Level` = x.`Strictness_Level`
ORDER BY x.`ord`;
