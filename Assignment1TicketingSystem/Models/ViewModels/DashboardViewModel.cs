using System.Collections.Generic;

namespace Assignment1TicketingSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;

        // Tickets for attendees
        public List<Purchase> UpcomingPurchases { get; set; } = new();
        public List<Purchase> PastPurchases { get; set; } = new();

        // Events for organizers
        public List<Event> MyEvents { get; set; } = new();
    }
}
