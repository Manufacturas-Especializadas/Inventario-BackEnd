using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, computedColumnSql: "'IC-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)", stored: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedByUserId = table.Column<int>(type: "int", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedByUserId = table.Column<int>(type: "int", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<int>(type: "int", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCounts_Users_CancelledByUserId",
                        column: x => x.CancelledByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryCounts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryCounts_Users_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryCounts_Users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryCounts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCountItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryCountId = table.Column<int>(type: "int", nullable: false),
                    PPEProductId = table.Column<int>(type: "int", nullable: false),
                    CountedQuantity = table.Column<int>(type: "int", nullable: true),
                    SystemQuantitySnapshot = table.Column<int>(type: "int", nullable: true),
                    Variance = table.Column<int>(type: "int", nullable: true),
                    CountedByUserId = table.Column<int>(type: "int", nullable: true),
                    CountedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCountItems", x => x.Id);
                    table.CheckConstraint("CK_InventoryCountItems_CountedQuantity", "[CountedQuantity] IS NULL OR [CountedQuantity] >= 0");
                    table.CheckConstraint("CK_InventoryCountItems_SystemQuantity", "[SystemQuantitySnapshot] IS NULL OR [SystemQuantitySnapshot] >= 0");
                    table.ForeignKey(
                        name: "FK_InventoryCountItems_InventoryCounts_InventoryCountId",
                        column: x => x.InventoryCountId,
                        principalTable: "InventoryCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryCountItems_PPEProducts_PPEProductId",
                        column: x => x.PPEProductId,
                        principalTable: "PPEProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryCountItems_Users_CountedByUserId",
                        column: x => x.CountedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountItems_CountedByUserId",
                table: "InventoryCountItems",
                column: "CountedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountItems_InventoryCountId_PPEProductId",
                table: "InventoryCountItems",
                columns: new[] { "InventoryCountId", "PPEProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountItems_PPEProductId",
                table: "InventoryCountItems",
                column: "PPEProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCounts_CancelledByUserId",
                table: "InventoryCounts",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCounts_CreatedByUserId",
                table: "InventoryCounts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCounts_Folio",
                table: "InventoryCounts",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCounts_PostedByUserId",
                table: "InventoryCounts",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCounts_SubmittedByUserId",
                table: "InventoryCounts",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCounts_WarehouseId",
                table: "InventoryCounts",
                column: "WarehouseId",
                unique: true,
                filter: "[Status] IN ('Draft', 'PendingReview')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryCountItems");

            migrationBuilder.DropTable(
                name: "InventoryCounts");
        }
    }
}
