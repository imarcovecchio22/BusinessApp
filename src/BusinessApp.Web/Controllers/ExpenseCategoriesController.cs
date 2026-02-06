using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessApp.Web.Controllers;

[Authorize]
public class ExpenseCategoriesController : Controller
{
    private readonly IExpenseCategoryService _categoryService;
    private readonly ILogger<ExpenseCategoriesController> _logger;

    public ExpenseCategoriesController(IExpenseCategoryService categoryService, ILogger<ExpenseCategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExpenseCategoryDto createDto)
    {
        if (!ModelState.IsValid) return View(createDto);

        try
        {
            await _categoryService.CreateAsync(createDto);
            _logger.LogInformation("Categoría de gasto creada: {Name}", createDto.Name);
            TempData["Success"] = "Categoría creada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría");
            ModelState.AddModelError("", ex.Message);
            return View(createDto);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();

        var updateDto = new UpdateExpenseCategoryDto
        {
            Name = category.Name,
            Description = category.Description,
            Icon = category.Icon,
            Color = category.Color,
            IsActive = category.IsActive
        };

        ViewBag.CategoryId = id;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateExpenseCategoryDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CategoryId = id;
            return View(updateDto);
        }

        try
        {
            await _categoryService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Categoría actualizada: {Id}", id);
            TempData["Success"] = "Categoría actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría");
            ModelState.AddModelError("", ex.Message);
            ViewBag.CategoryId = id;
            return View(updateDto);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            _logger.LogInformation("Categoría eliminada: {Id}", id);
            TempData["Success"] = "Categoría eliminada exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría");
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
