namespace BusinessApp.Application.DTOs;

public class DashboardDto
{
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int OutOfStockProducts { get; set; }
    public int TotalCategories { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<CategoryStatsDto> CategoryStats { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class CategoryStatsDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalValue { get; set; }
    public string? Color { get; set; }
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // Customer, Product, etc.
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
}
