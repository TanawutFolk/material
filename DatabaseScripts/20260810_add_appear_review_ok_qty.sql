-- ============================================================================
-- Appearance Pending Review : เปลี่ยนจาก OK/NG ทั้งก้อน -> ระบุจำนวนได้
-- ============================================================================
-- ปัญหาเดิม : db_appearance_pending 1 แถว = อาการ NG 1 อย่าง ถือ QTY_NG เป็นก้อน
--             (เช่น 3 ชิ้น) แต่ RESULT เป็น tinyint ค่าเดียว
--             -> admin ตัดสินได้แค่ "OK ทั้ง 3" หรือ "NG ทั้ง 3"
--             -> เคสจริงที่ต้องการ "2 OK 1 NG" เก็บไม่ได้
--
-- ของใหม่   : เพิ่ม REVIEW_OK_QTY = จำนวนชิ้นที่ admin ตัดสินว่า OK จริง (0..QTY_NG)
--             จำนวน NG ที่เหลือ derive เอา = QTY_NG - REVIEW_OK_QTY
--             (เก็บค่าเดียว ไม่มีทางที่ 2 ค่าจะขัดกันเอง)
--
-- RESULT    : *** คงไว้ ความหมายเดิมไม่เปลี่ยน *** เพราะมี 4 query ใช้
--             `RESULT IS NULL` เป็นตัวชี้ว่า "ยังรอ admin ตัดสิน"
--               QAdataSQL.cs:380   CountProcessStatusPending  (badge นับ pending)
--               QAdataSQL.cs:2084  CountAppearPendingUnreviewed (กันปิดงานทั้งที่ค้าง)
--               QAdataSQL.cs:2107  SearchForAppearPending      (list หน้าเลือก)
--               QAdataSQL.cs:2148  SearchAppearPendingData     (โหลดหน้า pending)
--             นิยามหลังแก้ :  NULL = ยังไม่ review
--                             1    = OK หมดทั้งก้อน (REVIEW_OK_QTY = QTY_NG)
--                             0    = ยังมี NG เหลืออย่างน้อย 1 ชิ้น
--             -> 4 query ข้างบนไม่ต้องแก้แม้แต่ตัวเดียว
--
-- *** รันกับ DB ทดสอบก่อน แล้วค่อยขึ้น qa_system ***
-- ============================================================================

USE `_test_qa_system_tanawut`;


-- ----------------------------------------------------------------------------
-- STEP 1 : เพิ่มคอลัมน์
-- ----------------------------------------------------------------------------
ALTER TABLE `db_appearance_pending`
  ADD COLUMN `REVIEW_OK_QTY` INT NULL DEFAULT NULL
  COMMENT 'จำนวนที่ admin ตัดสินว่า OK จริง 0 ถึง QTY_NG / NG ที่เหลือ = QTY_NG ลบค่านี้'
  AFTER `RESULT`;


-- ----------------------------------------------------------------------------
-- STEP 2 : backfill แถวที่ review ไปแล้ว ให้ตรงกับความหมายใหม่
--          RESULT=1 -> OK ทั้งก้อน , RESULT=0 -> NG ทั้งก้อน
-- ----------------------------------------------------------------------------
UPDATE `db_appearance_pending`
   SET `REVIEW_OK_QTY` = CASE WHEN `RESULT` = 1 THEN COALESCE(`QTY_NG`, 0) ELSE 0 END
 WHERE `RESULT` IS NOT NULL;


-- ----------------------------------------------------------------------------
-- STEP 3 (เลือกได้ / แตะข้อมูลเก่า) : เขียนยอด review ย้อนกลับ db_appearance_data
--
--   ก่อนหน้านี้ผลการ review ไม่เคยถูกเขียนกลับ db_appearance_data เลย
--   -> ชิ้นที่ admin ตัดสินว่า OK จริง ยังถูกนับเป็น NG อยู่
--   ตั้งแต่นี้โค้ดจะเขียนกลับทุกครั้งที่ record (RecalcAppearDataFromPending)
--   บล็อกนี้คือการไล่ปรับของเก่าให้ตรงกัน — ถ้าไม่อยากแตะประวัติ ข้ามได้
--
--   สูตร (คำนวณใหม่ทั้งก้อน ไม่ใช่บวกเพิ่ม จึงรันซ้ำได้ยอดไม่เพี้ยน) :
--       REMAIN_NG = SUM(QTY_NG) - SUM(REVIEW_OK_QTY)
--       QTY_NG    = REMAIN_NG
--       QTY_OK    = QTY_SELECT - REMAIN_NG
--       JUDGE     = 1 ถ้า REMAIN_NG = 0 มิฉะนั้น 0
-- ----------------------------------------------------------------------------

