using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Data;
using Assignment1TicketingSystem.Models;

namespace Assignment1TicketingSystem.Controllers
{
    [Route("purchase")]
    public class PurchaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(ApplicationDbContext context, ILogger<PurchaseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /purchase/select
        [HttpGet("select")]
        public async Task<IActionResult> Select()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.Category)
                    .Where(e => e.AvailableTickets > 0)
                    .OrderBy(e => e.EventDateTime)
                    .ToListAsync();

                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading events for purchase");
                return RedirectToAction("Error", "Home", new { message = "Error loading events" });
            }
        }

        // GET: /purchase/checkout/{eventId}
        [HttpGet("checkout/{eventId}")]
        public async Task<IActionResult> Checkout(int eventId)
        {
            try
            {
                var @event = await _context.Events
                    .Include(e => e.Category)
                    .FirstOrDefaultAsync(e => e.EventId == eventId);

                if (@event == null || @event.AvailableTickets <= 0)
                {
                    TempData["Error"] = "Event not found or sold out";
                    return RedirectToAction(nameof(Select));
                }

                ViewBag.Event = @event;
                ViewBag.MaxTickets = @event.AvailableTickets;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading checkout");
                return RedirectToAction("Error", "Home", new { message = "Error loading checkout" });
            }
        }

        // POST: /purchase/create
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int eventId, 
            string guestName, 
            string guestEmail, 
            string? guestPhone, 
            int quantity)
        {
            try
            {
                var @event = await _context.Events.FindAsync(eventId);
                if (@event == null)
                    return NotFound();

                // Validate ticket availability
                if (quantity <= 0 || quantity > @event.AvailableTickets)
                {
                    ModelState.AddModelError("quantity", "Invalid ticket quantity");
                    ViewBag.Event = @event;
                    ViewBag.MaxTickets = @event.AvailableTickets;
                    return View("Checkout");
                }

                // Validate guest info
                if (string.IsNullOrEmpty(guestName) || string.IsNullOrEmpty(guestEmail))
                {
                    ModelState.AddModelError("", "Guest name and email are required");
                    ViewBag.Event = @event;
                    ViewBag.MaxTickets = @event.AvailableTickets;
                    return View("Checkout");
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Create purchase
                        var purchase = new Purchase
                        {
                            GuestName = guestName,
                            GuestEmail = guestEmail,
                            GuestPhone = guestPhone,
                            PurchaseDate = DateTime.UtcNow,
                            TotalCost = @event.TicketPrice * quantity,
                            Status = "Completed"
                        };

                        _context.Purchases.Add(purchase);
                        await _context.SaveChangesAsync();

                        // Create purchase item
                        var purchaseItem = new PurchaseItem
                        {
                            PurchaseId = purchase.PurchaseId,
                            EventId = eventId,
                            Quantity = quantity,
                            UnitPrice = @event.TicketPrice
                        };

                        _context.PurchaseItems.Add(purchaseItem);

                        // Update available tickets
                        @event.AvailableTickets -= quantity;
                        _context.Events.Update(@event);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["Success"] = "Purchase completed successfully!";
                        return RedirectToAction(nameof(Confirmation), new { purchaseId = purchase.PurchaseId });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error processing purchase");
                        ModelState.AddModelError("", "An error occurred while processing your purchase. Please try again.");
                        ViewBag.Event = @event;
                        ViewBag.MaxTickets = @event.AvailableTickets;
                        return View("Checkout");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in purchase create");
                return RedirectToAction("Error", "Home", new { message = "Error processing purchase" });
            }
        }

        // GET: /purchase/confirmation/{purchaseId}
        [HttpGet("confirmation/{purchaseId}")]
        public async Task<IActionResult> Confirmation(int purchaseId)
        {
            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Event)
                    .ThenInclude(e => e.Category)
                    .FirstOrDefaultAsync(p => p.PurchaseId == purchaseId);

                if (purchase == null)
                    return NotFound();

                return View(purchase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading confirmation");
                return RedirectToAction("Error", "Home", new { message = "Error loading confirmation" });
            }
        }

        // GET: /purchase/history
        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            try
            {
                var purchases = await _context.Purchases
                    .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Event)
                    .OrderByDescending(p => p.PurchaseDate)
                    .ToListAsync();

                return View(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading purchase history");
                return RedirectToAction("Error", "Home", new { message = "Error loading purchase history" });
            }
        }
    }
}