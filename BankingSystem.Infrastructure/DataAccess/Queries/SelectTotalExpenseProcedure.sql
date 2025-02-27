CREATE PROCEDURE SelectTotalExpense @email NVARCHAR(255), @fromDate DateTime, @toDate DateTime
AS
BEGIN
    SELECT ct.[Type], SUM(td.BankProfit + td.Amount) AS Expense
        FROM TransactionDetails AS td 
        JOIN Account AS a ON td.FromAccountId = a.Id
        JOIN Person AS p ON p.Id = a.PersonId 
        JOIN CurrencyType AS ct ON ct.Id = td.CurrencyId
        WHERE p.Email = @email 
        AND  td.FromAccountId != td.ToAccountId 
        AND td.PerformedAt >= @fromDate AND td.PerformedAt <= @toDate
        GROUP BY ct.[Type]
END