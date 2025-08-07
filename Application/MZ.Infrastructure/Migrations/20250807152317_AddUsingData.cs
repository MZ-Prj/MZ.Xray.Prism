using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUsingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UsingDate",
                table: "User",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsingDate",
                table: "User");
        }
    }
}
