using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        private readonly SupermarketDbContext _db;

        public BrandsController(SupermarketDbContext db)
        {
            _db = db;
        }

        // GET: api/brands
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _db.Brands
                .AsNoTracking()
                .OrderBy(b => b.BrandId)
                .Select(b => new BrandResponseDto { BrandId = b.BrandId, BrandName = b.BrandName, Description = b.Description, LogoUrl = b.LogoUrl })
                .ToListAsync();

            return Ok(brands);
        }

        // GET: api/brands/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _db.Brands.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BrandId == id);

            return brand == null
                ? NotFound(new { message = "Không tìm thấy thương hiệu!" })
                : Ok(new BrandResponseDto { BrandId = brand.BrandId, BrandName = brand.BrandName, Description = brand.Description, LogoUrl = brand.LogoUrl });
        }

        // GET: api/brands/search?keyword=...
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Vui lòng nhập từ khóa!" });

            var brands = await _db.Brands.AsNoTracking()
                .Where(b => b.BrandName.Contains(keyword.Trim()))
                .OrderBy(b => b.BrandId)
                .Select(b => new BrandResponseDto { BrandId = b.BrandId, BrandName = b.BrandName, Description = b.Description, LogoUrl = b.LogoUrl })
                .ToListAsync();

            return Ok(brands);
        }

        // POST: api/brands
        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create([FromBody] BrandRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.BrandName))
                return BadRequest(new { message = "Tên thương hiệu không được để trống!" });

            var newBrand = new Brand
            {
                BrandName = request.BrandName.Trim(),
                Description = request.Description,
                LogoUrl = request.LogoUrl
            };
            _db.Brands.Add(newBrand);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newBrand.BrandId },
                new BrandResponseDto { BrandId = newBrand.BrandId, BrandName = newBrand.BrandName, Description = newBrand.Description, LogoUrl = newBrand.LogoUrl });
        }

        // PUT: api/brands/5
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] BrandRequestDto updateBrand)
        {
            if (string.IsNullOrWhiteSpace(updateBrand.BrandName))
                return BadRequest(new { message = "Tên thương hiệu không được để trống!" });

            var brand = await _db.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new { message = "Không tìm thấy thương hiệu cần sửa!" });

            brand.BrandName = updateBrand.BrandName.Trim();
            brand.Description = updateBrand.Description;
            brand.LogoUrl = updateBrand.LogoUrl;
            await _db.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/brands/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _db.Brands.FindAsync(id);
            if (brand == null)
                return NotFound(new { message = "Không tìm thấy thương hiệu cần xóa!" });

            if (await _db.Products.AnyAsync(p => p.BrandId == id))
                return Conflict(new { message = "Không thể xóa thương hiệu đang được sản phẩm sử dụng!" });

            _db.Brands.Remove(brand);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
