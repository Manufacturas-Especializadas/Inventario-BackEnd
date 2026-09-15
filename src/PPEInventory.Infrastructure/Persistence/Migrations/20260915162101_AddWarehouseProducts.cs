using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseProducts",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    PPEProductId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseProducts", x => new { x.WarehouseId, x.PPEProductId });
                    table.ForeignKey(
                        name: "FK_WarehouseProducts_PPEProducts_PPEProductId",
                        column: x => x.PPEProductId,
                        principalTable: "PPEProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseProducts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseProducts_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarehouseProducts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProducts_CreatedByUserId",
                table: "WarehouseProducts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProducts_PPEProductId",
                table: "WarehouseProducts",
                column: "PPEProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProducts_UpdatedByUserId",
                table: "WarehouseProducts",
                column: "UpdatedByUserId");

            migrationBuilder.Sql(
    """
    INSERT INTO WarehouseProducts
    (
        WarehouseId,
        PPEProductId,
        IsActive,
        CreatedAt,
        CreatedByUserId,
        UpdatedAt,
        UpdatedByUserId
    )
    SELECT DISTINCT
        ib.WarehouseId,
        ib.PPEProductId,
        1,
        SYSUTCDATETIME(),
        NULL,
        NULL,
        NULL
    FROM InventoryBalances AS ib
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM WarehouseProducts AS wp
        WHERE wp.WarehouseId = ib.WarehouseId
          AND wp.PPEProductId = ib.PPEProductId
    );
    """
);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseProducts");
        }
    }
}
