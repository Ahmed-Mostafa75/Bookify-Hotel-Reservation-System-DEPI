using Bookify.Data.Entities;

namespace Bookify.Data.Repositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);
}