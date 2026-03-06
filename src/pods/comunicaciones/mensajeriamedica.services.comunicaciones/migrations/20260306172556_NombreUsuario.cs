using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mensajeriamedica.services.comunicaciones.migrations
{
    /// <inheritdoc />
    public partial class NombreUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "msj$usuariocentrocostos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "msj$usuariocentrocostos");
        }
    }
}
