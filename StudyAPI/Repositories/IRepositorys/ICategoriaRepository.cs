using StudyAPI.Domain;
using StudyAPI.Pagination;
using X.PagedList;

namespace StudyAPI.Repositories.IRepositorys;

public interface ICategoriaRepository : IGenericRepository<Categoria>
{
    Task<IPagedList<Categoria>> GetCategoriasForPagination(CategoriaParameters categoriasParams);

    Task<IPagedList<Categoria>> GetCategoriasByName(CategoriaFiltroNome? categoriaFiltroNome);
}