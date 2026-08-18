using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPPERequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPERequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false, computedColumnSql: "'EPP-' + CONVERT(varchar(4), DATEPART(year, [CreatedAt])) + '-' + RIGHT('000000' + CONVERT(varchar(20), [Id]), 6)", stored: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    RequestReasonId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredByUserId = table.Column<int>(type: "int", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelledByUserId = table.Column<int>(type: "int", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPERequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPERequests_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPERequests_RequestReasons_RequestReasonId",
                        column: x => x.RequestReasonId,
                        principalTable: "RequestReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPERequests_Users_CancelledByUserId",
                        column: x => x.CancelledByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPERequests_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPERequests_Users_DeliveredByUserId",
                        column: x => x.DeliveredByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPERequests_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPERequestItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PPERequestId = table.Column<int>(type: "int", nullable: false),
                    PPEProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPERequestItems", x => x.Id);
                    table.CheckConstraint("CK_PPERequestItems_Quantity", "[Quantity] > 0");
                    table.ForeignKey(
                        name: "FK_PPERequestItems_PPEProducts_PPEProductId",
                        column: x => x.PPEProductId,
                        principalTable: "PPEProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPERequestItems_PPERequests_PPERequestId",
                        column: x => x.PPERequestId,
                        principalTable: "PPERequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RequestReasons",
                columns: new[] { "Id", "Code", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "INITIAL_ASSIGNMENT", "Initial PPE assignment.", true, "Initial Assignment" },
                    { 2, "SCHEDULED_REPLACEMENT", "Replacement according to scheduled useful life.", true, "Scheduled Replacement" },
                    { 3, "WEAR", "Replacement due to normal wear.", true, "Wear" },
                    { 4, "DAMAGE", "Replacement due to damage.", true, "Damage" },
                    { 5, "LOST", "Replacement because PPE was lost.", true, "Lost" },
                    { 6, "JOB_CHANGE", "PPE required because of job or position change.", true, "Job Change" },
                    { 7, "OTHER", "Other justified reason.", true, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPERequestItems_PPEProductId",
                table: "PPERequestItems",
                column: "PPEProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequestItems_PPERequestId_PPEProductId",
                table: "PPERequestItems",
                columns: new[] { "PPERequestId", "PPEProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_CancelledByUserId",
                table: "PPERequests",
                column: "CancelledByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_CreatedByUserId",
                table: "PPERequests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_DeliveredByUserId",
                table: "PPERequests",
                column: "DeliveredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_EmployeeId",
                table: "PPERequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_Folio",
                table: "PPERequests",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_RequestReasonId",
                table: "PPERequests",
                column: "RequestReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_WarehouseId",
                table: "PPERequests",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestReasons_Code",
                table: "RequestReasons",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPERequestItems");

            migrationBuilder.DropTable(
                name: "PPERequests");

            migrationBuilder.DropTable(
                name: "RequestReasons");
        }
    }
}
