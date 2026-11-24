using Microsoft.EntityFrameworkCore;
using Assignment3_EquipmentRental_Team2.Models;

namespace Assignment3_EquipmentRental_Team2.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<Equipment> equipment { get; set; }
        public DbSet<RentalInfo> rentals { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Username = "admin2",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = null
                }
                ); 

            modelBuilder.Entity<Equipment>().HasData(
                new Equipment { Id = 1, Name = "Projector", Description = "HD projector", IsAvailable = true },
                new Equipment { Id = 2, Name = "Camera", Description = "DSLR camera", IsAvailable = true },
                new Equipment { Id = 3, Name = "Laptop", Description = "Dev laptop", IsAvailable = true });
        }
    }
}
