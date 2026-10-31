CREATE OR ALTER PROCEDURE dbo.sp_RevenueByShift
    @StartAt datetime2,
    @EndAt datetime2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        COUNT(*) AS InvoiceCount,
        COALESCE(SUM(Total), 0) AS Revenue
    FROM dbo.Orders
    WHERE Status = N'PAID'
      AND CreatedAt >= @StartAt
      AND CreatedAt < @EndAt;
END;
GO
