using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using BusinessApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessApp.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var dashboard = new DashboardDto
        {
            TotalCustomers = await _context.Customers.CountAsync(),
            ActiveCustomers = await _context.Customers.CountAsync(c => c.IsActive),
            TotalProducts = await _context.Products.CountAsync(),
            LowStockProducts = await _context.Products
                .CountAsync(p => p.Type == ProductType.Product && 
                               p.MinStock.HasValue && 
                               p.Stock <= p.MinStock.Value && 
                               p.Stock > 0),
            OutOfStockProducts = await _context.Products
                .CountAsync(p => p.Type == ProductType.Product && p.Stock <= 0),
            TotalCategories = await _context.Categories.CountAsync(c => c.IsActive)
        };

        // Calculate total inventory value
        var products = await _context.Products
            .Where(p => p.Type == ProductType.Product && p.IsActive)
            .ToListAsync();
        
        dashboard.TotalInventoryValue = products.Sum(p => p.Price * p.Stock);

        // Category stats
        dashboard.CategoryStats = await _context.Categories
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .Select(c => new CategoryStatsDto
            {
                CategoryName = c.Name,
                ProductCount = c.Products.Count(p => p.IsActive),
                TotalValue = c.Products.Where(p => p.IsActive).Sum(p => p.Price * p.Stock),
                Color = c.Color
            })
            .OrderByDescending(c => c.ProductCount)
            .Take(5)
            .ToListAsync();

        // Recent activities
        var recentCustomers = await _context.Customers
            .OrderByDescending(c => c.CreatedAt)
            .Take(3)
            .ToListAsync();

        var recentProducts = await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();

        dashboard.RecentActivities = new List<RecentActivityDto>();

        foreach (var customer in recentCustomers)
        {
            dashboard.RecentActivities.Add(new RecentActivityDto
            {
                Type = "Customer",
                Description = $"Nuevo cliente: {customer.FullName}",
                Timestamp = customer.CreatedAt,
                Icon = "bi-person-plus"
            });
        }

        foreach (var product in recentProducts)
        {
            dashboard.RecentActivities.Add(new RecentActivityDto
            {
                Type = "Product",
                Description = $"Nuevo producto: {product.Name}",
                Timestamp = product.CreatedAt,
                Icon = "bi-box-seam"
            });
        }

        dashboard.RecentActivities = dashboard.RecentActivities
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .ToList();

        return dashboard;
    }
}
