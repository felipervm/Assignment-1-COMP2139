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
        public async Task<IActionResult> Index(int? categoryId)
        {
            try
            {
                var events = _context.Events
                    .Include(e => e.Category)
                    .Include(e => e.PurchaseItems)
                    .AsQueryable();

                if (categoryId.HasValue && categoryId > 0)
                {
                    events = events.Where(e => e.CategoryId == categoryId);
                }

                var eventList = await events.OrderBy(e => e.EventDateTime).ToListAsync();
                var categories = await _context.Categories.ToListAsync();

                ViewBag.Categories = categories;
                ViewBag.SelectedCategory = categoryId;

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
                var totalEvents = await _context.Events.CountAsync();
                var totalCategories = await _context.Categories.CountAsync();
                var lowStockEvents = await _context.Events
                    .Include(e => e.Category)
                    .Where(e => e.AvailableTickets > 0 && e.AvailableTickets < 5)
                    .ToListAsync();
                var soldOutEvents = await _context.Events
                    .Include(e => e.Category)
                    .Where(e => e.AvailableTickets == 0)
                    .ToListAsync();

                ViewBag.TotalEvents = totalEvents;
                ViewBag.TotalCategories = totalCategories;
                ViewBag.LowStockEvents = lowStockEvents;
                ViewBag.SoldOutEvents = soldOutEvents;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading overview");
                return RedirectToAction("Error", "Home", new { message = "Error loading overview" });
            }
        }

        // GET: /events/search
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            string? title, 
            string? dateRange, 
            int? categoryId, 
            string? availability, 
            string? sortBy)
        {
            try
            {
                var events = _context.Events
                    .Include(e => e.Category)
                    .Include(e => e.PurchaseItems)
                    .AsQueryable();

                // Filter by title
                if (!string.IsNullOrEmpty(title))
                {
                    events = events.Where(e => e.Title.Contains(title));
                }

                // Filter by category
                if (categoryId.HasValue && categoryId > 0)
                {
                    events = events.Where(e => e.CategoryId == categoryId);
                }

                // Filter by date range
                if (!string.IsNullOrEmpty(dateRange))
                {
                    var today = DateTime.UtcNow.Date;
                    switch (dateRange)
                    {
                        case "today":
                            events = events.Where(e => e.EventDateTime.Date == today);
                            break;
                        case "week":
                            events = events.Where(e => e.EventDateTime >= today && e.EventDateTime < today.AddDays(7));
                            break;
                        case "month":
                            events = events.Where(e => e.EventDateTime >= today && e.EventDateTime < today.AddMonths(1));
                            break;
                        case "upcoming":
                            events = events.Where(e => e.EventDateTime >= today);
                            break;
                    }
                }

                // Filter by ticket availability
                if (!string.IsNullOrEmpty(availability))
                {
                    switch (availability)
                    {
                        case "available":
                            events = events.Where(e => e.AvailableTickets > 0);
                            break;
                        case "low":
                            events = events.Where(e => e.AvailableTickets > 0 && e.AvailableTickets < 5);
                            break;
                        case "soldout":
                            events = events.Where(e => e.AvailableTickets == 0);
                            break;
                    }
                }

                // Sorting
                switch (sortBy)
                {
                    case "date":
                        events = events.OrderBy(e => e.EventDateTime);
                        break;
                    case "price_asc":
                        events = events.OrderBy(e => e.TicketPrice);
                        break;
                    case "price_desc":
                        events = events.OrderByDescending(e => e.TicketPrice);
                        break;
                    default:
                        events = events.OrderBy(e => e.Title);
                        break;
                }

                var eventList = await events.ToListAsync();
                var categories = await _context.Categories.ToListAsync();

                ViewBag.Categories = categories;
                ViewBag.SearchTitle = title;
                ViewBag.SearchDateRange = dateRange;
                ViewBag.SearchCategory = categoryId;
                ViewBag.SearchAvailability = availability;
                ViewBag.SearchSort = sortBy;

                return View("Index", eventList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching events");
                return RedirectToAction("Error", "Home", new { message = "Error searching events" });
            }
        }

        // GET: /events/create
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
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

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View(@event);
        }

        // GET: /events/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View(@event);
        }

        // POST: /events/edit/{id}
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,CategoryId,EventDateTime,TicketPrice,AvailableTickets")] Event @event)
        {
            if (id != @event.EventId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events.FindAsync(id);
                    if (existingEvent == null)
                        return NotFound();

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

            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            return View(@event);
        }

        // GET: /events/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }

        // POST: /events/delete/{id}
        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var @event = await _context.Events.FindAsync(id);
                if (@event != null)
                {
                    _context.Events.Remove(@event);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Event deleted successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event");
                TempData["Error"] = "Error deleting event. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /events/details/{id}
        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var @event = await _context.Events
                .Include(e => e.Category)
                .Include(e => e.PurchaseItems)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
                return NotFound();

            return View(@event);
        }
    }
}