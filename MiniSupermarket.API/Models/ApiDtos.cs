namespace MiniSupermarket.API.Models
{
    public sealed class BrandRequestDto
    {
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
    }

    public sealed class BrandResponseDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
    }

    public sealed class RoleRequestDto { public string RoleName { get; set; } = string.Empty; }
    public sealed class RoleResponseDto { public int RoleId { get; set; } public string RoleName { get; set; } = string.Empty; }

    public sealed class CategoryRequestDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public sealed class CategoryResponseDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public sealed class ProductResponseDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public CategoryResponseDto? Category { get; set; }
        public string Unit { get; set; } = "cái";
        public bool IsActive { get; set; } = true;
    }

    public sealed class CustomerRequestDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; } = "Chuẩn";
    }

    public sealed class CustomerResponseDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int RewardPoints { get; set; }
        public string MembershipRank { get; set; } = "Chuẩn";
        public DateTime? CreatedAt { get; set; }
    }

    public sealed class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderDetailResponseDto> OrderDetails { get; set; } = new();
    }

    public sealed class OrderDetailResponseDto
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public sealed class CheckoutResponseDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int PointsRedeemed { get; set; }
        public decimal Change { get; set; }
    }

    public sealed class RevenuePointResponseDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }
}
