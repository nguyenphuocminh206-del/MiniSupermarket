# MiniSuperMarket — Buổi 1

**Web API và WinForms CRUD nhóm hàng**

Đây là một trong 5 mốc học tập được tách lại từ project hiện có. Lịch sử các nhánh là lộ trình học được dựng lại, không phải lịch sử thực hiện bài tập ban đầu.

## Lộ trình 5 buổi

| Nhánh | Nội dung | Mốc |
|---|---|---|
| `buoi1` | Web API CRUD nhóm hàng, tìm kiếm, WinForms gọi HttpClient | Dữ liệu trong bộ nhớ |
| `buoi2` | JWT, đăng nhập Admin/Cashier, bảo vệ API và nút thao tác | Kế thừa buoi1 |
| `buoi3` | SQL Server, EF Core, migration, seed, quản lý khách hàng | Kế thừa buoi2 |
| `buoi4` | Shell quản trị, sản phẩm/phân trang, thương hiệu, tài khoản, vai trò | Kế thừa buoi3 |
| `buoi5` | POS, thanh toán, đơn hàng, hoàn kho, dashboard, báo cáo, xuất hóa đơn | Project đầy đủ |

## Nội dung buổi 1

- Thiết kế Category; GET danh sách/chi tiết, POST, PUT, DELETE và tìm kiếm nhóm hàng.
- Kết nối WinForms với Web API qua HttpClient, hiển thị DataGridView và cập nhật dữ liệu.
- Dữ liệu nằm trong bộ nhớ; khởi động lại API sẽ trở về dữ liệu mẫu. Chưa cần SQL Server.

## Chạy project

Yêu cầu Windows, .NET 8 SDK hoặc SDK mới hơn hỗ trợ net8.0; cài .NET 8 Desktop Runtime để chạy WinForms. Có thể mở `MiniSuperMarket.sln` bằng Visual Studio hỗ trợ .NET 8.

```powershell
git clone --branch buoi1 https://github.com/nguyenphuocminh206-del/MiniSupermarket.git
cd MiniSupermarket
dotnet restore MiniSuperMarket.sln
dotnet build MiniSuperMarket.sln --configuration Release
```

Chạy API trong terminal thứ nhất:

```powershell
dotnet dev-certs https --trust
dotnet run --project MiniSupermarket.API --launch-profile https
```

Mở Swagger tại https://localhost:7158/swagger. Client sử dụng https://localhost:7158/api/.

Chạy WinForms trong terminal thứ hai:

```powershell
dotnet run --project MiniSupermarket.WinForms
```

Buổi 1 mở thẳng form nhóm hàng, chưa cần đăng nhập.

## Tự kiểm tra

- [ ] GET /api/categories trả danh sách mà không cần token.
- [ ] Thêm một nhóm hàng, tìm theo tên, sửa rồi xóa trên WinForms.
- [ ] Khởi động lại API và quan sát dữ liệu in-memory được khởi tạo lại.

## Chuyển buổi

```powershell
git fetch origin
git switch buoi2
```

Dừng API và WinForms trước khi chuyển nhánh, rồi build/chạy lại theo README của nhánh mới.
