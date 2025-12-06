using Assignment1TicketingSystem.Data;
using Microsoft.AspNetCore.Mvc;
[Route("api/cart")]
public class CartApiController : Controller
{
    private readonly ApplicationDbContext _context;
    public CartApiController(ApplicationDbContext context) => _context = context;

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] CartAddDto dto)
    {
        var ev = await _context.Events.FindAsync(dto.EventId);
        if (ev == null) return NotFound();

        // store cart in session or db - simple response for demo:
        var remaining = ev.AvailableTickets - dto.Qty;
        var totalItems = 1; // calculate from session
        var totalPrice = dto.Qty * ev.TicketPrice;
        return Ok(new { totalItems, totalPrice = totalPrice.ToString("C"), remaining });
    }

    public class CartAddDto { public int EventId { get; set; } public int Qty { get; set; } }
}
