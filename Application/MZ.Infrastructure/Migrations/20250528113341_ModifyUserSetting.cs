using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyUserSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserSetting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserSetting",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
