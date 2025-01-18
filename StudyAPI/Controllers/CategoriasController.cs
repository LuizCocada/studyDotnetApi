using Microsoft.AspNetCore.Mvc;
using StudyAPI.Domain;
using StudyAPI.Repositories;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly IGenericRepository<Categoria> _repository;
    private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IGenericRepository<Categoria> repository, ILogger<CategoriasController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Categoria>> GetAllCategories()
    {
        var categorias = _repository.GetAll();
        if (categorias.Count() <= 0)
        {
            _logger.LogWarning("LOGGER: Categorias não encontradas...");
            return NotFound("Categorias não encontradas.");
        }
        return Ok(categorias);
    }
    
    [HttpGet("{id}", Name = "ObterCategoria")]
    public ActionResult<Categoria> GetCategoriaById(int id)
    {
        var categoria = _repository.Get(categoria => categoria.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"LOGGER: Categoria com ID: {id} não encontrada...");
            return NotFound($"Categoria com ID: {id} não encontrada.");
        }

        return categoria;
    }

    [HttpPost]
    public ActionResult AddCategoria(Categoria? categoria)
    {
        if (categoria is null)
        {
            _logger.LogWarning("LOGGER: Categoria não pode ser nula...");
            return BadRequest("Categoria não pode ser nula.");
        }
        
        var categoriaCriada = _repository.Add(categoria);
        return new CreatedAtRouteResult("obterCategoria", new { id = categoriaCriada.CategoriaId }, categoriaCriada);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCategoria(int id, Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning("Id da categoria nao corresponde ao passado");
            return BadRequest("Id da categoria invalido");
        }

        var categoriaAtualizada = _repository.Update(categoria);
        return Ok(categoriaAtualizada);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCategoria(int id)
    {
        var categoria = _repository.Get(categoria => categoria.CategoriaId == id);
        if (categoria is null)
        {
            _logger.LogWarning($"LOGGER: Categorias com ID: {id} não encontrada...");
            return NotFound($"Categoria com ID: {id} não encontrada.");
        }
        
        var categoriaExcluida = _repository.Delete(categoria);
        return Ok($"Categoria: {categoriaExcluida.Nome} removida com sucesso.");
    }
}