-- BEFORE : ดูว่าแถวไหนจะเปลี่ยน
SELECT a.APPEARANCE_ID, a.REPORT_NO, a.QTY_SELECT,
       a.QTY_OK  AS QTY_OK_เดิม,
       a.QTY_NG  AS QTY_NG_เดิม,
       a.JUDGE   AS JUDGE_เดิม,
       SUM(COALESCE(p.QTY_NG, 0)) - SUM(COALESCE(p.REVIEW_OK_QTY, 0)) AS REMAIN_NG,
       a.QTY_SELECT - (SUM(COALESCE(p.QTY_NG, 0)) - SUM(COALESCE(p.REVIEW_OK_QTY, 0))) AS QTY_OK_ใหม่
  FROM `db_appearance_data` a
  JOIN `db_appearance_pending` p ON a.APPEARANCE_ID = p.APPEARANCE_ID
 WHERE a.INUSE = 1
   AND p.RESULT IS NOT NULL
 GROUP BY a.APPEARANCE_ID, a.REPORT_NO, a.QTY_SELECT, a.QTY_OK, a.QTY_NG, a.JUDGE
HAVING a.QTY_NG <> REMAIN_NG;

UPDATE `db_appearance_data` a
  JOIN (
        SELECT `APPEARANCE_ID`,
               SUM(COALESCE(`QTY_NG`, 0)) - SUM(COALESCE(`REVIEW_OK_QTY`, 0)) AS REMAIN_NG
          FROM `db_appearance_pending`
         WHERE `APPEARANCE_ID` IS NOT NULL
         GROUP BY `APPEARANCE_ID`
         HAVING SUM(CASE WHEN `RESULT` IS NULL THEN 1 ELSE 0 END) = 0
       ) p ON a.`APPEARANCE_ID` = p.`APPEARANCE_ID`
   SET a.`QTY_NG`     = p.REMAIN_NG,
       a.`QTY_OK`     = a.`QTY_SELECT` - p.REMAIN_NG,
       a.`JUDGE`      = CASE WHEN p.REMAIN_NG = 0 THEN 1 ELSE 0 END,
       a.`UPDATETIME` = NOW()
 WHERE a.`INUSE` = 1;


-- ----------------------------------------------------------------------------
-- ตรวจผล : ทั้ง 3 คอลัมน์ต้องขึ้น match ทุกแถว
-- ----------------------------------------------------------------------------
SELECT a.APPEARANCE_ID, a.REPORT_NO, a.QTY_SELECT, a.QTY_OK, a.QTY_NG, a.JUDGE,
       COALESCE(SUM(p.QTY_NG), 0)        AS SUM_PEND_NG,
       COALESCE(SUM(p.REVIEW_OK_QTY), 0) AS SUM_REVIEW_OK,
       CASE WHEN a.QTY_SELECT = a.QTY_OK + a.QTY_NG THEN 'match' ELSE 'MISMATCH' END AS CHK_SELECT,
       CASE WHEN a.JUDGE = (a.QTY_NG = 0)           THEN 'match' ELSE 'MISMATCH' END AS CHK_JUDGE,
       CASE WHEN SUM(CASE WHEN p.RESULT IS NULL THEN 1 ELSE 0 END) > 0 THEN 'pending อยู่'
            WHEN a.QTY_NG = COALESCE(SUM(p.QTY_NG), 0) - COALESCE(SUM(p.REVIEW_OK_QTY), 0) THEN 'match'
            ELSE 'MISMATCH' END AS CHK_NG
  FROM `db_appearance_data` a
  LEFT JOIN `db_appearance_pending` p ON a.APPEARANCE_ID = p.APPEARANCE_ID
 WHERE a.INUSE = 1
 GROUP BY a.APPEARANCE_ID, a.REPORT_NO, a.QTY_SELECT, a.QTY_OK, a.QTY_NG, a.JUDGE
 ORDER BY a.APPEARANCE_ID DESC;


-- ============================================================================
-- ROLLBACK (ถ้าต้องถอย)
-- ============================================================================
-- ALTER TABLE `db_appearance_pending` DROP COLUMN `REVIEW_OK_QTY`;
--
-- คืนค่า db_appearance_data กลับเป็น "NG ทั้งก้อนตามที่ operator บันทึก" :
-- UPDATE `db_appearance_data` a
--   JOIN (SELECT `APPEARANCE_ID`, SUM(COALESCE(`QTY_NG`,0)) AS TOTAL_NG
--           FROM `db_appearance_pending` WHERE `APPEARANCE_ID` IS NOT NULL
--          GROUP BY `APPEARANCE_ID`) p ON a.`APPEARANCE_ID` = p.`APPEARANCE_ID`
--    SET a.`QTY_NG` = p.TOTAL_NG,
--        a.`QTY_OK` = a.`QTY_SELECT` - p.TOTAL_NG,
--        a.`JUDGE`  = CASE WHEN p.TOTAL_NG = 0 THEN 1 ELSE 0 END
--  WHERE a.`INUSE` = 1;
-- ============================================================================
