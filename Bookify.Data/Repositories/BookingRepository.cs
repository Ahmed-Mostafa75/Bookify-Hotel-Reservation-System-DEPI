using Bookify.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data.Repositories;

public class BookingRepository : GenericRepository<Booking>, IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
    {
        return await _context.Bookings
            .Include(b => b.Room)!
            .ThenInclude(r => r!.RoomType)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}