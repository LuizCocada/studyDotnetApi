namespace StudyAPI.Pagination;

public class ProdutoFiltroPreco : QueryStringParameters
{
    public decimal? Preco { get; set; }
    public string? PrecoCriterio { get; set; }
}