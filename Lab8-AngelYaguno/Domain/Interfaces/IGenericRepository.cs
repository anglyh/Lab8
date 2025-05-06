using System.Linq.Expressions;

namespace Lab8_AngelYaguno.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T?> GetById(int id);
    Task Add(T entity);
    Task Update(T entity);
    Task<bool> Delete(int id);
}