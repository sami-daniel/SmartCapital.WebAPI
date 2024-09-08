-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema smartcapitaldatabase
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `smartcapitaldatabase` ;

-- -----------------------------------------------------
-- Schema smartcapitaldatabase
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `smartcapitaldatabase` DEFAULT CHARACTER SET utf8 ;
SHOW WARNINGS;
USE `smartcapitaldatabase` ;

-- -----------------------------------------------------
-- Table `Users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Users` ;

SHOW WARNINGS;
CREATE TABLE IF NOT EXISTS `Users` (
  `UserID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `UserName` VARCHAR(255) NOT NULL,
  `UserPassword` CHAR(60) NOT NULL,
  `UserCreationDate` DATETIME NOT NULL,
  PRIMARY KEY (`UserID`))
ENGINE = InnoDB;

SHOW WARNINGS;
CREATE UNIQUE INDEX `UserName_UNIQUE` ON `Users` (`UserName` ASC) VISIBLE;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `Profiles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Profiles` ;

SHOW WARNINGS;
CREATE TABLE IF NOT EXISTS `Profiles` (
  `ProfileID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `ProfileCreationDate` DATETIME NOT NULL,
  `ProfileName` VARCHAR(255) NOT NULL,
  `ProfileOpeningBalance` DECIMAL(12,2) NULL,
  `Users_UserID` INT UNSIGNED NOT NULL,
  PRIMARY KEY (`ProfileID`, `Users_UserID`),
  CONSTRAINT `fk_Profiles_Users1`
    FOREIGN KEY (`Users_UserID`)
    REFERENCES `Users` (`UserID`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SHOW WARNINGS;
CREATE INDEX `fk_Profiles_Users1_idx` ON `Profiles` (`Users_UserID` ASC) VISIBLE;

SHOW WARNINGS;
CREATE UNIQUE INDEX `ProfileID_UNIQUE` ON `Profiles` (`ProfileID` ASC) VISIBLE;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `Categories`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Categories` ;

SHOW WARNINGS;
CREATE TABLE IF NOT EXISTS `Categories` (
  `CategoryID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `CategoryName` VARCHAR(255) NOT NULL,
  `CategoryCreationDate` DATETIME NOT NULL,
  `CategoryDescription` MEDIUMTEXT NULL,
  PRIMARY KEY (`CategoryID`))
ENGINE = InnoDB;

SHOW WARNINGS;
CREATE UNIQUE INDEX `CategoryName_UNIQUE` ON `Categories` (`CategoryName` ASC) VISIBLE;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `Incomes`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Incomes` ;

SHOW WARNINGS;
CREATE TABLE IF NOT EXISTS `Incomes` (
  `IncomeID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `IncomeDate` DATE NOT NULL,
  `IncomeTitle` VARCHAR(255) NOT NULL,
  `IncomeAmount` DECIMAL(12,2) NOT NULL,
  `IncomeCreationDate` DATETIME NOT NULL,
  `IncomeDescription` MEDIUMTEXT NULL,
  `ProfileID` INT UNSIGNED NOT NULL,
  `CategoryID` INT UNSIGNED NULL,
  PRIMARY KEY (`IncomeID`, `ProfileID`),
  CONSTRAINT `fk_Incomes_Profiles`
    FOREIGN KEY (`ProfileID`)
    REFERENCES `Profiles` (`ProfileID`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Incomes_Category`
    FOREIGN KEY (`CategoryID`)
    REFERENCES `Categories` (`CategoryID`)
    ON DELETE SET NULL
    ON UPDATE NO ACTION)
ENGINE = InnoDB
KEY_BLOCK_SIZE = 4;

SHOW WARNINGS;
CREATE INDEX `fk_Incomes_Category1_idx` ON `Incomes` (`CategoryID` ASC) VISIBLE;

SHOW WARNINGS;
CREATE INDEX `fk_Incomes_Profiles_idx` ON `Incomes` (`ProfileID` ASC) VISIBLE;

SHOW WARNINGS;

-- -----------------------------------------------------
-- Table `Expenses`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `Expenses` ;

SHOW WARNINGS;
CREATE TABLE IF NOT EXISTS `Expenses` (
  `ExpenseID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `ExpenseDate` DATE NOT NULL,
  `ExpenseTitle` VARCHAR(255) NOT NULL,
  `ExpenseAmount` DECIMAL(12,2) NOT NULL,
  `ExpenseCreationDate` DATETIME NOT NULL,
  `ExpenseDescription` MEDIUMTEXT NULL,
  `ProfileID` INT UNSIGNED NOT NULL,
  `CategoryID` INT UNSIGNED NULL,
  PRIMARY KEY (`ExpenseID`, `ProfileID`),
  CONSTRAINT `fk_Expenses_Profiles`
    FOREIGN KEY (`ProfileID`)
    REFERENCES `Profiles` (`ProfileID`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Expenses_Category`
    FOREIGN KEY (`CategoryID`)
    REFERENCES `Categories` (`CategoryID`)
    ON DELETE SET NULL
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

SHOW WARNINGS;
CREATE INDEX `fk_Expenses_Profiles1_idx` ON `Expenses` (`ProfileID` ASC) VISIBLE;

SHOW WARNINGS;
CREATE INDEX `fk_Expenses_Category1_idx` ON `Expenses` (`CategoryID` ASC) VISIBLE;

SHOW WARNINGS;
USE `smartcapitaldatabase` ;

-- -----------------------------------------------------
-- View `Savings_Result`
-- -----------------------------------------------------
DROP VIEW IF EXISTS `Savings_Result` ;
SHOW WARNINGS;
USE `smartcapitaldatabase`;
CREATE  OR REPLACE VIEW `Savings_Result` AS
    SELECT 
        p.ProfileID,
        p.ProfileName,
        SUM(IncomeAmount) TotalIncome,
        SUM(ExpenseAmount) TotalExpense,
        SUM(IncomeAmount) - SUM(ExpenseAmount) TotalEconomy
    FROM
        Profiles p
            JOIN
        Incomes USING (ProfileID)
            JOIN
        Expenses USING (ProfileID)
    GROUP BY p.ProfileID;
SHOW WARNINGS;
USE `smartcapitaldatabase`;

DELIMITER $$

USE `smartcapitaldatabase`$$
DROP TRIGGER IF EXISTS `Users_BEFORE_INSERT` $$
SHOW WARNINGS$$
USE `smartcapitaldatabase`$$
CREATE DEFINER = CURRENT_USER TRIGGER `smartcapitaldatabase`.`Users_BEFORE_INSERT` BEFORE INSERT ON `Users` FOR EACH ROW
BEGIN
	SET NEW.UserCreationDate = NOW();
END$$

SHOW WARNINGS$$

USE `smartcapitaldatabase`$$
DROP TRIGGER IF EXISTS `Profiles_BEFORE_INSERT` $$
SHOW WARNINGS$$
USE `smartcapitaldatabase`$$
CREATE DEFINER = CURRENT_USER TRIGGER `smartcapitaldatabase`.`Profiles_BEFORE_INSERT` BEFORE INSERT ON `Profiles` FOR EACH ROW
BEGIN
	SET NEW.ProfileCreationDate = NOW();
END$$

SHOW WARNINGS$$

USE `smartcapitaldatabase`$$
DROP TRIGGER IF EXISTS `Categories_BEFORE_INSERT_WRONG_SCHEMA` $$
SHOW WARNINGS$$
USE `smartcapitaldatabase`$$
CREATE DEFINER = CURRENT_USER TRIGGER `smartcapitaldatabase`.`Categories_BEFORE_INSERT` BEFORE INSERT ON `Categories` FOR EACH ROW
BEGIN
	SET NEW.CategoryCreationDate = NOW();
END$$

SHOW WARNINGS$$

USE `smartcapitaldatabase`$$
DROP TRIGGER IF EXISTS `Incomes_BEFORE_INSERT` $$
SHOW WARNINGS$$
USE `smartcapitaldatabase`$$
CREATE DEFINER = CURRENT_USER TRIGGER `smartcapitaldatabase`.`Incomes_BEFORE_INSERT` BEFORE INSERT ON `Incomes` FOR EACH ROW
BEGIN
	SET NEW.IncomeCreationDate = NOW();
END$$

SHOW WARNINGS$$

USE `smartcapitaldatabase`$$
DROP TRIGGER IF EXISTS `Expenses_BEFORE_INSERT` $$
SHOW WARNINGS$$
USE `smartcapitaldatabase`$$
CREATE DEFINER = CURRENT_USER TRIGGER `smartcapitaldatabase`.`Expenses_BEFORE_INSERT` BEFORE INSERT ON `Expenses` FOR EACH ROW
BEGIN
	SET NEW.ExpenseCreationDate = NOW();
END$$

SHOW WARNINGS$$

DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
