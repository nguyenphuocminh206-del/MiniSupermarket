using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using System.Data;
using System.Globalization;
using System.Text;

namespace MiniSupermarket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : ControllerBase
    {
        private readonly SupermarketDbContext _db;
        private readonly IConfiguration _configuration;

        public ReportsController(SupermarketDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport([FromQuery] string? date)
        {
            DateTime targetDate;

            if (!DateTime.TryParse(date, out targetDate))
            {
                targetDate = DateTime.Today;
            }

            var startDate = targetDate.Date;
            var endDate = startDate.AddDays(1);

            var orders = await _db.Orders
                .Where(o =>
                    o.CreatedAt >= startDate &&
                    o.CreatedAt < endDate &&
                    o.Status == "PAID")
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Customer)
                .AsNoTracking()
                .ToListAsync();

            int totalOrders = orders.Count;

            decimal totalRevenue = orders.Sum(o => o.Total);

            var bestSellerGroup = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => new
                {
                    i.ProductId,
                    ProductName = i.Product != null
                        ? i.Product.ProductName
                        : $"SP #{i.ProductId}"
                })
                .Select(g => new
                {
                    g.Key.ProductName,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalAmount = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .FirstOrDefault();

            var topProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => new { i.ProductId, ProductName = i.Product?.ProductName ?? $"SP #{i.ProductId}" })
                .Select(g => new { g.Key.ProductId, g.Key.ProductName, Quantity = g.Sum(i => i.Quantity), Revenue = g.Sum(i => i.LineTotal) })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToList();

            string bestSeller = bestSellerGroup != null
                ? $"{bestSellerGroup.ProductName} ({bestSellerGroup.TotalQuantity} sp)"
                : "Chưa có";

            var recentOrders = orders
                .OrderByDescending(o => o.CreatedAt)
                .Take(20)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderCode,
                    o.CreatedAt,
                    o.Total,
                    o.Status,
                    CustomerName = o.Customer != null
                        ? o.Customer.CustomerName
                        : "Khách vãng lai"
                })
                .ToList();

            return Ok(new
            {
                Date = startDate.ToString("yyyy-MM-dd"),
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                BestSeller = bestSeller,
                TopProducts = topProducts,
                RecentOrders = recentOrders
            });
        }

        [HttpGet("summary")]
        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetSummary([FromQuery] DateTime? date = null)
        {
            var day = (date ?? DateTime.Today).Date;
            var nextDay = day.AddDays(1);
            var paidOrders = _db.Orders.AsNoTracking()
                .Where(o => o.Status == "PAID" && o.CreatedAt >= day && o.CreatedAt < nextDay);

            return Ok(new
            {
                Date = day.ToString("yyyy-MM-dd"),
                Revenue = await paidOrders.SumAsync(o => (decimal?)o.Total) ?? 0m,
                InvoiceCount = await paidOrders.CountAsync(),
                NewCustomers = await _db.Customers.AsNoTracking()
                    .CountAsync(c => c.CreatedAt >= day && c.CreatedAt < nextDay),
                LowStockCount = await _db.Products.AsNoTracking().CountAsync(p => p.StockQuantity <= 10)
            });
        }

        [HttpGet("revenue-series")]
        public async Task<IActionResult> GetRevenueSeries([FromQuery] DateTime from, [FromQuery] DateTime to,
            [FromQuery] string period = "day")
        {
            if (to.Date < from.Date) return BadRequest(new { message = "Ngày kết thúc phải sau ngày bắt đầu." });
            period = period.Trim().ToLowerInvariant();
            if (period is not ("day" or "week" or "month"))
                return BadRequest(new { message = "Kỳ báo cáo phải là day, week hoặc month." });

            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                return Problem("Chưa cấu hình chuỗi kết nối cơ sở dữ liệu.");

            var endExclusive = to.Date.AddDays(1);
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("dbo.sp_RevenueSeries", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@StartAt", System.Data.SqlDbType.DateTime2).Value = from.Date;
            command.Parameters.Add("@EndAt", System.Data.SqlDbType.DateTime2).Value = endExclusive;
            command.Parameters.Add("@Period", System.Data.SqlDbType.NVarChar, 10).Value = period;

            var points = new List<RevenuePointResponseDto>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                points.Add(new RevenuePointResponseDto
                {
                    Date = reader.GetDateTime(0),
                    Revenue = reader.GetDecimal(1)
                });
            }
            return Ok(points);
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null, [FromQuery] int take = 5)
        {
            var start = (from ?? DateTime.Today).Date;
            var end = (to ?? start).Date.AddDays(1);
            take = Math.Clamp(take, 1, 20);
            var products = await _db.OrderItems.AsNoTracking()
                .Where(i => i.Order != null && i.Order.Status == "PAID" &&
                            i.Order.CreatedAt >= start && i.Order.CreatedAt < end)
                .GroupBy(i => new { i.ProductId, ProductName = i.Product!.ProductName })
                .Select(g => new { g.Key.ProductId, g.Key.ProductName, Quantity = g.Sum(i => i.Quantity), Revenue = g.Sum(i => i.LineTotal) })
                .OrderByDescending(x => x.Quantity).Take(take).ToListAsync();
            return Ok(products);
        }

        [HttpGet("export/products")]
        public async Task<IActionResult> ExportProducts()
        {
            var rows = await _db.Products.AsNoTracking().Include(p => p.Category)
                .OrderBy(p => p.ProductName).ToListAsync();
            var csv = new StringBuilder("ProductId,Barcode,ProductName,Category,Price,StockQuantity\r\n");
            foreach (var p in rows)
                csv.AppendLine(string.Join(',', new[]
                {
                    Csv(p.ProductId.ToString(CultureInfo.InvariantCulture)), Csv(p.Barcode), Csv(p.ProductName),
                    Csv(p.Category?.CategoryName ?? ""), Csv(p.Price.ToString(CultureInfo.InvariantCulture)),
                    Csv(p.StockQuantity.ToString(CultureInfo.InvariantCulture))
                }));
            return CsvFile(csv.ToString(), "products.csv");
        }

        [HttpGet("export/orders")]
        public async Task<IActionResult> ExportOrders([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var start = (from ?? DateTime.Today).Date;
            var end = (to ?? start).Date.AddDays(1);
            var rows = await _db.Orders.AsNoTracking().Include(o => o.Customer)
                .Where(o => o.CreatedAt >= start && o.CreatedAt < end)
                .OrderBy(o => o.CreatedAt).ToListAsync();
            var csv = new StringBuilder("OrderId,OrderCode,CreatedAt,Customer,Status,Total\r\n");
            foreach (var o in rows)
                csv.AppendLine(string.Join(',', new[]
                {
                    Csv(o.OrderId.ToString(CultureInfo.InvariantCulture)), Csv(o.OrderCode),
                    Csv(o.CreatedAt.ToString("s", CultureInfo.InvariantCulture)),
                    Csv(o.Customer?.CustomerName ?? "Khách vãng lai"), Csv(o.Status),
                    Csv(o.Total.ToString(CultureInfo.InvariantCulture))
                }));
            return CsvFile(csv.ToString(), "orders.csv");
        }

        [HttpGet("revenue-by-shift")]
        public async Task<IActionResult> GetRevenueByShift([FromQuery] DateTime startAt, [FromQuery] DateTime endAt)
        {
            if (endAt <= startAt) return BadRequest(new { message = "Khoảng thời gian không hợp lệ." });
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString)) return Problem("Chưa cấu hình chuỗi kết nối.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand("dbo.sp_RevenueByShift", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@StartAt", startAt);
            command.Parameters.AddWithValue("@EndAt", endAt);
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return Ok(new { InvoiceCount = 0, Revenue = 0m });
            return Ok(new { InvoiceCount = reader.GetInt32(0), Revenue = reader.GetDecimal(1) });
        }

        private static string Csv(string value)
        {
            if (value.Length > 0 && "=+-@\t\r".Contains(value[0])) value = "'" + value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private FileContentResult CsvFile(string value, string fileName)
        {
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(value)).ToArray();
            return File(bytes, "text/csv; charset=utf-8", fileName);
        }
    }

}
