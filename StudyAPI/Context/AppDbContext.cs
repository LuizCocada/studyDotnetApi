using Microsoft.EntityFrameworkCore;
using StudyAPI.Domain;

namespace StudyAPI.Context;

public class AppDbContext : DbContext //representa uma sessao com o banco de dados.
{
    public AppDbContext(DbContextOptions<AppDbContext> options) :
        base(options) //passando as opções(connection string, etc) para a classe base do dbContext
    {
    }

    public DbSet<Categoria> Categorias { get; set; } //mapeando entidade Categoria para a tabela Categorias
    public DbSet<Produto> Produtos { get; set; } //mapeando entidade Produto para a tabela Produtos
}