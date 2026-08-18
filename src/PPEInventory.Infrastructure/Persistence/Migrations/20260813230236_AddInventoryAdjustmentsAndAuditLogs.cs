using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAdjustmentsAndAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OldValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValuesJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false, computedColumnSql: "'ADJ-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)", stored: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustments_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAdjustmentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryAdjustmentId = table.Column<int>(type: "int", nullable: false),
                    PPEProductId = table.Column<int>(type: "int", nullable: false),
                    QuantityAdjustment = table.Column<int>(type: "int", nullable: false),
                    PreviousOnHandQuantity = table.Column<int>(type: "int", nullable: false),
                    NewOnHandQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAdjustmentItems", x => x.Id);
                    table.CheckConstraint("CK_InventoryAdjustmentItems_NewOnHand", "[NewOnHandQuantity] >= 0");
                    table.CheckConstraint("CK_InventoryAdjustmentItems_PreviousOnHand", "[PreviousOnHandQuantity] >= 0");
                    table.CheckConstraint("CK_InventoryAdjustmentItems_QuantityAdjustment", "[QuantityAdjustment] <> 0");
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentItems_InventoryAdjustments_InventoryAdjustmentId",
                        column: x => x.InventoryAdjustmentId,
                        principalTable: "InventoryAdjustments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAdjustmentItems_PPEProducts_PPEProductId",
                        column: x => x.PPEProductId,
                        principalTable: "PPEProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedByUserId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "PerformedByUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentItems_InventoryAdjustmentId_PPEProductId",
                table: "InventoryAdjustmentItems",
                columns: new[] { "InventoryAdjustmentId", "PPEProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustmentItems_PPEProductId",
                table: "InventoryAdjustmentItems",
                column: "PPEProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_CreatedByUserId",
                table: "InventoryAdjustments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_Folio",
                table: "InventoryAdjustments",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAdjustments_WarehouseId",
                table: "InventoryAdjustments",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "InventoryAdjustmentItems");

            migrationBuilder.DropTable(
                name: "InventoryAdjustments");
        }
    }
}
