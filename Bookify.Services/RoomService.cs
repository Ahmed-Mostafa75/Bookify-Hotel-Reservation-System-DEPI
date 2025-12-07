using Bookify.Data;
using Bookify.Data.Entities;

namespace Bookify.Services;

public class RoomService : IRoomService
{
    private readonly IUnitOfWork _uow;

    public RoomService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public Task<IEnumerable<RoomType>> GetRoomTypesAsync()
    {
        return _uow.Repository<RoomType>().GetAllAsync();
    }

    public Task<IEnumerable<Room>> SearchAvailableAsync(DateTime checkIn, DateTime checkOut, int? roomTypeId = null)
    {
        return _uow.Rooms.GetAvailableRoomsAsync(checkIn, checkOut, roomTypeId);
    }
}