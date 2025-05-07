using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDevSteam.Migrations
{
    /// <inheritdoc />
    public partial class UsuarioJogo123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuariosJogos",
                columns: table => new
                {
                    UsuarioJogoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosJogos", x => x.UsuarioJogoId);
                    table.ForeignKey(
                        name: "FK_UsuariosJogos_Jogos_GameId",
                        column: x => x.GameId,
                        principalTable: "Jogos",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosJogos_Jogos_GameId1",
                        column: x => x.GameId1,
                        principalTable: "Jogos",
                        principalColumn: "GameId");
                    table.ForeignKey(
                        name: "FK_UsuariosJogos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosJogos_GameId",
                table: "UsuariosJogos",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosJogos_GameId1",
                table: "UsuariosJogos",
                column: "GameId1");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosJogos_UsuarioId",
                table: "UsuariosJogos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuariosJogos");
        }
    }
}
