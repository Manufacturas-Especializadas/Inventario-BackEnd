using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPPECatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PPECategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPECategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPECategories_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPECategories_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPEProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SKU = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, computedColumnSql: "'EPP-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)", stored: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Size = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Specification = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    StockUnit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MinimumStock = table.Column<int>(type: "int", nullable: false),
                    MaxQuantityPerRequest = table.Column<int>(type: "int", nullable: true),
                    ReplacementIntervalDays = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPEProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPEProducts_PPECategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PPECategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPEProducts_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPEProducts_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPECategories_CreatedByUserId",
                table: "PPECategories",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPECategories_Name",
                table: "PPECategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPECategories_UpdatedByUserId",
                table: "PPECategories",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_CategoryId",
                table: "PPEProducts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_CreatedByUserId",
                table: "PPEProducts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_SKU",
                table: "PPEProducts",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_UpdatedByUserId",
                table: "PPEProducts",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPEProducts");

            migrationBuilder.DropTable(
                name: "PPECategories");
        }
    }
}
