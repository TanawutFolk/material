-- ============================================================================
-- ล้างข้อมูลใบตรวจทั้งหมด เพื่อเริ่มทดสอบใหม่
-- ============================================================================
-- ลบเฉพาะ "ข้อมูลการตรวจ" ไม่แตะ master data
--
-- ลบ    : db_receive_mat , db_report_status , db_report_lot_no ,
--         db_packing_check , db_packing_size , db_inspection_data ,
--         db_regular_data , db_function_data ,
--         db_dimension_data , db_dimension_piece_judge ,
--         db_appearance_data , db_appearance_pending , info_report_active
--
-- ไม่แตะ : info_mat_inspection_list , info_*_sampling , info_*_equipment ,
--          info_equipment_serial , info_equipment_type , info_strictness*
--          (คือค่าตั้งที่เพิ่งแก้ให้ตรงกับ Index_พี่หนู.xlsx ไปทั้งหมด)
--
-- สำรองไว้แล้วที่ _backup_reports_20260827.sql (1,196 แถว)
-- ถ้าจะกู้คืนให้รันไฟล์นั้น
--
-- ลบตามลำดับลูกก่อนแม่ เผื่อมี foreign key
--
-- *** รันกับ DB ทดสอบเท่านั้น ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- BEFORE
-- ----------------------------------------------------------------------------
SELECT 'BEFORE' AS phase,
       (SELECT COUNT(*) FROM `db_receive_mat`)           AS receive_mat,
       (SELECT COUNT(*) FROM `db_report_status`)         AS report_status,
       (SELECT COUNT(*) FROM `db_dimension_data`)        AS dimension_data,
       (SELECT COUNT(*) FROM `db_appearance_data`)       AS appearance_data,
       (SELECT COUNT(*) FROM `info_report_active`)       AS report_active;


-- ----------------------------------------------------------------------------
-- ข้อมูลผลตรวจ
-- ----------------------------------------------------------------------------
DELETE FROM `db_appearance_pending`;
DELETE FROM `db_appearance_data`;
DELETE FROM `db_dimension_piece_judge`;
DELETE FROM `db_dimension_data`;
DELETE FROM `db_function_data`;
DELETE FROM `db_regular_data`;
DELETE FROM `db_inspection_data`;
DELETE FROM `db_packing_check`;
DELETE FROM `db_packing_size`;
DELETE FROM `db_report_lot_no`;


-- ----------------------------------------------------------------------------
-- ตัวล็อกใบที่ค้างอยู่ ถ้าไม่ล้างจะเปิดใบไม่ได้เพราะระบบคิดว่ามีคนทำอยู่
-- ----------------------------------------------------------------------------
DELETE FROM `info_report_active`;


-- ----------------------------------------------------------------------------
-- หัวใบและสถานะ
-- ----------------------------------------------------------------------------
DELETE FROM `db_report_status`;
DELETE FROM `db_receive_mat`;


-- ----------------------------------------------------------------------------
-- AFTER : ต้องเป็น 0 ทุกตาราง
-- ----------------------------------------------------------------------------
SELECT 'AFTER' AS phase,
       (SELECT COUNT(*) FROM `db_receive_mat`)           AS receive_mat,
       (SELECT COUNT(*) FROM `db_report_status`)         AS report_status,
       (SELECT COUNT(*) FROM `db_report_lot_no`)         AS report_lot_no,
       (SELECT COUNT(*) FROM `db_packing_check`)         AS packing_check,
       (SELECT COUNT(*) FROM `db_packing_size`)          AS packing_size,
       (SELECT COUNT(*) FROM `db_inspection_data`)       AS inspection_data,
       (SELECT COUNT(*) FROM `db_regular_data`)          AS regular_data,
       (SELECT COUNT(*) FROM `db_function_data`)         AS function_data,
       (SELECT COUNT(*) FROM `db_dimension_data`)        AS dimension_data,
       (SELECT COUNT(*) FROM `db_dimension_piece_judge`) AS piece_judge,
       (SELECT COUNT(*) FROM `db_appearance_data`)       AS appearance_data,
       (SELECT COUNT(*) FROM `db_appearance_pending`)    AS appearance_pending,
       (SELECT COUNT(*) FROM `info_report_active`)       AS report_active;


-- ----------------------------------------------------------------------------
-- ยืนยันว่า master data ยังอยู่ครบ
-- ----------------------------------------------------------------------------
SELECT 'master ยังอยู่' AS phase,
       (SELECT COUNT(*) FROM `info_mat_inspection_list`) AS inspection_list,
       (SELECT COUNT(*) FROM `info_dimension_equipment`) AS dim_points,
       (SELECT COUNT(*) FROM `info_regular_equipment`)   AS reg_points,
       (SELECT COUNT(*) FROM `info_equipment_serial`)    AS equipment_serial;
