using StudyAPI.Context;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ICategoriaRepository? _categoriaRepository;
    private IProdutoRepository? _produtoRepository;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ICategoriaRepository CategoriaRepository
    {
        get
        {
            return _categoriaRepository = _categoriaRepository ?? new CategoriaRepository(_context);
        }
    }
    
    public IProdutoRepository ProdutoRepository
    {
        get
        {
            return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_context);
        }
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}