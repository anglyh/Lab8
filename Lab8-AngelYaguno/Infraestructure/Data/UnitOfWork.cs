using System.Collections;
using Lab8_AngelYaguno.Domain.Interfaces;
using Lab8_AngelYaguno.Models;

namespace Lab8_AngelYaguno.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private Hashtable? _repositories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _repositories = new Hashtable();
    }

    public IGenericRepository<T> Repository<T>() where T : class
    {
        var type = typeof(T);
            
        if (_repositories?.ContainsKey(type) == true)
        {
            return (IGenericRepository<T>)_repositories[type]!;
        }

        var repositoryType = typeof(GenericRepository<>);
        var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);

        if (repositoryInstance != null)
        {
            _repositories?.Add(type, repositoryInstance);
            return (IGenericRepository<T>)repositoryInstance;
        }

        throw new Exception($"No se pudo crear el repositorio para este tipo {type}");
    }

    public async Task<int> Complete()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}