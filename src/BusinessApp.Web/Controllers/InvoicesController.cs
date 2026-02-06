using BusinessApp.Application.DTOs;
using BusinessApp.Application.Interfaces;
using BusinessApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessApp.Web.Controllers;

[Authorize]
public class InvoicesController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IInvoiceService invoiceService,
        ICustomerService customerService,
        IProductService productService,
        ILogger<InvoicesController> logger)
    {
        _invoiceService = invoiceService;
        _customerService = customerService;
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? searchTerm, Guid? customerId, InvoiceStatus? status, DateTime? fromDate, DateTime? toDate)
    {
        List<InvoiceDto> invoices;

        if (!string.IsNullOrEmpty(searchTerm) || customerId.HasValue || status.HasValue || fromDate.HasValue || toDate.HasValue)
        {
            var searchDto = new InvoiceSearchDto
            {
                SearchTerm = searchTerm,
                CustomerId = customerId,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate
            };
            invoices = await _invoiceService.SearchAsync(searchDto);
        }
        else
        {
            invoices = await _invoiceService.GetAllAsync();
        }

        var customers = await _customerService.GetAllAsync();
        ViewBag.Customers = new SelectList(customers, "Id", "FullName");
        ViewBag.SearchTerm = searchTerm;
        ViewBag.CustomerId = customerId;
        ViewBag.Status = status;
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate = toDate;

        return View(invoices);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();
        return View(invoice);
    }

    public async Task<IActionResult> Create()
    {
        var customers = await _customerService.GetAllAsync();
        var products = await _productService.GetAllAsync();
        
        ViewBag.Customers = new SelectList(customers.Where(c => c.IsActive), "Id", "FullName");
        ViewBag.Products = products.Where(p => p.IsActive).ToList();
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInvoiceDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var customers = await _customerService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            ViewBag.Customers = new SelectList(customers.Where(c => c.IsActive), "Id", "FullName");
            ViewBag.Products = products.Where(p => p.IsActive).ToList();
            return View(createDto);
        }

        try
        {
            var invoice = await _invoiceService.CreateAsync(createDto);
            _logger.LogInformation("Factura creada: {InvoiceNumber}", invoice.InvoiceNumber);
            TempData["Success"] = "Factura creada exitosamente";
            return RedirectToAction(nameof(Details), new { id = invoice.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear factura");
            ModelState.AddModelError("", ex.Message);
            var customers = await _customerService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            ViewBag.Customers = new SelectList(customers.Where(c => c.IsActive), "Id", "FullName");
            ViewBag.Products = products.Where(p => p.IsActive).ToList();
            return View(createDto);
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(Guid id, InvoiceStatus status)
    {
        try
        {
            await _invoiceService.UpdateStatusAsync(id, status);
            _logger.LogInformation("Estado de factura actualizado: {Id} -> {Status}", id, status);
            TempData["Success"] = "Estado actualizado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado: {Id}", id);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> AddPayment(CreatePaymentDto createDto)
    {
        try
        {
            await _invoiceService.AddPaymentAsync(createDto);
            _logger.LogInformation("Pago registrado para factura: {InvoiceId}", createDto.InvoiceId);
            TempData["Success"] = "Pago registrado exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar pago");
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = createDto.InvoiceId });
    }

    public async Task<IActionResult> Pdf(Guid id)
    {
        try
        {
            var pdf = await _invoiceService.GeneratePdfAsync(id);
            return File(pdf, "application/pdf", $"Factura-{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF: {Id}", id);
            TempData["Error"] = "Error al generar el PDF";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();
        return View(invoice);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            await _invoiceService.DeleteAsync(id);
            _logger.LogInformation("Factura eliminada: {Id}", id);
            TempData["Success"] = "Factura eliminada exitosamente";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar factura: {Id}", id);
            TempData["Error"] = "Error al eliminar la factura";
        }

        return RedirectToAction(nameof(Index));
    }
}
