using StudyAPI.Context;
using StudyAPI.Models;
using StudyAPI.Pagination;
using StudyAPI.Repositories.IRepositorys;
using X.PagedList;

namespace StudyAPI.Repositories;

public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetProdutosByCategoria(int id)
    {
        var produtos = await GetAll();
        return produtos.Where(p => p.CategoriaId == id);
    }

    public async Task<IPagedList<Produto>> GetProdutosForPagination(ProdutosParameters produtosParams)
    {
        var produtos = await GetAll();
        var produtosOrdenados = produtos.OrderBy(p => p.CategoriaId).AsQueryable();

        return await produtosOrdenados.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);

        // return PagedList<Produto>.ToPagedList(produtosOrdenados, produtosParams.PageNumber,
        //     produtosParams.PageSize);
    }

    public async Task<IPagedList<Produto>> GetProdutosFilterPreco(ProdutoFiltroPreco produtoPrecoParams)
    {
        var produtos = await GetAll();
        var produtosQueryable = produtos.AsQueryable();

        if (produtoPrecoParams.Preco.HasValue && !string.IsNullOrEmpty(produtoPrecoParams.PrecoCriterio))
        {
            if (produtoPrecoParams.PrecoCriterio.Equals("maior", StringComparison.OrdinalIgnoreCase))
            {
                produtosQueryable = produtosQueryable.Where(p => p.Preco > produtoPrecoParams.Preco);
            }
            else if (produtoPrecoParams.PrecoCriterio == "menor")
            {
                produtosQueryable = produtosQueryable.Where(p => p.Preco < produtoPrecoParams.Preco);
            }
            else if (produtoPrecoParams.PrecoCriterio == "igual")
            {
                produtosQueryable = produtosQueryable.Where(p => p.Preco == produtoPrecoParams.Preco);
            }
        }


        return await produtosQueryable.ToPagedListAsync(produtoPrecoParams.PageNumber,
            produtoPrecoParams.PageSize);

       // return PagedList<Produto>.ToPagedList(produtosQueryable, produtoPrecoParams.PageNumber,
       //      produtoPrecoParams.PageSize);
    }
}