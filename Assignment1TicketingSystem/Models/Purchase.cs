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

        // 🔥 MISSING PROPERTY ADDED
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } 
            = new List<PurchaseItem>();

        // Helper property
        [NotMapped]
        public int TotalTickets => PurchaseItems.Sum(pi => pi.Quantity);
    }
}

     

