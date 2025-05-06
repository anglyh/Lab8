using System.Linq.Expressions;
using Lab8_AngelYaguno.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab8_AngelYaguno.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual async Task<IEnumerable<T>> GetAll()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public virtual async Task<T?> GetById(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task Add(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public virtual Task Update(T entity)
    {
        _context.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> Delete(int id)
    {
        var entity = await _context.Set<T>().FindAsync(id);
        if (entity == null) return false;
        _context.Set<T>().Remove(entity);
        return true;
    }
}