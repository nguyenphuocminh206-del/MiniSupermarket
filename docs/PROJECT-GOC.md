# 🛒 HỆ THỐNG QUẢN LÝ SIÊU THỊ MINI (MINISUPERMARKET SYSTEM)
> **Môn học:** Lập trình Ứng dụng .NET Core (Mã môn: 229162)
> **Buổi 1:** Web API quản lý nhóm hàng và kết nối WinForms Client (CRUD)
> **Buổi 2:** Xác thực (Authentication) bằng JWT và Phân quyền (Authorization) theo Role
> **Buổi 3:** SQL Server + Entity Framework Core (Code-First), Migration, Data Seeding, quản lý Khách hàng thân thiết
> **Buổi 4:** WinForms Shell (sidebar) và giao diện phân quyền theo vai trò Admin / Cashier / Warehouse
> **Buổi 6:** EF Core nâng cao (Eager Loading, DTO, Phân trang Server-side, AsNoTracking) và tích hợp Client - Server bằng `HttpClient`

---

## 🏗️ 1. Mô hình Kiến trúc Hệ thống (Client - Server)
* **`MiniSupermarket.API` (Backend):** ASP.NET Core Web API xử lý nghiệp vụ, truy xuất SQL Server qua EF Core, xác thực JWT và cung cấp RESTful API.
* **`MiniSupermarket.WinForms` (Frontend Client):** Ứng dụng Windows Forms tại quầy. Người dùng đăng nhập (`FormLogin`) để lấy JWT, sau đó mọi form gọi API qua một `HttpClient` dùng chung (`SessionManager.ApiClientService`) kèm Bearer Token.
* **SQL Server LocalDB:** Cơ sở dữ liệu `MiniSupermarketDb`, tạo và cập nhật bằng EF Core Migration.

```text
 WinForms Client  ──(HTTP + JWT Bearer)──►  ASP.NET Core Web API  ──(EF Core)──►  SQL Server LocalDB
 (FormLogin, Shell, POS, ...)               (Controllers, DTO)                    (MiniSupermarketDb)
```

---

## 🛠️ 2. Công nghệ Sử dụng
* **Ngôn ngữ / Nền tảng:** C# (.NET 8.0)
* **Backend:** ASP.NET Core Web API, LINQ, async/await, DTO, Swagger (có nút Authorize cho JWT)
* **Cơ sở dữ liệu:** SQL Server LocalDB, EF Core 8.0 (Code-First, Migration, Data Seeding, Data Annotations)
* **Bảo mật:** JWT Bearer Authentication, Role-based Authorization
* **Frontend:** Windows Forms (.NET 8.0), `System.Net.Http.Json`
* **Công cụ kiểm thử:** Swagger UI, SQL Server Object Explorer / SSMS

---

