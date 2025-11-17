using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POWStudio.Migrations
{
    /// <inheritdoc />
    public partial class AddRateTableAndPolishGameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TittleImageUrl",
                table: "Game",
                newName: "TitleImageUrl");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Game",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    bRecomended = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rate_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rate_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rate_GameId",
                table: "Rate",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Rate_UserId",
                table: "Rate",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rate");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Game");

            migrationBuilder.RenameColumn(
                name: "TitleImageUrl",
                table: "Game",
                newName: "TittleImageUrl");
        }
    }
}
