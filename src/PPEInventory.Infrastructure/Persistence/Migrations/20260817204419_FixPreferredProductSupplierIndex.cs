using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixPreferredProductSupplierIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductSuppliers_PPEProductId",
                table: "ProductSuppliers");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuppliers_PPEProductId",
                table: "ProductSuppliers",
                column: "PPEProductId",
                unique: true,
                filter: "[IsPreferred] = 1 AND [IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductSuppliers_PPEProductId",
                table: "ProductSuppliers");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuppliers_PPEProductId",
                table: "ProductSuppliers",
                column: "PPEProductId",
                unique: true,
                filter: "[IsPreferred] = 1");
        }
    }
}
