using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using BusinessApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BusinessApp.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return null;

        return MapToDto(category);
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(MapToDto).ToList();
    }

    public async Task<List<CategoryDto>> GetActiveAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto createDto)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Icon = createDto.Icon,
            Color = createDto.Color,
            IsActive = true
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto updateDto)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            throw new Exception("Categoría no encontrada");
        }

        category.Name = updateDto.Name;
        category.Description = updateDto.Description;
        category.Icon = updateDto.Icon;
        category.Color = updateDto.Color;
        category.IsActive = updateDto.IsActive;

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return false;

        if (category.Products.Any())
        {
            throw new Exception("No se puede eliminar una categoría con productos asociados");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            Color = category.Color,
            IsActive = category.IsActive,
            ProductCount = category.Products?.Count ?? 0,
            CreatedAt = category.CreatedAt
        };
    }
}
