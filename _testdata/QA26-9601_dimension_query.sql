-- Dimension values recorded for QA26-9601
SELECT d.`SAMPLING_NO`,
       d.`POINT_ORDER`,
       d.`VALUE`,
       m.`CRITERIA_MIN`,
       m.`CRITERIA_MAX`,
       d.`JUDGE` AS `POINT_JUDGE`,
       s.`EQUIPMENT_SERIAL`
FROM `db_dimension_data` d
JOIN `db_receive_mat` r
  ON r.`Report_No` = d.`REPORT_NO`
JOIN `info_dimension_equipment` m
  ON m.`M_CODE` = r.`M_Code`
 AND m.`POINT_ORDER` = d.`POINT_ORDER`
LEFT JOIN `info_equipment_serial` s
  ON s.`ID` = d.`EQUIPMENT_SERIAL_ID`
WHERE d.`REPORT_NO` = 'QA26-9601'
  AND d.`INUSE` = 1
ORDER BY d.`SAMPLING_NO`, d.`POINT_ORDER`;

-- Recalculate MAX-MIN from the 9 measured points and compare it with the saved result.
SELECT d.`SAMPLING_NO`,
       MIN(d.`VALUE`) AS `MIN_VALUE`,
       MAX(d.`VALUE`) AS `MAX_VALUE`,
       ROUND(MAX(d.`VALUE`) - MIN(d.`VALUE`), 4) AS `CALCULATED_DIFF`,
       p.`DIFFERENCE` AS `SAVED_DIFF`,
       p.`TOLERANCE`,
       IF(MAX(d.`VALUE`) - MIN(d.`VALUE`) > p.`TOLERANCE`, 'NG', 'OK') AS `CALCULATED_JUDGE`,
       IF(p.`JUDGE` = 1, 'OK', 'NG') AS `SAVED_JUDGE`
FROM `db_dimension_data` d
JOIN `db_dimension_piece_judge` p
  ON p.`REPORT_NO` = d.`REPORT_NO`
 AND p.`SAMPLING_NO` = d.`SAMPLING_NO`
 AND p.`INUSE` = 1
WHERE d.`REPORT_NO` = 'QA26-9601'
  AND d.`INUSE` = 1
GROUP BY d.`SAMPLING_NO`, p.`DIFFERENCE`, p.`TOLERANCE`, p.`JUDGE`
ORDER BY d.`SAMPLING_NO`;

SELECT `Report_No`,
       `Dimension_Check`,
       `Dimension_Check_Lot_No`,
       `Report_Status`
FROM `db_report_status`
WHERE `Report_No` = 'QA26-9601';
