using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudyAPI.DTOs.CategoryDTO;
using StudyAPI.DTOs.Mappings;
using StudyAPI.Models;
using StudyAPI.Pagination;
using StudyAPI.Repositories.IRepositorys;
using X.PagedList;

namespace StudyAPI.Controllers;

[EnableCors("OrigensComAcessoPermitido")] //portas permitidas
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IUnitOfWork uof, ILogger<CategoriasController> logger)
    {
        _uof = uof;
        _logger = logger;
    }

    [HttpGet]
    //[Authorize]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAllCategories()
    {
        var categorias = await _uof.CategoriaRepository.GetAll();
        if (categorias.Count() <= 0)
        {
            _logger.LogWarning("LOGGER: Categorias não encontradas...");
            return NotFound("Categorias não encontradas.");
        }

        var categoriasDto = categorias.ToCategoriaDtoList();
        return Ok(categoriasDto);
    }

    private ActionResult<IEnumerable<CategoriaDto>> ObterCategoriaFiltrados(
        IPagedList<Categoria> categoriasWithMetadata)
    {
        if (!categoriasWithMetadata.Any())
        {
            _logger.LogWarning("Lista de Categorias vazia.");
            return NotFound("Lista de Categorias vazia.");
        }

        var metadata = new
        {
            categoriasWithMetadata.Count,
            categoriasWithMetadata.PageSize,
            categoriasWithMetadata.PageCount,
            categoriasWithMetadata.TotalItemCount,
            categoriasWithMetadata.HasNextPage,
            categoriasWithMetadata.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriasDto = CategoriaDtoMappingExtension
            .ToCategoriaDtoList(categoriasWithMetadata);

        return Ok(categoriasDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAllCategoriesPagination(
        [FromQuery] CategoriaParameters categoriaParameters)
    {
        var categoriasWithMetadata = await _uof.CategoriaRepository
            .GetCategoriasForPagination(categoriaParameters);

        return ObterCategoriaFiltrados(categoriasWithMetadata);
    }

    [HttpGet("filter/Nome/Pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasByNomePagination(
        [FromQuery] CategoriaFiltroNome categoriaName)
    {
        var categoriasWithMetadata = await _uof.CategoriaRepository
            .GetCategoriasByName(categoriaName);

        return ObterCategoriaFiltrados(categoriasWithMetadata);
    }

    [DisableCors] //desabilitar cors
    [HttpGet("{id}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDto>> GetCategoriaById(int id)
    {
        var categoria = await _uof.CategoriaRepository.Get(categoria => categoria.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"LOGGER: Categoria com ID: {id} não encontrada...");
            return NotFound($"Categoria com ID: {id} não encontrada.");
        }

        var categoriaDto = categoria.ToCaregoriaDto();

        return Ok(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> AddCategoria(CategoriaDto? categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning("LOGGER: Categoria não pode ser nula...");
            return BadRequest("Categoria não pode ser nula.");
        }

        var categoria = categoriaDto.ToCategoria();

        var categoriaCriada = _uof.CategoriaRepository.Add(categoria!);
        await _uof.Commit();

        var categoriaToDto = categoriaCriada.ToCaregoriaDto();

        return new CreatedAtRouteResult("obterCategoria", new { id = categoriaToDto!.CategoriaId }, categoriaToDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoriaDto>> UpdateCategoria(int id, CategoriaDto categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning("Id da categoria nao corresponde ao passado");
            return BadRequest("Id da categoria invalido");
        }

        if (categoria is null)
        {
            _logger.LogWarning("LOGGER: Categoria não pode ser nula...");
            return BadRequest("Categoria não pode ser nula.");
        }

        var dtoToCategoria = categoria.ToCategoria();

        var categoriaAtualizada = _uof.CategoriaRepository.Update(dtoToCategoria!);
        await _uof.Commit();

        var categoriaToDto = categoriaAtualizada.ToCaregoriaDto();

        return Ok(categoriaToDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoriaDto>> DeleteCategoria(int id)
    {
        var categoria = await _uof.CategoriaRepository.Get(categoria => categoria.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"LOGGER: Categorias com ID: {id} não encontrada...");
            return NotFound($"Categoria com ID: {id} não encontrada.");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        await _uof.Commit();

        var categoriaToDto = categoriaExcluida.ToCaregoriaDto();

        return Ok($"Categoria: {categoriaToDto!.Nome} removida com sucesso.");
    }
}