## 📂 3. Cấu trúc Solution
```text
MiniSuperMarket/
├── MiniSupermarket.API/
│   ├── Controllers/
│   │   ├── AuthController.cs          # Đăng nhập & phát hành JWT
│   │   ├── CategoriesController.cs    # CRUD & Search nhóm hàng
│   │   ├── BrandController.cs         # CRUD & Search thương hiệu
│   │   ├── ProductsController.cs      # CRUD, quét mã vạch, phân trang sản phẩm
│   │   ├── CustomersController.cs     # CRUD, Search, phân trang khách hàng
│   │   ├── OrderController.cs         # Đơn hàng & thanh toán POS (checkout)
│   │   ├── ReportsController.cs       # Báo cáo doanh thu theo ngày
│   │   ├── UsersController.cs         # Quản lý tài khoản (Admin)
│   │   └── RolesController.cs         # Quản lý vai trò
│   ├── Data/SupermarketDbContext.cs   # DbContext, quan hệ, Data Seeding
│   ├── Models/                        # Category, Brand, Product, Customer, Order, OrderItem, User, Role, PagedResponseDto
│   ├── Migrations/                    # Lịch sử migration của EF Core
│   ├── appsettings.json               # Connection string & cấu hình JWT
│   └── Program.cs                     # DbContext, JWT, Swagger, cấu hình JSON
│
└── MiniSupermarket.WinForms/
    ├── FormLogin.cs                   # Đăng nhập, lưu Token + Role
    ├── FormMainShell.cs               # Khung chính: sidebar, nạp form con, phân quyền theo Role
    ├── FormPOS.cs                     # Bán hàng: quét mã vạch, giỏ hàng, thanh toán
    ├── FormProductManagement.cs       # Quản lý sản phẩm (phân trang Server-side)
    ├── FormCategoryManagement.cs      # Quản lý nhóm hàng
    ├── FormCustomerManagement.cs      # Quản lý khách hàng thân thiết
    ├── FormBrandManagement.cs         # Quản lý thương hiệu
    ├── FormOrderManagement.cs         # Quản lý đơn hàng
    ├── FormReportManagement.cs        # Báo cáo doanh thu
    ├── FormUserManagement.cs          # Quản lý tài khoản
    ├── FormRoleManagement.cs          # Quản lý vai trò
    ├── Models/AppModels.cs            # DTO phía Client (CategoryDto, ProductDto, ProductItemViewDto, PagedViewResult<T>)
    └── SessionManager .cs             # Token/Role của phiên + ApiClientService (HttpClient dùng chung)
```

---

## 🗄️ 4. Cơ sở dữ liệu (SQL Server + Entity Framework Core)

### 4.1. Kết nối
`appsettings.json`, khóa `ConnectionStrings:DefaultConnection`:
```text
Server=(localdb)\MSSQLLocalDB;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;
```

### 4.2. Các bảng và quan hệ
| Bảng | Nội dung chính | Quan hệ |
|---|---|---|
| `Categories` | `CategoryId`, `CategoryName`, `Description` | 1 - N với `Products` |
| `Brands` | `BrandId`, `BrandName`, `Description` | 1 - N với `Products` |
| `Products` | `ProductId`, `Barcode`, `ProductName`, `Price` (decimal 18,2), `StockQuantity` | FK `CategoryId`, `BrandId` |
| `Customers` | `CustomerId`, `CustomerName`, `PhoneNumber`, `Address`, `RewardPoints` (mặc định 0), `MembershipRank` (mặc định `Chuẩn`) | 1 - N với `Orders` |
| `Orders` | `OrderCode`, `PaymentMethod`, `Status`, `CreatedAt`, `Subtotal`, `Discount`, `Total`, `PointsRedeemed`, `PointsEarned`, `InventoryDeducted` | FK `CashierId` (User), `CustomerId` (có thể null); đủ dữ liệu để hoàn kho/điểm khi hủy |
| `OrderItems` | `Quantity`, `UnitPrice`, `LineTotal` | FK `OrderId`, `ProductId` |
| `Users` | `Username`, `PasswordHash`, `FullName`, `Email`, `Phone`, `IsActive`, `CreatedAt` | FK `RoleId` |
| `Roles` | `RoleId`, `RoleName` (`ADMIN`, `WAREHOUSE`, `CASHIER`) | 1 - N với `Users` |

### 4.3. Dữ liệu mẫu (Data Seeding, `HasData`)
3 vai trò, 15 tài khoản, 12 nhóm hàng, 14 thương hiệu, 33 sản phẩm và 15 khách hàng.

### 4.4. Migration
Chạy trong **Package Manager Console** (Default project là `MiniSupermarket.API`):
```powershell
Add-Migration <TênMigration>   # Sinh migration từ thay đổi của Model
Update-Database                # Áp dụng migration và cài stored procedure vào SQL Server
Get-Migration                  # Xem migration đã Applied hay còn Pending
```

---

## 🔐 5. Xác thực & Phân quyền

### 5.1. Đăng nhập
* **`POST /api/auth/login`** (Public). Body: `{ "username": "...", "password": "..." }`
* Hệ thống kiểm tra tài khoản trong bảng `Users` (mật khẩu băm). Ngoài ra có 2 tài khoản dự phòng: `admin` / `123456` (Admin) và `cashier` / `123456` (Cashier).
* Thành công trả `200 OK`: `{ "success": true, "token": "<JWT>", "role": "Admin" }`. Sai thông tin trả `401`.
* Token ký `HmacSha256`, hiệu lực **2 giờ**, chứa claim `Name` và `Role`.

