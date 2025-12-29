using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdf.revision.servicios.data.migrations
{
    /// <inheritdoc />
    public partial class FixesProduccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tecla",
                table: "tipo_documento",
                type: "varchar(1)",
                maxLength: 1,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ConteoLocal",
                table: "archivo_pdf",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RutaLocal",
                table: "archivo_pdf",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tecla",
                table: "tipo_documento");

            migrationBuilder.DropColumn(
                name: "ConteoLocal",
                table: "archivo_pdf");

            migrationBuilder.DropColumn(
                name: "RutaLocal",
                table: "archivo_pdf");
        }
    }
}
