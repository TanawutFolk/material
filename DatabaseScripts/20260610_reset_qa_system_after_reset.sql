-- Reset transaction data for a fresh user run.
-- This script is intentionally locked to qa_system_after_reset.
-- It preserves all info_* master tables.

USE `qa_system_after_reset`;

DROP PROCEDURE IF EXISTS `assert_reset_database`;
DELIMITER //
CREATE PROCEDURE `assert_reset_database`()
BEGIN
    IF DATABASE() <> 'qa_system_after_reset' THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Blocked: this reset may only run on qa_system_after_reset';
    END IF;
END//
DELIMITER ;

CALL `assert_reset_database`();
DROP PROCEDURE `assert_reset_database`;

-- Keep a snapshot inside a separate schema before deleting transaction data.
CREATE DATABASE IF NOT EXISTS `qa_system_after_reset_backup_20260610`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_appearance_pending`
LIKE `qa_system_after_reset`.`db_appearance_pending`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_appearance_pending`
SELECT * FROM `qa_system_after_reset`.`db_appearance_pending`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_appearance_data`
LIKE `qa_system_after_reset`.`db_appearance_data`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_appearance_data`
SELECT * FROM `qa_system_after_reset`.`db_appearance_data`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_dimension_data`
LIKE `qa_system_after_reset`.`db_dimension_data`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_dimension_data`
SELECT * FROM `qa_system_after_reset`.`db_dimension_data`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_function_data`
LIKE `qa_system_after_reset`.`db_function_data`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_function_data`
SELECT * FROM `qa_system_after_reset`.`db_function_data`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_inspection_data`
LIKE `qa_system_after_reset`.`db_inspection_data`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_inspection_data`
SELECT * FROM `qa_system_after_reset`.`db_inspection_data`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_regular_data`
LIKE `qa_system_after_reset`.`db_regular_data`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_regular_data`
SELECT * FROM `qa_system_after_reset`.`db_regular_data`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_packing_check`
LIKE `qa_system_after_reset`.`db_packing_check`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_packing_check`
SELECT * FROM `qa_system_after_reset`.`db_packing_check`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_packing_size`
LIKE `qa_system_after_reset`.`db_packing_size`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_packing_size`
SELECT * FROM `qa_system_after_reset`.`db_packing_size`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_report_lot_no`
LIKE `qa_system_after_reset`.`db_report_lot_no`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_report_lot_no`
SELECT * FROM `qa_system_after_reset`.`db_report_lot_no`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_report_status`
LIKE `qa_system_after_reset`.`db_report_status`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_report_status`
SELECT * FROM `qa_system_after_reset`.`db_report_status`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_receive_mat`
LIKE `qa_system_after_reset`.`db_receive_mat`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_receive_mat`
SELECT * FROM `qa_system_after_reset`.`db_receive_mat`;

CREATE TABLE `qa_system_after_reset_backup_20260610`.`db_receive_refresh_log`
LIKE `qa_system_after_reset`.`db_receive_refresh_log`;
INSERT INTO `qa_system_after_reset_backup_20260610`.`db_receive_refresh_log`
SELECT * FROM `qa_system_after_reset`.`db_receive_refresh_log`;

SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE `db_appearance_pending`;
TRUNCATE TABLE `db_appearance_data`;
TRUNCATE TABLE `db_dimension_data`;
TRUNCATE TABLE `db_function_data`;
TRUNCATE TABLE `db_inspection_data`;
TRUNCATE TABLE `db_regular_data`;
TRUNCATE TABLE `db_packing_check`;
TRUNCATE TABLE `db_packing_size`;
TRUNCATE TABLE `db_report_lot_no`;
TRUNCATE TABLE `db_report_status`;
TRUNCATE TABLE `db_receive_mat`;
TRUNCATE TABLE `db_receive_refresh_log`;

SET FOREIGN_KEY_CHECKS = 1;

-- Every value must be zero before the application is opened.
SELECT 'db_appearance_pending' AS table_name, COUNT(*) AS row_count FROM `db_appearance_pending`
UNION ALL SELECT 'db_appearance_data', COUNT(*) FROM `db_appearance_data`
UNION ALL SELECT 'db_dimension_data', COUNT(*) FROM `db_dimension_data`
UNION ALL SELECT 'db_function_data', COUNT(*) FROM `db_function_data`
UNION ALL SELECT 'db_inspection_data', COUNT(*) FROM `db_inspection_data`
UNION ALL SELECT 'db_regular_data', COUNT(*) FROM `db_regular_data`
UNION ALL SELECT 'db_packing_check', COUNT(*) FROM `db_packing_check`
UNION ALL SELECT 'db_packing_size', COUNT(*) FROM `db_packing_size`
UNION ALL SELECT 'db_report_lot_no', COUNT(*) FROM `db_report_lot_no`
UNION ALL SELECT 'db_report_status', COUNT(*) FROM `db_report_status`
UNION ALL SELECT 'db_receive_mat', COUNT(*) FROM `db_receive_mat`
UNION ALL SELECT 'db_receive_refresh_log', COUNT(*) FROM `db_receive_refresh_log`;

