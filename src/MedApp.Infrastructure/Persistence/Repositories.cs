using MedApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedApp.Infrastructure.Persistence;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly MedAppDbContext _db;
    public EfRepository(MedAppDbContext db) => _db = db;

    public async Task AddAsync(T entity, CancellationToken ct = default) => await _db.Set<T>().AddAsync(entity, ct);
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default) => await _db.Set<T>().AddRangeAsync(entities, ct);
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) => await _db.Set<T>().FindAsync([id], ct);
    public IQueryable<T> Query() => _db.Set<T>().AsQueryable().AsNoTracking();
    public void Remove(T entity) => _db.Set<T>().Remove(entity);
    public void Update(T entity) => _db.Set<T>().Update(entity);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly MedAppDbContext _db;
    public UnitOfWork(MedAppDbContext db) => _db = db;
    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
