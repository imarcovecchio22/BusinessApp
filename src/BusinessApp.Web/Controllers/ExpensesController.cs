using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessApp.Web.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IExpenseService _expenseService;
    private readonly IExpenseCategoryService _categoryService;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IExpenseService expenseService, IExpenseCategoryService categoryService, ILogger<ExpensesController> logger)
    {
        _expenseService = expenseService;
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchTerm, Guid? categoryId, DateTime? fromDate, DateTime? toDate, bool? isPaid)
    {
        List<ExpenseDto> expenses;

        if (!string.IsNullOrEmpty(searchTerm) || categoryId.HasValue || fromDate.HasValue || toDate.HasValue || isPaid.HasValue)
        {
            expenses = await _expenseService.SearchAsync(new ExpenseSearchDto
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                FromDate = fromDate,
                ToDate = toDate,
                IsPaid = isPaid
            });
        }
        else
        {
            expenses = await _expenseService.GetAllAsync();
        }

        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CategoryId = categoryId;
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;
        ViewBag.IsPaid = isPaid;

        return View(expenses);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        if (expense == null) return NotFound();
        return View(expense);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateExpenseDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Name");
            return View(createDto);
        }

        try
        {
            await _expenseService.CreateAsync(createDto);
            _logger.LogInformation("Gasto creado: {Description}", createDto.Description);
            TempData["Success"] = "Gasto registrado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear gasto");
            ModelState.AddModelError("", ex.Message);
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Name");
            return View(createDto);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        if (expense == null) return NotFound();

        var updateDto = new UpdateExpenseDto
        {
            Description = expense.Description,
            Amount = expense.Amount,
            ExpenseDate = expense.ExpenseDate,
            ExpenseCategoryId = expense.ExpenseCategoryId,
            Vendor = expense.Vendor,
            ReceiptNumber = expense.ReceiptNumber,
            AttachmentUrl = expense.AttachmentUrl,
            Notes = expense.Notes,
            IsPaid = expense.IsPaid
        };

        var categories = await _categoryService.GetAllAsync();
        ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Name", expense.ExpenseCategoryId);
        ViewBag.ExpenseId = id;
        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateExpenseDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Name", updateDto.ExpenseCategoryId);
            ViewBag.ExpenseId = id;
            return View(updateDto);
        }

        try
        {
            await _expenseService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Gasto actualizado: {Id}", id);
            TempData["Success"] = "Gasto actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar gasto");
            ModelState.AddModelError("", ex.Message);
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories.Where(c => c.IsActive), "Id", "Name", updateDto.ExpenseCategoryId);
            ViewBag.ExpenseId = id;
            return View(updateDto);
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        if (expense == null) return NotFound();
        return View(expense);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _expenseService.DeleteAsync(id);
            _logger.LogInformation("Gasto eliminado: {Id}", id);
            TempData["Success"] = "Gasto eliminado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar gasto");
            TempData["Error"] = "Error al eliminar el gasto";
        }

        return RedirectToAction(nameof(Index));
    }
}
