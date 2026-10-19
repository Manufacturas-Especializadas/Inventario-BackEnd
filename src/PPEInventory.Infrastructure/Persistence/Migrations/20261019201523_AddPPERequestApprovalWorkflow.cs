using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPPERequestApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "PPERequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByUserId",
                table: "PPERequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "PPERequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RejectedByUserId",
                table: "PPERequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "PPERequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_ApprovedByUserId",
                table: "PPERequests",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_RejectedByUserId",
                table: "PPERequests",
                column: "RejectedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PPERequests_Users_ApprovedByUserId",
                table: "PPERequests",
                column: "ApprovedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PPERequests_Users_RejectedByUserId",
                table: "PPERequests",
                column: "RejectedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPERequests_Users_ApprovedByUserId",
                table: "PPERequests");

            migrationBuilder.DropForeignKey(
                name: "FK_PPERequests_Users_RejectedByUserId",
                table: "PPERequests");

            migrationBuilder.DropIndex(
                name: "IX_PPERequests_ApprovedByUserId",
                table: "PPERequests");

            migrationBuilder.DropIndex(
                name: "IX_PPERequests_RejectedByUserId",
                table: "PPERequests");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "PPERequests");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "PPERequests");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "PPERequests");

            migrationBuilder.DropColumn(
                name: "RejectedByUserId",
                table: "PPERequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "PPERequests");
        }
    }
}
