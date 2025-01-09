using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudyAPI.Domain;

[Table("Categorias")] //redundante pois no DbContext ja estamos mapeando, mas é uma boa prática
public class Categoria
{
    public Categoria() //inicializa a lista de produtos evitando possiveis exceções de NullReferenceException
    {
        Produtos = new List<Produto>();
    }
    [Key] //redundante pois por ter Id ele já é considerado chave primária
    public int CategoriaId { get; set; }
    
    [Required]
    [StringLength(80)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(300)]
    public string ImageUrl { get; set; } = string.Empty;
    public ICollection<Produto>? Produtos { get; set; } //categoria um ou mais produtos
}