using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1TicketingSystem.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Event Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Event date and time is required")]
        [Display(Name = "Date & Time")]
        [DataType(DataType.DateTime)]
        public DateTime EventDateTime { get; set; }

        [Required(ErrorMessage = "Ticket price is required")]
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 10000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Ticket Price")]
        public decimal TicketPrice { get; set; }

        [Required(ErrorMessage = "Number of available tickets is required")]
        [Range(0, 100000, ErrorMessage = "Available tickets must be a positive number")]
        [Display(Name = "Available Tickets")]
        public int AvailableTickets { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<PurchaseItem>? PurchaseItems { get; set; } = new List<PurchaseItem>();

        // Helper Properties
        [NotMapped]
        public bool IsLowStock => AvailableTickets < 5 && AvailableTickets > 0;

        [NotMapped]
        public bool IsSoldOut => AvailableTickets == 0;

        [NotMapped]
        public int TotalTicketsSold => PurchaseItems?.Sum(pi => pi.Quantity) ?? 0;

        [NotMapped]
        public string AvailabilityStatus
        {
            get
            {
                if (IsSoldOut) return "Sold Out";
                if (IsLowStock) return "Low Stock";
                return "Available";
            }
        }
    }
}