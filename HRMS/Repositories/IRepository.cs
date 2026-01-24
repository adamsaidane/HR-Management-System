using System.Linq.Expressions;

namespace HRMS.Repositories;

public interface IRepository<T> where T : class
{
    // Récupération
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    IQueryable<T> GetAllQueryable();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    // Ajout
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    
    // Modification
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    
    // Suppression
    void Remove(T entity);
    void Delete(T entity);
    void RemoveRange(IEnumerable<T> entities);
    
    // Comptage
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    // Existence
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}