-- ============================================================================
-- เติมจุดวัดที่ขาดตาม Index_พี่หนู.xlsx  ชีต Appendix D1-D5 Update 18-Aug-26
-- ============================================================================
-- 6 M-CODE ที่เปิดธง Check_Need ไว้แต่ไม่มีจุดวัดใน DB
-- ทำให้หน้าจอเปิดงานได้แต่ตารางกรอกค่าว่างเปล่า (โชว์ 1/0 คือ 0 หน้า)
--
-- ชื่อเครื่องมือใส่ตามที่เอกสารเขียน รวม S/N ไว้ในชื่อ
-- ตามรูปแบบเดิมของ type 3 (Gauge QA-MD-008-03,04) และ 17 (QA-JG-009-02 Plug frame BS122)
--
-- จุดที่เอกสารเขียน OK/OK คือการตัดสินผ่านไม่ผ่าน เก็บเป็น MIN = MAX = 1 และไม่มีหน่วย
-- ตรงกับที่ระบบใช้อยู่กับ Jig และ Gauge
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ---------------------------------------------------------------------------
-- BEFORE
-- ---------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, m.`M_CODE`,
       (SELECT COUNT(*) FROM `info_regular_equipment` x WHERE x.`M_CODE` = m.`M_CODE`) AS reg_pts,
       (SELECT COUNT(*) FROM `info_dimension_equipment` x WHERE x.`M_CODE` = m.`M_CODE`) AS dim_pts
FROM `info_mat_inspection_list` m
WHERE m.`M_CODE` IN ('R00FA039-SHI', 'RCOMM002-SAN', 'FS050-FTC', 'RCOMM001-SHI', 'RCOMM002-SHI', 'RCOMM003-SHI') ORDER BY m.`M_CODE`;


-- ---------------------------------------------------------------------------
-- ชนิดเครื่องมือใหม่ที่ยังไม่มีในระบบ
-- ---------------------------------------------------------------------------
INSERT INTO `info_equipment_type` (`Equipment_Type`, `Equipment_Name`)
SELECT 18, 'Pin Gauge QA-MD-008-05' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `info_equipment_type` t WHERE t.`Equipment_Name` = 'Pin Gauge QA-MD-008-05');
INSERT INTO `info_equipment_type` (`Equipment_Type`, `Equipment_Name`)
SELECT 19, 'Pin Gauge GO MA-MD-003-35' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `info_equipment_type` t WHERE t.`Equipment_Name` = 'Pin Gauge GO MA-MD-003-35');
INSERT INTO `info_equipment_type` (`Equipment_Type`, `Equipment_Name`)
SELECT 20, 'Pin Gauge NO GO MA-MD-003-35' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `info_equipment_type` t WHERE t.`Equipment_Name` = 'Pin Gauge NO GO MA-MD-003-35');
INSERT INTO `info_equipment_type` (`Equipment_Type`, `Equipment_Name`)
SELECT 21, 'Force Gauge FB-MD-502-001' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `info_equipment_type` t WHERE t.`Equipment_Name` = 'Force Gauge FB-MD-502-001');
INSERT INTO `info_equipment_type` (`Equipment_Type`, `Equipment_Name`)
SELECT 22, 'Pin gauge GO QA-MD-008-15' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `info_equipment_type` t WHERE t.`Equipment_Name` = 'Pin gauge GO QA-MD-008-15');
INSERT INTO `info_equipment_type` (`Equipment_Type`, `Equipment_Name`)
SELECT 23, 'Pin gauge NO GO QA-MD-008-16' FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM `info_equipment_type` t WHERE t.`Equipment_Name` = 'Pin gauge NO GO QA-MD-008-16');

