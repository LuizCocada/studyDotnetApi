using StudyAPI.Domain;
using StudyAPI.Pagination;
using X.PagedList;

namespace StudyAPI.Repositories.IRepositorys;

public interface IProdutoRepository : IGenericRepository<Produto>
{
    Task<IEnumerable<Produto>> GetProdutosByCategoria(int id);

    Task<IPagedList<Produto>> GetProdutosForPagination(ProdutosParameters produtosParams);

    Task<IPagedList<Produto>> GetProdutosFilterPreco(ProdutoFiltroPreco produtoPrecoParams);
}


//metodo especifico + todos os metodos do IGenericRepository