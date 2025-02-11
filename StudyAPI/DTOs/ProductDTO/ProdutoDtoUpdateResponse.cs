using StudyAPI.Models;

namespace StudyAPI.DTOs.ProductDTO;

public class ProdutoDtoUpdateResponse
{
    public int ProdutoId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public decimal Preco { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public float Estoque { get; set; }

    public DateTime DataCadastro { get; set; }

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }
}