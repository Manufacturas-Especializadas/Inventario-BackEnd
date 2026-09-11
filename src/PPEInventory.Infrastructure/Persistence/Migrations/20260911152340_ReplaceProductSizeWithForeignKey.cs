using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceProductSizeWithForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "PPEProducts");

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "PPEProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_SizeId",
                table: "PPEProducts",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPEProducts_ProductSizes_SizeId",
                table: "PPEProducts",
                column: "SizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPEProducts_ProductSizes_SizeId",
                table: "PPEProducts");

            migrationBuilder.DropIndex(
                name: "IX_PPEProducts_SizeId",
                table: "PPEProducts");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "PPEProducts");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "PPEProducts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
