 CREATE VIEW BankProfitView AS 
SELECT 
    Currency,
    SUM(CASE WHEN PerformedAt > DATEADD(MONTH, -1, GETDATE()) THEN BankProfit ELSE 0 END) AS LastMonthProfit,
    SUM(CASE WHEN PerformedAt > DATEADD(MONTH, -6, GETDATE()) THEN BankProfit ELSE 0 END) AS LastSixMonthProfit,
    SUM(CASE WHEN PerformedAt > DATEADD(YEAR, -1, GETDATE()) THEN BankProfit ELSE 0 END) AS LastYearProfit
FROM TransactionDetails 
GROUP BY Currency;