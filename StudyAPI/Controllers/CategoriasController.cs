using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Domain;

namespace StudyAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAllCategories()
    {
        var categorias = _context.Categorias.AsNoTracking().ToList();
        if (categorias is null)
            return NotFound("Categorias nao encontradas.");
        
        return categorias;
    }

    [HttpGet("CategoriasProdutos")]
    public ActionResult<IEnumerable<Categoria>> GetAllCategoriasProdutos()
    {
        var categorias = _context.Categorias.Include(c => c.Produtos).AsNoTracking().ToList();
        if (categorias is null)
            return NotFound("Categorias nao encontradas.");
        
        return categorias;
    }

    [HttpGet("{id}", Name = "ObterCategoria")]
    public ActionResult<Categoria> GetCategoriaById(int id)
    {
        var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);
        if (categoria is null)
            return NotFound("Categoria nao encontrada.");
        
        return categoria;
    }

    [HttpPost]
    public ActionResult AddCategoria(Categoria request)
    {
        if(request is null)
            return BadRequest("Categoria nao pode ser nula.");
        
        _context.Categorias.Add(request);
        _context.SaveChanges();
        
        return new CreatedAtRouteResult("obterCategoria", new { id = request.CategoriaId }, request);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCategoria(int id, Categoria request)
    {
        if(id != request.CategoriaId)
            return BadRequest("Id da categoria nao corresponde ao id passado.");
        
        _context.Entry(request).State = EntityState.Modified;
        _context.SaveChanges();

        return Ok(request);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCategoria(int id)
    {
        var categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);
        if (categoria is null)
            return NotFound("Categoria nao encontrada.");
        
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        
        return Ok("Categoria removida com sucesso.");
    }
    
    
    
}