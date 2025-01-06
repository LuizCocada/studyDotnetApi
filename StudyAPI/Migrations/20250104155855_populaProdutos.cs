using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAPI.Migrations
{
    /// <inheritdoc />
    public partial class populaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(" INSERT INTO Produtos (Nome, Descricao, Preco, ImageUrl, Estoque, DataCadastro, CategoriaId) VALUES ('Suco de Uva', 'Suco de uva natual 500ml', 10.00, 'Sucodeuva.png', 10, '2025-01-04', 1)");
            migrationBuilder.Sql(" INSERT INTO Produtos (Nome, Descricao, Preco, ImageUrl, Estoque, DataCadastro, CategoriaId) VALUES ('Picanha', 'picanha 1kg', 200.00, 'Picanha.png', 5, '2025-01-04', 2)");
            migrationBuilder.Sql(" INSERT INTO Produtos (Nome, Descricao, Preco, ImageUrl, Estoque, DataCadastro, CategoriaId) VALUES ('Sorvete de morango', 'Sorvete natural de morango', 18.00, 'sorvete.png', 8, '2025-01-04', 3)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Produtos");
        }
    }
}
