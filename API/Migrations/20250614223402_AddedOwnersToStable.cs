using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddedOwnersToStable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Stables",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stables_OwnerId",
                table: "Stables",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stables_AspNetUsers_OwnerId",
                table: "Stables",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stables_AspNetUsers_OwnerId",
                table: "Stables");

            migrationBuilder.DropIndex(
                name: "IX_Stables_OwnerId",
                table: "Stables");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Stables");
        }
    }
}
