using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pdf.revision.servicios.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
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
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ruta = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estado = table.Column<int>(type: "int", nullable: false),
                    ultima_revision = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    total_paginas = table.Column<int>(type: "int", nullable: false),
                    prioridad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_archivo_pdf", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tipo_documento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_documento", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "revision_pdf",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    fecha_inicio_revision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    usuario_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    archivo_pdf_id = table.Column<int>(type: "int", nullable: false),
                    fecha_fin_revision = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revision_pdf", x => x.id);
                    table.ForeignKey(
                        name: "FK_revision_pdf_archivo_pdf_archivo_pdf_id",
                        column: x => x.archivo_pdf_id,
                        principalTable: "archivo_pdf",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "parte_documental",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    archivo_pdf_id = table.Column<int>(type: "int", nullable: false),
                    pagina_inicio = table.Column<int>(type: "int", nullable: false),
                    pagina_fin = table.Column<int>(type: "int", nullable: false),
                    tipo_documento_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parte_documental", x => x.id);
                    table.ForeignKey(
                        name: "FK_parte_documental_archivo_pdf_archivo_pdf_id",
                        column: x => x.archivo_pdf_id,
                        principalTable: "archivo_pdf",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parte_documental_tipo_documento_tipo_documento_id",
                        column: x => x.tipo_documento_id,
                        principalTable: "tipo_documento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_parte_documental_archivo_pdf_id",
                table: "parte_documental",
                column: "archivo_pdf_id");

            migrationBuilder.CreateIndex(
                name: "IX_parte_documental_tipo_documento_id",
                table: "parte_documental",
                column: "tipo_documento_id");

            migrationBuilder.CreateIndex(
                name: "IX_revision_pdf_archivo_pdf_id",
                table: "revision_pdf",
                column: "archivo_pdf_id");
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
