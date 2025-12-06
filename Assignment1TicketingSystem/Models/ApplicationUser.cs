using Microsoft.AspNetCore.Identity;

namespace Assignment1TicketingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // profile fields
        public string? FullName { get; set; }
        public string? PhoneNumberAlt { get; set; }
        public string? ProfilePicturePath { get; set; }
    }
}
