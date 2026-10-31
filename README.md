# MiniSuperMarket — Buổi 5

**POS, đơn hàng, báo cáo và tích hợp hoàn chỉnh**

Đây là một trong 5 mốc học tập được tách lại từ project hiện có. Lịch sử các nhánh là lộ trình học được dựng lại, không phải lịch sử thực hiện bài tập ban đầu.

## Lộ trình 5 buổi

| Nhánh | Nội dung | Mốc |
|---|---|---|
| `buoi1` | Web API CRUD nhóm hàng, tìm kiếm, WinForms gọi HttpClient | Dữ liệu trong bộ nhớ |
| `buoi2` | JWT, đăng nhập Admin/Cashier, bảo vệ API và nút thao tác | Kế thừa buoi1 |
| `buoi3` | SQL Server, EF Core, migration, seed, quản lý khách hàng | Kế thừa buoi2 |
| `buoi4` | Shell quản trị, sản phẩm/phân trang, thương hiệu, tài khoản, vai trò | Kế thừa buoi3 |
| `buoi5` | POS, thanh toán, đơn hàng, hoàn kho, dashboard, báo cáo, xuất hóa đơn | Project đầy đủ |

## Nội dung buổi 5

- Tích hợp đầy đủ project hiện tại: POS, mã vạch, giỏ hàng và thanh toán.
- Quản lý đơn, trừ/hoàn tồn kho, điểm thưởng và transaction.
- Dashboard, báo cáo doanh thu, stored procedure, xuất hóa đơn Excel và CSV.
- Giao diện cuối cùng phân quyền Admin / Warehouse / Cashier; tài liệu gốc ở docs/PROJECT-GOC.md.

## Chạy project

Yêu cầu Windows, .NET 8 SDK hoặc SDK mới hơn hỗ trợ net8.0; cài .NET 8 Desktop Runtime để chạy WinForms. Có thể mở `MiniSuperMarket.sln` bằng Visual Studio hỗ trợ .NET 8.

```powershell
git clone --branch buoi5 https://github.com/nguyenphuocminh206-del/MiniSupermarket.git
cd MiniSupermarket
dotnet restore MiniSuperMarket.sln
dotnet build MiniSuperMarket.sln --configuration Release
```

### Chuẩn bị cơ sở dữ liệu

Yêu cầu SQL Server LocalDB. Chuỗi kết nối mặc định dùng database riêng **MiniSupermarketHocTapDb** cho các nhánh học tập, không dùng database project gốc.

```powershell
dotnet tool restore
dotnet ef database update --project MiniSupermarket.API
```

Buổi 3–5 dùng cùng schema và migration; chuyển nhánh không cần xóa database. Các stored procedure được cài bằng migration.

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

Tài khoản thực hành: `admin01` / `123456`, `warehouse01` / `123456`, `cashier01` / `123456`. Tài khoản demo dự phòng: `admin` / `123456`, `cashier` / `123456`.

Các tài khoản và khóa JWT có sẵn chỉ phục vụ học tập trên máy cá nhân.

## Tự kiểm tra

- [ ] Đăng nhập cashier01/123456, nhập mã vạch 1000000001 và thanh toán.
- [ ] Kiểm tra tồn kho, đơn hàng và điểm thưởng sau thanh toán; thử hủy bằng Admin.
- [ ] Mở dashboard/báo cáo, kiểm tra thống kê và thử xuất hóa đơn.
- [ ] Kiểm tra các vai trò Admin, Warehouse, Cashier trên giao diện và API.

## Chuyển buổi

```powershell
git fetch origin
git switch buoi1
```

Dừng API và WinForms trước khi chuyển nhánh, rồi build/chạy lại theo README của nhánh mới.
