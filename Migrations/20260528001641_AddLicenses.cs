using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProviderAssignmentStarter.Migrations
{
    /// <inheritdoc />
    public partial class AddLicenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Licenses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Licenses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Licenses");
        }
    }
}
