-- ============================================================================
-- ตรวจว่า APPEARANCE_ID ผูก db_appearance_pending -> db_appearance_data ถูกต้อง
-- ============================================================================
-- ใช้หลังแก้ให้ผูกด้วย LAST_INSERT_ID() แทน GetLatestAppearDataId
--   QAdataSQL.InsertAppearDataWithPending  : INSERT -> SET @appear_id -> INSERT pending
--
-- ทุกบล็อกต้องได้ 0 แถว ถ้ามีแถวโผล่ = ยังผูกผิดอยู่
-- อ่านอย่างเดียว รันซ้ำได้ ไม่แก้ข้อมูล
-- ============================================================================

USE `_test_qa_system_tanawut`;

SET @report_like = 'QA26-95%';


-- ----------------------------------------------------------------------------
-- 1) pending ที่ APPEARANCE_ID เป็น NULL  (ต้องไม่มี)
-- ----------------------------------------------------------------------------
SELECT '1. APPEARANCE_ID เป็น NULL' AS check_name, p.*
FROM `db_appearance_pending` p
WHERE p.`REPORT_NO` LIKE @report_like
  AND p.`APPEARANCE_ID` IS NULL;


-- ----------------------------------------------------------------------------
-- 2) APPEARANCE_ID ชี้ไปยังแถวที่ไม่มีอยู่จริง  (ต้องไม่มี)
-- ----------------------------------------------------------------------------
SELECT '2. ชี้ไปแถวที่ไม่มีอยู่' AS check_name, p.`APPEARANCE_PENDING_ID`, p.`APPEARANCE_ID`, p.`REPORT_NO`
FROM `db_appearance_pending` p
LEFT JOIN `db_appearance_data` a ON a.`APPEARANCE_ID` = p.`APPEARANCE_ID`
WHERE p.`REPORT_NO` LIKE @report_like
  AND a.`APPEARANCE_ID` IS NULL;


-- ----------------------------------------------------------------------------
-- 3) ผูกข้ามแถว : ชี้ไปแถวที่ REPORT_NO / BATCH / COUNT / LOT_NO ไม่ตรงกัน
--    นี่คืออาการของบั๊กเดิมโดยตรง  (ต้องไม่มี)
-- ----------------------------------------------------------------------------
SELECT '3. ผูกข้ามแถว' AS check_name,
       p.`APPEARANCE_PENDING_ID`, p.`APPEARANCE_ID`,
       p.`REPORT_NO` AS pend_report, a.`REPORT_NO` AS data_report,
       p.`BATCH`     AS pend_batch,  a.`BATCH`     AS data_batch,
       p.`COUNT`     AS pend_count,  a.`COUNT`     AS data_count,
       p.`LOT_NO`    AS pend_lot,    a.`LOT_NO`    AS data_lot,
       a.`EMP_ID`, a.`APPEARANCE_DATE`
FROM `db_appearance_pending` p
JOIN `db_appearance_data` a ON a.`APPEARANCE_ID` = p.`APPEARANCE_ID`
WHERE p.`REPORT_NO` LIKE @report_like
  AND (p.`REPORT_NO` <> a.`REPORT_NO`
    OR p.`BATCH`     <> a.`BATCH`
    OR p.`COUNT`     <> a.`COUNT`
    OR COALESCE(p.`LOT_NO`, '') <> COALESCE(a.`LOT_NO`, ''));


-- ----------------------------------------------------------------------------
-- 4) ยอด NG ไม่ตรงกัน : SUM(pending.QTY_NG) ต้องเท่ากับ data.QTY_NG
--    เช็คก่อน admin review เท่านั้น (RESULT IS NULL ทุกแถว)
-- ----------------------------------------------------------------------------
SELECT '4. ยอด NG ไม่ตรง' AS check_name,
       a.`APPEARANCE_ID`, a.`REPORT_NO`, a.`BATCH`, a.`COUNT`,
       a.`QTY_NG` AS data_ng, SUM(p.`QTY_NG`) AS pending_ng, COUNT(*) AS ng_rows
FROM `db_appearance_data` a
JOIN `db_appearance_pending` p ON p.`APPEARANCE_ID` = a.`APPEARANCE_ID`
WHERE a.`REPORT_NO` LIKE @report_like
  AND a.`INUSE` = 1
GROUP BY a.`APPEARANCE_ID`, a.`REPORT_NO`, a.`BATCH`, a.`COUNT`, a.`QTY_NG`
HAVING SUM(COALESCE(p.`RESULT`, -1) = -1) = COUNT(*)
   AND a.`QTY_NG` <> SUM(p.`QTY_NG`);


-- ----------------------------------------------------------------------------
-- 5) แถวหลักบอกว่ามี NG แต่ไม่มีรายละเอียดสักแถว
--    อาการของกรณีที่แถวหลัก commit แล้วแต่ pending ล้ม  (ต้องไม่มี)
-- ----------------------------------------------------------------------------
SELECT '5. มี NG แต่ไม่มีรายละเอียด' AS check_name,
       a.`APPEARANCE_ID`, a.`REPORT_NO`, a.`BATCH`, a.`COUNT`, a.`QTY_NG`, a.`EMP_ID`
FROM `db_appearance_data` a
WHERE a.`REPORT_NO` LIKE @report_like
  AND a.`INUSE` = 1
  AND COALESCE(a.`QTY_NG`, 0) > 0
  AND NOT EXISTS (SELECT 1 FROM `db_appearance_pending` p
                  WHERE p.`APPEARANCE_ID` = a.`APPEARANCE_ID`);


-- ----------------------------------------------------------------------------
-- 6) ภาพรวมสิ่งที่บันทึกไปแล้ว — ไว้ตาดูว่าตรงกับที่กดจริงไหม
-- ----------------------------------------------------------------------------
SELECT a.`APPEARANCE_ID` AS aid, a.`REPORT_NO`, a.`BATCH`, a.`COUNT`, a.`LOT_NO`,
       a.`QTY_SELECT`, a.`QTY_OK`, a.`QTY_NG`, a.`JUDGE`,
       a.`EMP_ID`, a.`APPEARANCE_DATE`, a.`UPDATETIME`,
       (SELECT COUNT(*) FROM `db_appearance_pending` p
        WHERE p.`APPEARANCE_ID` = a.`APPEARANCE_ID`) AS ng_rows,
       (SELECT GROUP_CONCAT(CONCAT(p.`NG_COUNT`, ':', p.`QTY_NG`) ORDER BY p.`NG_COUNT`)
        FROM `db_appearance_pending` p
        WHERE p.`APPEARANCE_ID` = a.`APPEARANCE_ID`) AS ng_detail
FROM `db_appearance_data` a
WHERE a.`REPORT_NO` LIKE @report_like
ORDER BY a.`REPORT_NO`, a.`BATCH`, a.`COUNT`, a.`APPEARANCE_ID`;
