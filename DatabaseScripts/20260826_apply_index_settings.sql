-- ============================================================================
-- ปรับค่าตั้ง M-CODE ให้ตรงกับ Index_พี่หนู.xlsx
-- ชีต : Appendix D1-D5 Update 18-Aug-26
-- ============================================================================
-- สร้างจากการอ่านไฟล์ Excel ตรงๆ ไม่ได้อ่านจากภาพ
-- เทียบทั้งหมด 263 M-CODE : ตรงอยู่แล้ว 234 , แก้ตามเอกสาร 26 , ติดปัญหา 3
--
-- ยึดเอกสารเป็นหลักตามที่ตกลงกัน
-- แถวที่เอกสารเป็น "-" จะปิดธง Check_Need เป็น 0 แต่ไม่ลบแถวใน info_*_sampling
-- เผื่อวันหลังเปิดใช้อีกจะได้ไม่ต้องตั้งค่าใหม่
--
-- INSERT ... ON DUPLICATE KEY UPDATE เพราะบาง M-CODE ยังไม่มีแถวใน info_*_sampling เลย
--
-- *** รันกับ DB ทดสอบเท่านั้น รันซ้ำได้ ***
-- ============================================================================

USE `_test_qa_system_tanawut`;

-- ---- CAM010  (เอกสารแถว 185) ----
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'CAM010';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('CAM010', 4, 3, 1, 2, 1, 'I,J,K,L')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=1;

-- ---- FS400-CAM  (เอกสารแถว 211) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'FS400-CAM';

-- ---- FS401-CAM  (เอกสารแถว 212) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'FS401-CAM';

-- ---- FS402-CAM  (เอกสารแถว 213) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'FS402-CAM';

-- ---- FS403-CAM  (เอกสารแถว 214) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'FS403-CAM';

-- ---- FS404-CAM  (เอกสารแถว 215) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'FS404-CAM';

-- ---- CAM010-NB  (เอกสารแถว 225) ----
UPDATE `info_mat_inspection_list` SET `Regular_Check_Need` = 1 WHERE `M_CODE` = 'CAM010-NB';
INSERT INTO `info_regular_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('CAM010-NB', 4, 4, 2, 0, 0, 'I,J,K,L')
ON DUPLICATE KEY UPDATE `Sampling_Type`=4, `Sampling_Qty`=2, `Strictness_Type`=0, `Strictness_Level`=0;

-- ---- RCOMM015-CAM  (เอกสารแถว 230) ----
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM015-CAM';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM015-CAM', 4, 3, 1, 2, 3, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=3;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM015-CAM';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM015-CAM', 4, 3, 1, 2, 3, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=3;

-- ---- RCOMM017-CAM  (เอกสารแถว 237) ----
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM017-CAM';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM017-CAM', 4, 3, 1, 2, 3, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=3;

-- ---- SHIN004  (เอกสารแถว 264) ----
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'SHIN004';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('SHIN004', 4, 3, 1, 1, 2, 'A,B,C,D')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;

-- ---- R00FA039-SHI  (เอกสารแถว 368) ----
UPDATE `info_mat_inspection_list` SET `Regular_Check_Need` = 1 WHERE `M_CODE` = 'R00FA039-SHI';
INSERT INTO `info_regular_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('R00FA039-SHI', 4, 4, 1, 0, 0, 'E,F,G,H')
ON DUPLICATE KEY UPDATE `Sampling_Type`=4, `Sampling_Qty`=1, `Strictness_Type`=0, `Strictness_Level`=0;
UPDATE `info_mat_inspection_list` SET `Function_Check_Need` = 1 WHERE `M_CODE` = 'R00FA039-SHI';
INSERT INTO `info_function_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('R00FA039-SHI', 4, 3, 2, 1, 2, 'E,F,G,H')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=2, `Strictness_Type`=1, `Strictness_Level`=2;
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'R00FA039-SHI';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('R00FA039-SHI', 4, 3, 2, 1, 2, 'E,F,G,H')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=2, `Strictness_Type`=1, `Strictness_Level`=2;

-- ---- SAN008  (เอกสารแถว 381) ----
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'SAN008';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('SAN008', 4, 3, 1, 2, 1, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=1;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'SAN008';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('SAN008', 4, 3, 1, 2, 1, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=1;

-- ---- SAN010  (เอกสารแถว 384) ----
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'SAN010';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('SAN010', 4, 3, 1, 1, 1, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=1;

-- ---- SAN043  (เอกสารแถว 399) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'SAN043';

-- ---- SAN044  (เอกสารแถว 403) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'SAN044';

-- ---- SAN045  (เอกสารแถว 404) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'SAN045';

-- ---- FPS037-SAN  (เอกสารแถว 405) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'FPS037-SAN';
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'FPS037-SAN';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('FPS037-SAN', 4, 3, 1, 2, 1, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=1;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'FPS037-SAN';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('FPS037-SAN', 4, 3, 1, 2, 1, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=2, `Strictness_Level`=1;

-- ---- RKNAI047-SAN  (เอกสารแถว 418) ----
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 0 WHERE `M_CODE` = 'RKNAI047-SAN';

-- ---- RCOMM001-SAN  (เอกสารแถว 420) ----
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM001-SAN';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM001-SAN', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;

-- ---- RCOMM002-SAN  (เอกสารแถว 423) ----
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM002-SAN';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM002-SAN', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM002-SAN';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM002-SAN', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;

-- ---- BS221L  (เอกสารแถว 432) ----
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'BS221L';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('BS221L', 0, 1, 0, 0, 0, '0')
ON DUPLICATE KEY UPDATE `Sampling_Type`=1, `Sampling_Qty`=0, `Strictness_Type`=0, `Strictness_Level`=0;

-- ---- RFLTFB05-JIN  (เอกสารแถว 602) ----
UPDATE `info_mat_inspection_list` SET `Function_Check_Need` = 1 WHERE `M_CODE` = 'RFLTFB05-JIN';
INSERT INTO `info_function_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RFLTFB05-JIN', 0, 3, 0, 1, 1, '0')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=0, `Strictness_Type`=1, `Strictness_Level`=1;

