using Bookify.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Data.Repositories;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    private readonly ApplicationDbContext _context;

    public RoomRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int? roomTypeId = null)
    {
        var overlappingBookings = _context.Bookings.Where(b =>
            (checkIn < b.CheckOut) && (checkOut > b.CheckIn));

        var bookedRoomIds = await overlappingBookings.Select(b => b.RoomId).Distinct().ToListAsync();

        var query = _context.Rooms
            .Include(r => r.RoomType)
            .Where(r => !bookedRoomIds.Contains(r.Id) && r.IsAvailable);

        if (roomTypeId.HasValue)
        {
            query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
        }

        return await query.ToListAsync();
    }
}