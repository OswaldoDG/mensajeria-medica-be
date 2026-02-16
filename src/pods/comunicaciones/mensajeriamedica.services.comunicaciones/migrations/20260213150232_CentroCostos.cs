using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mensajeriamedica.services.comunicaciones.migrations
{
    /// <inheritdoc />
    public partial class CentroCostos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "msj$centrocostos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Eliminado = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msj$centrocostos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "msj$unidadcostos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CentroCostosId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clave = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msj$unidadcostos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msj$unidadcostos_msj$centrocostos_CentroCostosId",
                        column: x => x.CentroCostosId,
                        principalTable: "msj$centrocostos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "msj$usuariocentrocostos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CentroCostosId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msj$usuariocentrocostos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msj$usuariocentrocostos_msj$centrocostos_CentroCostosId",
                        column: x => x.CentroCostosId,
                        principalTable: "msj$centrocostos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_msj$centrocostos_Id",
                table: "msj$centrocostos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_msj$unidadcostos_CentroCostosId",
                table: "msj$unidadcostos",
                column: "CentroCostosId");

            migrationBuilder.CreateIndex(
                name: "IX_msj$unidadcostos_Id",
                table: "msj$unidadcostos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_msj$usuariocentrocostos_CentroCostosId",
                table: "msj$usuariocentrocostos",
                column: "CentroCostosId");

            migrationBuilder.CreateIndex(
                name: "IX_msj$usuariocentrocostos_Id",
                table: "msj$usuariocentrocostos",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "msj$unidadcostos");

            migrationBuilder.DropTable(
                name: "msj$usuariocentrocostos");

            migrationBuilder.DropTable(
                name: "msj$centrocostos");
        }
    }
}
