using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdf.revision.servicios.data.migrations
{
    /// <inheritdoc />
    public partial class ProcesosMUltiples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConteoLOcal",
                table: "archivo_pdf",
                newName: "ConteoLocal");

            migrationBuilder.AddColumn<string>(
                name: "IdProceso",
                table: "archivo_pdf",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdProceso",
                table: "archivo_pdf");

            migrationBuilder.RenameColumn(
                name: "ConteoLocal",
                table: "archivo_pdf",
                newName: "ConteoLOcal");
        }
    }
}
