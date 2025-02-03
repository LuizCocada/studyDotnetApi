using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudyAPI.Domain;
using StudyAPI.DTOs;
using StudyAPI.DTOs.ProductDTO;
using StudyAPI.Pagination;
using StudyAPI.Repositories.IRepositorys;
using X.PagedList;

namespace StudyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IMapper _mapper;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IUnitOfWork uof, IMapper mapper, ILogger<ProdutosController> logger)
    {
        _uof = uof;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("produtosPorCategoria/{id}")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosByCategoriaId(int id)
    {
        var produtos = await _uof.ProdutoRepository.GetProdutosByCategoria(id);
        if (produtos.Count() <= 0)
        {
            _logger.LogWarning("Lista de produtos vazia.");
            return NotFound("Lista de produtos vazia.");
        }

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);


        return Ok(produtosDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAllProducts()
    {
        var produtos = await _uof.ProdutoRepository.GetAll();
        if (produtos.Count() <= 0)
        {
            _logger.LogWarning("Lista de produtos vazia.");
            return NotFound("Lista de produtos vazia.");
        }

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutosFiltrados(IPagedList<Produto> produtosWithMetadata)
    {
        if (!produtosWithMetadata.Any())
        {
            _logger.LogWarning("Lista de produtos vazia.");
            return NotFound("Lista de produtos vazia.");
        }

        var metadata = new
        {
            produtosWithMetadata.Count,
            produtosWithMetadata.PageSize,
            produtosWithMetadata.PageCount,
            produtosWithMetadata.TotalItemCount,
            produtosWithMetadata.HasNextPage,
            produtosWithMetadata.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtosWithMetadata);

        return Ok(produtosDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAllProductsPagination(
        [FromQuery] ProdutosParameters produtosParams)
    {
        var produtosWithMetadata = await _uof.ProdutoRepository
            .GetProdutosForPagination(produtosParams);

        return ObterProdutosFiltrados(produtosWithMetadata);
    }

    [HttpGet("filter/preco/pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPrecoPagination(
        [FromQuery] ProdutoFiltroPreco produtoFiltroPreco)
    {
        var produtosWithMetadata = await _uof.ProdutoRepository
            .GetProdutosFilterPreco(produtoFiltroPreco);

        return ObterProdutosFiltrados(produtosWithMetadata);
    }


    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> GetProductById(int id)
    {
        var produto = await _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
        if (produto is null)
        {
            _logger.LogWarning("Produto nao foi encontrado.");
            return NotFound("Produto nao foi encontrado.");
        }

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return produtoDto;
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct(ProdutoDTO? produtoDto)
    {
        if (produtoDto is null)
        {
            _logger.LogWarning("Produto nao pode ser nulo.");
            return BadRequest("Produto nao pode ser nulo.");
        }

        var produto = _mapper.Map<Produto>(produtoDto);
        var produtoCriado = _uof.ProdutoRepository.Add(produto);
        await _uof.Commit();

        var produtoCriadoDto = _mapper.Map<ProdutoDTO>(produtoCriado);


        return new CreatedAtRouteResult("ObterProduto", new { id = produtoCriadoDto.ProdutoId }, produtoCriadoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDtoUpdateResponse>> Patch(int id,
        JsonPatchDocument<ProdutoDtoUpdateRequest>? patchProdutoDto) //estoque e data
    {
        if (id <= 0 || patchProdutoDto is null)
        {
            _logger.LogWarning("Id do produto ou produto não pode ser nulo.");
            return BadRequest("Id do produto ou produto não pode ser nulo.");
        }

        var produto = await _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
        if (produto is null)
        {
            _logger.LogWarning("Produto não encontrado.");
            return NotFound("Produto não encontrado.");
        }

        var produtoToDto = _mapper.Map<ProdutoDtoUpdateRequest>(produto);

        patchProdutoDto.ApplyTo(produtoToDto, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(produtoToDto, produto);

        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        await _uof.Commit();

        var responseProdutoDto = _mapper.Map<ProdutoDtoUpdateResponse>(produtoAtualizado);

        return Ok(responseProdutoDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProdutoDTO>> UpdateProduct(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.ProdutoId)
        {
            _logger.LogWarning("Id do produto não corresponde ao id passado.");
            return BadRequest("Id do produto não corresponde ao id passado.");
        }

        var produto = _mapper.Map<Produto>(produtoDto);
        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        await _uof.Commit();

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ProdutoDTO>> DeleteProduct(int id)
    {
        var produtoExiste = await _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
        if (produtoExiste is null)
        {
            _logger.LogWarning("Produto não encontrado.");
            return NotFound("Produto não encontrado.");
        }

        var produtoRemovido = _uof.ProdutoRepository.Delete(produtoExiste);
        await _uof.Commit();

        var produtoRemovidoDto = _mapper.Map<ProdutoDTO>(produtoRemovido);


        return Ok(
            $"Produto {produtoRemovidoDto.Nome} de Id = {produtoRemovidoDto.ProdutoId} foi removido com sucesso!");
    }
}