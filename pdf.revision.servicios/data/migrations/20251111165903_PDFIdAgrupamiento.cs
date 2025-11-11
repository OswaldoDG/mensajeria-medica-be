using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdf.revision.servicios.data.migrations
{
    /// <inheritdoc />
    public partial class PDFIdAgrupamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdAgrupamiento",
                table: "parte_documental",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdAgrupamiento",
                table: "parte_documental");
        }
    }
}
