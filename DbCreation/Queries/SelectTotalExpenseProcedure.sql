CREATE PROCEDURE SelectTotalExpense @email NVARCHAR(255), @fromDate DateTime, @toDate DateTime
AS
BEGIN
    SELECT a.Currency, SUM(td.BankProfit + td.Amount) AS Expense
        FROM TransactionDetails AS td 
        JOIN Account AS a ON td.FromAccountId = a.Id
        JOIN Person AS p ON p.Id = a.PersonId 
        WHERE p.Email = @email 
		AND ((a.PersonId != (Select PersonId from Account WHERE Id = td.ToAccountId)
        AND  td.FromAccountId != td.ToAccountId) OR IsATM = 1)
        AND td.PerformedAt >= @fromDate AND td.PerformedAt <= @toDate
        GROUP BY a.Currency
END