using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDevSteam.Migrations
{
    /// <inheritdoc />
    public partial class categoriasmidia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "urlImagem",
                table: "Jogos",
                newName: "imageBanner");

            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Jogos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.CategoriaId);
                });

            migrationBuilder.CreateTable(
                name: "JogosMidia",
                columns: table => new
                {
                    JogoMidiaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogosMidia", x => x.JogoMidiaId);
                    table.ForeignKey(
                        name: "FK_JogosMidia_Jogos_GameId",
                        column: x => x.GameId,
                        principalTable: "Jogos",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JogosCategorias",
                columns: table => new
                {
                    JogoCategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogosCategorias", x => x.JogoCategoriaId);
                    table.ForeignKey(
                        name: "FK_JogosCategorias_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JogosCategorias_Jogos_GameId",
                        column: x => x.GameId,
                        principalTable: "Jogos",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JogosCategorias_CategoriaId",
                table: "JogosCategorias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_JogosCategorias_GameId",
                table: "JogosCategorias",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_JogosMidia_GameId",
                table: "JogosMidia",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JogosCategorias");

            migrationBuilder.DropTable(
                name: "JogosMidia");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Jogos");

            migrationBuilder.RenameColumn(
                name: "imageBanner",
                table: "Jogos",
                newName: "urlImagem");
        }
    }
}
