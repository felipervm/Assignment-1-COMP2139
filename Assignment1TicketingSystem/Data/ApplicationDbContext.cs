using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Assignment1TicketingSystem.Models;

namespace Assignment1TicketingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<PurchaseItem> PurchaseItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category table
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);
                entity.Property(c => c.CreatedDate).IsRequired();
            });

            // Event table
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TicketPrice).HasColumnType("decimal(18,2)");
            });

            // Purchase table
            builder.Entity<Purchase>(entity =>
            {
                entity.HasKey(p => p.PurchaseId);
                entity.Property(p => p.TotalCost).HasColumnType("decimal(18,2)");
                entity.Property(p => p.PurchaseDate).IsRequired();
            });

            // PurchaseItem table
            builder.Entity<PurchaseItem>(entity =>
            {
                entity.HasKey(pi => pi.PurchaseItemId);
                entity.Property(pi => pi.UnitPrice).HasColumnType("decimal(18,2)");
            });

            // Add any relationships here if needed
            builder.Entity<PurchaseItem>()
                   .HasOne(pi => pi.Purchase)
                   .WithMany(p => p.PurchaseItems)
                   .HasForeignKey(pi => pi.PurchaseId);

            builder.Entity<Event>()
                   .HasOne(e => e.Category)
                   .WithMany(c => c.Events)
                   .HasForeignKey(e => e.CategoryId);
        }
    }
}

