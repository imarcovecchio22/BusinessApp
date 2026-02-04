using BusinessApp.Domain.Common;

namespace BusinessApp.Domain.Entities;

public enum ProductType
{
    Product = 1,
    Service = 2
}

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Sku { get; set; } // Stock Keeping Unit
    public decimal Price { get; set; }
    public decimal? Cost { get; set; }
    public ProductType Type { get; set; } = ProductType.Product;
    public int Stock { get; set; } = 0;
    public int? MinStock { get; set; } // Alert threshold
    public bool IsActive { get; set; } = true;
    public string? ImageUrl { get; set; }
    
    // Foreign Keys
    public Guid? CategoryId { get; set; }
    
    // Navigation properties
    public virtual Category? Category { get; set; }
    
    // Computed properties
    public bool IsLowStock => MinStock.HasValue && Stock <= MinStock.Value;
    public bool IsOutOfStock => Type == ProductType.Product && Stock <= 0;
    public decimal? ProfitMargin => Cost.HasValue && Cost > 0 ? ((Price - Cost.Value) / Cost.Value) * 100 : null;
}
