using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Models;
using StudyAPI.Pagination;
using StudyAPI.Repositories.IRepositorys;
using X.PagedList;
using ArgumentNullException = System.ArgumentNullException;

namespace StudyAPI.Repositories;

public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }


    public async Task<IPagedList<Categoria>> GetCategoriasForPagination(CategoriaParameters categoriasParams)
    {
        var categorias = await GetAll();
        var categoriasOrdenadas = categorias.OrderBy(c => c.CategoriaId).AsQueryable();

        return await categoriasOrdenadas.ToPagedListAsync(categoriasParams.PageNumber, categoriasParams.PageSize);

        // return Pagination.PagedList<Categoria>.ToPagedList(categoriasOrdenadas, categoriasParams.PageNumber,
        //     categoriasParams.PageSize);
    }

    public async Task<IPagedList<Categoria>> GetCategoriasByName(CategoriaFiltroNome categoriaFiltroNome)
    {
        var categorias = await GetAll();
        var categoriasOrdenadas = categorias.OrderBy(c => c.Nome).AsQueryable();

        if (!string.IsNullOrEmpty(categoriaFiltroNome.Nome))
        {
            categoriasOrdenadas = categoriasOrdenadas.Where(c =>
                c.Nome.Contains(categoriaFiltroNome.Nome, StringComparison.OrdinalIgnoreCase));
        }

        return await categoriasOrdenadas.ToPagedListAsync(categoriaFiltroNome.PageNumber, categoriaFiltroNome.PageSize);
        // return Pagination.PagedList<Categoria>.ToPagedList(categoriasOrdenadas, categoriaFiltroNome.PageNumber,
        //     categoriaFiltroNome.PageSize);
    }
}