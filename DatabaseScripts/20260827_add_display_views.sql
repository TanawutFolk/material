-- ============================================================================
-- VIEW สำหรับ "ดู" ค่าวัดให้ตรงกับหน้าจอโปรแกรม
-- ============================================================================
-- ปัญหา : คอลัมน์ค่าวัดเป็น decimal(12,6) ซึ่ง MySQL เติมศูนย์ให้เต็ม scale เสมอ
--         query ดิบจึงเห็น 6.146800 ทั้งที่หน้าจอกับใบ Excel โชว์ 6.1468
--         อ่านเทียบกันแล้วสับสน ทั้งที่เป็นค่าเดียวกัน
--
-- ทำไมไม่แก้ที่ชนิดคอลัมน์ :
--   - decimal บังคับเติมเต็ม scale เป็นนิยามของมัน ตั้งให้ไม่เติมไม่ได้
--   - ถ้าเปลี่ยนเป็น double จะไม่เติมศูนย์ก็จริง แต่เทียบค่าไม่แม่น
--     6.5532 - 6.1468 ด้วย double ได้ 0.40640000000000053
--     จุดที่ผลต่างเท่าเกณฑ์พอดีจะตัดสินพลาด งาน QA รับไม่ได้
--   - ถ้าลด scale เช่น (12,4) ค่าที่ละเอียดกว่านั้นจะถูกปัดเงียบๆ ข้อมูลหายถาวร
--
-- จึงเก็บ decimal(12,6) ไว้เหมือนเดิม แล้วทำ VIEW ไว้ดูแทน
-- VIEW ไม่เก็บข้อมูลซ้ำ ไม่แตะตารางจริง ลบทิ้งเมื่อไหร่ก็ได้ด้วย DROP VIEW
--
-- วิธีตัด : TRIM(TRAILING '0') แล้ว TRIM(TRAILING '.')
--           6.146800 -> 6.1468 , 1.150000 -> 1.15 , 100.000000 -> 100 , 0.000000 -> 0
--           ใช้ตัดข้อความตรงๆ ไม่ผ่าน float จึงไม่มีทางเพี้ยน
--
-- ผลลัพธ์ตรงกับ Utilities/NumberDisplay.cs ที่ฝั่งโปรแกรมใช้
--
-- รันซ้ำได้ : CREATE OR REPLACE
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- เกณฑ์จุดวัด Dimension
CREATE OR REPLACE VIEW `v_dimension_criteria` AS
SELECT `M_CODE`,
       `POINT_ORDER`,
       `POINT_NAME`,
       `EQUIPMENT_TYPE`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `CRITERIA_MIN`)) AS `CRITERIA_MIN`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `CRITERIA_MAX`)) AS `CRITERIA_MAX`,
       `UNIT`,
       `JUDGE_TYPE`
FROM `info_dimension_equipment`;


-- เกณฑ์จุดวัด Regular
CREATE OR REPLACE VIEW `v_regular_criteria` AS
SELECT `M_CODE`,
       `POINT_ORDER`,
       `POINT_NAME`,
       `EQUIPMENT_TYPE`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `CRITERIA_MIN`)) AS `CRITERIA_MIN`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `CRITERIA_MAX`)) AS `CRITERIA_MAX`,
       `UNIT`,
       `JUDGE_TYPE`
FROM `info_regular_equipment`;


-- ค่าที่วัดได้จริง Dimension
CREATE OR REPLACE VIEW `v_dimension_result` AS
SELECT `Report_No`,
       `CAVITY_NAME`,
       `SAMPLING_NO`,
       `POINT_ORDER`,
       `COUNT`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `VALUE`)) AS `VALUE`,
       `JUDGE`,
       `EQUIPMENT_SERIAL_ID`,
       `EMP_ID`,
       `DIMENSION_DATE`,
       `INUSE`
FROM `db_dimension_data`;


-- ค่าที่วัดได้จริง Regular
CREATE OR REPLACE VIEW `v_regular_result` AS
SELECT `REGULAR_NO`,
       `CAVITY_NAME`,
       `SAMPLING_NO`,
       `POINT_ORDER`,
       `COUNT`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `VALUE`)) AS `VALUE`,
       `JUDGE`,
       `EQUIPMENT_SERIAL_ID`,
       `EMP_ID`,
       `INUSE`
FROM `db_regular_data`;


-- ผลตัดสินรายชิ้นของแบบ All
CREATE OR REPLACE VIEW `v_dimension_piece_judge` AS
SELECT `REPORT_NO`,
       `SAMPLING_NO`,
       `CAVITY_NAME`,
       `COUNT`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `DIFFERENCE`)) AS `DIFFERENCE`,
       TRIM(TRAILING '.' FROM TRIM(TRAILING '0' FROM `TOLERANCE`))  AS `TOLERANCE`,
       `JUDGE`,
       `EMP_ID`,
       `JUDGE_DATE`,
       `INUSE`
FROM `db_dimension_piece_judge`;


-- ----------------------------------------------------------------------------
-- ตรวจผล : ต้องได้ตรงกับที่หน้าจอโชว์
-- ----------------------------------------------------------------------------
SELECT 'ตารางจริง' AS src, `CRITERIA_MIN`, `CRITERIA_MAX`
FROM `info_dimension_equipment` WHERE `M_CODE` = 'FL001-JIN' LIMIT 1;

SELECT 'VIEW' AS src, `CRITERIA_MIN`, `CRITERIA_MAX`
FROM `v_dimension_criteria` WHERE `M_CODE` = 'FL001-JIN' LIMIT 1;
