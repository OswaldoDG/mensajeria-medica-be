using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contabee.services.identity.migrations
{
    /// <inheritdoc />
    public partial class RevisionEntidadesIdentidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DispositivoId",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispositivoId",
                table: "AspNetUsers");
        }
    }
}
