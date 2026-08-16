-- ============================================================
-- STEP 1: Add new path columns (safe — does NOT remove BLOBs)
-- ============================================================
ALTER TABLE ecc_dof_wukrostmarycollege.student_profile
    ADD COLUMN photo_path       VARCHAR(500) NULL AFTER photo,
    ADD COLUMN attachment_path  VARCHAR(500) NULL AFTER attachment;

-- ============================================================
-- STEP 2 (run AFTER app migration copies files):
--   DROP the BLOB columns — only run this after verifying paths are set
-- ============================================================
-- ALTER TABLE ecc_dof_wukrostmarycollege.student_profile
--     DROP COLUMN photo,
--     DROP COLUMN attachment;
