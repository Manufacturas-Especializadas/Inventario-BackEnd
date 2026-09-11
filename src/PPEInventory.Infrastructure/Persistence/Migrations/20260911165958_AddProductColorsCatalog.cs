using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductColorsCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "PPEProducts");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "PPEProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductColors",
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
                    table.PrimaryKey("PK_ProductColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColors_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductColors_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPEProducts_ColorId",
                table: "PPEProducts",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColors_CreatedByUserId",
                table: "ProductColors",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColors_Name",
                table: "ProductColors",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductColors_UpdatedByUserId",
                table: "ProductColors",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPEProducts_ProductColors_ColorId",
                table: "PPEProducts",
                column: "ColorId",
                principalTable: "ProductColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPEProducts_ProductColors_ColorId",
                table: "PPEProducts");

            migrationBuilder.DropTable(
                name: "ProductColors");

            migrationBuilder.DropIndex(
                name: "IX_PPEProducts_ColorId",
                table: "PPEProducts");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "PPEProducts");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "PPEProducts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
