using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Domain;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Repositories;

public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
{
    private readonly AppDbContext _context;
    public ProdutoRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public IEnumerable<Produto> GetProdutosByCategoria(int id)
    {
        return GetAll().Where(p => p.CategoriaId == id).ToList();
    }
}