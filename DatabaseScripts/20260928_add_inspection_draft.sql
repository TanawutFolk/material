-- ============================================================================
-- ตารางเก็บ "ร่าง" ของหน้าตรวจที่ยังกรอกไม่ครบ (Save แล้วกลับมาทำต่อได้)
-- ============================================================================
-- ปัญหาเดิม : หน้า Packing / Regular / Function / Dimension มีปุ่มเดียวคือบันทึกผลจริง
--             ซึ่งต้องกรอกครบทุกช่องก่อน แล้วพาไปขั้นตอนถัดไปทันที
--             ถ้ากรอกไม่ครบแล้วกดย้อนกลับ ค่าที่พิมพ์ไว้หายหมด เพราะอยู่แค่บนหน้าจอ
--             (ย้อนกลับแค่เปลี่ยนสถานะเป็น Unfinished ไม่ได้เก็บค่าอะไร)
--
-- ของใหม่   : ปุ่มเดียวสลับได้ 2 แบบ
--               ยังไม่ครบ -> Save     : เก็บค่าที่กรอกไว้ลงตารางนี้ อยู่หน้าเดิม
--               ครบแล้ว   -> Continue : บันทึกผลจริงลง db_*_data เหมือนเดิม แล้วลบร่างทิ้ง
--             เปิดหน้าใหม่ครั้งหน้า ถ้ามีร่างจะดึงค่ากลับมาเติมให้
--
-- ทำไมแยกตาราง ไม่ใส่ลง db_*_data ตรง ๆ :
--   db_*_data ถูกอ่านโดยรายงาน / Export Excel / หน้า Pending หลายจุด
--   ถ้าร่างไปปนอยู่ในนั้น ทุก query ต้องรู้จักแยกร่างออก พลาดจุดเดียวค่าที่ยังไม่ตัดสินจะโผล่ในเอกสาร
--
-- DRAFT_DATA : DataSet ในรูป XML (มี schema) หนึ่งหน้าจอเก็บได้หลายตาราง
--              เช่น Dimension = ตารางค่าที่วัด + ตาราง cavity
--              ตอนโหลดกลับจะจับคู่ทีละแถวกับตารางที่สร้างใหม่จาก master
--              ถ้า master เปลี่ยนไประหว่างนั้น แถวที่ไม่มีคู่จะถูกข้ามไป ไม่ใช่ทับทั้งตาราง
--
-- PROCESS    : ชื่อเดียวกับที่ใช้ใน info_report_active / db_report_status
--              เช่น Packing_Check , Regular_Check , Function_Check , Dimension_Check
--
-- รันซ้ำได้ : CREATE TABLE IF NOT EXISTS
-- *** รันกับ DB ทดสอบก่อน แล้วค่อยขึ้น qa_system ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, COUNT(*) AS has_table
FROM `information_schema`.`TABLES`
WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'db_inspection_draft';


-- ----------------------------------------------------------------------------
-- สร้างตาราง
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `db_inspection_draft` (
    `REPORT_NO`   varchar(20)  NOT NULL,
    `PROCESS`     varchar(50)  NOT NULL  COMMENT 'Packing_Check / Regular_Check / Function_Check / Dimension_Check',
    `DRAFT_DATA`  longtext     NOT NULL  COMMENT 'DataSet XML (WriteSchema) ของค่าที่กรอกค้างไว้',
    `EMP_ID`      varchar(20)      NULL  COMMENT 'คนที่กด Save ล่าสุด',
    `SAVE_DATE`   datetime     NOT NULL  DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`REPORT_NO`, `PROCESS`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase, `COLUMN_NAME`, `COLUMN_TYPE`, `IS_NULLABLE`
FROM `information_schema`.`COLUMNS`
WHERE `TABLE_SCHEMA` = DATABASE() AND `TABLE_NAME` = 'db_inspection_draft'
ORDER BY `ORDINAL_POSITION`;
