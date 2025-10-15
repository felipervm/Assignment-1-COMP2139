using Assignment1TicketingSystem.Models;

namespace Assignment1TicketingSystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Categories.Any() || context.Events.Any())
            {
                return; // Database has been seeded
            }

            // Seed Categories
            var categories = new Category[]
            {
                new Category
                {
                    Name = "Webinar",
                    Description = "Online educational sessions and professional training",
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Concert",
                    Description = "Live music performances and entertainment events",
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Workshop",
                    Description = "Hands-on learning and skill development sessions",
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Conference",
                    Description = "Large-scale professional gatherings and networking",
                    CreatedDate = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Meetup",
                    Description = "Casual networking and community-focused events",
                    CreatedDate = DateTime.UtcNow
                }
            };

            foreach (var category in categories)
            {
                context.Categories.Add(category);
            }
            context.SaveChanges();

            // Seed Events
            var baseDateTime = DateTime.UtcNow;
            var events = new Event[]
            {
                new Event
                {
                    Title = "ASP.NET Core Fundamentals",
                    Description = "Learn the basics of ASP.NET Core MVC development. Perfect for beginners looking to start their web development journey.",
                    CategoryId = categories[0].CategoryId,
                    EventDateTime = baseDateTime.AddDays(7).AddHours(14),
                    TicketPrice = 29.99m,
                    AvailableTickets = 100,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Live Jazz Night",
                    Description = "Enjoy an evening of smooth jazz with local musicians. A perfect night out for jazz enthusiasts.",
                    CategoryId = categories[1].CategoryId,
                    EventDateTime = baseDateTime.AddDays(14).AddHours(19),
                    TicketPrice = 49.99m,
                    AvailableTickets = 3,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Web Design Workshop",
                    Description = "Master modern web design techniques, UI/UX principles, and industry tools. Hands-on workshop suitable for all levels.",
                    CategoryId = categories[2].CategoryId,
                    EventDateTime = baseDateTime.AddDays(3).AddHours(10),
                    TicketPrice = 79.99m,
                    AvailableTickets = 30,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Tech Leaders Conference 2025",
                    Description = "Connect with industry leaders and innovators. Network with the brightest minds in technology.",
                    CategoryId = categories[3].CategoryId,
                    EventDateTime = baseDateTime.AddDays(30).AddHours(9),
                    TicketPrice = 199.99m,
                    AvailableTickets = 0,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "C# Programming Meetup",
                    Description = "Network with C# developers in your area. Share knowledge, experiences, and build connections.",
                    CategoryId = categories[4].CategoryId,
                    EventDateTime = baseDateTime.AddDays(10).AddHours(18),
                    TicketPrice = 0m,
                    AvailableTickets = 50,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Database Design Masterclass",
                    Description = "Advanced techniques in database optimization, normalization, and performance tuning.",
                    CategoryId = categories[0].CategoryId,
                    EventDateTime = baseDateTime.AddDays(21).AddHours(15),
                    TicketPrice = 89.99m,
                    AvailableTickets = 25,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Cloud Architecture Summit",
                    Description = "Explore cloud computing architectures, best practices, and real-world implementations.",
                    CategoryId = categories[3].CategoryId,
                    EventDateTime = baseDateTime.AddDays(45).AddHours(10),
                    TicketPrice = 149.99m,
                    AvailableTickets = 40,
                    CreatedDate = DateTime.UtcNow
                },
                new Event
                {
                    Title = "Python for Data Science",
                    Description = "Learn Python programming with a focus on data analysis, visualization, and machine learning basics.",
                    CategoryId = categories[2].CategoryId,
                    EventDateTime = baseDateTime.AddDays(5).AddHours(16),
                    TicketPrice = 69.99m,
                    AvailableTickets = 2,
                    CreatedDate = DateTime.UtcNow
                }
            };

            foreach (var @event in events)
            {
                context.Events.Add(@event);
            }
            context.SaveChanges();
        }
    }
}