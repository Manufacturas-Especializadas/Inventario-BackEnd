using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseUnitId",
                table: "ProductSuppliers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockUnitId",
                table: "PPEProducts",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
    """
    UPDATE P
    SET P.StockUnitId = U.Id
    FROM PPEProducts P
    INNER JOIN Units U
        ON U.Name = LTRIM(RTRIM(P.StockUnit));
    """
);

            migrationBuilder.Sql(
                """
    UPDATE PS
    SET PS.PurchaseUnitId = U.Id
    FROM ProductSuppliers PS
    INNER JOIN Units U
        ON U.Name = LTRIM(RTRIM(PS.PurchaseUnit));
    """
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuppliers_PurchaseUnitId",
                table: "ProductSuppliers",
                column: "PurchaseUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_StockUnitId",
                table: "PPEProducts",
                column: "StockUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPEProducts_Units_StockUnitId",
                table: "PPEProducts",
                column: "StockUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Units_PurchaseUnitId",
                table: "ProductSuppliers",
                column: "PurchaseUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPEProducts_Units_StockUnitId",
                table: "PPEProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Units_PurchaseUnitId",
                table: "ProductSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_ProductSuppliers_PurchaseUnitId",
                table: "ProductSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_PPEProducts_StockUnitId",
                table: "PPEProducts");

            migrationBuilder.DropColumn(
                name: "PurchaseUnitId",
                table: "ProductSuppliers");

            migrationBuilder.DropColumn(
                name: "StockUnitId",
                table: "PPEProducts");
        }
    }
}
