using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    public class ProductInput
    {
        [Required, StringLength(50)]
        public string Barcode { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Range(0, int.MaxValue)]
        public int BrandId { get; set; }
    }

    // DTO cho phân trang sản phẩm
    public class ProductItemDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    // Lớp gói kết quả trả về phân trang
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _db;
        private readonly IConfiguration _configuration;
        public ProductsController(SupermarketDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        private static ProductResponseDto Map(Product p) => new()
        {
            ProductId = p.ProductId,
            Barcode = p.Barcode,
            ProductName = p.ProductName,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            CategoryId = p.CategoryId,
            BrandId = p.BrandId,
            CategoryName = p.Category?.CategoryName ?? string.Empty,
            Category = p.Category == null ? null : new CategoryResponseDto
            {
                CategoryId = p.Category.CategoryId,
                CategoryName = p.Category.CategoryName,
                Description = p.Category.Description
            }
        };

        // GET api/products?keyword=&categoryId=
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int? categoryId)
        {
            var q = _db.Products.Include(p => p.Category).AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                q = q.Where(p => p.ProductName.Contains(keyword) || p.Barcode.Contains(keyword));
            }
            if (categoryId > 0) q = q.Where(p => p.CategoryId == categoryId);
            var list = await q.OrderBy(p => p.ProductId).ToListAsync();
            return Ok(list.Select(Map));
        }

        // GET: api/products/paged?keyword=sua&pageNumber=1&pageSize=5&categoryId=1
        [HttpGet("paged")]
        public async Task<IActionResult> GetPagedProducts(
            [FromQuery] string? keyword,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] int? categoryId = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 5;

            var query = _db.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(search)
                                      || p.Barcode.Contains(search));
            }

            // Lọc theo nhóm hàng nếu người dùng chọn một nhóm cụ thể (> 0)
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = await query
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
                })
                .ToListAsync();

            return Ok(new PagedResult<ProductItemDto>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            });
        }

        // Bài tập 6.2: GET api/products/paged-by-category?categoryId=1&pageNumber=1&pageSize=5
        [HttpGet("paged-by-category")]
        public Task<IActionResult> GetProductsPagedByCategory(
            [FromQuery] int? categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 5)
            => GetPagedProducts(null, pageNumber, pageSize, categoryId);

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _db.Products.Include(x => x.Category).AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == id);
            return p == null ? NotFound(new { message = "Không tìm thấy sản phẩm!" }) : Ok(Map(p));
        }

        // Dùng cho POS quét mã vạch
        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                return Problem("Chưa cấu hình chuỗi kết nối cơ sở dữ liệu.");

            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = new SqlCommand(
                @"SELECT p.ProductId, p.Barcode, p.ProductName, p.Price, p.StockQuantity,
                         p.CategoryId, c.CategoryName
                  FROM Products AS p
                  LEFT JOIN Categories AS c ON c.CategoryId = p.CategoryId
                  WHERE p.Barcode = @barcode", connection);
            command.Parameters.AddWithValue("@barcode", barcode.Trim());

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFound(new { message = "Không tìm thấy sản phẩm!" });

            return Ok(new ProductItemDto
            {
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Barcode = reader.GetString(reader.GetOrdinal("Barcode")),
                ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                StockQuantity = reader.GetInt32(reader.GetOrdinal("StockQuantity")),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName"))
                    ? string.Empty : reader.GetString(reader.GetOrdinal("CategoryName"))
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create([FromBody] ProductInput i)
        {
            if (string.IsNullOrWhiteSpace(i.Barcode) || string.IsNullOrWhiteSpace(i.ProductName))
                return BadRequest(new { message = "Mã vạch và tên sản phẩm không được trống!" });
            if (await _db.Products.AnyAsync(p => p.Barcode == i.Barcode))
                return BadRequest(new { message = "Mã vạch này đã tồn tại!" });
            if (!await _db.Categories.AnyAsync(c => c.CategoryId == i.CategoryId))
                return BadRequest(new { message = "Nhóm hàng không hợp lệ!" });

            int brandId = i.BrandId;
            if (brandId > 0 && !await _db.Brands.AnyAsync(b => b.BrandId == brandId))
                return BadRequest(new { message = "Thương hiệu không hợp lệ!" });
            if (brandId <= 0)
                brandId = await _db.Brands.OrderBy(b => b.BrandId).Select(b => b.BrandId).FirstOrDefaultAsync();
            if (brandId <= 0)
                return BadRequest(new { message = "Cần tạo ít nhất một thương hiệu trước khi thêm sản phẩm!" });

            var p = new Product
            {
                Barcode = i.Barcode.Trim(),
                ProductName = i.ProductName.Trim(),
                Price = i.Price,
                StockQuantity = i.StockQuantity,
                CategoryId = i.CategoryId,
                BrandId = brandId
            };
            _db.Products.Add(p);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = p.ProductId }, new { p.ProductId });
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductInput i)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm!" });
            if (string.IsNullOrWhiteSpace(i.Barcode) || string.IsNullOrWhiteSpace(i.ProductName))
                return BadRequest(new { message = "Mã vạch và tên sản phẩm không được trống!" });
            if (await _db.Products.AnyAsync(x => x.Barcode == i.Barcode && x.ProductId != id))
                return BadRequest(new { message = "Mã vạch bị trùng với sản phẩm khác!" });
            if (!await _db.Categories.AnyAsync(c => c.CategoryId == i.CategoryId))
                return BadRequest(new { message = "Nhóm hàng không hợp lệ!" });
            if (i.BrandId > 0 && !await _db.Brands.AnyAsync(b => b.BrandId == i.BrandId))
                return BadRequest(new { message = "Thương hiệu không hợp lệ!" });

            p.Barcode = i.Barcode.Trim();
            p.ProductName = i.ProductName.Trim();
            p.Price = i.Price;
            p.StockQuantity = i.StockQuantity;
            p.CategoryId = i.CategoryId;
            if (i.BrandId > 0 && await _db.Brands.AnyAsync(b => b.BrandId == i.BrandId)) p.BrandId = i.BrandId;
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound(new { message = "Không tìm thấy sản phẩm!" });
            if (await _db.OrderItems.AnyAsync(o => o.ProductId == id))
                return BadRequest(new { message = "Sản phẩm đã có trong hóa đơn, không thể xóa!" });
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
