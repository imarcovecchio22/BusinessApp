using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using BusinessApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessApp.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        return MapToDto(product);
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> SearchAsync(ProductSearchDto searchDto)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.SearchTerm))
        {
            var term = searchDto.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(term) ||
                (p.Description != null && p.Description.ToLower().Contains(term)) ||
                (p.Sku != null && p.Sku.ToLower().Contains(term))
            );
        }

        if (searchDto.Type.HasValue)
        {
            query = query.Where(p => p.Type == searchDto.Type.Value);
        }

        if (searchDto.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);
        }

        if (searchDto.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == searchDto.IsActive.Value);
        }

        if (searchDto.LowStock == true)
        {
            query = query.Where(p => p.Type == ProductType.Product && 
                                    p.MinStock.HasValue && 
                                    p.Stock <= p.MinStock.Value);
        }

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Sku = createDto.Sku,
            Price = createDto.Price,
            Cost = createDto.Cost,
            Type = createDto.Type,
            Stock = createDto.Stock,
            MinStock = createDto.MinStock,
            ImageUrl = createDto.ImageUrl,
            CategoryId = createDto.CategoryId,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Reload to get category
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            throw new Exception("Producto no encontrado");
        }

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Sku = updateDto.Sku;
        product.Price = updateDto.Price;
        product.Cost = updateDto.Cost;
        product.Type = updateDto.Type;
        product.Stock = updateDto.Stock;
        product.MinStock = updateDto.MinStock;
        product.IsActive = updateDto.IsActive;
        product.ImageUrl = updateDto.ImageUrl;
        product.CategoryId = updateDto.CategoryId;

        await _context.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateStockAsync(Guid id, int quantity)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        if (product.Type == ProductType.Service)
        {
            throw new Exception("No se puede actualizar stock de servicios");
        }

        product.Stock += quantity;

        if (product.Stock < 0)
        {
            throw new Exception("Stock no puede ser negativo");
        }

        await _context.SaveChangesAsync();

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price,
            Cost = product.Cost,
            Type = product.Type,
            TypeName = product.Type == ProductType.Product ? "Producto" : "Servicio",
            Stock = product.Stock,
            MinStock = product.MinStock,
            IsActive = product.IsActive,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            IsLowStock = product.IsLowStock,
            IsOutOfStock = product.IsOutOfStock,
            ProfitMargin = product.ProfitMargin,
            CreatedAt = product.CreatedAt
        };
    }
}
