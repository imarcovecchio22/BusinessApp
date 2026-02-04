using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessApp.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchTerm, ProductType? type, Guid? categoryId, bool? isActive, bool? lowStock)
    {
        List<ProductDto> products;

        if (!string.IsNullOrEmpty(searchTerm) || type.HasValue || categoryId.HasValue || isActive.HasValue || lowStock == true)
        {
            var searchDto = new ProductSearchDto
            {
                SearchTerm = searchTerm,
                Type = type,
                CategoryId = categoryId,
                IsActive = isActive,
                LowStock = lowStock
            };
            products = await _productService.SearchAsync(searchDto);
        }
        else
        {
            products = await _productService.GetAllAsync();
        }

        var categories = await _categoryService.GetActiveAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.SearchTerm = searchTerm;
        ViewBag.Type = type;
        ViewBag.CategoryId = categoryId;
        ViewBag.IsActive = isActive;
        ViewBag.LowStock = lowStock;

        return View(products);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetActiveAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(createDto);
        }

        try
        {
            await _productService.CreateAsync(createDto);
            _logger.LogInformation("Producto creado: {Name}", createDto.Name);
            TempData["Success"] = "Producto creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear producto: {Name}", createDto.Name);
            ModelState.AddModelError("", ex.Message);
            var categories = await _categoryService.GetActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(createDto);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        var updateDto = new UpdateProductDto
        {
            Name = product.Name,
            Description = product.Description,
            Sku = product.Sku,
            Price = product.Price,
            Cost = product.Cost,
            Type = product.Type,
            Stock = product.Stock,
            MinStock = product.MinStock,
            IsActive = product.IsActive,
            ImageUrl = product.ImageUrl,
            CategoryId = product.CategoryId
        };

        var categories = await _categoryService.GetActiveAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
        ViewBag.ProductId = id;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateProductDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", updateDto.CategoryId);
            ViewBag.ProductId = id;
            return View(updateDto);
        }

        try
        {
            await _productService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Producto actualizado: {Id}", id);
            TempData["Success"] = "Producto actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar producto: {Id}", id);
            ModelState.AddModelError("", ex.Message);
            var categories = await _categoryService.GetActiveAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", updateDto.CategoryId);
            ViewBag.ProductId = id;
            return View(updateDto);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var result = await _productService.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Producto eliminado: {Id}", id);
                TempData["Success"] = "Producto eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el producto";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar producto: {Id}", id);
            TempData["Error"] = "Error al eliminar el producto";
        }

        return RedirectToAction(nameof(Index));
    }
}
