using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment1TicketingSystem.Models
{
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
