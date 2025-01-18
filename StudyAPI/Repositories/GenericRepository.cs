using System.Linq.Expressions;
using StudyAPI.Context;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class 
{
    protected readonly AppDbContext Context;
    public GenericRepository(AppDbContext context)
    {
        Context = context;
    }
    
    public IEnumerable<T> GetAll()
    {
        return Context.Set<T>().ToList();
    }

    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return Context.Set<T>().FirstOrDefault(predicate);
    }

    public T Add(T entity)
    {
        Context.Set<T>().Add(entity);
        Context.SaveChanges();
        return entity;
    }

    public T Update(T entity)
    {
        Context.Set<T>().Update(entity);
        Context.SaveChanges();
        return entity;
    }

    public T Delete(T entity)
    {
        Context.Set<T>().Remove(entity);
        Context.SaveChanges();
        return entity;
    }
}