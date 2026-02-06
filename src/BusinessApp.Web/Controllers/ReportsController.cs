using BusinessApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessApp.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    public IActionResult Sales(DateTime? fromDate, DateTime? toDate)
    {
        ViewBag.FromDate = fromDate ?? DateTime.Now.AddMonths(-6);
        ViewBag.ToDate = toDate ?? DateTime.Now;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SalesData(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var report = await _reportService.GetSalesReportAsync(fromDate, toDate);
            return Json(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte de ventas");
            return BadRequest(new { error = ex.Message });
        }
    }

    public IActionResult Financial(DateTime? fromDate, DateTime? toDate)
    {
        ViewBag.FromDate = fromDate ?? DateTime.Now.AddMonths(-6);
        ViewBag.ToDate = toDate ?? DateTime.Now;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> FinancialData(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var report = await _reportService.GetFinancialReportAsync(fromDate, toDate);
            return Json(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar reporte financiero");
            return BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IActionResult> ExportSales(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var csvBytes = await _reportService.ExportSalesReportToExcelAsync(fromDate, toDate);
            return File(csvBytes, "text/csv", $"Ventas-{fromDate:yyyy-MM-dd}-{toDate:yyyy-MM-dd}.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al exportar reporte");
            TempData["Error"] = "Error al exportar el reporte";
            return RedirectToAction(nameof(Sales));
        }
    }
}
