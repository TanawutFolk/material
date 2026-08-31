CREATE TABLE IF NOT EXISTS info_function_equipment
(
    M_CODE          VARCHAR(100) NOT NULL,
    POINT_ORDER     SMALLINT NOT NULL,
    EQUIPMENT_TYPE  SMALLINT NULL,
    POINT_NAME      VARCHAR(100) NULL,
    POINT_CAL       VARCHAR(10) NOT NULL DEFAULT '0',
    CRITERIA_MIN    DOUBLE NULL,
    CRITERIA_MAX    DOUBLE NULL,
    UNIT            VARCHAR(10) NULL,
    UpdateTime      DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (M_CODE, POINT_ORDER)
);

SET @function_equipment_column_exists =
(
    SELECT COUNT(*)
    FROM information_schema.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
      AND TABLE_NAME = 'db_function_data'
      AND COLUMN_NAME = 'EQUIPMENT_SERIAL_ID'
);

SET @function_equipment_column_sql = IF
(
    @function_equipment_column_exists = 0,
    'ALTER TABLE db_function_data ADD COLUMN EQUIPMENT_SERIAL_ID INT NULL AFTER LOT_NO',
    'SELECT 1'
);

PREPARE function_equipment_stmt FROM @function_equipment_column_sql;
EXECUTE function_equipment_stmt;
DEALLOCATE PREPARE function_equipment_stmt;
