-- ============================================================================
-- ใส่ค่าวัดจริงให้ template WideCavity และ Standard เพื่อดูฟอร์ม FM-QA-B08-F
-- ============================================================================
-- 1) QA26-9302 / CAM008  -> WideCavity
--    Cavity M,N,O,P  6 จุด  จุด 1-3 Caliper (S/N B16272968) จุด 4-6 Microscope (S/N 1G19405)
--    ค่าที่ใส่ลอกจากใบจริง QA26-007_CAM008_2026-01-08 จะได้เทียบกับต้นฉบับตรงๆ
--    มีค่า Function ด้วย 4 cavity
--
-- 2) QA26-0026 / BS220L  -> Standard
--    ไม่มี cavity  6 จุด Caliper  5 ตัวอย่าง
--    ตั้งใจให้ตัวอย่างที่ 3 จุดที่ 2 หลุดเกณฑ์ เพื่อดูว่า NG ขึ้นจริง
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

SET @caliper    = 135;   -- B16272968
SET @microscope = 131;   -- 1G19405


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase, `REPORT_NO`, COUNT(*) AS rows_
FROM `db_dimension_data` WHERE `REPORT_NO` IN ('QA26-9302','QA26-0026') GROUP BY `REPORT_NO`;


-- ----------------------------------------------------------------------------
-- 1) WideCavity : CAM008
-- ----------------------------------------------------------------------------
DELETE FROM `db_function_data`  WHERE `REPORT_NO` = 'QA26-9302';
DELETE FROM `db_dimension_data` WHERE `REPORT_NO` = 'QA26-9302';
DELETE FROM `db_receive_mat`    WHERE `Report_No` = 'QA26-9302';

INSERT INTO `db_receive_mat`
    (`Report_No`, `Receive_Date`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`,
     `Invoice_No`, `Vendor_Name`, `Lot_Size`, `Inspection_Qty`, `Emp_Issue_Report`, `Issue_Date`)
VALUES
    ('QA26-9302', '2026-08-19', 1, '', 'CAM008', 'FSC Stopper L Gray',
     'TEST-CAM008', 'CAM PLAS', 4000, 4000, 'TEST', NOW());

-- ค่าวัด : 1 ตัวอย่างต่อ cavity ตามที่ setting ไว้ (Sampling_Qty = 1, Cavity 4)
INSERT INTO `db_dimension_data`
    (`REPORT_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `DIMENSION_DATE`, `INUSE`)
VALUES
    ('QA26-9302', 1, 1, 1, 'M', @caliper,    11.57, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 2, 1, 1, 'M', @caliper,     5.76, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 3, 1, 1, 'M', @caliper,     6.38, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 4, 1, 1, 'M', @microscope,  0.15, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 5, 1, 1, 'M', @microscope, 1.035, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 6, 1, 1, 'M', @microscope, 1.204, 1, 'TEST', NOW(), 1),

    ('QA26-9302', 1, 2, 1, 'N', @caliper,    11.54, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 2, 2, 1, 'N', @caliper,     5.78, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 3, 2, 1, 'N', @caliper,     6.36, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 4, 2, 1, 'N', @microscope, 0.138, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 5, 2, 1, 'N', @microscope,  1.04, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 6, 2, 1, 'N', @microscope, 1.178, 1, 'TEST', NOW(), 1),

    ('QA26-9302', 1, 3, 1, 'O', @caliper,    11.54, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 2, 3, 1, 'O', @caliper,     5.75, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 3, 3, 1, 'O', @caliper,     6.35, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 4, 3, 1, 'O', @microscope, 0.139, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 5, 3, 1, 'O', @microscope, 1.043, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 6, 3, 1, 'O', @microscope, 1.173, 1, 'TEST', NOW(), 1),

    ('QA26-9302', 1, 4, 1, 'P', @caliper,    11.54, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 2, 4, 1, 'P', @caliper,     5.76, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 3, 4, 1, 'P', @caliper,     6.37, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 4, 4, 1, 'P', @microscope, 0.122, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 5, 4, 1, 'P', @microscope, 1.043, 1, 'TEST', NOW(), 1),
    ('QA26-9302', 6, 4, 1, 'P', @microscope, 1.186, 1, 'TEST', NOW(), 1);

INSERT INTO `db_function_data`
    (`REPORT_NO`, `COUNT`, `SAMPLING_NO`, `CAVITY_NAME`, `LOT_NO`, `JUDGE`, `REMARK`, `EMP_ID`, `FUNCTION_DATE`, `INUSE`)
VALUES
    ('QA26-9302', 1, 1, 'M', '', 1, '', 'TEST', NOW(), 1),
    ('QA26-9302', 1, 2, 'N', '', 1, '', 'TEST', NOW(), 1),
    ('QA26-9302', 1, 3, 'O', '', 1, '', 'TEST', NOW(), 1),
    ('QA26-9302', 1, 4, 'P', '', 1, '', 'TEST', NOW(), 1);


-- ----------------------------------------------------------------------------
-- 2) Standard : BS220L (QA26-0026 มีใบอยู่แล้ว เติมเฉพาะค่าวัด)
--    ตัวอย่างที่ 3 จุดที่ 2 ใส่ 11.9 ซึ่งเกิน CRITERIA_MAX 11.8 -> JUDGE 0
-- ----------------------------------------------------------------------------
DELETE FROM `db_dimension_data` WHERE `REPORT_NO` = 'QA26-0026';

INSERT INTO `db_dimension_data`
    (`REPORT_NO`, `POINT_ORDER`, `SAMPLING_NO`, `COUNT`, `CAVITY_NAME`,
     `EQUIPMENT_SERIAL_ID`, `VALUE`, `JUDGE`, `EMP_ID`, `DIMENSION_DATE`, `INUSE`)
SELECT 'QA26-0026',
       pt.`POINT_ORDER`,
       sn.`n`,
       1,
       '0',
       @caliper,
       ROUND(IF(sn.`n` = 3 AND pt.`POINT_ORDER` = 2,
                11.9,
                pt.`CRITERIA_MIN` + ((pt.`CRITERIA_MAX` - pt.`CRITERIA_MIN`) * ((sn.`n` * 2 + pt.`POINT_ORDER`) MOD 9) / 10)), 3),
       IF(sn.`n` = 3 AND pt.`POINT_ORDER` = 2, 0, 1),
       'TEST', NOW(), 1
FROM (SELECT `POINT_ORDER`, `CRITERIA_MIN`, `CRITERIA_MAX`
      FROM `info_dimension_equipment` WHERE `M_CODE` = 'BS220L') pt
CROSS JOIN (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) sn;


-- ----------------------------------------------------------------------------
-- AFTER
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase, `REPORT_NO`,
       COUNT(*) AS rows_,
       COUNT(DISTINCT `SAMPLING_NO`) AS samples,
       SUM(`JUDGE` = 0) AS ng_points
FROM `db_dimension_data` WHERE `REPORT_NO` IN ('QA26-9302','QA26-0026') GROUP BY `REPORT_NO`;
