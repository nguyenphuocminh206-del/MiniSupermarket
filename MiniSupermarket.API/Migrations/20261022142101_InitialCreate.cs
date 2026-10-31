    using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniSupermarket.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.BrandId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RewardPoints = table.Column<int>(type: "int", nullable: false),
                    MembershipRank = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "BrandId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CashierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                    table.ForeignKey(
                        name: "FK_Orders_Users_CashierId",
                        column: x => x.CashierId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "BrandId", "BrandName", "Description", "LogoUrl" },
                values: new object[,]
                {
                    { 1, "Vinamilk", "Sữa và các sản phẩm từ sữa hàng đầu Việt Nam", null },
                    { 2, "TH true MILK", "Sữa tươi sạch và các sản phẩm từ sữa tươi", null },
                    { 3, "Coca-Cola", "Nước giải khát có ga Coca-Cola, Sprite, Fanta", null },
                    { 4, "PepsiCo", "Nước giải khát Pepsi, Aquafina, trà Ô Long Tea Plus", null },
                    { 5, "Masan Consumer", "Gia vị Chin-su, Nam Ngư và mì Omachi", null },
                    { 6, "Acecook", "Mì ăn liền Hảo Hảo, Modern, Lẩu Thái", null },
                    { 7, "Unilever", "Hóa mỹ phẩm và đồ gia dụng OMO, Sunlight, Clear, P/S", null },
                    { 8, "Nestlé", "Cà phê Nescafé, Milo và các sản phẩm dinh dưỡng", null },
                    { 9, "Orion", "Bánh Chocopie, Custas và các loại bánh snack Hàn Quốc", null },
                    { 10, "Kinh Đô", "Bánh quy, bánh kẹo truyền thống Việt Nam", null },
                    { 11, "Vissan", "Thịt chế biến, xúc xích, thực phẩm đông lạnh", null },
                    { 12, "Heineken Việt Nam", "Bia Heineken, Tiger, Bia Việt", null },
                    { 13, "SunMart", "Nhãn hàng riêng của siêu thị mini SunMart 24/7", null },
                    { 14, "Thiên Long", "Bút viết và văn phòng phẩm Việt Nam", null }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "Description" },
                values: new object[,]
                {
                    { 1, "Sữa & Sản phẩm từ sữa", "Sữa tươi, sữa chua, sữa đặc, phô mai" },
                    { 2, "Đồ uống không cồn", "Nước ngọt, nước suối, trà đóng chai, cà phê hòa tan" },
                    { 3, "Bia & Đồ uống có cồn", "Bia lon, bia chai, rượu (bán cho khách đủ 18 tuổi)" },
                    { 4, "Mì & Thực phẩm ăn liền", "Mì gói, mì ly, phở, cháo ăn liền" },
                    { 5, "Gia vị & Nhu yếu phẩm", "Nước mắm, nước tương, tương ớt, muối, đường, dầu ăn" },
                    { 6, "Bánh kẹo & Snack", "Bánh quy, bánh bông lan, snack, kẹo" },
                    { 7, "Thịt, Cá & Đông lạnh", "Thịt tươi, xúc xích, cá viên, thực phẩm đông lạnh" },
                    { 8, "Rau củ quả & Trứng tươi", "Rau xanh, củ quả, trái cây, trứng gà" },
                    { 9, "Gạo & Đồ khô", "Gạo, nếp, đậu, ngũ cốc, bột" },
                    { 10, "Chăm sóc cá nhân", "Dầu gội, sữa tắm, kem đánh răng, khăn giấy" },
                    { 11, "Đồ dùng gia đình & Vệ sinh", "Nước giặt, nước rửa chén, giấy vệ sinh, túi rác" },
                    { 12, "Văn phòng phẩm & Tiện ích", "Bút, vở, pin, bật lửa và các vật dụng tiện ích" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "CustomerName", "MembershipRank", "PhoneNumber", "RewardPoints" },
                values: new object[,]
                {
                    { 1, "25 Nguyễn Huệ, Quận 1, TP.HCM", "Nguyễn Văn A", "Vàng", "0901122334", 1500 },
                    { 2, "118 Võ Văn Tần, Quận 3, TP.HCM", "Trần Thị B", "Bạc", "0918877665", 500 },
                    { 3, "72 Nguyễn Trãi, Quận 5, TP.HCM", "Lê Văn C", "Chuẩn", "0983344556", 100 },
                    { 4, "156 Thành Thái, Quận 10, TP.HCM", "Phạm Thị D", "Vàng", "0905678123", 2300 },
                    { 5, "43 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM", "Hoàng Văn E", "Bạc", "0912345678", 800 },
                    { 6, "89 Phạm Văn Đồng, Quận Gò Vấp, TP.HCM", "Võ Thị F", "Kim Cương", "0987654321", 3200 },
                    { 7, "215 Cộng Hòa, Quận Tân Bình, TP.HCM", "Đặng Văn G", "Chuẩn", "0909876543", 250 },
                    { 8, "36 Phan Đình Phùng, Quận Phú Nhuận, TP.HCM", "Bùi Thị H", "Bạc", "0913456789", 1200 },
                    { 9, "102 Nguyễn Thị Thập, Quận 7, TP.HCM", "Đỗ Văn I", "Kim Cương", "0981234567", 4500 },
                    { 10, "68 Hậu Giang, Quận 6, TP.HCM", "Nguyễn Thị K", "Chuẩn", "0903456789", 150 },
                    { 11, "145 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", "Trương Văn L", "Vàng", "0915678901", 1800 },
                    { 12, "234 Lê Văn Khương, Quận 12, TP.HCM", "Phan Thị M", "Bạc", "0986789012", 700 },
                    { 13, "57 Lũy Bán Bích, Quận Tân Phú, TP.HCM", "Lý Văn N", "Kim Cương", "0907890123", 5000 },
                    { 14, "321 Tỉnh Lộ 10, Quận Bình Tân, TP.HCM", "Huỳnh Thị P", "Chuẩn", "0918901234", 350 },
                    { 15, "78 Nguyễn Hữu Trí, Huyện Bình Chánh, TP.HCM", "Mai Văn Q", "Vàng", "0989012345", 2600 }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[,]
                {
                    { 1, "ADMIN" },
                    { 2, "WAREHOUSE" },
                    { 3, "CASHIER" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Barcode", "BrandId", "CategoryId", "Price", "ProductName", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "SMT000001", 1, 1, 32000m, "Sữa tươi tiệt trùng Vinamilk 100% có đường 1L", 120 },
                    { 2, "SMT000002", 2, 1, 36000m, "Sữa tươi TH true MILK ít đường 180ml (lốc 4 hộp)", 90 },
                    { 3, "SMT000003", 1, 1, 28000m, "Sữa chua ăn Vinamilk có đường (lốc 4 hộp)", 100 },
                    { 4, "SMT000004", 3, 2, 10000m, "Nước ngọt Coca-Cola lon 330ml", 300 },
                    { 5, "SMT000005", 4, 2, 18000m, "Nước ngọt Pepsi chai 1.5L", 150 },
                    { 6, "SMT000006", 4, 2, 5000m, "Nước tinh khiết Aquafina chai 500ml", 400 },
                    { 7, "SMT000007", 4, 2, 12000m, "Trà Ô Long Tea Plus chai 455ml", 200 },
                    { 8, "SMT000008", 12, 3, 20000m, "Bia Heineken lon 330ml", 240 },
                    { 9, "SMT000009", 12, 3, 17000m, "Bia Tiger lon 330ml", 240 },
                    { 10, "SMT000010", 6, 4, 4500m, "Mì Hảo Hảo tôm chua cay gói 75g", 500 },
                    { 11, "SMT000011", 6, 4, 11000m, "Mì ly Modern lẩu Thái tôm 65g", 200 },
                    { 12, "SMT000012", 5, 4, 8500m, "Mì Omachi sườn hầm ngũ quả gói 80g", 300 },
                    { 13, "SMT000013", 5, 5, 22000m, "Nước tương Chin-su chai 500ml", 150 },
                    { 14, "SMT000014", 5, 5, 38000m, "Nước mắm Nam Ngư chai 500ml", 120 },
                    { 15, "SMT000015", 5, 5, 15000m, "Tương ớt Chin-su chai 250g", 130 },
                    { 16, "SMT000016", 13, 5, 8000m, "Muối i-ốt SunMart gói 500g", 100 },
                    { 17, "SMT000017", 9, 6, 58000m, "Bánh Chocopie Orion hộp 12 cái", 80 },
                    { 18, "SMT000018", 9, 6, 52000m, "Bánh Custas Orion hộp 12 cái", 70 },
                    { 19, "SMT000019", 10, 6, 24000m, "Bánh quy AFC Kinh Đô gói 200g", 100 },
                    { 20, "SMT000020", 8, 2, 62000m, "Cà phê hòa tan Nescafé 3in1 hộp 20 gói", 90 },
                    { 21, "SMT000021", 11, 7, 32000m, "Xúc xích tiệt trùng Vissan gói 175g", 80 },
                    { 22, "SMT000022", 13, 7, 75000m, "Thịt ba rọi heo SunMart khay 500g", 40 },
                    { 23, "SMT000023", 11, 7, 55000m, "Cá viên chiên đông lạnh Vissan gói 500g", 50 },
                    { 24, "SMT000024", 13, 8, 35000m, "Trứng gà SunMart hộp 10 quả", 60 },
                    { 25, "SMT000025", 13, 8, 20000m, "Cà chua Đà Lạt khay 500g", 50 },
                    { 26, "SMT000026", 13, 8, 10000m, "Rau muống sạch bó 300g", 60 },
                    { 27, "SMT000027", 13, 9, 135000m, "Gạo ST25 SunMart túi 5kg", 60 },
                    { 28, "SMT000028", 7, 10, 95000m, "Dầu gội Clear Men sạch gàu chai 340g", 50 },
                    { 29, "SMT000029", 7, 10, 32000m, "Kem đánh răng P/S bảo vệ 123 tuýp 180g", 80 },
                    { 30, "SMT000030", 7, 11, 30000m, "Nước rửa chén Sunlight chanh chai 750g", 70 },
                    { 31, "SMT000031", 7, 11, 160000m, "Nước giặt OMO Matic cửa trước túi 3.6kg", 40 },
                    { 32, "SMT000032", 13, 11, 45000m, "Giấy vệ sinh SunMart lốc 10 cuộn", 100 },
                    { 33, "SMT000033", 14, 12, 45000m, "Bút bi Thiên Long TL-027 hộp 20 cây", 40 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FullName", "IsActive", "PasswordHash", "Phone", "RoleId", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), "admin01@sunmart.vn", "Nguyễn Văn Admin", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000001", 1, "admin01" },
                    { 2, new DateTime(2026, 1, 2, 8, 0, 0, 0, DateTimeKind.Unspecified), "admin02@sunmart.vn", "Trần Thị Admin", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000002", 1, "admin02" },
                    { 3, new DateTime(2026, 1, 3, 8, 0, 0, 0, DateTimeKind.Unspecified), "admin03@sunmart.vn", "Lê Văn Quản", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000003", 1, "admin03" },
                    { 4, new DateTime(2026, 1, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), "warehouse01@sunmart.vn", "Phạm Văn Kho", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000004", 2, "warehouse01" },
                    { 5, new DateTime(2026, 1, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), "warehouse02@sunmart.vn", "Hoàng Thị Hương", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000005", 2, "warehouse02" },
                    { 6, new DateTime(2026, 1, 6, 8, 0, 0, 0, DateTimeKind.Unspecified), "warehouse03@sunmart.vn", "Võ Minh Đức", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000006", 2, "warehouse03" },
                    { 7, new DateTime(2026, 1, 7, 8, 0, 0, 0, DateTimeKind.Unspecified), "warehouse04@sunmart.vn", "Đặng Quốc Anh", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000007", 2, "warehouse04" },
                    { 8, new DateTime(2026, 1, 8, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier01@sunmart.vn", "Nguyễn Thị Lan", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000008", 3, "cashier01" },
                    { 9, new DateTime(2026, 1, 9, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier02@sunmart.vn", "Trần Văn Nam", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000009", 3, "cashier02" },
                    { 10, new DateTime(2026, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier03@sunmart.vn", "Phan Thị Mai", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000010", 3, "cashier03" },
                    { 11, new DateTime(2026, 1, 11, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier04@sunmart.vn", "Lý Hoàng Long", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000011", 3, "cashier04" },
                    { 12, new DateTime(2026, 1, 12, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier05@sunmart.vn", "Bùi Thị Ngọc", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000012", 3, "cashier05" },
                    { 13, new DateTime(2026, 1, 13, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier06@sunmart.vn", "Đỗ Minh Tâm", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000013", 3, "cashier06" },
                    { 14, new DateTime(2026, 1, 14, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier07@sunmart.vn", "Huỳnh Văn Phúc", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000014", 3, "cashier07" },
                    { 15, new DateTime(2026, 1, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "cashier08@sunmart.vn", "Mai Thị Thu", true, "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "0901000015", 3, "cashier08" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CashierId", "CreatedAt", "CustomerId", "Discount", "OrderCode", "PaymentMethod", "Status", "Subtotal", "Total" },
                values: new object[,]
                {
                    { 1, 8, new DateTime(2026, 1, 1, 14, 30, 0, 0, DateTimeKind.Unspecified), 1, 0m, "HD00001", "CASH", "PAID", 169000m, 169000m },
                    { 2, 9, new DateTime(2026, 1, 5, 9, 15, 0, 0, DateTimeKind.Unspecified), 2, 0m, "HD00002", "CARD", "PAID", 230000m, 230000m },
                    { 3, 10, new DateTime(2026, 1, 10, 11, 0, 0, 0, DateTimeKind.Unspecified), 6, 22550m, "HD00003", "BANK_TRANSFER", "PAID", 451000m, 428450m },
                    { 4, 11, new DateTime(2026, 1, 12, 16, 45, 0, 0, DateTimeKind.Unspecified), 9, 0m, "HD00004", "CASH", "CANCELLED", 95000m, 95000m },
                    { 5, 12, new DateTime(2026, 1, 15, 8, 20, 0, 0, DateTimeKind.Unspecified), 13, 15500m, "HD00005", "MOMO", "PAID", 310000m, 294500m },
                    { 6, 13, new DateTime(2026, 1, 16, 22, 10, 0, 0, DateTimeKind.Unspecified), 3, 0m, "HD00006", "CASH", "PAID", 108500m, 108500m },
                    { 7, 14, new DateTime(2026, 1, 18, 7, 45, 0, 0, DateTimeKind.Unspecified), 4, 0m, "HD00007", "MOMO", "PAID", 192000m, 192000m },
                    { 8, 14, new DateTime(2026, 1, 20, 0, 30, 0, 0, DateTimeKind.Unspecified), 5, 0m, "HD00008", "CASH", "PAID", 110000m, 110000m }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderItemId", "LineTotal", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 60000m, 1, 4, 6, 10000m },
                    { 2, 45000m, 1, 10, 10, 4500m },
                    { 3, 64000m, 1, 1, 2, 32000m },
                    { 4, 135000m, 2, 27, 1, 135000m },
                    { 5, 38000m, 2, 14, 1, 38000m },
                    { 6, 22000m, 2, 13, 1, 22000m },
                    { 7, 35000m, 2, 24, 1, 35000m },
                    { 8, 240000m, 3, 8, 12, 20000m },
                    { 9, 116000m, 3, 17, 2, 58000m },
                    { 10, 95000m, 3, 28, 1, 95000m },
                    { 11, 75000m, 4, 22, 1, 75000m },
                    { 12, 20000m, 4, 25, 1, 20000m },
                    { 13, 160000m, 5, 31, 1, 160000m },
                    { 14, 90000m, 5, 32, 2, 45000m },
                    { 15, 60000m, 5, 30, 2, 30000m },
                    { 16, 36000m, 6, 5, 2, 18000m },
                    { 17, 30000m, 6, 6, 6, 5000m },
                    { 18, 42500m, 6, 12, 5, 8500m },
                    { 19, 72000m, 7, 2, 2, 36000m },
                    { 20, 56000m, 7, 3, 2, 28000m },
                    { 21, 64000m, 7, 21, 2, 32000m },
                    { 22, 62000m, 8, 20, 1, 62000m },
                    { 23, 48000m, 8, 19, 2, 24000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CashierId",
                table: "Orders",
                column: "CashierId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
