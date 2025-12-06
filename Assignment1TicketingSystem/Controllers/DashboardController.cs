using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Models;
using Assignment1TicketingSystem.Models.ViewModels;
using Assignment1TicketingSystem.Data;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var purchases = await _context.Purchases
            .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Event)
            .Where(p => p.GuestEmail == user.Email)
            .ToListAsync();

        var upcoming = purchases
            .Where(p => p.PurchaseDate >= DateTime.UtcNow.AddDays(-1))
            .ToList();

        var past = purchases
            .Where(p => p.PurchaseDate < DateTime.UtcNow.AddDays(-1))
            .ToList();

        var isOrganizer = await _userManager.IsInRoleAsync(user, "Organizer");

        var myEvents = isOrganizer
            ? await _context.Events.Where(e => e.CreatorId == user.Id).ToListAsync()
            : new List<Event>();

        var vm = new DashboardViewModel
        {
            User = user,
            UpcomingPurchases = upcoming,
            PastPurchases = past,
            MyEvents = myEvents
        };

        return View(vm);
    }
}
