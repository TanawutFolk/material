-- ============================================================================
-- ตารางเก็บผลตัดสินราย "ชิ้น" ของ Dimension แบบ All
-- ============================================================================
-- เดิม db_dimension_data เก็บได้แค่ผลราย "จุด" (JUDGE = POINT_JUDGE)
-- แต่แบบ All ยังมีอีกเกณฑ์คือผลต่างของทุกจุดภายในชิ้นเดียวกันต้องไม่เกินที่กำหนด
--   Dif (MAX-MIN) <= (CRITERIA_MAX - CRITERIA_MIN) / 2
-- ซึ่งเมื่อก่อนไปฝังเป็นสูตรใน Excel ทำให้
--   1) ค้นด้วย SQL ไม่ได้ ต้องไล่เปิดไฟล์ทีละใบ
--   2) ใครแก้ตัวเลขในไฟล์ ผลตัดสินพลิกทันที เอกสารที่เซ็นไปแล้วไม่ตรงกับไฟล์
--   3) ถ้าแก้เกณฑ์ใน master แล้ว export ใบเก่าซ้ำ จะได้ผลคนละแบบกับตอนตรวจจริง
--
-- จึงเก็บ TOLERANCE ที่ใช้ตัดสินไว้ด้วย ไม่ใช่ไปคำนวณใหม่ทุกครั้ง
-- ทำให้ export ซ้ำกี่รอบก็ได้ผลเดิม และตอบได้ว่าตอนนั้นใช้เกณฑ์เท่าไหร่
--
-- COUNT = รอบการตรวจ นับแบบเดียวกับ db_dimension_data (ตรวจซ้ำได้)
-- INUSE = 0 คือรอบเก่าที่ถูกแทนที่แล้ว
--
-- รันซ้ำได้ : เช็คก่อนว่ามีตารางแล้วหรือยัง
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, COUNT(*) AS has_table
FROM `information_schema`.`TABLES`
WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'db_dimension_piece_judge';


-- ----------------------------------------------------------------------------
-- สร้างตาราง
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `db_dimension_piece_judge` (
    `REPORT_NO`   varchar(20)  NOT NULL,
    `SAMPLING_NO` smallint     NOT NULL,
    `COUNT`       smallint     NOT NULL,
    `CAVITY_NAME` varchar(20)      NULL,
    `DIFFERENCE`  double           NULL  COMMENT 'MAX-MIN ของทุกจุดในชิ้นนั้น',
    `TOLERANCE`   double           NULL  COMMENT 'เกณฑ์ที่ใช้ตัดสิน ณ ตอนบันทึก',
    `JUDGE`       tinyint          NULL  COMMENT '1 = ผ่าน , 0 = เกินเกณฑ์',
    `EMP_ID`      varchar(20)      NULL,
    `JUDGE_DATE`  datetime         NULL,
    `INUSE`       tinyint      NOT NULL  DEFAULT 1,
    `UPDATETIME`  timestamp        NULL  DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`REPORT_NO`, `SAMPLING_NO`, `COUNT`),
    KEY `idx_report_inuse` (`REPORT_NO`, `INUSE`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase, `COLUMN_NAME`, `COLUMN_TYPE`, `IS_NULLABLE`
FROM `information_schema`.`COLUMNS`
WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'db_dimension_piece_judge'
ORDER BY `ORDINAL_POSITION`;