### 5.2. Phân quyền endpoint
| Controller | Quyền truy cập |
|---|---|
| `auth` | Public |
| `categories` | Đăng nhập; **POST / PUT / DELETE chỉ Admin**; `admin-dashboard` chỉ Admin; `staff-pos` Admin + Cashier |
| `products` | GET: JWT; POST / PUT / DELETE: Admin hoặc Warehouse |
| `customers` | Chỉ Admin |
| `orders` | GET và duyệt/hủy/xóa: Admin; checkout: Admin hoặc Cashier |
| `reports`, `users` | **Chỉ Admin** |
| `brands` | GET: JWT; POST / PUT / DELETE: Admin hoặc Warehouse |
| `roles` | Chỉ Admin |

> Gọi endpoint không kèm Token trả `401 Unauthorized`; đăng nhập đúng nhưng sai Role trả `403 Forbidden`.

### 5.3. Phân quyền giao diện WinForms (`FormMainShell`)
| Vai trò | Chức năng hiển thị | Màn hình mở đầu |
|---|---|---|
| **Admin** | Toàn bộ (POS, Danh mục, Sản phẩm, Khách hàng, Báo cáo, Tài khoản, ...) | Quản lý nhóm hàng |
| **Cashier** | Bán hàng (POS) | POS |
| **Warehouse** | Quản lý Danh mục và Sản phẩm | Quản lý nhóm hàng |

---

## 🚀 6. Tính năng Buổi 6: EF Core nâng cao & tích hợp Client - Server

### 6.1. Backend (API)
| Tính năng | Chi tiết |
|---|---|
| **Eager Loading** | `.Include(p => p.Category)` lấy sản phẩm kèm nhóm hàng trong **1 câu SQL JOIN**, tránh lỗi N+1 Query |
| **DTO làm phẳng dữ liệu** | `ProductItemDto` đưa `CategoryName` thành thuộc tính phẳng, không lộ cấu trúc CSDL |
| **Chống vòng lặp JSON** | `ReferenceHandler.IgnoreCycles` cấu hình trong `Program.cs` |
| **AsNoTracking** | Dùng cho mọi truy vấn `GET` chỉ đọc, giảm RAM và CPU |
| **Phân trang Server-side** | `.OrderBy()` → `.Skip((pageNumber - 1) * pageSize)` → `.Take(pageSize)`, EF Core sinh `OFFSET ... FETCH` |
| **Kẹp tham số** | `pageNumber < 1` về 1; `pageSize < 1` hoặc `> 50` về 5 |
| **Tìm kiếm kết hợp phân trang** | Từ khóa theo tên (không phân biệt hoa thường) hoặc mã vạch; tổng số trang tính lại theo kết quả lọc |
| **Lọc theo nhóm hàng** | Tham số tùy chọn `categoryId` trên `paged`, và endpoint riêng `paged-by-category` |
| **Phân trang khách hàng** | Sắp xếp theo `RewardPoints` giảm dần (hạng VIP lên đầu) |

**Endpoint phân trang**

| Endpoint | Tham số | Mô tả |
|---|---|---|
| `GET /api/products/paged` | `keyword`, `pageNumber=1`, `pageSize=5`, `categoryId` | Sản phẩm phân trang, tìm kiếm, lọc nhóm hàng |
| `GET /api/products/paged-by-category` | `categoryId`, `pageNumber=1`, `pageSize=5` | Phân trang theo nhóm hàng |
| `GET /api/customers/paged` | `pageNumber=1`, `pageSize=5` | Khách hàng phân trang theo điểm thưởng |

