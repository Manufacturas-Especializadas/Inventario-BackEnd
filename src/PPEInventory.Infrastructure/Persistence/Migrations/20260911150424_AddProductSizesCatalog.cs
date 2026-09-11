using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSizesCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSizes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSizes_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_CreatedByUserId",
                table: "ProductSizes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_Name",
                table: "ProductSizes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductSizes_UpdatedByUserId",
                table: "ProductSizes",
                column: "UpdatedByUserId");

            migrationBuilder.Sql(
    """
    INSERT INTO ProductSizes
    (
        Name,
        IsActive,
        CreatedAt,
        CreatedByUserId,
        UpdatedAt,
        UpdatedByUserId
    )
    SELECT
        Existing.SizeName,
        1,
        SYSUTCDATETIME(),
        NULL,
        NULL,
        NULL
    FROM
    (
        SELECT DISTINCT
            LTRIM(RTRIM(Size)) AS SizeName
        FROM PPEProducts
        WHERE NULLIF(LTRIM(RTRIM(Size)), '') IS NOT NULL
    ) AS Existing
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM ProductSizes PS
        WHERE PS.Name = Existing.SizeName
    );
    """
);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSizes");
        }
    }
}
