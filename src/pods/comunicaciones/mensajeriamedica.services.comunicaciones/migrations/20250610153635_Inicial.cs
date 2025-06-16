using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mensajeriamedica.services.comunicaciones.migrations
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
                name: "msj$estadisticasmensajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServidorId = table.Column<long>(type: "bigint", nullable: false),
                    SucursalId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ano = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Dia = table.Column<int>(type: "int", nullable: false),
                    Procesados = table.Column<int>(type: "int", nullable: false),
                    Enviados = table.Column<int>(type: "int", nullable: false),
                    Erroneos = table.Column<int>(type: "int", nullable: false),
                    Duplicados = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msj$estadisticasmensajes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "msj$servidores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FolderFtp = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msj$servidores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "msj$mensajes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Hash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Telefono = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreContacto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServidorId = table.Column<long>(type: "bigint", nullable: false),
                    SucursalId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_msj$mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_msj$mensajes_msj$servidores_ServidorId",
                        column: x => x.ServidorId,
                        principalTable: "msj$servidores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_msj$estadisticasmensajes_Id",
                table: "msj$estadisticasmensajes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_msj$mensajes_Estado",
                table: "msj$mensajes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_msj$mensajes_FechaCreacion",
                table: "msj$mensajes",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_msj$mensajes_Hash",
                table: "msj$mensajes",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_msj$mensajes_Hash_FechaCreacion_Estado_SucursalId",
                table: "msj$mensajes",
                columns: new[] { "Hash", "FechaCreacion", "Estado", "SucursalId" });

            migrationBuilder.CreateIndex(
                name: "IX_msj$mensajes_ServidorId",
                table: "msj$mensajes",
                column: "ServidorId");

            migrationBuilder.CreateIndex(
                name: "IX_msj$mensajes_SucursalId",
                table: "msj$mensajes",
                column: "SucursalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "msj$estadisticasmensajes");

            migrationBuilder.DropTable(
                name: "msj$mensajes");

            migrationBuilder.DropTable(
                name: "msj$servidores");
        }
    }
}
