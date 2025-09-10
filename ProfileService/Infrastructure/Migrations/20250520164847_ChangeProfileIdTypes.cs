using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProfileIdTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileLocations",
                table: "ProfileLocations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProfileLocations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProfileLocations",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileLocations",
                table: "ProfileLocations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileLocations",
                table: "ProfileLocations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProfileLocations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "ProfileLocations",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileLocations",
                table: "ProfileLocations",
                column: "UserId");
        }
    }
}
