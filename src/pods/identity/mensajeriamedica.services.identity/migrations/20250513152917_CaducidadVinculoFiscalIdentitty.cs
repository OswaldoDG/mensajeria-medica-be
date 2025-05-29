using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contabee.services.identity.migrations
{
    /// <inheritdoc />
    public partial class CaducidadVinculoFiscalIdentitty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_seguridad$tokensloginless_Caduca_Caducidad",
                table: "seguridad$tokensloginless");

            migrationBuilder.DropIndex(
                name: "IX_seguridad$tokensloginless_CuentaFiscalId",
                table: "seguridad$tokensloginless");

            migrationBuilder.DropColumn(
                name: "Caduca",
                table: "seguridad$tokensloginless");

            migrationBuilder.DropColumn(
                name: "Caducidad",
                table: "seguridad$tokensloginless");

            migrationBuilder.DropColumn(
                name: "CuentaFiscalId",
                table: "seguridad$tokensloginless");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Caduca",
                table: "seguridad$tokensloginless",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Caducidad",
                table: "seguridad$tokensloginless",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CuentaFiscalId",
                table: "seguridad$tokensloginless",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tokensloginless_Caduca_Caducidad",
                table: "seguridad$tokensloginless",
                columns: new[] { "Caduca", "Caducidad" });

            migrationBuilder.CreateIndex(
                name: "IX_seguridad$tokensloginless_CuentaFiscalId",
                table: "seguridad$tokensloginless",
                column: "CuentaFiscalId");
        }
    }
}
