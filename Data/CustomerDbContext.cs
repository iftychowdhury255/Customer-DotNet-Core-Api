using Microsoft.EntityFrameworkCore;
using CustomerCoreApi.Models;

namespace CustomerCoreApi.Data
{
    public class CustomerDbContext : DbContext
    {
        public CustomerDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProductDetails -> Product relationship
            modelBuilder.Entity<ProductDetails>()
                .HasOne(pd => pd.Product)
                .WithMany(p => p.ProductDetails)
                .HasForeignKey(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductDetails -> Customer relationship
            modelBuilder.Entity<ProductDetails>()
                .HasOne(pd => pd.Customer)
                .WithMany()
                .HasForeignKey(pd => pd.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional: Add indexes for better performance
            modelBuilder.Entity<ProductDetails>()
                .HasIndex(pd => pd.ProductId);

            modelBuilder.Entity<ProductDetails>()
                .HasIndex(pd => pd.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}