**Cấu trúc JSON trả về (`PagedResult<T>`)**
```json
{
  "currentPage": 1,
  "pageSize": 5,
  "totalItems": 33,
  "totalPages": 7,
  "items": [ { "productId": 1, "barcode": "...", "productName": "...", "price": 0, "stockQuantity": 0, "categoryId": 1, "categoryName": "..." } ]
}
```

### 6.2. Frontend (WinForms)
* **Một `HttpClient` dùng chung** (`SessionManager.ApiClientService.Client`): `BaseAddress` cố định, `Timeout` 30 giây, tự gắn `Authorization: Bearer <token>` ở mỗi lần truy cập. Cách này tránh lỗi cạn kiệt cổng mạng (Socket Exhaustion) do `new HttpClient()` liên tục. Các form Sản phẩm, Khách hàng, Thương hiệu, Nhóm hàng đã dùng client chung này và **không** `Dispose` nó.
* **`FormProductManagement` phân trang Server-side:**
  * Lưới chỉ tải 5 sản phẩm mỗi trang từ `products/paged`.
  * Nút `<< Trang trước` / `Trang sau >>` tự khóa ở trang đầu và trang cuối.
  * Nhãn hiển thị dạng `Trang 1 / 3 (Tổng: 15 SP)`.
  * Tìm kiếm theo từ khóa hoặc đổi nhóm hàng sẽ quay về trang 1.
  * Xóa hết dòng ở trang cuối thì tự lùi về trang cuối còn dữ liệu.
  * Từ khóa tiếng Việt được mã hóa bằng `Uri.EscapeDataString`.
* **Xử lý lỗi HTTP:** thông báo tiếng Việt cho `401`, `403`, `400`, `500` và mất kết nối API.

### 6.3. Checklist nghiệm thu Buổi 6
| # | Hạng mục | Cách kiểm tra |
|---|---|---|
| 1 | Backend Paging | Swagger `GET /api/Products/paged?pageNumber=1&pageSize=5` trả `totalItems: 15`, `totalPages: 3` |
| 2 | Eager Loading | Trường `categoryName` có giá trị (không null) |
| 3 | Tìm kiếm linh hoạt | Thêm `keyword`, số trang được tính lại theo kết quả |
| 4 | WinForms Paging UI | Bấm Trang trước / Trang sau, dữ liệu đổi mượt, nút khóa đúng chỗ |
| 5 | CRUD Client | Thêm / Sửa / Xóa sản phẩm trên WinForms cập nhật vào CSDL, tổng số trang tự thay đổi |

---

## 🖥️ 7. Chức năng WinForms khác
* **`FormLogin`:** kiểm tra bỏ trống, gọi `POST /api/auth/login`, lưu Token, tên đăng nhập và Role vào `SessionManager`; checkout gắn đơn với đúng tài khoản đang đăng nhập.
* **`FormMainShell`:** khung một cửa sổ với sidebar, nạp form con động, hiển thị tên và vai trò, ẩn nút theo quyền.
* **`FormPOS`:** quét hoặc nhập mã vạch (`products/barcode/{barcode}`), quản lý giỏ hàng, nhập tiền khách đưa và tính tiền thối, thanh toán qua `orders/checkout`, xóa giỏ hàng, hỗ trợ phím tắt.
* **Hóa đơn sau thanh toán:** POS hỏi người dùng có muốn lưu hóa đơn Excel `.xlsx` để xem hoặc in hay không.
* **Thanh toán (`POST /api/orders/checkout`):** xử lý trong transaction Serializable, từ chối giỏ hàng/số lượng/tồn kho/điểm không hợp lệ; lấy giá từ CSDL, trừ tồn kho và ghi điểm thưởng.
* **Quản lý đơn:** chỉ nhận trạng thái duyệt hoặc hủy hợp lệ; hủy đơn đã thanh toán hoàn kho và đảo điểm thưởng trong cùng transaction. Không xóa đơn còn ghi nhận đã trừ kho.
* **Báo cáo:** bốn thẻ KPI, biểu đồ cột ngày/tuần/tháng bằng WinForms Chart, Top 5 sản phẩm và xuất tồn kho/hóa đơn CSV.
* **`FormCategoryManagement`, `FormCustomerManagement`, `FormBrandManagement`:** CRUD, tìm kiếm, tải lại; khách hàng có kiểm tra dữ liệu (bắt buộc tên và số điện thoại, số điện thoại tối đa 15 ký tự).
* **`FormOrderManagement`, `FormReportManagement`, `FormUserManagement`, `FormRoleManagement`:** quản lý đơn hàng, báo cáo doanh thu theo ngày (Admin), tài khoản (Admin) và vai trò.

