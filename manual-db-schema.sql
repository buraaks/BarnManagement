CREATE TABLE IF NOT EXISTS `Users` (
  `Id` char(36) COLLATE ascii_general_ci NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Username` varchar(100) NOT NULL,
  `PasswordHash` longblob NOT NULL,
  `Balance` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ__Users__A9D1053481A44601` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Farms` (
  `Id` char(36) COLLATE ascii_general_ci NOT NULL,
  `Name` varchar(255) NOT NULL,
  `OwnerId` char(36) COLLATE ascii_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Farms_OwnerId` (`OwnerId`),
  CONSTRAINT `FK_Farms_Users`
    FOREIGN KEY (`OwnerId`) REFERENCES `Users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Animals` (
  `Id` char(36) COLLATE ascii_general_ci NOT NULL,
  `FarmId` char(36) COLLATE ascii_general_ci NOT NULL,
  `Species` varchar(100) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `BirthDate` datetime(6) NOT NULL,
  `LifeSpanDays` int NOT NULL,
  `ProductionInterval` int NOT NULL,
  `NextProductionAt` datetime(6) DEFAULT NULL,
  `PurchasePrice` decimal(18,2) NOT NULL,
  `SellPrice` decimal(18,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Animals_FarmId` (`FarmId`),
  CONSTRAINT `FK_Animals_Farms`
    FOREIGN KEY (`FarmId`) REFERENCES `Farms` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `Products` (
  `Id` char(36) COLLATE ascii_general_ci NOT NULL,
  `FarmId` char(36) COLLATE ascii_general_ci NOT NULL,
  `ProductType` varchar(100) NOT NULL,
  `Quantity` int NOT NULL,
  `SalePrice` decimal(18,2) NOT NULL,
  `ProducedAt` datetime(6) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_FarmId` (`FarmId`),
  CONSTRAINT `FK_Products_Farms`
    FOREIGN KEY (`FarmId`) REFERENCES `Farms` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
