using Microsoft.AspNetCore.Identity.EntityFrameworkCore;using Microsoft.EntityFrameworkCore;using StudyAPI.Models;namespace StudyAPI.Context;public class AppDbContext : IdentityDbContext<ApplicationUser>{    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }    public DbSet<Categoria> Categorias { get; set; }    public DbSet<Produto> Produtos { get; set; }    protected override void OnModelCreating(ModelBuilder builder)    {        base.OnModelCreating(builder);    }}/*    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }- está passando as opcoes do dbContextOptions para a classe base(DbContext)    public DbSet<Categoria> Categorias { get; set; }- mapeando entidade Categoria para a tabela Categorias    public DbSet<Produto> Produtos { get; set; }- mapeando entidade Produto para a tabela Produtos*/