using Bookify.Data.Entities;

namespace Bookify.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomType>> GetRoomTypesAsync();
    Task<IEnumerable<Room>> SearchAvailableAsync(DateTime checkIn, DateTime checkOut, int? roomTypeId = null);
}