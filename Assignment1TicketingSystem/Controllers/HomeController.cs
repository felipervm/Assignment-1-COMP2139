using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Data;

namespace Assignment1TicketingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var totalEvents = await _context.Events.CountAsync();
                var upcomingEvents = await _context.Events
                    .Include(e => e.Category)
                    .Where(e => e.EventDateTime > DateTime.UtcNow)
                    .OrderBy(e => e.EventDateTime)
                    .Take(6)
                    .ToListAsync();

                var totalCategories = await _context.Categories.CountAsync();

                ViewBag.TotalEvents = totalEvents;
                ViewBag.TotalCategories = totalCategories;
                ViewBag.UpcomingEvents = upcomingEvents;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading homepage");
                return RedirectToAction(nameof(Error), new { message = "Error loading events" });
            }
        }

        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string? message)
        {
            ViewBag.Message = message ?? "An error occurred";
            return View();
        }
    }
}