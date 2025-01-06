using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudyAPI.Domain;

[Table("Produtos")] //redundante pois no DbContext ja estamos mapeando, mas é uma boa prática.
public class Produto
{
    [Key] //redundante pois por ter Id ele já é considerado chave primária
    public int ProdutoId { get; set; } // Necessita do nome Id para o Entity Framework reconhecer como chave primária
    
    [Required]
    [StringLength(80)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(300)]
    public string Descricao { get; set; } = string.Empty;
    
    [Required]
    [Column(TypeName = "decimal(10,2)")] //tipo decimal no banco de dados
    public decimal Preco { get; set; }
    
    [Required]
    [StringLength(300)]
    public string ImageUrl { get; set; } = string.Empty;
    
    public float Estoque { get; set; }
    
    public DateTime DataCadastro { get; set; }
    
    public int CategoriaId { get; set; } //relacionamento com a categoria
    
    [JsonIgnore]//isso pois Categoria é uma classe de navegação e nao precisa ser serializada.
    public Categoria? Categoria { get; set; } //cada produto esta mapeado a uma categoria
}