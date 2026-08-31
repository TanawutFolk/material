-- ============================================================================
-- เติมจุดวัด Regular ของ SHIN005-01 ให้ครบตามเอกสาร
-- ============================================================================
-- เอกสารระบุ 13 จุด ทั้งหมดใช้ Microscope แต่ใน info_regular_equipment มีแค่ 3 จุด
--   มีแล้ว : 9 , 10 , 11
--   ขาด    : 6 ด้าน PET , 6 ด้าน Y , 7 ด้าน PET , 7 ด้าน Y ,
--            13 ด้าน PET , 13 ด้าน Y , 8 ด้านบน , 8 ด้านล่าง ,
--            12 ด้านบน , 12 ด้านล่าง
--
-- ของเดิม POINT_ORDER 1-3 ไม่แตะ เพราะ db_regular_data อ้างถึงอยู่
-- จุดที่เติมใหม่ต่อจาก 4 เป็นต้นไป เรียงตามลำดับในเอกสาร
--
-- EQUIPMENT_TYPE = 1 คือ Microscope
-- จุดที่ MIN = 0 ฟอร์ม B08 จะพิมพ์ออกมาเป็น "<= 0.05 mm"
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, COUNT(*) AS points
FROM `info_regular_equipment` WHERE `M_CODE` = 'SHIN005-01';


-- ----------------------------------------------------------------------------
-- เติมเฉพาะจุดที่ยังไม่มี  (เทียบด้วย POINT_NAME)
-- ----------------------------------------------------------------------------
INSERT INTO `info_regular_equipment`
    (`M_CODE`, `POINT_ORDER`, `EQUIPMENT_TYPE`, `POINT_NAME`, `POINT_CAL`,
     `CRITERIA_MIN`, `CRITERIA_MAX`, `UNIT`)
SELECT 'SHIN005-01', p.`ord`, 1, p.`name`, '0', p.`min_`, p.`max_`, 'mm'
FROM (
              SELECT  4 ord, '6 ด้าน PET'  name, 11.35 min_, 11.45 max_
    UNION ALL SELECT  5,     '6 ด้าน Y',         11.35, 11.45
    UNION ALL SELECT  6,     '7 ด้าน PET',       10.30, 10.38
    UNION ALL SELECT  7,     '7 ด้าน Y',         10.30, 10.38
    UNION ALL SELECT  8,     '13 ด้าน PET',       0.00,  0.05
    UNION ALL SELECT  9,     '13 ด้าน Y',         0.00,  0.05
    UNION ALL SELECT 10,     '8 ด้านบน',         14.65, 14.75
    UNION ALL SELECT 11,     '8 ด้านล่าง',       14.65, 14.75
    UNION ALL SELECT 12,     '12 ด้านบน',         0.00,  0.05
    UNION ALL SELECT 13,     '12 ด้านล่าง',       0.00,  0.05
) p
WHERE NOT EXISTS (
    SELECT 1 FROM `info_regular_equipment` e
    WHERE e.`M_CODE` = 'SHIN005-01' AND e.`POINT_NAME` = p.`name`
);


-- ----------------------------------------------------------------------------
-- AFTER : ต้องได้ 13 จุด เรียงตามเอกสาร
-- ----------------------------------------------------------------------------
SELECT e.`POINT_ORDER`, e.`POINT_NAME`, e.`CRITERIA_MIN`, e.`CRITERIA_MAX`,
       e.`UNIT`, t.`Equipment_Name`
FROM `info_regular_equipment` e
LEFT JOIN `info_equipment_type` t ON t.`Equipment_Type` = e.`EQUIPMENT_TYPE`
WHERE e.`M_CODE` = 'SHIN005-01'
ORDER BY e.`POINT_ORDER`;
