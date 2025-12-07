using Bookify.Data.Repositories;

namespace Bookify.Data;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    IRoomRepository Rooms { get; }
    IBookingRepository Bookings { get; }
    Task<IDisposable> BeginTransactionAsync();
    Task<int> SaveChangesAsync();
}