using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. DỮ LIỆU MẪU CHO BẢNG ROLE (Sửa lỗi Foreign Key)
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "ADMIN" },
                new Role { RoleId = 2, RoleName = "WAREHOUSE" },
                new Role { RoleId = 3, RoleName = "CASHIER" }
            );

            // 2. THƯƠNG HIỆU
            modelBuilder.Entity<Brand>().HasData(
                new Brand { BrandId = 1, BrandName = "Vinamilk", Description = "Sữa và các sản phẩm từ sữa hàng đầu Việt Nam" },
                new Brand { BrandId = 2, BrandName = "TH true MILK", Description = "Sữa tươi sạch và các sản phẩm từ sữa tươi" },
                new Brand { BrandId = 3, BrandName = "Coca-Cola", Description = "Nước giải khát có ga Coca-Cola, Sprite, Fanta" },
                new Brand { BrandId = 4, BrandName = "PepsiCo", Description = "Nước giải khát Pepsi, Aquafina, trà Ô Long Tea Plus" },
                new Brand { BrandId = 5, BrandName = "Masan Consumer", Description = "Gia vị Chin-su, Nam Ngư và mì Omachi" },
                new Brand { BrandId = 6, BrandName = "Acecook", Description = "Mì ăn liền Hảo Hảo, Modern, Lẩu Thái" },
                new Brand { BrandId = 7, BrandName = "Unilever", Description = "Hóa mỹ phẩm và đồ gia dụng OMO, Sunlight, Clear, P/S" },
                new Brand { BrandId = 8, BrandName = "Nestlé", Description = "Cà phê Nescafé, Milo và các sản phẩm dinh dưỡng" },
                new Brand { BrandId = 9, BrandName = "Orion", Description = "Bánh Chocopie, Custas và các loại bánh snack Hàn Quốc" },
                new Brand { BrandId = 10, BrandName = "Kinh Đô", Description = "Bánh quy, bánh kẹo truyền thống Việt Nam" },
                new Brand { BrandId = 11, BrandName = "Vissan", Description = "Thịt chế biến, xúc xích, thực phẩm đông lạnh" },
                new Brand { BrandId = 12, BrandName = "Heineken Việt Nam", Description = "Bia Heineken, Tiger, Bia Việt" },
                new Brand { BrandId = 13, BrandName = "SunMart", Description = "Nhãn hàng riêng của siêu thị mini SunMart 24/7" },
                new Brand { BrandId = 14, BrandName = "Thiên Long", Description = "Bút viết và văn phòng phẩm Việt Nam" }
            );

            // 3. DANH MỤC
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Sữa & Sản phẩm từ sữa", Description = "Sữa tươi, sữa chua, sữa đặc, phô mai" },
                new Category { CategoryId = 2, CategoryName = "Đồ uống không cồn", Description = "Nước ngọt, nước suối, trà đóng chai, cà phê hòa tan" },
                new Category { CategoryId = 3, CategoryName = "Bia & Đồ uống có cồn", Description = "Bia lon, bia chai, rượu (bán cho khách đủ 18 tuổi)" },
                new Category { CategoryId = 4, CategoryName = "Mì & Thực phẩm ăn liền", Description = "Mì gói, mì ly, phở, cháo ăn liền" },
                new Category { CategoryId = 5, CategoryName = "Gia vị & Nhu yếu phẩm", Description = "Nước mắm, nước tương, tương ớt, muối, đường, dầu ăn" },
                new Category { CategoryId = 6, CategoryName = "Bánh kẹo & Snack", Description = "Bánh quy, bánh bông lan, snack, kẹo" },
                new Category { CategoryId = 7, CategoryName = "Thịt, Cá & Đông lạnh", Description = "Thịt tươi, xúc xích, cá viên, thực phẩm đông lạnh" },
                new Category { CategoryId = 8, CategoryName = "Rau củ quả & Trứng tươi", Description = "Rau xanh, củ quả, trái cây, trứng gà" },
                new Category { CategoryId = 9, CategoryName = "Gạo & Đồ khô", Description = "Gạo, nếp, đậu, ngũ cốc, bột" },
                new Category { CategoryId = 10, CategoryName = "Chăm sóc cá nhân", Description = "Dầu gội, sữa tắm, kem đánh răng, khăn giấy" },
                new Category { CategoryId = 11, CategoryName = "Đồ dùng gia đình & Vệ sinh", Description = "Nước giặt, nước rửa chén, giấy vệ sinh, túi rác" },
                new Category { CategoryId = 12, CategoryName = "Văn phòng phẩm & Tiện ích", Description = "Bút, vở, pin, bật lửa và các vật dụng tiện ích" }
            );

            // 4. KHÁCH HÀNG
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Nguyễn Văn A", PhoneNumber = "0901122334", Address = "25 Nguyễn Huệ, Quận 1, TP.HCM", RewardPoints = 1500, MembershipRank = "Vàng" },
                new Customer { CustomerId = 2, CustomerName = "Trần Thị B", PhoneNumber = "0918877665", Address = "118 Võ Văn Tần, Quận 3, TP.HCM", RewardPoints = 500, MembershipRank = "Bạc" },
                new Customer { CustomerId = 3, CustomerName = "Lê Văn C", PhoneNumber = "0983344556", Address = "72 Nguyễn Trãi, Quận 5, TP.HCM", RewardPoints = 100, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 4, CustomerName = "Phạm Thị D", PhoneNumber = "0905678123", Address = "156 Thành Thái, Quận 10, TP.HCM", RewardPoints = 2300, MembershipRank = "Vàng" },
                new Customer { CustomerId = 5, CustomerName = "Hoàng Văn E", PhoneNumber = "0912345678", Address = "43 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM", RewardPoints = 800, MembershipRank = "Bạc" },
                new Customer { CustomerId = 6, CustomerName = "Võ Thị F", PhoneNumber = "0987654321", Address = "89 Phạm Văn Đồng, Quận Gò Vấp, TP.HCM", RewardPoints = 3200, MembershipRank = "Kim Cương" },
                new Customer { CustomerId = 7, CustomerName = "Đặng Văn G", PhoneNumber = "0909876543", Address = "215 Cộng Hòa, Quận Tân Bình, TP.HCM", RewardPoints = 250, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 8, CustomerName = "Bùi Thị H", PhoneNumber = "0913456789", Address = "36 Phan Đình Phùng, Quận Phú Nhuận, TP.HCM", RewardPoints = 1200, MembershipRank = "Bạc" },
                new Customer { CustomerId = 9, CustomerName = "Đỗ Văn I", PhoneNumber = "0981234567", Address = "102 Nguyễn Thị Thập, Quận 7, TP.HCM", RewardPoints = 4500, MembershipRank = "Kim Cương" },
                new Customer { CustomerId = 10, CustomerName = "Nguyễn Thị K", PhoneNumber = "0903456789", Address = "68 Hậu Giang, Quận 6, TP.HCM", RewardPoints = 150, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 11, CustomerName = "Trương Văn L", PhoneNumber = "0915678901", Address = "145 Võ Văn Ngân, TP. Thủ Đức, TP.HCM", RewardPoints = 1800, MembershipRank = "Vàng" },
                new Customer { CustomerId = 12, CustomerName = "Phan Thị M", PhoneNumber = "0986789012", Address = "234 Lê Văn Khương, Quận 12, TP.HCM", RewardPoints = 700, MembershipRank = "Bạc" },
                new Customer { CustomerId = 13, CustomerName = "Lý Văn N", PhoneNumber = "0907890123", Address = "57 Lũy Bán Bích, Quận Tân Phú, TP.HCM", RewardPoints = 5000, MembershipRank = "Kim Cương" },
                new Customer { CustomerId = 14, CustomerName = "Huỳnh Thị P", PhoneNumber = "0918901234", Address = "321 Tỉnh Lộ 10, Quận Bình Tân, TP.HCM", RewardPoints = 350, MembershipRank = "Chuẩn" },
                new Customer { CustomerId = 15, CustomerName = "Mai Văn Q", PhoneNumber = "0989012345", Address = "78 Nguyễn Hữu Trí, Huyện Bình Chánh, TP.HCM", RewardPoints = 2600, MembershipRank = "Vàng" }
            );

            // 5. SẢN PHẨM
            modelBuilder.Entity<Product>().HasData(
                // Sữa & sản phẩm từ sữa
                new Product { ProductId = 1, Barcode = "1000000001", ProductName = "Sữa tươi tiệt trùng Vinamilk 100% có đường 1L", Price = 32000m, StockQuantity = 120, CategoryId = 1, BrandId = 1 },
                new Product { ProductId = 2, Barcode = "1000000002", ProductName = "Sữa tươi TH true MILK ít đường 180ml (lốc 4 hộp)", Price = 36000m, StockQuantity = 90, CategoryId = 1, BrandId = 2 },
                new Product { ProductId = 3, Barcode = "1000000003", ProductName = "Sữa chua ăn Vinamilk có đường (lốc 4 hộp)", Price = 28000m, StockQuantity = 100, CategoryId = 1, BrandId = 1 },
                // Đồ uống không cồn
                new Product { ProductId = 4, Barcode = "1000000004", ProductName = "Nước ngọt Coca-Cola lon 330ml", Price = 10000m, StockQuantity = 300, CategoryId = 2, BrandId = 3 },
                new Product { ProductId = 5, Barcode = "1000000005", ProductName = "Nước ngọt Pepsi chai 1.5L", Price = 18000m, StockQuantity = 150, CategoryId = 2, BrandId = 4 },
                new Product { ProductId = 6, Barcode = "1000000006", ProductName = "Nước tinh khiết Aquafina chai 500ml", Price = 5000m, StockQuantity = 400, CategoryId = 2, BrandId = 4 },
                new Product { ProductId = 7, Barcode = "1000000007", ProductName = "Trà Ô Long Tea Plus chai 455ml", Price = 12000m, StockQuantity = 200, CategoryId = 2, BrandId = 4 },
                // Bia
                new Product { ProductId = 8, Barcode = "1000000008", ProductName = "Bia Heineken lon 330ml", Price = 20000m, StockQuantity = 240, CategoryId = 3, BrandId = 12 },
                new Product { ProductId = 9, Barcode = "1000000009", ProductName = "Bia Tiger lon 330ml", Price = 17000m, StockQuantity = 240, CategoryId = 3, BrandId = 12 },
                // Mì & thực phẩm ăn liền
                new Product { ProductId = 10, Barcode = "1000000010", ProductName = "Mì Hảo Hảo tôm chua cay gói 75g", Price = 4500m, StockQuantity = 500, CategoryId = 4, BrandId = 6 },
                new Product { ProductId = 11, Barcode = "1000000011", ProductName = "Mì ly Modern lẩu Thái tôm 65g", Price = 11000m, StockQuantity = 200, CategoryId = 4, BrandId = 6 },
                new Product { ProductId = 12, Barcode = "1000000012", ProductName = "Mì Omachi sườn hầm ngũ quả gói 80g", Price = 8500m, StockQuantity = 300, CategoryId = 4, BrandId = 5 },
                // Gia vị & nhu yếu phẩm
                new Product { ProductId = 13, Barcode = "1000000013", ProductName = "Nước tương Chin-su chai 500ml", Price = 22000m, StockQuantity = 150, CategoryId = 5, BrandId = 5 },
                new Product { ProductId = 14, Barcode = "1000000014", ProductName = "Nước mắm Nam Ngư chai 500ml", Price = 38000m, StockQuantity = 120, CategoryId = 5, BrandId = 5 },
                new Product { ProductId = 15, Barcode = "1000000015", ProductName = "Tương ớt Chin-su chai 250g", Price = 15000m, StockQuantity = 130, CategoryId = 5, BrandId = 5 },
                new Product { ProductId = 16, Barcode = "1000000016", ProductName = "Muối i-ốt SunMart gói 500g", Price = 8000m, StockQuantity = 100, CategoryId = 5, BrandId = 13 },
                // Bánh kẹo & snack
                new Product { ProductId = 17, Barcode = "1000000017", ProductName = "Bánh Chocopie Orion hộp 12 cái", Price = 58000m, StockQuantity = 80, CategoryId = 6, BrandId = 9 },
                new Product { ProductId = 18, Barcode = "1000000018", ProductName = "Bánh Custas Orion hộp 12 cái", Price = 52000m, StockQuantity = 70, CategoryId = 6, BrandId = 9 },
                new Product { ProductId = 19, Barcode = "1000000019", ProductName = "Bánh quy AFC Kinh Đô gói 200g", Price = 24000m, StockQuantity = 100, CategoryId = 6, BrandId = 10 },
                new Product { ProductId = 20, Barcode = "1000000020", ProductName = "Cà phê hòa tan Nescafé 3in1 hộp 20 gói", Price = 62000m, StockQuantity = 90, CategoryId = 2, BrandId = 8 },
                // Thịt, cá & đông lạnh
                new Product { ProductId = 21, Barcode = "1000000021", ProductName = "Xúc xích tiệt trùng Vissan gói 175g", Price = 32000m, StockQuantity = 80, CategoryId = 7, BrandId = 11 },
                new Product { ProductId = 22, Barcode = "1000000022", ProductName = "Thịt ba rọi heo SunMart khay 500g", Price = 75000m, StockQuantity = 40, CategoryId = 7, BrandId = 13 },
                new Product { ProductId = 23, Barcode = "1000000023", ProductName = "Cá viên chiên đông lạnh Vissan gói 500g", Price = 55000m, StockQuantity = 50, CategoryId = 7, BrandId = 11 },
                // Rau củ quả & trứng
                new Product { ProductId = 24, Barcode = "1000000024", ProductName = "Trứng gà SunMart hộp 10 quả", Price = 35000m, StockQuantity = 60, CategoryId = 8, BrandId = 13 },
                new Product { ProductId = 25, Barcode = "1000000025", ProductName = "Cà chua Đà Lạt khay 500g", Price = 20000m, StockQuantity = 50, CategoryId = 8, BrandId = 13 },
                new Product { ProductId = 26, Barcode = "1000000026", ProductName = "Rau muống sạch bó 300g", Price = 10000m, StockQuantity = 60, CategoryId = 8, BrandId = 13 },
                // Gạo & đồ khô
                new Product { ProductId = 27, Barcode = "1000000027", ProductName = "Gạo ST25 SunMart túi 5kg", Price = 135000m, StockQuantity = 60, CategoryId = 9, BrandId = 13 },
                // Chăm sóc cá nhân
                new Product { ProductId = 28, Barcode = "1000000028", ProductName = "Dầu gội Clear Men sạch gàu chai 340g", Price = 95000m, StockQuantity = 50, CategoryId = 10, BrandId = 7 },
                new Product { ProductId = 29, Barcode = "1000000029", ProductName = "Kem đánh răng P/S bảo vệ 123 tuýp 180g", Price = 32000m, StockQuantity = 80, CategoryId = 10, BrandId = 7 },
                // Đồ dùng gia đình & vệ sinh
                new Product { ProductId = 30, Barcode = "1000000030", ProductName = "Nước rửa chén Sunlight chanh chai 750g", Price = 30000m, StockQuantity = 70, CategoryId = 11, BrandId = 7 },
                new Product { ProductId = 31, Barcode = "1000000031", ProductName = "Nước giặt OMO Matic cửa trước túi 3.6kg", Price = 160000m, StockQuantity = 40, CategoryId = 11, BrandId = 7 },
                new Product { ProductId = 32, Barcode = "1000000032", ProductName = "Giấy vệ sinh SunMart lốc 10 cuộn", Price = 45000m, StockQuantity = 100, CategoryId = 11, BrandId = 13 },
                // Văn phòng phẩm & tiện ích
                new Product { ProductId = 33, Barcode = "1000000033", ProductName = "Bút bi Thiên Long TL-027 hộp 20 cây", Price = 45000m, StockQuantity = 40, CategoryId = 12, BrandId = 14 }
            );

            // 6. ĐƠN HÀNG (Subtotal = tổng LineTotal, Total = Subtotal - Discount)
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId = 1, OrderCode = "HD00001", Subtotal = 169000m, Discount = 0m, Total = 169000m, PaymentMethod = "CASH", Status = "PAID", CreatedAt = new DateTime(2026, 1, 1, 14, 30, 0), CustomerId = 1, CashierId = 8 },
                new Order { OrderId = 2, OrderCode = "HD00002", Subtotal = 230000m, Discount = 0m, Total = 230000m, PaymentMethod = "CARD", Status = "PAID", CreatedAt = new DateTime(2026, 1, 5, 9, 15, 0), CustomerId = 2, CashierId = 9 },
                new Order { OrderId = 3, OrderCode = "HD00003", Subtotal = 451000m, Discount = 22550m, Total = 428450m, PaymentMethod = "BANK_TRANSFER", Status = "PAID", CreatedAt = new DateTime(2026, 1, 10, 11, 0, 0), CustomerId = 6, CashierId = 10 },
                new Order { OrderId = 4, OrderCode = "HD00004", Subtotal = 95000m, Discount = 0m, Total = 95000m, PaymentMethod = "CASH", Status = "CANCELLED", CreatedAt = new DateTime(2026, 1, 12, 16, 45, 0), CustomerId = 9, CashierId = 11 },
                new Order { OrderId = 5, OrderCode = "HD00005", Subtotal = 310000m, Discount = 15500m, Total = 294500m, PaymentMethod = "MOMO", Status = "PAID", CreatedAt = new DateTime(2026, 1, 15, 8, 20, 0), CustomerId = 13, CashierId = 12 },
                new Order { OrderId = 6, OrderCode = "HD00006", Subtotal = 108500m, Discount = 0m, Total = 108500m, PaymentMethod = "CASH", Status = "PAID", CreatedAt = new DateTime(2026, 1, 16, 22, 10, 0), CustomerId = 3, CashierId = 13 },
                new Order { OrderId = 7, OrderCode = "HD00007", Subtotal = 192000m, Discount = 0m, Total = 192000m, PaymentMethod = "MOMO", Status = "PAID", CreatedAt = new DateTime(2026, 1, 18, 7, 45, 0), CustomerId = 4, CashierId = 14 },
                new Order { OrderId = 8, OrderCode = "HD00008", Subtotal = 110000m, Discount = 0m, Total = 110000m, PaymentMethod = "CASH", Status = "PAID", CreatedAt = new DateTime(2026, 1, 20, 0, 30, 0), CustomerId = 5, CashierId = 14 }
            );

            // 7. CHI TIẾT ĐƠN HÀNG
            modelBuilder.Entity<OrderItem>().HasData(
                // HD00001
                new OrderItem { OrderItemId = 1, OrderId = 1, ProductId = 4, Quantity = 6, UnitPrice = 10000m, LineTotal = 60000m },
                new OrderItem { OrderItemId = 2, OrderId = 1, ProductId = 10, Quantity = 10, UnitPrice = 4500m, LineTotal = 45000m },
                new OrderItem { OrderItemId = 3, OrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 32000m, LineTotal = 64000m },
                // HD00002
                new OrderItem { OrderItemId = 4, OrderId = 2, ProductId = 27, Quantity = 1, UnitPrice = 135000m, LineTotal = 135000m },
                new OrderItem { OrderItemId = 5, OrderId = 2, ProductId = 14, Quantity = 1, UnitPrice = 38000m, LineTotal = 38000m },
                new OrderItem { OrderItemId = 6, OrderId = 2, ProductId = 13, Quantity = 1, UnitPrice = 22000m, LineTotal = 22000m },
                new OrderItem { OrderItemId = 7, OrderId = 2, ProductId = 24, Quantity = 1, UnitPrice = 35000m, LineTotal = 35000m },
                // HD00003
                new OrderItem { OrderItemId = 8, OrderId = 3, ProductId = 8, Quantity = 12, UnitPrice = 20000m, LineTotal = 240000m },
                new OrderItem { OrderItemId = 9, OrderId = 3, ProductId = 17, Quantity = 2, UnitPrice = 58000m, LineTotal = 116000m },
                new OrderItem { OrderItemId = 10, OrderId = 3, ProductId = 28, Quantity = 1, UnitPrice = 95000m, LineTotal = 95000m },
                // HD00004 (đã hủy)
                new OrderItem { OrderItemId = 11, OrderId = 4, ProductId = 22, Quantity = 1, UnitPrice = 75000m, LineTotal = 75000m },
                new OrderItem { OrderItemId = 12, OrderId = 4, ProductId = 25, Quantity = 1, UnitPrice = 20000m, LineTotal = 20000m },
                // HD00005
                new OrderItem { OrderItemId = 13, OrderId = 5, ProductId = 31, Quantity = 1, UnitPrice = 160000m, LineTotal = 160000m },
                new OrderItem { OrderItemId = 14, OrderId = 5, ProductId = 32, Quantity = 2, UnitPrice = 45000m, LineTotal = 90000m },
                new OrderItem { OrderItemId = 15, OrderId = 5, ProductId = 30, Quantity = 2, UnitPrice = 30000m, LineTotal = 60000m },
                // HD00006
                new OrderItem { OrderItemId = 16, OrderId = 6, ProductId = 5, Quantity = 2, UnitPrice = 18000m, LineTotal = 36000m },
                new OrderItem { OrderItemId = 17, OrderId = 6, ProductId = 6, Quantity = 6, UnitPrice = 5000m, LineTotal = 30000m },
                new OrderItem { OrderItemId = 18, OrderId = 6, ProductId = 12, Quantity = 5, UnitPrice = 8500m, LineTotal = 42500m },
                // HD00007
                new OrderItem { OrderItemId = 19, OrderId = 7, ProductId = 2, Quantity = 2, UnitPrice = 36000m, LineTotal = 72000m },
                new OrderItem { OrderItemId = 20, OrderId = 7, ProductId = 3, Quantity = 2, UnitPrice = 28000m, LineTotal = 56000m },
                new OrderItem { OrderItemId = 21, OrderId = 7, ProductId = 21, Quantity = 2, UnitPrice = 32000m, LineTotal = 64000m },
                // HD00008
                new OrderItem { OrderItemId = 22, OrderId = 8, ProductId = 20, Quantity = 1, UnitPrice = 62000m, LineTotal = 62000m },
                new OrderItem { OrderItemId = 23, OrderId = 8, ProductId = 19, Quantity = 2, UnitPrice = 24000m, LineTotal = 48000m }
            );

            // 8. NGƯỜI DÙNG
            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, Username = "admin01", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Nguyễn Văn Admin", Email = "admin01@sunmart.vn", Phone = "0901000001", IsActive = true, CreatedAt = new DateTime(2026, 1, 1, 8, 0, 0), RoleId = 1 },
                new User { UserId = 2, Username = "admin02", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Trần Thị Admin", Email = "admin02@sunmart.vn", Phone = "0901000002", IsActive = true, CreatedAt = new DateTime(2026, 1, 2, 8, 0, 0), RoleId = 1 },
                new User { UserId = 3, Username = "admin03", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Lê Văn Quản", Email = "admin03@sunmart.vn", Phone = "0901000003", IsActive = true, CreatedAt = new DateTime(2026, 1, 3, 8, 0, 0), RoleId = 1 },
                new User { UserId = 4, Username = "warehouse01", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Phạm Văn Kho", Email = "warehouse01@sunmart.vn", Phone = "0901000004", IsActive = true, CreatedAt = new DateTime(2026, 1, 4, 8, 0, 0), RoleId = 2 },
                new User { UserId = 5, Username = "warehouse02", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Hoàng Thị Hương", Email = "warehouse02@sunmart.vn", Phone = "0901000005", IsActive = true, CreatedAt = new DateTime(2026, 1, 5, 8, 0, 0), RoleId = 2 },
                new User { UserId = 6, Username = "warehouse03", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Võ Minh Đức", Email = "warehouse03@sunmart.vn", Phone = "0901000006", IsActive = true, CreatedAt = new DateTime(2026, 1, 6, 8, 0, 0), RoleId = 2 },
                new User { UserId = 7, Username = "warehouse04", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Đặng Quốc Anh", Email = "warehouse04@sunmart.vn", Phone = "0901000007", IsActive = true, CreatedAt = new DateTime(2026, 1, 7, 8, 0, 0), RoleId = 2 },
                new User { UserId = 8, Username = "cashier01", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Nguyễn Thị Lan", Email = "cashier01@sunmart.vn", Phone = "0901000008", IsActive = true, CreatedAt = new DateTime(2026, 1, 8, 8, 0, 0), RoleId = 3 },
                new User { UserId = 9, Username = "cashier02", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Trần Văn Nam", Email = "cashier02@sunmart.vn", Phone = "0901000009", IsActive = true, CreatedAt = new DateTime(2026, 1, 9, 8, 0, 0), RoleId = 3 },
                new User { UserId = 10, Username = "cashier03", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Phan Thị Mai", Email = "cashier03@sunmart.vn", Phone = "0901000010", IsActive = true, CreatedAt = new DateTime(2026, 1, 10, 8, 0, 0), RoleId = 3 },
                new User { UserId = 11, Username = "cashier04", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Lý Hoàng Long", Email = "cashier04@sunmart.vn", Phone = "0901000011", IsActive = true, CreatedAt = new DateTime(2026, 1, 11, 8, 0, 0), RoleId = 3 },
                new User { UserId = 12, Username = "cashier05", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Bùi Thị Ngọc", Email = "cashier05@sunmart.vn", Phone = "0901000012", IsActive = true, CreatedAt = new DateTime(2026, 1, 12, 8, 0, 0), RoleId = 3 },
                new User { UserId = 13, Username = "cashier06", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Đỗ Minh Tâm", Email = "cashier06@sunmart.vn", Phone = "0901000013", IsActive = true, CreatedAt = new DateTime(2026, 1, 13, 8, 0, 0), RoleId = 3 },
                new User { UserId = 14, Username = "cashier07", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Huỳnh Văn Phúc", Email = "cashier07@sunmart.vn", Phone = "0901000014", IsActive = true, CreatedAt = new DateTime(2026, 1, 14, 8, 0, 0), RoleId = 3 },
                new User { UserId = 15, Username = "cashier08", PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", FullName = "Mai Thị Thu", Email = "cashier08@sunmart.vn", Phone = "0901000015", IsActive = true, CreatedAt = new DateTime(2026, 1, 15, 8, 0, 0), RoleId = 3 }
            );
        }
    }
}
