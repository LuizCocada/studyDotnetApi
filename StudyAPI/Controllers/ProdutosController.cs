using Microsoft.AspNetCore.Mvc;
using StudyAPI.Domain;
using StudyAPI.Repositories;
using StudyAPI.Repositories.IRepositorys;

namespace StudyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IGenericRepository<Produto> _repository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IGenericRepository<Produto> repository, IProdutoRepository produtoRepository, ILogger<ProdutosController> logger)
    {
        _repository = repository;
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    [HttpGet("produtosPorCategoria/{id}")]
    public ActionResult<Produto> GetProdutosByCategoriaId(int id)
    {
        var produtos = _produtoRepository.GetProdutosByCategoria(id);
        if (produtos.Count() <= 0)
        {
            _logger.LogWarning("Lista de produtos vazia.");
            return NotFound("Lista de produtos vazia.");
        }
        return Ok(produtos);
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> GetAllProducts()
    {
        var produtos = _repository.GetAll();
        if (produtos.Count() <= 0)
        {
            _logger.LogWarning("Lista de produtos vazia.");
            return NotFound("Lista de produtos vazia.");
        }

        return Ok(produtos);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<Produto> GetProductById(int id)
    {
        var produto = _repository.Get(p => p.ProdutoId == id);
        if (produto is null)
        {
            _logger.LogWarning("Produto nao foi encontrado.");
            return NotFound("Produto nao foi encontrado.");
        }

        return produto;
    }

    [HttpPost]
    public ActionResult AddProduct(Produto? produto)
    {
        if (produto is null)
        {
            _logger.LogWarning("Produto nao pode ser nulo.");
            return BadRequest("Produto nao pode ser nulo.");
        }

        var produtoCriado = _repository.Add(produto);

        return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriado.ProdutoId }, produtoCriado);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateProduct(int id, Produto produto)
    {
        if (id != produto.ProdutoId)
        {
            _logger.LogWarning("Id do produto não corresponde ao id passado.");
            return BadRequest("Id do produto não corresponde ao id passado.");
        }

        var produtoAtualizado = _repository.Update(produto);

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var produtoExiste = _repository.Get(p => p.ProdutoId == id);
        if (produtoExiste is null)
        {
            _logger.LogWarning("Produto não encontrado.");
            return NotFound("Produto não encontrado.");
        }

        _repository.Delete(produtoExiste);

        return Ok($"Produto {produtoExiste.Nome} de Id = {produtoExiste.ProdutoId} foi removido com sucesso!");
    }
}