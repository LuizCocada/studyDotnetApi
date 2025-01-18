using System.Linq.Expressions;

namespace StudyAPI.Repositories.IRepositorys;

public interface IGenericRepository<T>
{
    IEnumerable<T> GetAll();
    T? Get(Expression<Func<T, bool>> predicate);
    T Add(T entity);
    T Update(T entity);
    T Delete(T entity);
}