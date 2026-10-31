using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniSupermarket.API.Data;

#nullable disable

namespace MiniSupermarket.API.Migrations
{
    [DbContext(typeof(SupermarketDbContext))]
    [Migration("20261029210000_InstallRevenueReportingProcedures")]
    public partial class InstallRevenueReportingProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
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
END;");

            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE dbo.sp_RevenueSeries
    @StartAt datetime2,
    @EndAt datetime2,
    @Period nvarchar(10)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT period.PeriodStart, SUM(o.Total) AS Revenue
    FROM dbo.Orders AS o
    CROSS APPLY (VALUES (
        CASE LOWER(@Period)
            WHEN N'week' THEN DATEADD(day,
                -(DATEDIFF(day, CONVERT(date, '19000101'), CONVERT(date, o.CreatedAt)) % 7),
                CONVERT(date, o.CreatedAt))
            WHEN N'month' THEN DATEFROMPARTS(YEAR(o.CreatedAt), MONTH(o.CreatedAt), 1)
            ELSE CONVERT(date, o.CreatedAt)
        END
    )) AS period(PeriodStart)
    WHERE o.Status = N'PAID'
      AND o.CreatedAt >= @StartAt
      AND o.CreatedAt < @EndAt
    GROUP BY period.PeriodStart
    ORDER BY period.PeriodStart;
END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_RevenueByShift;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_RevenueSeries;");
        }
    }
}
