using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateZeffectControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ZeffectControl",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ZeffectControl_UserId",
                table: "ZeffectControl",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ZeffectControl_User_UserId",
                table: "ZeffectControl",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ZeffectControl_User_UserId",
                table: "ZeffectControl");

            migrationBuilder.DropIndex(
                name: "IX_ZeffectControl_UserId",
                table: "ZeffectControl");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ZeffectControl");
        }
    }
}
