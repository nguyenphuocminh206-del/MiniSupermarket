using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace MiniSupermarket.API.Controllers
{
    public class CheckoutItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CheckoutRequest
    {
        public string? CashierUsername { get; set; }
        public string? CustomerPhone { get; set; }
        public decimal CashReceived { get; set; }
        public int PointsToRedeem { get; set; }
        public List<CheckoutItem> Items { get; set; } = new();
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly SupermarketDbContext _db;
        public OrdersController(SupermarketDbContext db) => _db = db;

        private static OrderResponseDto Map(Order o) => new()
        {
            OrderId = o.OrderId,
            OrderCode = o.OrderCode,
            CustomerId = o.CustomerId ?? 0,
            OrderDate = o.CreatedAt,
            TotalAmount = o.Total,
            Status = o.Status,
            OrderDetails = o.Items.Select(i => new OrderDetailResponseDto
            {
                OrderDetailId = i.OrderItemId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        private static async Task<IActionResult> RollbackAndReturnAsync(
            IDbContextTransaction transaction,
            IActionResult response)
        {
            await transaction.RollbackAsync();
            return response;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Orders.Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt).AsNoTracking().ToListAsync();
            return Ok(list.Select(Map));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var o = await _db.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(x => x.OrderId == id);
            return o == null ? NotFound(new { message = "Không tìm thấy đơn hàng!" }) : Ok(Map(o));
        }

        // POS thanh toán
        [HttpPost("checkout")]
        [Authorize(Roles = "Admin,Cashier")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            if (req.Items == null || req.Items.Count == 0)
                return BadRequest(new { message = "Giỏ hàng trống!" });

            if (req.PointsToRedeem < 0)
                return BadRequest(new { message = "Số điểm sử dụng không hợp lệ!" });

            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var cashierUsername = User.Identity?.Name ?? req.CashierUsername;
                var cashier = string.IsNullOrWhiteSpace(cashierUsername)
                    ? null
                    : await _db.Users.FirstOrDefaultAsync(u => u.Username == cashierUsername);

                // Tài khoản dự phòng admin/cashier không có bản ghi cùng tên trong bảng Users.
                if (cashier == null &&
                    (string.Equals(cashierUsername, "admin", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(cashierUsername, "cashier", StringComparison.OrdinalIgnoreCase)))
                {
                    var fallbackRole = string.Equals(cashierUsername, "admin", StringComparison.OrdinalIgnoreCase)
                        ? "ADMIN"
                        : "CASHIER";
                    cashier = await _db.Users.Include(u => u.Role)
                        .Where(u => u.Role != null && u.Role.RoleName == fallbackRole)
                        .OrderBy(u => u.UserId).FirstOrDefaultAsync();
                }

                if (cashier == null)
                    return await RollbackAndReturnAsync(transaction,
                        BadRequest(new { message = "Không xác định được thu ngân đang đăng nhập." }));

                Customer? customer = null;
                if (!string.IsNullOrWhiteSpace(req.CustomerPhone))
                    customer = await _db.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == req.CustomerPhone.Trim());

                if (req.PointsToRedeem > 0 && customer == null)
                    return await RollbackAndReturnAsync(transaction,
                        BadRequest(new { message = "Cần chọn khách hàng để sử dụng điểm thưởng!" }));
                if (customer != null && req.PointsToRedeem > customer.RewardPoints)
                    return await RollbackAndReturnAsync(transaction,
                        BadRequest(new { message = "Khách hàng không đủ điểm thưởng!" }));

                var ids = req.Items.Select(i => i.ProductId).Distinct().ToList();
                var products = await _db.Products.Where(p => ids.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId);

                var order = new Order
                {
                    CashierId = cashier.UserId,
                    CustomerId = customer?.CustomerId,
                    PaymentMethod = "CASH",
                    Status = "PAID",
                    CreatedAt = DateTime.Now,
                    OrderCode = "TMP",
                    PointsRedeemed = req.PointsToRedeem,
                    InventoryDeducted = true
                };

                foreach (var line in req.Items.GroupBy(i => i.ProductId)
                             .Select(g => new { ProductId = g.Key, Quantity = g.Sum(i => i.Quantity) }))
                {
                    if (line.Quantity <= 0)
                        return await RollbackAndReturnAsync(transaction,
                            BadRequest(new { message = "Số lượng không hợp lệ!" }));
                    if (!products.TryGetValue(line.ProductId, out var p))
                        return await RollbackAndReturnAsync(transaction,
                            BadRequest(new { message = $"Sản phẩm #{line.ProductId} không tồn tại!" }));
                    if (p.StockQuantity < line.Quantity)
                        return await RollbackAndReturnAsync(transaction,
                            BadRequest(new { message = $"'{p.ProductName}' chỉ còn {p.StockQuantity} trong kho!" }));

                    p.StockQuantity -= line.Quantity;
                    order.Items.Add(new OrderItem
                    {
                        ProductId = p.ProductId,
                        Quantity = line.Quantity,
                        UnitPrice = p.Price,
                        LineTotal = p.Price * line.Quantity
                    });
                }

                order.Subtotal = order.Items.Sum(i => i.LineTotal);
                order.Discount = req.PointsToRedeem * 1000m;
                if (order.Discount > order.Subtotal)
                    return await RollbackAndReturnAsync(transaction,
                        BadRequest(new { message = "Số điểm đổi vượt quá giá trị hóa đơn!" }));
                order.Total = order.Subtotal - order.Discount;
                order.PointsEarned = (int)(order.Total / 10000m);

                if (req.CashReceived < order.Total)
                    return await RollbackAndReturnAsync(transaction,
                        BadRequest(new { message = "Tiền khách đưa chưa đủ!" }));

                if (customer != null)
                    customer.RewardPoints = customer.RewardPoints - order.PointsRedeemed + order.PointsEarned;

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                order.OrderCode = $"HD{order.OrderId:D5}";
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new CheckoutResponseDto
                {
                    OrderId = order.OrderId,
                    OrderCode = order.OrderCode,
                    Subtotal = order.Subtotal,
                    Discount = order.Discount,
                    Total = order.Total,
                    PointsRedeemed = order.PointsRedeemed,
                    Change = req.CashReceived - order.Total
                });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var requestedStatus = (status ?? string.Empty).Trim();
            var cancel = requestedStatus.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase)
                         || requestedStatus.Equals("CANCELED", StringComparison.OrdinalIgnoreCase)
                         || requestedStatus.Equals("Đã hủy", StringComparison.OrdinalIgnoreCase)
                         || requestedStatus.Equals("Da huy", StringComparison.OrdinalIgnoreCase);
            var approve = requestedStatus.Equals("APPROVED", StringComparison.OrdinalIgnoreCase)
                          || requestedStatus.Equals("Đã duyệt", StringComparison.OrdinalIgnoreCase)
                          || requestedStatus.Equals("Da duyet", StringComparison.OrdinalIgnoreCase);
            if (!cancel && !approve)
                return BadRequest(new { message = "Chỉ chấp nhận trạng thái duyệt hoặc hủy đơn." });

            await using var transaction = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var order = await _db.Orders.Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng!" });

            var currentStatus = order.Status.Trim();
            var isCancelled = currentStatus.Equals("CANCELLED", StringComparison.OrdinalIgnoreCase)
                              || currentStatus.Equals("CANCELED", StringComparison.OrdinalIgnoreCase)
                              || currentStatus.Contains("hủy", StringComparison.OrdinalIgnoreCase)
                              || currentStatus.Contains("huy", StringComparison.OrdinalIgnoreCase);
            var isPending = currentStatus.Equals("PENDING", StringComparison.OrdinalIgnoreCase)
                            || currentStatus.Equals("CHỜ XỬ LÝ", StringComparison.OrdinalIgnoreCase)
                            || currentStatus.Equals("Cho xu ly", StringComparison.OrdinalIgnoreCase);
            var isPaid = currentStatus.Equals("PAID", StringComparison.OrdinalIgnoreCase)
                         || currentStatus.Equals("APPROVED", StringComparison.OrdinalIgnoreCase)
                         || currentStatus.Equals("Đã duyệt", StringComparison.OrdinalIgnoreCase)
                         || currentStatus.Equals("Da duyet", StringComparison.OrdinalIgnoreCase);

            if (isCancelled)
                return Conflict(new { message = "Đơn hàng đã bị hủy trước đó." });
            if (approve && !isPending)
                return Conflict(new { message = "Chỉ có thể duyệt đơn đang chờ xử lý." });
            if (cancel && !isPending && !isPaid)
                return Conflict(new { message = "Trạng thái đơn hàng hiện tại không thể hủy." });

            if (cancel && order.InventoryDeducted)
            {
                var productIds = order.Items.Select(item => item.ProductId).Distinct().ToList();
                var products = await _db.Products.Where(product => productIds.Contains(product.ProductId))
                    .ToDictionaryAsync(product => product.ProductId);
                if (order.Items.Any(item => !products.ContainsKey(item.ProductId)))
                    return Conflict(new { message = "Không thể hoàn kho: thiếu sản phẩm trong chi tiết đơn." });

                if (order.Items.Any(item => item.Quantity > int.MaxValue - products[item.ProductId].StockQuantity))
                    return Conflict(new { message = "Không thể hoàn kho vì số lượng vượt giới hạn tồn kho." });

                foreach (var item in order.Items)
                    products[item.ProductId].StockQuantity += item.Quantity;

                if (order.CustomerId.HasValue)
                {
                    var customer = await _db.Customers.FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId.Value);
                    if (customer != null)
                    {
                        var restoredPoints = (long)customer.RewardPoints + order.PointsRedeemed - order.PointsEarned;
                        if (restoredPoints < 0 || restoredPoints > int.MaxValue)
                            return Conflict(new { message = "Không thể hủy vì khách đã dùng số điểm thưởng của hóa đơn này." });
                        customer.RewardPoints = (int)restoredPoints;
                    }
                }

                order.InventoryDeducted = false;
            }

            order.Status = cancel ? "CANCELLED" : "Đã duyệt";
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng!" });
            if (order.InventoryDeducted)
                return Conflict(new { message = "Hãy hủy đơn trước khi xóa để hoàn kho và điểm thưởng." });
            _db.OrderItems.RemoveRange(order.Items);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
