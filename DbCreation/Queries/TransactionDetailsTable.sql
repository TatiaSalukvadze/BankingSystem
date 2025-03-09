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