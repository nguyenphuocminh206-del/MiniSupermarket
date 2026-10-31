using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MiniSupermarket.API.Data;

#nullable disable

namespace MiniSupermarket.API.Migrations
{
    [DbContext(typeof(SupermarketDbContext))]
    [Migration("20261029200000_AddOrderCancellationTracking")]
    public partial class AddOrderCancellationTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PointsRedeemed",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PointsEarned",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "InventoryDeducted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                UPDATE Orders
                SET PointsRedeemed = CASE
                        WHEN OrderId > 8 AND Discount > 0 AND Discount % 1000 = 0 THEN CONVERT(int, Discount / 1000)
                        ELSE 0
                    END,
                    PointsEarned = CASE WHEN OrderId > 8 AND Status IN ('PAID', N'Đã duyệt')
                        THEN CONVERT(int, Total / 10000) ELSE 0 END,
                    InventoryDeducted = CASE WHEN OrderId > 8 AND Status IN ('PAID', N'Đã duyệt') THEN 1 ELSE 0 END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PointsRedeemed", table: "Orders");
            migrationBuilder.DropColumn(name: "PointsEarned", table: "Orders");
            migrationBuilder.DropColumn(name: "InventoryDeducted", table: "Orders");
        }
    }
}
