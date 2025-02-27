CREATE PROCEDURE SelectTransactionCountLastMonth
AS
BEGIN
	WITH LastMonthDays AS (
        SELECT CAST(DATEADD(MONTH, -1, GETDATE()) AS DATE) AS DayName

        UNION ALL

        SELECT DATEADD(DAY, 1, DayName) FROM LastMonthDays
        WHERE DayName < CAST(GETDATE() AS DATE)
    )

    SELECT lm.DayName,  COUNT(td.BankProfit) FROM TransactionDetails as td 
    right join LastMonthDays as lm on lm.DayName = CAST(PerformedAt AS DATE)
    Group by lm.DayName
    HAVING lm.DayName < CAST(GETDATE() AS DATE)

END