---

## 🚀 8. Hướng dẫn Chạy và Kiểm thử

**Yêu cầu:** Visual Studio 2022, .NET 8 SDK, SQL Server LocalDB.

**Bước 1: Tạo cơ sở dữ liệu**
1. Mở `MiniSuperMarket.sln` bằng Visual Studio 2022.
2. **Tools → NuGet Package Manager → Package Manager Console**, chọn Default project là `MiniSupermarket.API`.
3. Chạy `Update-Database`. Kiểm tra bảng và dữ liệu mẫu trong **SQL Server Object Explorer**.

**Bước 2: Chạy Backend**
1. Đặt `MiniSupermarket.API` làm Startup Project.
2. Chạy bằng launch profile **`https`** (địa chỉ `https://localhost:7158`) để khớp với địa chỉ WinForms đang gọi, rồi nhấn **F5**. Swagger UI tự mở.
3. Gọi `POST /api/auth/login` lấy Token, bấm **Authorize** và dán `Bearer <token>`.

**Bước 3: Chạy Frontend**
1. Đảm bảo địa chỉ `https://localhost:7158/api/` trong code WinForms khớp với cổng API đang chạy.
2. Chuột phải project `MiniSupermarket.WinForms` → **Debug → Start new instance**.
3. Đăng nhập bằng `admin` / `123456` hoặc `cashier` / `123456`, rồi thử các chức năng.

---

## 🧯 9. Xử lý sự cố thường gặp
| Hiện tượng | Cách xử lý |
|---|---|
| `Add-Migration` báo `Build failed` | Mở **Error List**, sửa lỗi rồi Rebuild Solution |
| WinForms báo lỗi kết nối Server | Kiểm tra API đang chạy bằng profile `https` và cổng `7158` khớp với code WinForms |
| Lỗi `401 Unauthorized` | Chưa đăng nhập hoặc Token hết hạn (2 giờ), cần đăng nhập lại |
| Lỗi `403 Forbidden` | Tài khoản không đủ quyền (ví dụ thao tác chỉ dành cho Admin) |
| Lỗi `405 Method Not Allowed` khi tải danh sách | Thiếu action `[HttpGet]` tương ứng trong Controller; kiểm tra lại và chạy lại API |
| Cả ứng dụng lỗi sau khi mở form Nhóm hàng | Có `using var client = ...` làm `Dispose` HttpClient dùng chung; phải dùng `var client = ...` |
| Tìm "sua" không ra "Sữa" | Tìm kiếm không phân biệt hoa thường nhưng việc phân biệt dấu phụ thuộc collation của SQL Server |

---

## 👨‍💻 10. Tác giả
* **Họ tên sinh viên:** Nguyễn Lê Chí Công
* **Mã sinh viên:** 2124110094
* **Lớp học phần:** CCQ2411C
# Bổ sung dashboard, barcode ADO.NET và báo cáo

Sau khi lấy mã nguồn mới, cập nhật cơ sở dữ liệu:

```powershell
dotnet ef database update --project MiniSupermarket.API
```

Các Stored Procedure `sp_RevenueByShift` và `sp_RevenueSeries` được cài tự động bởi migration khi chạy `dotnet ef database update --project MiniSupermarket.API`; file SQL trong `MiniSupermarket.API/Database/` cũng được giữ làm bản chạy thủ công.
`sp_RevenueSeries` nhóm doanh thu trên SQL Server theo ngày, tuần hoặc tháng; dashboard, Top 5 và xuất CSV gọi các endpoint trong `ReportsController`.
