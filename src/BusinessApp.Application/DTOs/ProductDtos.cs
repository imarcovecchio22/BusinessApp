using BusinessApp.Domain.Entities;

namespace BusinessApp.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public ProductType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public int Stock { get; set; }
    public int? MinStock { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsOutOfStock { get; set; }
    public decimal? ProfitMargin { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public ProductType Type { get; set; } = ProductType.Product;
    public int Stock { get; set; } = 0;
    public int? MinStock { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sku { get; set; }
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public ProductType Type { get; set; }
    public int Stock { get; set; }
    public int? MinStock { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? CategoryId { get; set; }
}

public class ProductSearchDto
{
    public string? SearchTerm { get; set; }
    public ProductType? Type { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    public bool? LowStock { get; set; }
}
