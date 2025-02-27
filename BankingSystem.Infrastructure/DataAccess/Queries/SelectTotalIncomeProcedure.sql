CREATE PROCEDURE SelectTotalIncome @fromDate DATETIME, @toDate DATETIME, @email NVARCHAR(255)
AS
BEGIN
	SELECT ct.[Type] AS Currency, 
			SUM(td.Amount) AS Income
	FROM TransactionDetails td
	JOIN Account a ON a.Id = td.ToAccountId  
	JOIN CurrencyType ct ON ct.Id = td.CurrencyId
	JOIN Person p ON p.Id = a.PersonId
	WHERE td.IsATM = 0 AND td.FromAccountId != td.ToAccountId 	
		AND td.PerformedAt >= @fromDate AND td.PerformedAt <=  @toDate
		AND p.Email = @email
	GROUP BY ct.[Type]
END;