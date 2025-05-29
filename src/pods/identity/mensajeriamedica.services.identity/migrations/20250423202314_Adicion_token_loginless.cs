using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contabee.services.identity.data.migrations
{
    /// <inheritdoc />
    public partial class Adicion_token_loginless : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "seguridad$tokensloginless",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Caduca = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Caducidad = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CuentaFiscalId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UsuarioCreador = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seguridad$tokensloginless", x => x.Id);
                    table.ForeignKey(
                        name: "FK_seguridad$tokensloginless_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tokensloginless_Caduca_Caducidad",
                table: "seguridad$tokensloginless",
                columns: new[] { "Caduca", "Caducidad" });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tokensloginless_Token",
                table: "seguridad$tokensloginless",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tokensloginless_UsuarioId",
                table: "seguridad$tokensloginless",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "seguridad$tokensloginless");
        }
    }
}
