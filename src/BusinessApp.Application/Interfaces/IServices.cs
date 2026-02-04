using BusinessApp.Application.DTOs;

namespace BusinessApp.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<List<CustomerDto>> GetAllAsync();
    Task<List<CustomerDto>> SearchAsync(CustomerSearchDto searchDto);
    Task<CustomerDto> CreateAsync(CreateCustomerDto createDto);
    Task<CustomerDto> UpdateAsync(Guid id, UpdateCustomerDto updateDto);
    Task<bool> DeleteAsync(Guid id);
}

public interface ICategoryService
{
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<List<CategoryDto>> GetAllAsync();
    Task<List<CategoryDto>> GetActiveAsync();
    Task<CategoryDto> CreateAsync(CreateCategoryDto createDto);
    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto updateDto);
    Task<bool> DeleteAsync(Guid id);
}

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<List<ProductDto>> GetAllAsync();
    Task<List<ProductDto>> SearchAsync(ProductSearchDto searchDto);
    Task<ProductDto> CreateAsync(CreateProductDto createDto);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto updateDto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UpdateStockAsync(Guid id, int quantity);
}

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync();
}
