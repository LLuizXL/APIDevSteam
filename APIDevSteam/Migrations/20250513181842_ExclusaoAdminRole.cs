using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDevSteam.Migrations
{
    /// <inheritdoc />
    public partial class ExclusaoAdminRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRole",
                table: "Usuarios");

            migrationBuilder.CreateTable(
                name: "Cartoes",
                columns: table => new
                {
                    CartaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeTitular = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numeroCartao = table.Column<int>(type: "int", nullable: false),
                    codSeguranca = table.Column<int>(type: "int", nullable: false),
                    dataValidade = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartoes", x => x.CartaoId);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCartoes",
                columns: table => new
                {
                    usuarioCartaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCartoes", x => x.usuarioCartaoId);
                    table.ForeignKey(
                        name: "FK_UsuarioCartoes_Cartoes_CartaoId",
                        column: x => x.CartaoId,
                        principalTable: "Cartoes",
                        principalColumn: "CartaoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCartoes_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCartoes_CartaoId",
                table: "UsuarioCartoes",
                column: "CartaoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCartoes_UsuarioId1",
                table: "UsuarioCartoes",
                column: "UsuarioId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioCartoes");

            migrationBuilder.DropTable(
                name: "Cartoes");

            migrationBuilder.AddColumn<bool>(
                name: "AdminRole",
                table: "Usuarios",
                type: "bit",
                nullable: true);
        }
    }
}
