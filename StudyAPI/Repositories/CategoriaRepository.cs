using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Domain;
using StudyAPI.Repositories.IRepositorys;
using ArgumentNullException = System.ArgumentNullException;

namespace StudyAPI.Repositories;

public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }
}