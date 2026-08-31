-- ============================================================================
-- เพิ่ม JUDGE_TYPE ให้จุดวัด : ระบุชัดว่าจุดนี้วัดเป็นตัวเลข หรือตัดสิน OK/NG
-- ============================================================================
-- เดิมระบบอนุมานจาก CRITERIA_MIN = CRITERIA_MAX แล้วถือว่าเป็นจุดตัดสิน OK/NG
-- ปัญหาคือกฎนี้กระจายอยู่ 5 ที่ในโค้ด และเขียนไม่ตรงกัน
--   userControlDimension.CellFormatting        เช็ค == 1 เป๊ะ
--   userControlDimension.IsMeasurablePoint     เช็ค min != max
--   userControlRegular.CellFormatting          เช็ค == 1 เป๊ะ
--   ExportExcellB08.DifferenceTolerance        เช็ค min == max
--   frmMCodeInspectionSetting                  เช็ค min == max
--
-- และที่สำคัญกว่า : เป็นข้อตกลงโดยปริยาย ไม่มีอะไรบันทึกไว้
-- ถ้าวันหลังมีคนแก้เกณฑ์จาก 1/1 เป็นตัวเลข ความหมายของข้อมูลเก่าจะเปลี่ยนไปเงียบๆ
-- งาน QA ต้องย้อนสืบได้เสมอว่าจุดนั้นตั้งใจให้วัดแบบไหน
--
-- JUDGE_TYPE 1 = Numeric   วัดเป็นตัวเลข เทียบกับช่วง MIN-MAX
--            2 = Pass/Fail ตัดสินผ่านไม่ผ่าน ผู้ตรวจเลือกจาก Dropdown OK/NG
--
-- backfill จากกฎเดิม (MIN = MAX -> 2) เพื่อให้ข้อมูลที่มีอยู่ความหมายไม่เปลี่ยน
--
-- รันซ้ำได้ : เช็คก่อนว่ามีคอลัมน์แล้วหรือยัง
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `TABLE_NAME`, COUNT(*) AS has_column
FROM `information_schema`.`COLUMNS`
WHERE `TABLE_SCHEMA` = DATABASE()
  AND `TABLE_NAME` IN ('info_dimension_equipment', 'info_regular_equipment')
  AND `COLUMN_NAME` = 'JUDGE_TYPE'
GROUP BY `TABLE_NAME`;


-- ----------------------------------------------------------------------------
-- 1) ตารางอ้างอิง ทำตามแบบเดียวกับ info_sampling_type / info_strictness_type
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `info_judge_type` (
    `JUDGE_TYPE`      smallint    NOT NULL,
    `JUDGE_TYPE_NAME` varchar(30)     NULL,
    `UpdateTime`      timestamp       NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`JUDGE_TYPE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `info_judge_type` (`JUDGE_TYPE`, `JUDGE_TYPE_NAME`)
SELECT t.`id`, t.`name`
FROM (SELECT 1 AS `id`, 'Numeric' AS `name`
      UNION ALL SELECT 2, 'Pass/Fail') t
WHERE NOT EXISTS (SELECT 1 FROM `info_judge_type` j WHERE j.`JUDGE_TYPE` = t.`id`);


-- ----------------------------------------------------------------------------
-- 2) เพิ่มคอลัมน์ให้ทั้งสองตาราง
-- ----------------------------------------------------------------------------
SET @exists = (SELECT COUNT(*) FROM `information_schema`.`COLUMNS`
               WHERE `TABLE_SCHEMA` = DATABASE()
                 AND `TABLE_NAME` = 'info_dimension_equipment'
                 AND `COLUMN_NAME` = 'JUDGE_TYPE');
SET @ddl = IF(@exists = 0,
    'ALTER TABLE `info_dimension_equipment` ADD COLUMN `JUDGE_TYPE` smallint NOT NULL DEFAULT 1 AFTER `UNIT`',
    'SELECT ''info_dimension_equipment.JUDGE_TYPE มีอยู่แล้ว'' AS note');
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @exists = (SELECT COUNT(*) FROM `information_schema`.`COLUMNS`
               WHERE `TABLE_SCHEMA` = DATABASE()
                 AND `TABLE_NAME` = 'info_regular_equipment'
                 AND `COLUMN_NAME` = 'JUDGE_TYPE');
SET @ddl = IF(@exists = 0,
    'ALTER TABLE `info_regular_equipment` ADD COLUMN `JUDGE_TYPE` smallint NOT NULL DEFAULT 1 AFTER `UNIT`',
    'SELECT ''info_regular_equipment.JUDGE_TYPE มีอยู่แล้ว'' AS note');
PREPARE stmt FROM @ddl; EXECUTE stmt; DEALLOCATE PREPARE stmt;


-- ----------------------------------------------------------------------------
-- 3) backfill ตามกฎเดิม ความหมายของข้อมูลที่มีอยู่จะได้ไม่เปลี่ยน
-- ----------------------------------------------------------------------------
UPDATE `info_dimension_equipment`
   SET `JUDGE_TYPE` = IF(`CRITERIA_MIN` = `CRITERIA_MAX`, 2, 1);

UPDATE `info_regular_equipment`
   SET `JUDGE_TYPE` = IF(`CRITERIA_MIN` = `CRITERIA_MAX`, 2, 1);


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'Dimension' AS src, j.`JUDGE_TYPE_NAME`, COUNT(*) AS points
FROM `info_dimension_equipment` e
LEFT JOIN `info_judge_type` j ON j.`JUDGE_TYPE` = e.`JUDGE_TYPE`
GROUP BY j.`JUDGE_TYPE_NAME`
UNION ALL
SELECT 'Regular', j.`JUDGE_TYPE_NAME`, COUNT(*)
FROM `info_regular_equipment` e
LEFT JOIN `info_judge_type` j ON j.`JUDGE_TYPE` = e.`JUDGE_TYPE`
GROUP BY j.`JUDGE_TYPE_NAME`;