-- ---- R00FA039-SHI : regular 13 จุด ----
DELETE FROM `info_regular_equipment` WHERE `M_CODE` = 'R00FA039-SHI';
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 1, 1, '9', '0', 1.15, 1.25, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 2, 1, '10', '0', 0.62, 0.78, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 3, 1, '11', '0', 4.95, 5.05, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 4, 1, '6 ด้าน PET', '0', 11.35, 11.45, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 5, 1, '6 ด้าน Y', '0', 11.35, 11.45, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 6, 1, '7 ด้าน PET', '0', 10.3, 10.38, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 7, 1, '7 ด้าน Y', '0', 10.3, 10.38, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 8, 1, '13 ด้าน PET', '0', 0.0, 0.05, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 9, 1, '13 ด้าน Y', '0', 0.0, 0.05, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 10, 1, '8 ด้านบน', '0', 14.65, 14.75, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 11, 1, '8 ด้านล่าง', '0', 14.65, 14.75, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 12, 1, '12 ด้านบน', '0', 0.0, 0.05, 'mm');
INSERT INTO `info_regular_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('R00FA039-SHI', 13, 1, '12 ด้านล่าง', '0', 0.0, 0.05, 'mm');

-- ---- RCOMM002-SAN : dimension 3 จุด ----
DELETE FROM `info_dimension_equipment` WHERE `M_CODE` = 'RCOMM002-SAN';
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM002-SAN', 1, 4, '1', '0', 8.94, 8.99, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM002-SAN', 2, 18, '2', '0', 1.0, 1.0, NULL);
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM002-SAN', 3, 18, '3', '0', 1.0, 1.0, NULL);

-- ---- FS050-FTC : dimension 7 จุด ----
DELETE FROM `info_dimension_equipment` WHERE `M_CODE` = 'FS050-FTC';
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 1, 4, '1', '0', 14.7, 15.1, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 2, 4, '2', '0', 9.9, 10.1, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 3, 4, '3', '0', 31.9, 32.1, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 4, 1, '4', '0', 5.01, 5.018, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 5, 19, '4', '0', 1.0, 1.0, NULL);
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 6, 20, '4', '0', 1.0, 1.0, NULL);
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('FS050-FTC', 7, 5, '5', '0', 9.5, 9.7, 'mm');

-- ---- RCOMM001-SHI : dimension 11 จุด ----
DELETE FROM `info_dimension_equipment` WHERE `M_CODE` = 'RCOMM001-SHI';
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 1, 21, '0', '0', 30.0, 50.0, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 2, 9, '1', '0', 12.1, 12.3, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 3, 1, '2', '0', 0.0, 0.1, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 4, 22, '3', '0', 1.0, 1.0, NULL);
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 5, 23, '3', '0', 1.0, 1.0, NULL);
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 6, 1, '4', '0', 0.75, 0.8, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 7, 1, '5', '0', 0.57, 0.59, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 8, 1, '6', '0', 0.75, 0.8, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 9, 1, '7', '0', 0.63, 0.65, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 10, 1, '8', '0', 0.75, 0.8, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM001-SHI', 11, 1, '9', '0', 0.57, 0.59, 'mm');

-- ---- RCOMM002-SHI : dimension 2 จุด ----
DELETE FROM `info_dimension_equipment` WHERE `M_CODE` = 'RCOMM002-SHI';
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM002-SHI', 1, 9, '1', '0', 1.29, 1.39, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM002-SHI', 2, 9, '2', '0', 8.91, 9.01, 'mm');

-- ---- RCOMM003-SHI : dimension 2 จุด ----
DELETE FROM `info_dimension_equipment` WHERE `M_CODE` = 'RCOMM003-SHI';
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM003-SHI', 1, 9, '1', '0', 1.31, 1.41, 'mm');
INSERT INTO `info_dimension_equipment` (`M_CODE`,`POINT_ORDER`,`EQUIPMENT_TYPE`,`POINT_NAME`,`POINT_CAL`,`CRITERIA_MIN`,`CRITERIA_MAX`,`UNIT`)
VALUES ('RCOMM003-SHI', 2, 9, '2', '0', 9.31, 9.41, 'mm');


-- ---------------------------------------------------------------------------
-- AFTER
-- ---------------------------------------------------------------------------
SELECT 'AFTER' AS phase, m.`M_CODE`,
       (SELECT COUNT(*) FROM `info_regular_equipment` x WHERE x.`M_CODE` = m.`M_CODE`) AS reg_pts,
       (SELECT COUNT(*) FROM `info_dimension_equipment` x WHERE x.`M_CODE` = m.`M_CODE`) AS dim_pts
FROM `info_mat_inspection_list` m
WHERE m.`M_CODE` IN ('R00FA039-SHI', 'RCOMM002-SAN', 'FS050-FTC', 'RCOMM001-SHI', 'RCOMM002-SHI', 'RCOMM003-SHI') ORDER BY m.`M_CODE`;

SELECT `Equipment_Type`, `Equipment_Name` FROM `info_equipment_type` WHERE `Equipment_Type` >= 18 ORDER BY `Equipment_Type`;
