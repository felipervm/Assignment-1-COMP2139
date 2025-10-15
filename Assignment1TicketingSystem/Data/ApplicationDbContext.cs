using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Models;

namespace Assignment1TicketingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category - Event: One-to-Many
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Purchase - PurchaseItem: One-to-Many
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Purchase)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Event - PurchaseItem: One-to-Many
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(pi => pi.Event)
                .WithMany(e => e.PurchaseItems)
                .HasForeignKey(pi => pi.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            modelBuilder.Entity<Event>()
                .Property(e => e.TicketPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Purchase>()
                .Property(p => p.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseItem>()
                .Property(pi => pi.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}