-- ---- RCOMM001-SHI  (เอกสารแถว 691) ----
UPDATE `info_mat_inspection_list` SET `Function_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM001-SHI';
INSERT INTO `info_function_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM001-SHI', 4, 3, 1, 1, 2, 'A,B,C,D')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM001-SHI';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM001-SHI', 4, 3, 1, 1, 2, 'A,B,C,D')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;

-- ---- RSL00019-FEC  (เอกสารแถว 702) ----
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RSL00019-FEC';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RSL00019-FEC', 0, 1, 0, 0, 0, '0')
ON DUPLICATE KEY UPDATE `Sampling_Type`=1, `Sampling_Qty`=0, `Strictness_Type`=0, `Strictness_Level`=0;

-- ---- RCOMM002-SHI  (เอกสารแถว 704) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'RCOMM002-SHI';
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM002-SHI';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM002-SHI', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM002-SHI';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM002-SHI', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;

-- ---- RCOMM003-SHI  (เอกสารแถว 706) ----
UPDATE `info_mat_inspection_list` SET `Keep_Data_Need` = 1 WHERE `M_CODE` = 'RCOMM003-SHI';
UPDATE `info_mat_inspection_list` SET `Dimension_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM003-SHI';
INSERT INTO `info_dimension_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM003-SHI', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;
UPDATE `info_mat_inspection_list` SET `Appearance_Check_Need` = 1 WHERE `M_CODE` = 'RCOMM003-SHI';
INSERT INTO `info_appearance_sampling` (`M_Code`,`Cavity_Qty`,`Sampling_Type`,`Sampling_Qty`,`Strictness_Type`,`Strictness_Level`,`Cavity_Name`)
VALUES ('RCOMM003-SHI', 4, 3, 1, 1, 2, '1,2,3,4')
ON DUPLICATE KEY UPDATE `Sampling_Type`=3, `Sampling_Qty`=1, `Strictness_Type`=1, `Strictness_Level`=2;


-- ---- ปิดธงที่ยังเป็น NULL ----
-- RCOMM002-SAN / RCOMM002-SHI / RCOMM003-SHI มีแถวใน info_mat_inspection_list
-- แต่ทุกคอลัมน์เป็น NULL อยู่ เอกสารระบุแค่ Keep / Dimension / Appearance
-- ช่องที่เอกสารเขียน "-" จึงตั้งเป็น 0 ให้ชัดเจน ไม่ปล่อยเป็น NULL
-- ส่วน Packing_Check_Mode กับ INUSE เอกสารไม่ได้ระบุ จึงไม่แตะ (แจ้งไว้ในรายงาน)
UPDATE `info_mat_inspection_list`
   SET `Regular_Check_Need`  = COALESCE(`Regular_Check_Need`, 0),
       `Function_Check_Need` = COALESCE(`Function_Check_Need`, 0)
 WHERE `M_CODE` IN ('RCOMM002-SAN', 'RCOMM002-SHI', 'RCOMM003-SHI');
