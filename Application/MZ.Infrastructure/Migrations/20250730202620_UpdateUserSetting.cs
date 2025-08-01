using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ZeffectControl",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserSetting",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserButton",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsVisibility = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserSettingId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserButton", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserButton_UserSetting_UserSettingId",
                        column: x => x.UserSettingId,
                        principalTable: "UserSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSetting_UserId",
                table: "UserSetting",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserButton_UserSettingId",
                table: "UserButton",
                column: "UserSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserButton");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting");

            migrationBuilder.DropIndex(
                name: "IX_UserSetting_UserId",
                table: "UserSetting");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ZeffectControl");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserSetting");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserSetting",
                table: "UserSetting",
                column: "UserId");
        }
    }
}
