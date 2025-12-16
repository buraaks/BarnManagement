using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarnManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAnimalIdFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Animals",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_AnimalId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AnimalId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_AnimalId",
                table: "Products",
                column: "AnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Animals",
                table: "Products",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id");
        }
    }
}
