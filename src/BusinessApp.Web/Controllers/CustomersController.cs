using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessApp.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    // GET: Customers
    public async Task<IActionResult> Index(string? searchTerm, bool? isActive, string? city, string? country)
    {
        List<CustomerDto> customers;

        if (!string.IsNullOrEmpty(searchTerm) || isActive.HasValue || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(country))
        {
            var searchDto = new CustomerSearchDto
            {
                SearchTerm = searchTerm,
                IsActive = isActive,
                City = city,
                Country = country
            };
            customers = await _customerService.SearchAsync(searchDto);
        }
        else
        {
            customers = await _customerService.GetAllAsync();
        }

        ViewBag.SearchTerm = searchTerm;
        ViewBag.IsActive = isActive;
        ViewBag.City = city;
        ViewBag.Country = country;

        return View(customers);
    }

    // GET: Customers/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // GET: Customers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCustomerDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createDto);
        }

        try
        {
            await _customerService.CreateAsync(createDto);
            _logger.LogInformation("Cliente creado: {Email}", createDto.Email);
            TempData["Success"] = "Cliente creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear cliente: {Email}", createDto.Email);
            ModelState.AddModelError("", ex.Message);
            return View(createDto);
        }
    }

    // GET: Customers/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateCustomerDto
        {
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            Country = customer.Country,
            PostalCode = customer.PostalCode,
            TaxId = customer.TaxId,
            IsActive = customer.IsActive,
            Notes = customer.Notes
        };

        ViewBag.CustomerId = id;
        return View(updateDto);
    }

    // POST: Customers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateCustomerDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CustomerId = id;
            return View(updateDto);
        }

        try
        {
            await _customerService.UpdateAsync(id, updateDto);
            _logger.LogInformation("Cliente actualizado: {Id}", id);
            TempData["Success"] = "Cliente actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cliente: {Id}", id);
            ModelState.AddModelError("", ex.Message);
            ViewBag.CustomerId = id;
            return View(updateDto);
        }
    }

    // GET: Customers/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // POST: Customers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var result = await _customerService.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Cliente eliminado: {Id}", id);
                TempData["Success"] = "Cliente eliminado exitosamente";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar el cliente";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar cliente: {Id}", id);
            TempData["Error"] = "Error al eliminar el cliente";
        }

        return RedirectToAction(nameof(Index));
    }
}
