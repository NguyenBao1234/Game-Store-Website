using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POWStudio.Migrations
{
    /// <inheritdoc />
    public partial class AddGameScreenshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameScreenshot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    ScreenshotUrl = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameScreenshot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameScreenshot_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameScreenshot_GameId_ScreenshotUrl",
                table: "GameScreenshot",
                columns: new[] { "GameId", "ScreenshotUrl" },
                unique: true,
                filter: "[ScreenshotUrl] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameScreenshot");
        }
    }
}
