using Bookify.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Transaction> Transactions => Set<Transaction>(); 

	protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RoomType>().Property(rt => rt.BasePricePerNight).HasColumnType("decimal(18,2)");
        builder.Entity<Booking>().Property(b => b.TotalCost).HasColumnType("decimal(18,2)");

        builder.Entity<Room>()
            .HasOne(r => r.RoomType)
            .WithMany()
            .HasForeignKey(r => r.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Booking>()
            .HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RoomType>().HasData(
            new RoomType { Id = 1, Name = "Single", Capacity = 1, BasePricePerNight = 80, Description = "Cozy single room", ImageUrl = "../images/singles.jpg", Amenities = "Wi-Fi,Air Conditioning,TV" },
            new RoomType { Id = 2, Name = "Double", Capacity = 2, BasePricePerNight = 120, Description = "Comfortable double room", ImageUrl = "https://images.unsplash.com/photo-1505691938895-1758d7feb511?w=1200&q=80", Amenities = "Wi-Fi,Air Conditioning,Mini Fridge,TV" },
            new RoomType { Id = 3, Name = "Suite", Capacity = 4, BasePricePerNight = 240, Description = "Spacious suite with living area", ImageUrl = "../images/Suite.jpg", Amenities = "Wi-Fi,Air Conditioning,Living Area,Mini Bar,Smart TV" }
        );

        builder.Entity<Room>().HasData(
            new Room { Id = 1, Number = "101", RoomTypeId = 1, IsAvailable = true },
            new Room { Id = 2, Number = "102", RoomTypeId = 1, IsAvailable = true },
            new Room { Id = 3, Number = "201", RoomTypeId = 2, IsAvailable = true },
            new Room { Id = 4, Number = "202", RoomTypeId = 2, IsAvailable = true },
            new Room { Id = 5, Number = "301", RoomTypeId = 3, IsAvailable = true }
        );
    }
}