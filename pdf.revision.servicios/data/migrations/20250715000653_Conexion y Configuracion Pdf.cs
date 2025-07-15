using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdf.revision.servicios.data.migrations
{
    /// <inheritdoc />
    public partial class ConexionyConfiguracionPdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "archivo_pdf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ruta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    UltimaRevision = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalPaginas = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_archivo_pdf", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipo_documento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_documento", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "revision_pdf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaInicioRevision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ArchivoPdfId = table.Column<int>(type: "int", nullable: false),
                    FechaFinRevision = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revision_pdf", x => x.Id);
                    table.ForeignKey(
                        name: "FK_revision_pdf_archivo_pdf_ArchivoPdfId",
                        column: x => x.ArchivoPdfId,
                        principalTable: "archivo_pdf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "parte_documental",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArchivoPdfId = table.Column<int>(type: "int", nullable: false),
                    PaginaInicio = table.Column<int>(type: "int", nullable: false),
                    PaginaFin = table.Column<int>(type: "int", nullable: false),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parte_documental", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parte_documental_archivo_pdf_ArchivoPdfId",
                        column: x => x.ArchivoPdfId,
                        principalTable: "archivo_pdf",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parte_documental_tipo_documento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "tipo_documento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_parte_documental_ArchivoPdfId",
                table: "parte_documental",
                column: "ArchivoPdfId");

            migrationBuilder.CreateIndex(
                name: "IX_parte_documental_TipoDocumentoId",
                table: "parte_documental",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_revision_pdf_ArchivoPdfId",
                table: "revision_pdf",
                column: "ArchivoPdfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parte_documental");

            migrationBuilder.DropTable(
                name: "revision_pdf");

            migrationBuilder.DropTable(
                name: "tipo_documento");

            migrationBuilder.DropTable(
                name: "archivo_pdf");
        }
    }
}
