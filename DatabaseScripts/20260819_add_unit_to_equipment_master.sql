-- ============================================================================
-- เพิ่มคอลัมน์ UNIT ให้ master จุดวัด Regular / Dimension
-- ============================================================================
-- เดิมตารางเก็บแค่ CRITERIA_MIN / CRITERIA_MAX ไม่มีหน่วย
-- ฟอร์ม FM-QA-B08-F ต้องพิมพ์หน่วยต่อท้ายเกณฑ์ เช่น "11.52 ~ 11.57 mm"
-- จุดวัดส่วนใหญ่เป็น mm แต่ไม่ทั้งหมด เช่นจุดที่ใช้ Force Gauge เป็น N
-- จึงเก็บเป็นราย point ไม่ฮาร์ดโค้ดในโค้ด
--
-- รันซ้ำได้ : เช็คก่อนว่ามีคอลัมน์แล้วหรือยัง
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `TABLE_NAME`, COUNT(*) AS has_unit_column
FROM `information_schema`.`COLUMNS`
WHERE `TABLE_SCHEMA` = DATABASE()
  AND `TABLE_NAME` IN ('info_dimension_equipment', 'info_regular_equipment')
  AND `COLUMN_NAME` = 'UNIT'
GROUP BY `TABLE_NAME`;


-- ----------------------------------------------------------------------------
-- 1) info_dimension_equipment
-- ----------------------------------------------------------------------------
SET @exists = (SELECT COUNT(*) FROM `information_schema`.`COLUMNS`
               WHERE `TABLE_SCHEMA` = DATABASE()
                 AND `TABLE_NAME` = 'info_dimension_equipment'
                 AND `COLUMN_NAME` = 'UNIT');

SET @ddl = IF(@exists = 0,
    'ALTER TABLE `info_dimension_equipment` ADD COLUMN `UNIT` varchar(10) NULL AFTER `CRITERIA_MAX`',
    'SELECT ''info_dimension_equipment.UNIT มีอยู่แล้ว'' AS note');
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;


-- ----------------------------------------------------------------------------
-- 2) info_regular_equipment (ใช้ตัวสร้าง SQL ตัวเดียวกัน ต้องมีคอลัมน์เหมือนกัน)
-- ----------------------------------------------------------------------------
SET @exists = (SELECT COUNT(*) FROM `information_schema`.`COLUMNS`
               WHERE `TABLE_SCHEMA` = DATABASE()
                 AND `TABLE_NAME` = 'info_regular_equipment'
                 AND `COLUMN_NAME` = 'UNIT');

SET @ddl = IF(@exists = 0,
    'ALTER TABLE `info_regular_equipment` ADD COLUMN `UNIT` varchar(10) NULL AFTER `CRITERIA_MAX`',
    'SELECT ''info_regular_equipment.UNIT มีอยู่แล้ว'' AS note');
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;


-- ----------------------------------------------------------------------------
-- 3) เติมค่าเริ่มต้นให้ของเดิม
--    ทุกจุดที่มีอยู่ตอนนี้ใช้ Caliper / Microscope / Height Gauge / Pin Gauge
--    ซึ่งวัดความยาวทั้งหมด จึงเป็น mm  ถ้ามีจุดที่ไม่ใช่ ให้ไปแก้ในหน้า Setting
-- ----------------------------------------------------------------------------
UPDATE `info_dimension_equipment` SET `UNIT` = 'mm' WHERE `UNIT` IS NULL OR `UNIT` = '';
UPDATE `info_regular_equipment`   SET `UNIT` = 'mm' WHERE `UNIT` IS NULL OR `UNIT` = '';


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase, 'info_dimension_equipment' AS table_name, `UNIT`, COUNT(*) AS rows_
FROM `info_dimension_equipment` GROUP BY `UNIT`;

SELECT 'AFTER' AS phase, 'info_regular_equipment' AS table_name, `UNIT`, COUNT(*) AS rows_
FROM `info_regular_equipment` GROUP BY `UNIT`;
