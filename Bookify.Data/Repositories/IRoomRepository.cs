using Bookify.Data.Entities;

namespace Bookify.Data.Repositories;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int? roomTypeId = null);
}