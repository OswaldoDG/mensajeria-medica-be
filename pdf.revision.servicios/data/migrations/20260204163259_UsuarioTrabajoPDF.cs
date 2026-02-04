using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdf.revision.servicios.data.migrations
{
    /// <inheritdoc />
    public partial class UsuarioTrabajoPDF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "archivo_pdf",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "archivo_pdf");
        }
    }
}
