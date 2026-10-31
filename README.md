# MiniSuperMarket — Buổi 2

**Đăng nhập JWT và phân quyền**

Đây là một trong 5 mốc học tập được tách lại từ project hiện có. Lịch sử các nhánh là lộ trình học được dựng lại, không phải lịch sử thực hiện bài tập ban đầu.

## Lộ trình 5 buổi

| Nhánh | Nội dung | Mốc |
|---|---|---|
| `buoi1` | Web API CRUD nhóm hàng, tìm kiếm, WinForms gọi HttpClient | Dữ liệu trong bộ nhớ |
| `buoi2` | JWT, đăng nhập Admin/Cashier, bảo vệ API và nút thao tác | Kế thừa buoi1 |
| `buoi3` | SQL Server, EF Core, migration, seed, quản lý khách hàng | Kế thừa buoi2 |
| `buoi4` | Shell quản trị, sản phẩm/phân trang, thương hiệu, tài khoản, vai trò | Kế thừa buoi3 |
| `buoi5` | POS, thanh toán, đơn hàng, hoàn kho, dashboard, báo cáo, xuất hóa đơn | Project đầy đủ |

## Nội dung buổi 2

- Bổ sung API đăng nhập và phát hành JWT chứa thông tin vai trò.
- Admin được thêm/sửa/xóa; Cashier được xem và tìm kiếm nhóm hàng.
- FormLogin lưu token; HttpClient gửi Bearer token và giao diện khóa nút theo quyền.

## Chạy project

Yêu cầu Windows, .NET 8 SDK hoặc SDK mới hơn hỗ trợ net8.0; cài .NET 8 Desktop Runtime để chạy WinForms. Có thể mở `MiniSuperMarket.sln` bằng Visual Studio hỗ trợ .NET 8.

```powershell
git clone --branch buoi2 https://github.com/nguyenphuocminh206-del/MiniSupermarket.git
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

Tài khoản thực hành: `admin` / `123456` hoặc `cashier` / `123456`.

Các tài khoản và khóa JWT có sẵn chỉ phục vụ học tập trên máy cá nhân.

## Tự kiểm tra

- [ ] Gọi GET /api/categories khi chưa đăng nhập: nhận 401.
- [ ] Đăng nhập admin/123456 và thử CRUD thành công.
- [ ] Đăng nhập cashier/123456: xem được danh sách, gọi POST bằng token Cashier nhận 403.

## Chuyển buổi

```powershell
git fetch origin
git switch buoi3
```

Dừng API và WinForms trước khi chuyển nhánh, rồi build/chạy lại theo README của nhánh mới.
