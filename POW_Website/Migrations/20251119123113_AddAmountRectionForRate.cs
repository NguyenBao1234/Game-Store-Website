using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POWStudio.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountRectionForRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DislikeAmount",
                table: "Rate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FunnyAmount",
                table: "Rate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LikeAmount",
                table: "Rate",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DislikeAmount",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "FunnyAmount",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "LikeAmount",
                table: "Rate");
        }
    }
}
