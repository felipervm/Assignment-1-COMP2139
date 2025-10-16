using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Data;
using Assignment1TicketingSystem.Models;

namespace Assignment1TicketingSystem.Controllers
{
    [Route("events")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventController> _logger;

        public EventController(ApplicationDbContext context, ILogger<EventController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /events
        [HttpGet("")]
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            try
            {
                var events = _context.Events
                    .Include(e => e.Category)
                    .Include(e => e.PurchaseItems)
                    .AsQueryable();

                // Filter by title
                if (!string.IsNullOrEmpty(search))
                    events = events.Where(e => e.Title.Contains(search));

                // Filter by category (ignore 0 or null)
                if (categoryId.HasValue && categoryId.Value > 0)
                    events = events.Where(e => e.CategoryId == categoryId);

                var eventList = await events.OrderBy(e => e.EventDateTime).ToListAsync();
                var categories = await _context.Categories.ToListAsync();

                ViewBag.Categories = categories;
                ViewBag.SearchTitle = search;
                ViewBag.SearchCategory = categoryId ?? 0;

                return View(eventList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading events");
                return RedirectToAction("Error", "Home", new { message = "Error loading events" });
            }
        }

        // GET: /events/overview
        [HttpGet("overview")]
        public async Task<IActionResult> Overview()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Category)
                    .ToListAsync();

                ViewBag.TotalEvents = events.Count;
                ViewBag.TotalCategories = events.Select(e => e.Category?.Name).Distinct().Count();
                ViewBag.LowStockEvents = events.Where(e => e.AvailableTickets > 0 && e.AvailableTickets < 5).ToList();
                ViewBag.SoldOutEvents = events.Where(e => e.AvailableTickets == 0).ToList();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading overview");
                return RedirectToAction("Error", "Home", new { message = "Error loading overview" });
            }
        }

        // GET: /events/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }

        // POST: /events/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CategoryId,EventDateTime,TicketPrice,AvailableTickets")] Event @event)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    @event.CreatedDate = DateTime.UtcNow;
                    _context.Events.Add(@event);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Event '{@event.Title}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating event");
                    ModelState.AddModelError("", "Error creating event. Please try again.");
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(@event);
        }

        // GET: /events/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(@event);
        }

        // POST: /events/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,CategoryId,EventDateTime,TicketPrice,AvailableTickets")] Event @event)
        {
            if (id != @event.EventId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events.FindAsync(id);
                    if (existingEvent == null) return NotFound();

                    existingEvent.Title = @event.Title;
                    existingEvent.Description = @event.Description;
                    existingEvent.CategoryId = @event.CategoryId;
                    existingEvent.EventDateTime = @event.EventDateTime;
                    existingEvent.TicketPrice = @event.TicketPrice;
                    existingEvent.AvailableTickets = @event.AvailableTickets;

                    _context.Update(existingEvent);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Event updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating event");
                    ModelState.AddModelError("", "Error updating event. Please try again.");
                }
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(@event);
        }

        // GET: /events/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.PurchaseItems) // needed for FK check
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: /events/delete/{id}
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var @event = await _context.Events
                    .Include(e => e.PurchaseItems)
                    .FirstOrDefaultAsync(e => e.EventId == id);

                if (@event != null)
                {
                    // Delete related purchase items first to prevent FK constraint error
                    if (@event.PurchaseItems.Any())
                    {
                        _context.PurchaseItems.RemoveRange(@event.PurchaseItems);
                    }

                    _context.Events.Remove(@event);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Event deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event");
                TempData["Error"] = "Error deleting event. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /events/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.PurchaseItems)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }
    }
}
