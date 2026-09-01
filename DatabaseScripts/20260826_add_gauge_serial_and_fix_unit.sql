-- ============================================================================
-- 1) เพิ่ม S/N ของ Gauge ตามเอกสาร  2) ล้างหน่วยของจุดที่เป็นการตัดสิน OK/NG
-- ============================================================================
-- เอกสาร SHIN005-01 ฝั่ง Dimension เขียนว่า
--   Gauge QA-MD-008-03,04   จุด 1-4   OK / OK
--   Microscope              จุด 5,12,13,14,15   มีค่า MIN-MAX
--
-- จุดวัดทั้ง 9 มีครบใน info_dimension_equipment แล้ว ตรงกับเอกสารทุกจุด
-- ที่ขาดคือ S/N ของ Gauge  ตอนนี้ type Gauge (Equipment_Type = 3) มี S/N อยู่ตัวเดียวคือ '-'
-- ผู้ตรวจจึงเลือก QA-MD-008-03 / -04 จาก Dropdown ไม่ได้
--
-- อีกเรื่อง : จุดที่ CRITERIA_MIN = CRITERIA_MAX คือการตัดสินผ่าน/ไม่ผ่านด้วย Jig หรือ Gauge
--             ไม่ใช่ค่าที่วัดออกมาเป็นตัวเลข จึงไม่ควรมีหน่วย
--             แต่ตอนเพิ่มคอลัมน์ UNIT ผม backfill 'mm' ลงไปหมดทุกแถว
--             ทำให้ฟอร์ม B08 พิมพ์ออกมาว่า "1~1 mm" ซึ่งไม่มีความหมาย
--             รวม 169 จุด : Jig 115 , Gauge 36 , Pin Gauge 8 , Pin Gauge GO/NO GO 8 , อื่นๆ 2
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase,
       (SELECT COUNT(*) FROM `info_equipment_serial`
        WHERE `EQUIPMENT_SERIAL` IN ('QA-MD-008-03', 'QA-MD-008-04')) AS gauge_serials,
       (SELECT COUNT(*) FROM `info_dimension_equipment`
        WHERE `CRITERIA_MIN` = `CRITERIA_MAX` AND `UNIT` = 'mm') AS dim_unit_ผิด,
       (SELECT COUNT(*) FROM `info_regular_equipment`
        WHERE `CRITERIA_MIN` = `CRITERIA_MAX` AND `UNIT` = 'mm') AS reg_unit_ผิด;


-- ----------------------------------------------------------------------------
-- 1) เพิ่ม S/N ของ Gauge  (Equipment_Type = 3)
--    เช็คก่อนว่ามีแล้วหรือยัง จะได้รันซ้ำได้
-- ----------------------------------------------------------------------------
INSERT INTO `info_equipment_serial` (`EQUIPMENT_SERIAL`, `EQUIPMENT_TYPE_ID`)
SELECT s.`serial`, 3
FROM (
              SELECT 'QA-MD-008-03' AS `serial`
    UNION ALL SELECT 'QA-MD-008-04'
) s
WHERE NOT EXISTS (
    SELECT 1 FROM `info_equipment_serial` e
    WHERE e.`EQUIPMENT_SERIAL` = s.`serial` AND e.`EQUIPMENT_TYPE_ID` = 3
);


-- ----------------------------------------------------------------------------
-- 2) จุดที่เป็นการตัดสิน OK/NG ไม่ต้องมีหน่วย
-- ----------------------------------------------------------------------------
UPDATE `info_dimension_equipment`
   SET `UNIT` = NULL
 WHERE `CRITERIA_MIN` = `CRITERIA_MAX`;

UPDATE `info_regular_equipment`
   SET `UNIT` = NULL
 WHERE `CRITERIA_MIN` = `CRITERIA_MAX`;


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'S/N ของ Gauge' AS phase, e.`ID`, e.`EQUIPMENT_SERIAL`, t.`Equipment_Name`
FROM `info_equipment_serial` e
LEFT JOIN `info_equipment_type` t ON t.`Equipment_Type` = e.`EQUIPMENT_TYPE_ID`
WHERE e.`EQUIPMENT_TYPE_ID` = 3
ORDER BY e.`EQUIPMENT_SERIAL`;

SELECT 'จุดวัด SHIN005-01' AS phase, e.`POINT_ORDER`, e.`POINT_NAME`,
       e.`CRITERIA_MIN`, e.`CRITERIA_MAX`, IFNULL(e.`UNIT`, '(ไม่มีหน่วย)') AS `unit`,
       t.`Equipment_Name`
FROM `info_dimension_equipment` e
LEFT JOIN `info_equipment_type` t ON t.`Equipment_Type` = e.`EQUIPMENT_TYPE`
WHERE e.`M_CODE` = 'SHIN005-01'
ORDER BY e.`POINT_ORDER`;
