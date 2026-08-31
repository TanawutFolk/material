-- =====================================================================
--  Test data : Appearance Check Pending
--  DB        : _test_qa_system_tanawut  (192.168.14.236)
--  Reports   : QA26-0027 (Test A) , QA26-0028 (Test B)
--
--  สคริปต์นี้ idempotent - รันซ้ำได้ทุกครั้งเพื่อ reset กลับมาสถานะเริ่มต้น
--  แตะเฉพาะ 2 report นี้เท่านั้น ไม่ยุ่งกับ QA26-0024 / QA26-0026 ของเดิม
-- =====================================================================

-- ---------- reset (ลบเฉพาะ 2 report นี้) ----------
DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` IN ('QA26-0027','QA26-0028');
DELETE FROM `db_appearance_data`    WHERE `REPORT_NO` IN ('QA26-0027','QA26-0028');
DELETE FROM `db_packing_size`       WHERE `Report_No` IN ('QA26-0027','QA26-0028');
DELETE FROM `db_report_lot_no`      WHERE `REPORT_NO` IN ('QA26-0027','QA26-0028');


-- =====================================================================
--  TEST A : QA26-0027 / SP050L
--  sampling_type = 1 (All) , Allow_Continue = 1 , ไม่มี Cavity
--  ตรวจไปแล้ว 100 / 280  -> ยังตรวจไม่ครบ
--  pending 3 แถว รวม 6 ชิ้น
-- =====================================================================
UPDATE `db_receive_mat` SET `Inspection_Qty` = 280 WHERE `Report_No` = 'QA26-0027';

INSERT INTO `db_packing_size` (`Report_No`,`BATCH`,`VALUE`,`PACK_COUNT`,`PACKING_SIZE`)
VALUES ('QA26-0027', 1, 280, 1, 280);

INSERT INTO `db_report_lot_no` (`REPORT_NO`,`LOT_NO`)
VALUES ('QA26-0027', 'LOT-A-2601');

INSERT INTO `db_appearance_data`
       (`REPORT_NO`,`BATCH`,`COUNT`,`QTY_SELECT`,`QTY_OK`,`QTY_NG`,`EMP_ID`,`JUDGE`,`INUSE`,`APPEARANCE_DATE`,`UPDATETIME`,`LOT_NO`)
VALUES ('QA26-0027', 1, 1, 100, 94, 6, 'S00823', 0, 1, CURDATE(), NOW(), 'LOT-A-2601');
SET @aidA := LAST_INSERT_ID();

INSERT INTO `db_appearance_pending`
       (`APPEARANCE_ID`,`REPORT_NO`,`BATCH`,`COUNT`,`LOT_NO`,`NG_COUNT`,`QTY_NG`,`NG_DETAIL`,`NG_MODE_ID`,`APPEARANCE_DATE`,`UPDATETIME`)
VALUES (@aidA,'QA26-0027',1,1,'LOT-A-2601',0,3,'Burr' ,1 ,NOW(),NOW()),
       (@aidA,'QA26-0027',1,1,'LOT-A-2601',1,2,'Dent' ,13,NOW(),NOW()),
       (@aidA,'QA26-0027',1,1,'LOT-A-2601',2,1,'Dirty',16,NOW(),NOW());

UPDATE `db_report_status`
   SET `Report_Status` = 6, `Appearance_Check` = 6, `Appearance_Check_Lot_No` = 'LOT-A-2601'
 WHERE `Report_No` = 'QA26-0027';


-- =====================================================================
--  TEST B : QA26-0028 / G043-SHIN
--  sampling_type = 3 (Sampling Table) , Allow_Continue = 0 , Cavity 4 (E,F,G,H)
--  ตรวจไปแล้ว 200 / 200  -> ตรวจครบแล้ว
--  pending 2 แถว รวม 4 ชิ้น
-- =====================================================================
UPDATE `db_receive_mat` SET `Inspection_Qty` = 200 WHERE `Report_No` = 'QA26-0028';

INSERT INTO `db_packing_size` (`Report_No`,`BATCH`,`VALUE`,`PACK_COUNT`,`PACKING_SIZE`)
VALUES ('QA26-0028', 1, 24000, 1, 200);

INSERT INTO `db_report_lot_no` (`REPORT_NO`,`LOT_NO`)
VALUES ('QA26-0028', 'LOT-B-2602');

INSERT INTO `db_appearance_data`
       (`REPORT_NO`,`BATCH`,`COUNT`,`QTY_SELECT`,`QTY_OK`,`QTY_NG`,`EMP_ID`,`JUDGE`,`INUSE`,`APPEARANCE_DATE`,`UPDATETIME`,`LOT_NO`)
VALUES ('QA26-0028', 1, 1, 200, 196, 4, 'S00823', 0, 1, CURDATE(), NOW(), 'LOT-B-2602');
SET @aidB := LAST_INSERT_ID();

INSERT INTO `db_appearance_pending`
       (`APPEARANCE_ID`,`REPORT_NO`,`BATCH`,`COUNT`,`LOT_NO`,`NG_COUNT`,`QTY_NG`,`NG_DETAIL`,`NG_MODE_ID`,`APPEARANCE_DATE`,`UPDATETIME`)
VALUES (@aidB,'QA26-0028',1,1,'LOT-B-2602',0,3,'Chip'  ,8 ,NOW(),NOW()),
       (@aidB,'QA26-0028',1,1,'LOT-B-2602',1,1,'Deform',12,NOW(),NOW());

UPDATE `db_report_status`
   SET `Report_Status` = 6, `Appearance_Check` = 6, `Appearance_Check_Lot_No` = 'LOT-B-2602'
 WHERE `Report_No` = 'QA26-0028';
