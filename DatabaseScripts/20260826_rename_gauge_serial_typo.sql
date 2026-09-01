-- ============================================================================
-- แก้ S/N ของ Gauge ที่สะกดผิด : GA-MD-008 -> QA-MD-008
-- ============================================================================
-- ตอนอ่านจากภาพเอกสาร ตัว Q ดูเหมือน G จึงใส่เป็น GA-MD-008-03 / -04
-- รูปแบบที่ระบบใช้จริงคือ QA-MD-xxx-yy (เช่น QA-MD-018-01 ของ Height Gauge)
--
-- 2 แถวนี้เพิ่งสร้างและยังไม่มี db_dimension_data / db_regular_data อ้างถึง
-- จึง UPDATE ชื่อได้เลยโดยไม่ต้องกังวลว่าค่าที่บันทึกไว้จะชี้ผิด
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

SELECT 'BEFORE' AS phase, `ID`, `EQUIPMENT_SERIAL`, `EQUIPMENT_TYPE_ID`
FROM `info_equipment_serial`
WHERE `EQUIPMENT_SERIAL` LIKE '%A-MD-008-%' ORDER BY `EQUIPMENT_SERIAL`;

-- กันชนกับของที่อาจมีอยู่แล้ว : แก้เฉพาะตอนที่ยังไม่มีชื่อใหม่
UPDATE `info_equipment_serial` e
   SET e.`EQUIPMENT_SERIAL` = REPLACE(e.`EQUIPMENT_SERIAL`, 'GA-MD-008-', 'QA-MD-008-')
 WHERE e.`EQUIPMENT_SERIAL` IN ('GA-MD-008-03', 'GA-MD-008-04')
   AND NOT EXISTS (
       SELECT 1 FROM (SELECT `EQUIPMENT_SERIAL` FROM `info_equipment_serial`) x
       WHERE x.`EQUIPMENT_SERIAL` = REPLACE(e.`EQUIPMENT_SERIAL`, 'GA-MD-008-', 'QA-MD-008-')
   );

SELECT 'AFTER' AS phase, e.`ID`, e.`EQUIPMENT_SERIAL`, t.`Equipment_Name`
FROM `info_equipment_serial` e
LEFT JOIN `info_equipment_type` t ON t.`Equipment_Type` = e.`EQUIPMENT_TYPE_ID`
WHERE e.`EQUIPMENT_TYPE_ID` = 3 ORDER BY e.`EQUIPMENT_SERIAL`;
