using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyAPI.Context;
using StudyAPI.Domain;

namespace StudyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase    
{
    private readonly AppDbContext _context;// recebe o contexto do banco de dados atraves do contrutor abaixo

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<Produto>> GetAllProducts() //retorna uma lista de produtos ou qualquer metodo do controllerBase.
    {
        var produtos = _context.Produtos.AsNoTracking().ToList();
        if (produtos is null)
        {
            return NotFound("Produtos nao encontrados.");
        }
        return produtos;
    }

    [HttpGet("{id}", Name = "ObterProduto")]
    public ActionResult<Produto> GetProductById(int id)
    {
        var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);
        if (produto is null)
        {
            return NotFound("Produto nao foi encontrado.");
        }
        return produto;
    }

    [HttpPost]
    public ActionResult AddProduct([FromBody] Produto request)
    {
        if (request is null)
            return BadRequest("Produto nao pode ser nulo.");
        
        _context.Produtos.Add(request);
        _context.SaveChanges();
        return new CreatedAtRouteResult("ObterProduto", new { id = request.ProdutoId }, request);
    }

    [HttpPut("{id}")]
    public ActionResult UpdateProduct(int id, Produto request)
    {
      if(id != request.ProdutoId)
        return BadRequest("Id do produto nao corresponde ao id passado.");
      
      _context.Entry(request).State = EntityState.Modified;
      _context.SaveChanges();
      
      return Ok(request);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);
        if(produto is null)
            return NotFound("Produto nao encontrado.");
        
        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        
        return Ok("Produto removido com sucesso.");
    }
    
}