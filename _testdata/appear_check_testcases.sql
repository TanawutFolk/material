-- =====================================================================
--  Test data : Appearance Check (หน้าตรวจปกติ ไม่ใช่ Pending)
--  DB        : _test_qa_system_tanawut  (192.168.14.236)
--  Reports   : QA26-0031 (Test C) , QA26-9001 (Test D - report สร้างใหม่)
--
--  idempotent - รันซ้ำได้เพื่อ reset
--  ไม่ยุ่งกับ QA26-0024 / QA26-0026 / QA26-0027 / QA26-0028
--
--  เงื่อนไขที่ report ต้องผ่านถึงจะโผล่ในลิสต์ (SearchForOpAppear):
--    (Inspection_Data_Check=3 AND Packing_Check=1) OR (Inspection_Data_Check=1 AND Keep_Data_Need=1)
--    AND Appearance_Check IN (NULL, 8, 2)
--    AND Report_Status IS NOT NULL  -- และต้องไม่ใช่ 6/0 ไม่งั้น CheckTimer เตะออก
--    AND Appearance_Check_Need = 1
--    AND Inspection_Qty IS NOT NULL
-- =====================================================================

-- ---------- reset ----------
DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` IN ('QA26-0031','QA26-9001');
DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` IN ('QA26-0031','QA26-9001');
DELETE FROM `db_packing_size`       WHERE `Report_No` IN ('QA26-0031','QA26-9001');
DELETE FROM `db_report_lot_no`      WHERE `REPORT_NO` IN ('QA26-0031','QA26-9001');
DELETE FROM `db_report_status`      WHERE `Report_No` = 'QA26-9001';
DELETE FROM `db_receive_mat`        WHERE `Report_No` = 'QA26-9001';


-- =====================================================================
--  TEST C : QA26-0031 / SHIN003  (report เดิม)
--  sampling_type = 3 (Sampling Table) , Allow_Continue = 0 , Cavity 4 (A,B,C,D)
--  packing : 4 แพ็ค x 500 = lot 2000 , ต้องตรวจรวม 20 -> แพ็คละ 5
--  lot no  : 2 lot -> ต้องเลือกเอง (ทดสอบ dropdown LOT_NO)
-- =====================================================================
UPDATE `db_receive_mat`
   SET `Lot_Size` = 2000, `Inspection_Qty` = 20
 WHERE `Report_No` = 'QA26-0031';

INSERT INTO `db_packing_size` (`Report_No`,`BATCH`,`VALUE`,`PACK_COUNT`,`PACKING_SIZE`)
VALUES ('QA26-0031', 1, 500, 4, 20);

INSERT INTO `db_report_lot_no` (`REPORT_NO`,`LOT_NO`)
VALUES ('QA26-0031','LOT-C-A01'),
       ('QA26-0031','LOT-C-B02');

UPDATE `db_report_status`
   SET `Packing_Check` = 1,
       `Inspection_Data_Check` = 3,
       `Appearance_Check` = NULL,
       `Appearance_Check_Lot_No` = NULL,
       `Report_Status` = 1
 WHERE `Report_No` = 'QA26-0031';


-- =====================================================================
--  TEST D : QA26-9001 / SP051L  (สร้าง report ใหม่ เลข 9xxx กันชนกับที่โปรแกรม gen)
--  sampling_type = 1 (All) , Allow_Continue = 1 , ไม่มี Cavity
--  packing : 3 แพ็ค x 40 = lot 120 , All mode -> ตรวจทั้ง 120 -> แพ็คละ 40
--  lot no  : 1 lot -> เติมให้อัตโนมัติ
-- =====================================================================
INSERT INTO `db_receive_mat`
       (`Report_No`,`Receive_Date`,`Report_Type`,`Regular_No`,`M_Code`,`Material_Name`,
        `Invoice_No`,`Vendor_Name`,`Lot_Size`,`Inspection_Qty`,`Emp_Issue_Report`,`Issue_Date`)
VALUES ('QA26-9001','2026-06-08',1,NULL,'SP051L','LID : BM-POT-08507-B',
        'FFT26/00006','Takahata Precision (Thailand) Ltd.',120,120,'S00823',NOW());

INSERT INTO `db_report_status`
       (`Report_No`,`Packing_Check`,`Inspection_Data_Check`,`Appearance_Check`,`Report_Status`)
VALUES ('QA26-9001', 1, 3, NULL, 1);

INSERT INTO `db_packing_size` (`Report_No`,`BATCH`,`VALUE`,`PACK_COUNT`,`PACKING_SIZE`)
VALUES ('QA26-9001', 1, 40, 3, 120);

INSERT INTO `db_report_lot_no` (`REPORT_NO`,`LOT_NO`)
VALUES ('QA26-9001','LOT-D-2604');
