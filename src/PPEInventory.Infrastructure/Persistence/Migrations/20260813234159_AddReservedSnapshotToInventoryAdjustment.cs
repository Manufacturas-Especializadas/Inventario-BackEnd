using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservedSnapshotToInventoryAdjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantitySnapshot",
                table: "InventoryAdjustmentItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryAdjustmentItems_Reserved",
                table: "InventoryAdjustmentItems",
                sql: "[ReservedQuantitySnapshot] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryAdjustmentItems_Reserved",
                table: "InventoryAdjustmentItems");

            migrationBuilder.DropColumn(
                name: "ReservedQuantitySnapshot",
                table: "InventoryAdjustmentItems");
        }
    }
}
