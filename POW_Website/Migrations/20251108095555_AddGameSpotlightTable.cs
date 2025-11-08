using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POWStudio.Migrations
{
    /// <inheritdoc />
    public partial class AddGameSpotlightTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSpotlight",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSpotlight", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_GameSpotlight_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSpotlight");
        }
    }
}
