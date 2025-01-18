using StudyAPI.Domain;

namespace StudyAPI.Repositories.IRepositorys;

public interface IProdutoRepository : IGenericRepository<Produto>
{
    IEnumerable<Produto> GetProdutosByCategoria(int id);
}