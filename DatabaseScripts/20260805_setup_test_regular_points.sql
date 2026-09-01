-- ============================================================================
-- ตั้งค่า Regular Check ของ M-Code ทดสอบ R179S622-YTC
-- ============================================================================
-- ของเดิม : 4 จุด ใช้ Jig ทั้งหมด และ CRITERIA_MIN = CRITERIA_MAX = 1
--           ซึ่งโค้ด dtg_regular_CellFormatting (userControlRegular.cs:1232)
--           ตีความว่าเป็น dropdown OK/NG -> ไม่ได้ทดสอบการกรอกค่าวัดเลย
--
-- ของใหม่ : ผสมทั้ง 2 แบบ เพื่อให้ทดสอบได้ครบ
--           จุด 1-3 = กรอกตัวเลข ตัดสินด้วย MIN/MAX  (TextBox)
--           จุด 4   = OK/NG                          (ComboBox)
--
-- เลือก equipment ที่มี serial จริงในระบบ จะได้ทดสอบ dropdown serial ด้วย
--           Caliper(4)    -> 151855352 , B16272968 , B23055688   (3 ตัวเลือก)
--           Micrometer(2) -> 15149700
--           Microscope(1) -> 1G19405
--           Jig(11)       -> "-"
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE - จุดวัด Regular' AS phase, e.`POINT_ORDER`, e.`EQUIPMENT_TYPE`,
       t.`Equipment_Name`, e.`POINT_NAME`, e.`POINT_CAL`, e.`CRITERIA_MIN`, e.`CRITERIA_MAX`
FROM `info_regular_equipment` e
LEFT JOIN `info_equipment_type` t ON e.`EQUIPMENT_TYPE` = t.`Equipment_Type`
WHERE e.`M_CODE` = 'R179S622-YTC' ORDER BY e.`POINT_ORDER`;

SELECT 'BEFORE - sampling' AS phase, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`,
       `Strictness_Type`, `Strictness_Level`, `Cavity_Name`
FROM `info_regular_sampling` WHERE `M_Code` = 'R179S622-YTC';


-- ----------------------------------------------------------------------------
-- 1) sampling : Fix 5 ชิ้น (คงเดิม เขียนซ้ำเพื่อความชัดเจน)
-- ----------------------------------------------------------------------------
INSERT INTO `info_regular_sampling`
    (`M_Code`, `Cavity_Qty`, `Sampling_Type`, `Sampling_Qty`, `Strictness_Type`, `Strictness_Level`, `Cavity_Name`)
VALUES
    ('R179S622-YTC', 0, 2, 5, 0, 0, '0')
ON DUPLICATE KEY UPDATE
    `Cavity_Qty`=VALUES(`Cavity_Qty`), `Sampling_Type`=VALUES(`Sampling_Type`),
    `Sampling_Qty`=VALUES(`Sampling_Qty`), `Strictness_Type`=VALUES(`Strictness_Type`),
    `Strictness_Level`=VALUES(`Strictness_Level`), `Cavity_Name`=VALUES(`Cavity_Name`);


-- ----------------------------------------------------------------------------
-- 2) จุดวัด : เขียนใหม่ทั้งชุด
-- ----------------------------------------------------------------------------
DELETE FROM `info_regular_equipment` WHERE `M_CODE` = 'R179S622-YTC';

INSERT INTO `info_regular_equipment`
    (`M_CODE`, `POINT_ORDER`, `EQUIPMENT_TYPE`, `POINT_NAME`, `POINT_CAL`, `CRITERIA_MIN`, `CRITERIA_MAX`)
VALUES
    -- จุด 1 : ทดสอบทศนิยม 5 ตำแหน่ง + serial มีให้เลือก 3 ตัว
    ('R179S622-YTC', 1,  4, 'A-Width',  '0',  6.14685,  6.55321),
    -- จุด 2 : ตัวเลขทั่วไป
    ('R179S622-YTC', 2,  2, 'B-Thick',  '0',   10.200,   10.800),
    -- จุด 3 : ตัวเลขทั่วไป คนละ equipment
    ('R179S622-YTC', 3,  1, 'C-Hole',   '0',    2.500,    3.500),
    -- จุด 4 : MIN=MAX=1 -> กลายเป็น dropdown OK/NG
    ('R179S622-YTC', 4, 11, 'D-Visual', '0',        1,        1);


-- ----------------------------------------------------------------------------
-- ตรวจสอบ
-- ----------------------------------------------------------------------------
SELECT 'AFTER - จุดวัด Regular' AS phase, e.`POINT_ORDER`, e.`EQUIPMENT_TYPE`,
       t.`Equipment_Name`, e.`POINT_NAME`, e.`CRITERIA_MIN`, e.`CRITERIA_MAX`,
       CASE WHEN e.`CRITERIA_MIN` = 1 AND e.`CRITERIA_MAX` = 1
            THEN 'dropdown OK/NG' ELSE 'กรอกตัวเลข' END AS โหมด,
       (SELECT COUNT(*) FROM `info_equipment_serial` s WHERE s.`EQUIPMENT_TYPE_ID` = e.`EQUIPMENT_TYPE`) AS serial_ให้เลือก
FROM `info_regular_equipment` e
LEFT JOIN `info_equipment_type` t ON e.`EQUIPMENT_TYPE` = t.`Equipment_Type`
WHERE e.`M_CODE` = 'R179S622-YTC' ORDER BY e.`POINT_ORDER`;

-- ต้องได้ 4 จุด : กรอกตัวเลข 3 + OK/NG 1
SELECT 'CHECK จำนวนจุด' AS check_name,
       SUM(`CRITERIA_MIN` <> 1 OR `CRITERIA_MAX` <> 1) AS จุดกรอกตัวเลข,
       SUM(`CRITERIA_MIN`  = 1 AND `CRITERIA_MAX`  = 1) AS จุด_OK_NG,
       COUNT(*) AS รวม
FROM `info_regular_equipment` WHERE `M_CODE` = 'R179S622-YTC';

-- ทศนิยมต้องไม่ถูกปัด
SELECT 'CHECK ทศนิยม 5 ตำแหน่ง' AS check_name, `POINT_NAME`, `CRITERIA_MIN`, `CRITERIA_MAX`,
       (`CRITERIA_MIN` = 6.14685) AS min_ตรง, (`CRITERIA_MAX` = 6.55321) AS max_ตรง
FROM `info_regular_equipment` WHERE `M_CODE` = 'R179S622-YTC' AND `POINT_ORDER` = 1;


-- ============================================================================
-- ROLLBACK - คืนเป็นแบบเดิม (Jig ทั้ง 4 จุด , OK/NG ทั้งหมด)
-- ============================================================================
-- DELETE FROM `info_regular_equipment` WHERE `M_CODE` = 'R179S622-YTC';
-- INSERT INTO `info_regular_equipment`
--     (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`)
-- VALUES ('R179S622-YTC',1,11,'1','0',1,1),
--        ('R179S622-YTC',2,11,'2','0',1,1),
--        ('R179S622-YTC',3,11,'3','0',1,1),
--        ('R179S622-YTC',4,11,'4','0',1,1);
