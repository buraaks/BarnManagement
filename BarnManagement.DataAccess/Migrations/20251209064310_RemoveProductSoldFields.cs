using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarnManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductSoldFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SoldAt",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SoldAt",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }
    }
}
