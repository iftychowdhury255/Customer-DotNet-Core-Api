using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerCoreApi.Migrations
{
    /// <inheritdoc />
    public partial class initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDetails_Products_ProductId1",
                table: "ProductDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductDetails_ProductId1",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                table: "Customers");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "ProductDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId1",
                table: "ProductDetails",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDetails_Products_ProductId1",
                table: "ProductDetails",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");
        }
    }
}
