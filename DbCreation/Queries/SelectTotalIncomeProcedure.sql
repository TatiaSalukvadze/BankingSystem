CREATE PROCEDURE SelectTotalIncome @fromDate DATETIME, @toDate DATETIME, @email NVARCHAR(255)
AS
BEGIN
	SELECT a.Currency, 
			SUM(td.Amount) AS Income
	FROM TransactionDetails td
	JOIN Account a ON a.Id = td.ToAccountId
	JOIN Person p ON p.Id = a.PersonId
	WHERE td.IsATM = 0 AND td.FromAccountId != td.ToAccountId 	
		AND a.PersonId != (Select PersonId from Account WHERE Id = td.FromAccountId) 
		AND td.PerformedAt >= @fromDate AND td.PerformedAt <=  @toDate
		AND p.Email = @email
	GROUP BY a.Currency
END;