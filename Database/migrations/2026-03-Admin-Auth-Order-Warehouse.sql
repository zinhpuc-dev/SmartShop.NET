-- REVIEW ONLY: do not run until an approved backup exists and the migration is approved.
-- This is additive. Existing rows and 190 sample records are not deleted or rewritten.

ALTER TABLE `DON_HANG`
	ADD COLUMN `MaKho` CHAR(6) NULL,
	ADD INDEX `IX_DON_HANG_MaKho` (`MaKho`),
	ADD CONSTRAINT `FK_DON_HANG_KHO` FOREIGN KEY (`MaKho`) REFERENCES `KHO` (`MaKho`);

-- Impact:
-- * Adds nullable order warehouse allocation; existing orders stay NULL and must be assigned
--   before an Admin may complete a stock-impacting transition.
--
-- Rollback (only after backup and if allocation data does not need to be retained):
-- ALTER TABLE `DON_HANG` DROP FOREIGN KEY `FK_DON_HANG_KHO`,
--     DROP INDEX `IX_DON_HANG_MaKho`, DROP COLUMN `MaKho`;
