using BusinessApp.Domain.Common;

namespace BusinessApp.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; } // Bootstrap icon class
    public string? Color { get; set; } // Hex color code
    public bool IsActive { get; set; } = true;
    
    // Navigation property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
