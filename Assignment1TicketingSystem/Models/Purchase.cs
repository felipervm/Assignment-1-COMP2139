using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1TicketingSystem.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Guest name is required")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
        [Display(Name = "Full Name")]
        public string GuestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email Address")]
        public string GuestEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone format")]
        [Display(Name = "Phone Number")]
        public string? GuestPhone { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total cost must be greater than 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Completed";

        // Navigation Property
        public virtual ICollection<PurchaseItem>? PurchaseItems { get; set; } = new List<PurchaseItem>();

        // Helper Property
        [NotMapped]
        public int TotalTickets => PurchaseItems?.Sum(pi => pi.Quantity) ?? 0;
    }

    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }

        [Required]
        [ForeignKey("Purchase")]
        public int PurchaseId { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        // Navigation Properties
        public virtual Purchase? Purchase { get; set; }
        public virtual Event? Event { get; set; }

        // Helper Property
        [NotMapped]
        public decimal ItemTotal => Quantity * UnitPrice;
    }
}