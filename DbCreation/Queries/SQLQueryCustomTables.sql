﻿Use BankingSystem
Go

CREATE TABLE Person (
    Id INT IDENTITY(1,1) PRIMARY KEY,  
    IdentityUserId NVARCHAR(450) NOT NULL UNIQUE, 
    [Name] NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
	IDNumber CHAR(11) NOT NULL UNIQUE,
	Birthdate Date NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE(),
	CONSTRAINT IdNumberCheck CHECK(PATINDEX('%[^0-9]%', IDNumber) = 0 ),
	CONSTRAINT EmailFormatCheck CHECK(Email LIKE '%_@_%._%'),
    FOREIGN KEY (IdentityUserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

GO
CREATE TABLE Account(
	Id INT IDENTITY(1,1) PRIMARY KEY,  
    PersonId INT NOT NULL,
	IBAN CHAR(22) NOT NULL UNIQUE,
	Amount DECIMAL(18,2) NOT NULL,
	Currency CHAR(3) NOT NULL,
	CONSTRAINT IBANFormatCheck CHECK(PATINDEX('GE[0-9][0-9]CD%', IBAN) = 1),
	CONSTRAINT CurrencyCheck CHECK(Currency IN('GEL','USD','EUR')),
	FOREIGN KEY (PersonId) REFERENCES Person(Id) ON DELETE CASCADE
);

GO
CREATE TABLE Card(
	Id INT IDENTITY(1,1) PRIMARY KEY,  
    AccountId INT NOT NULL,
	CardNumber NVARCHAR(100) NOT NULL UNIQUE,
	ExpirationDate CHAR(5) NOT NULL,
	CVV NVARCHAR(100) NOT NULL,
	PIN NVARCHAR(100) NOT NULL,
	CONSTRAINT ExpirationDateCheck CHECK(PATINDEX('[0-9][0-9]/[0-9][0-9]', ExpirationDate) = 1),
	FOREIGN KEY (AccountId) REFERENCES Account(Id) ON DELETE CASCADE
);

GO
CREATE TABLE TransactionDetails(
	Id INT IDENTITY(1,1) PRIMARY KEY,  
	BankProfit DECIMAL(18,2) NOT NULL,
	Amount DECIMAL(18,2) NOT NULL,
	FromAccountId INT NOT NULL,
	ToAccountId INT NOT NULL,
	Currency CHAR(3) NOT NULL,	
	IsATM BIT NOT NULL DEFAULT 0,
	PerformedAt DATETIME DEFAULT GETDATE(),
	CONSTRAINT BankProfitCheck CHECK(BankProfit >= 0 ),
	CONSTRAINT AmountCheck CHECK(Amount >= 0 ),
	CONSTRAINT TransactionCurrencyCheck CHECK(Currency IN('GEL','USD','EUR')),
	FOREIGN KEY (FromAccountId) REFERENCES  Account(Id),
	FOREIGN KEY (ToAccountId) REFERENCES  Account(Id)
);

GO
CREATE TABLE RefreshToken (
    Id INT IDENTITY(1,1) PRIMARY KEY,  
    IdentityUserId NVARCHAR(450) NOT NULL, 
    Token NVARCHAR(450) NOT NULL UNIQUE,
    ExpirationDate DATETIME NOT NULL,
	DeviceId NVARCHAR(450) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (IdentityUserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);





