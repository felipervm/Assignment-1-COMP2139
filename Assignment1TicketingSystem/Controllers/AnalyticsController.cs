using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Data;


[Authorize(Roles = "Organizer,Admin")]
[Route("analytics")]
public class AnalyticsController : Controller
{
    private readonly ApplicationDbContext _context;
    public AnalyticsController(ApplicationDbContext ctx) => _context = ctx;

    [HttpGet("sales-by-category")]
    public async Task<IActionResult> SalesByCategory()
    {
        var data = await _context.Categories
            .Select(c => new {
                category = c.Name,
                tickets = c.Events.SelectMany(e => e.PurchaseItems!).Sum(pi => (int?)pi.Quantity) ?? 0
            }).ToListAsync();
        return Json(data);
    }

    [HttpGet("revenue-per-month")]
    public async Task<IActionResult> RevenuePerMonth()
    {
        var now = DateTime.UtcNow;
        var data = await _context.Purchases
            .Where(p => p.PurchaseDate > now.AddMonths(-12))
            .GroupBy(p => new { p.PurchaseDate.Year, p.PurchaseDate.Month })
            .Select(g => new {
                month = $"{g.Key.Year}-{g.Key.Month}",
                revenue = g.Sum(x => x.TotalCost)
            }).ToListAsync();
        return Json(data);
    }

    [HttpGet("top5")]
    public async Task<IActionResult> Top5()
    {
        var data = await _context.Events
            .OrderByDescending(e => e.TotalTicketsSold)
            .Take(5)
            .Select(e => new { title = e.Title, sold = e.TotalTicketsSold })
            .ToListAsync();
        return Json(data);
    }
}
