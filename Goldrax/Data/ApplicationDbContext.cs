using Goldrax.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goldrax.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);

        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                    
                new IdentityRole() { Name="Admin", ConcurrencyStamp= "1", NormalizedName = "ADMIN"},
                new IdentityRole() { Name = "Customer", ConcurrencyStamp = "2", NormalizedName = "CUSTOMER" },
                new IdentityRole() { Name = "Seller", ConcurrencyStamp = "3", NormalizedName = "SELLER" }

                );
        }
    }
}
