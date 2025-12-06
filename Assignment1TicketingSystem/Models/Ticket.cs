namespace Assignment1TicketingSystem.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public string Status { get; set; }   // Open / Closed / InProgress
        
        public DateTime CreatedAt { get; set; }
    }
}
