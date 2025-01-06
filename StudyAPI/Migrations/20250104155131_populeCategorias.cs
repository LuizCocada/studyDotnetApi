using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyAPI.Migrations
{
    /// <inheritdoc />
    public partial class populeCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Categorias (Nome, ImageUrl) VALUES ('Bebidas', 'Bebidas.png')");
            migrationBuilder.Sql("INSERT INTO Categorias (Nome, ImageUrl) VALUES ('Carnes', 'Carne.png')");
            migrationBuilder.Sql("INSERT INTO Categorias (Nome, ImageUrl) VALUES ('Sobremesas', 'Sobremesas.png')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Categorias");
        }
    }
}
