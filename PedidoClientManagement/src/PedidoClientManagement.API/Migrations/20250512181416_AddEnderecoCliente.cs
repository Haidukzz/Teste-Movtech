using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PedidoClientManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEnderecoCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Clientes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Clientes");
        }
    }
}
