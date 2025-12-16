using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarnManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DecoupleProductFromAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AnimalId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "FarmId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_FarmId",
                table: "Products",
                column: "FarmId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Farms",
                table: "Products",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Farms",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_FarmId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FarmId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "AnimalId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
