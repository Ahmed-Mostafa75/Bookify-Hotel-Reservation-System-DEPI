using Bookify.Data.Repositories;

namespace Bookify.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context, IRoomRepository rooms, IBookingRepository bookings)
    {
        _context = context;
        Rooms = rooms;
        Bookings = bookings;
    }

    public IRoomRepository Rooms { get; }
    public IBookingRepository Bookings { get; }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
        if (_repositories.ContainsKey(type))
        {
            return (IGenericRepository<T>)_repositories[type];
        }
        var repoInstance = new GenericRepository<T>(_context);
        _repositories[type] = repoInstance;
        return repoInstance;
    }

    public async Task<IDisposable> BeginTransactionAsync()
    {
        var tx = await _context.Database.BeginTransactionAsync();
        return tx;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}