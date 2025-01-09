using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Domain;
using StudyAPI.Filters;

namespace StudyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public CategoriasController(AppDbContext context, ILogger<CategoriasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [ServiceFilter(typeof(ApiLoggingFilter))] //usando o middleware de log
    public ActionResult<IEnumerable<Categoria>> GetAllCategories()
    {
        var categorias = _context.Categorias.AsNoTracking().ToList();
        if (categorias is null)
        {
            _logger.LogWarning("LOGGER: Categorias não encontradas...");
            return NotFound("Categorias nao encontradas.");
        }

        return categorias;
    }

    [HttpGet("CategoriasProdutos")]
    public ActionResult<IEnumerable<Categoria>> GetAllCategoriasProdutos()
    {
        var categorias = _context.Categorias.Include(c => c.Produtos).AsNoTracking().ToList();
        if (categorias is null)
        {
            _logger.LogWarning("LOGGER: Categorias com Produtos não encontradas...");
            return NotFound("Categorias com produtos não encontradas.");
        }

        return categorias;
    }

    [HttpGet("{id}", Name = "ObterCategoria")]
    public ActionResult<Categoria> GetCategoriaById(int id)
    {
        var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"LOGGER: Categoria com ID: {id} não encontrada...");
            return NotFound($"Categoria com ID: {id} não encontrada.");
        }

        return categoria;
    }

    [HttpPost]
    public ActionResult AddCategoria(Categoria? request)
    {
        if (request is null)
        {
            _logger.LogWarning($"LOGGER: dados invalidos");
            return BadRequest($"Dados invalidos");
        }

        _context.Categorias.Add(request);
        _context.SaveChanges();

        return new CreatedAtRouteResult("obterCategoria", new { id = request.CategoriaId }, request);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCategoria(int id, Categoria request)
    {
        if (id != request.CategoriaId)
        {
            _logger.LogWarning("Id da categoria nao corresponde ao passado");
            return BadRequest("Id da categoria invalido");
        }

        _context.Entry(request).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(request);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCategoria(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"LOGGER: Categorias com ID: {id} não encontrada...");
            return NotFound($"Categoria com ID: {id} não encontrada.");
        }

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();

        return Ok("Categoria removida com sucesso.");
    }
}