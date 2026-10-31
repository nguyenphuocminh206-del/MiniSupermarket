using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Màn hình quản lý khách hàng chỉ dành cho Admin
    public class CustomersController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        // Tiêm DbContext thông qua Constructor Injection
        public CustomersController(SupermarketDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // 1. GET: Lấy toàn bộ danh sách khách hàng
        // GET /api/customers
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .Select(c => new CustomerResponseDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    RewardPoints = c.RewardPoints,
                    MembershipRank = c.MembershipRank,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            return Ok(customers);
        }

        // =========================================================
        // 1b. GET: Phân trang khách hàng, xếp theo điểm thưởng giảm dần (Bài tập 6.1)
        // GET /api/customers/paged?pageNumber=1&pageSize=5
        // =========================================================
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedCustomers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 5;

            var query = _context.Customers.AsNoTracking().AsQueryable();

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = await query
                .OrderByDescending(c => c.RewardPoints) // Vinh danh khách hàng VIP
                .ThenBy(c => c.CustomerId)              // Giữ thứ tự ổn định khi trùng điểm
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerResponseDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    RewardPoints = c.RewardPoints,
                    MembershipRank = c.MembershipRank,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(new PagedResult<CustomerResponseDto>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            });
        }

        // =========================================================
        // 2. GET: Lấy chi tiết khách hàng theo ID
        // GET /api/customers/{id}
        // =========================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerResponseDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    RewardPoints = c.RewardPoints,
                    MembershipRank = c.MembershipRank,
                    CreatedAt = c.CreatedAt
                }).FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy khách hàng!"
                });
            }

            return Ok(customer);
        }

        // =========================================================
        // 3. SEARCH: Tìm kiếm theo tên hoặc số điện thoại
        // GET /api/customers/search?keyword=Nguyen
        // GET /api/customers/search?keyword=0901122334
        // =========================================================
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new
                {
                    message = "Vui lòng nhập từ khóa tìm kiếm!"
                });
            }

            var customers = await _context.Customers
                .AsNoTracking()
                .Where(c =>
                    c.CustomerName.Contains(keyword) ||
                    c.PhoneNumber.Contains(keyword))
                .Select(c => new CustomerResponseDto
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    RewardPoints = c.RewardPoints,
                    MembershipRank = c.MembershipRank,
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            return Ok(customers);
        }

        // =========================================================
        // 4. POST: Thêm mới khách hàng
        // POST /api/customers
        // =========================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(request.CustomerName))
            {
                return BadRequest(new
                {
                    message = "Tên khách hàng không được để trống!"
                });
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest(new
                {
                    message = "Số điện thoại không được để trống!"
                });
            }

            // Kiểm tra số điện thoại đã tồn tại
            var phoneExists = await _context.Customers
                .AnyAsync(c => c.PhoneNumber == request.PhoneNumber);

            if (phoneExists)
            {
                return Conflict(new
                {
                    message = "Số điện thoại đã được đăng ký!"
                });
            }

            // Giá trị mặc định
            var newCustomer = new Customer
            {
                CustomerName = request.CustomerName.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                Address = request.Address,
                RewardPoints = 0,
                MembershipRank = "Chuẩn",
                CreatedAt = DateTime.Now
            };

            _context.Customers.Add(newCustomer);

            // Lưu vào SQL Server
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = newCustomer.CustomerId },
                new CustomerResponseDto
                {
                    CustomerId = newCustomer.CustomerId,
                    CustomerName = newCustomer.CustomerName,
                    PhoneNumber = newCustomer.PhoneNumber,
                    Address = newCustomer.Address,
                    RewardPoints = newCustomer.RewardPoints,
                    MembershipRank = newCustomer.MembershipRank,
                    CreatedAt = newCustomer.CreatedAt
                }
            );
        }

        // =========================================================
        // 5. PUT: Cập nhật thông tin khách hàng
        // PUT /api/customers/{id}
        // =========================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] CustomerRequestDto updateCustomer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy khách hàng cần cập nhật!"
                });
            }

            if (string.IsNullOrWhiteSpace(updateCustomer.CustomerName))
            {
                return BadRequest(new
                {
                    message = "Tên khách hàng không được để trống!"
                });
            }

            if (string.IsNullOrWhiteSpace(updateCustomer.PhoneNumber))
            {
                return BadRequest(new
                {
                    message = "Số điện thoại không được để trống!"
                });
            }

            // Kiểm tra số điện thoại có bị trùng với khách hàng khác
            var phoneExists = await _context.Customers
                .AnyAsync(c =>
                    c.PhoneNumber == updateCustomer.PhoneNumber &&
                    c.CustomerId != id);

            if (phoneExists)
            {
                return Conflict(new
                {
                    message = "Số điện thoại đã được sử dụng bởi khách hàng khác!"
                });
            }

            // Cập nhật thông tin
            customer.CustomerName = updateCustomer.CustomerName;
            customer.PhoneNumber = updateCustomer.PhoneNumber;
            customer.Address = updateCustomer.Address;
            customer.MembershipRank = updateCustomer.MembershipRank;
            customer.RewardPoints = updateCustomer.RewardPoints;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // =========================================================
        // 6. DELETE: Xóa khách hàng
        // DELETE /api/customers/{id}
        // =========================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy khách hàng cần xóa!"
                });
            }

            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
