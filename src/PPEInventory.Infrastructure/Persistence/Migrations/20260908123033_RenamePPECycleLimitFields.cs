using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamePPECycleLimitFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPERequests_OrganizationalUnits_OrganizationalUnitId",
                table: "PPERequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrganizationalUnitPPELimits_MaxQuantity",
                table: "OrganizationalUnitPPELimits");

            migrationBuilder.RenameColumn(
                name: "OrganizationalUnitId",
                table: "PPERequests",
                newName: "RequestedForOrganizationalUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_PPERequests_OrganizationalUnitId",
                table: "PPERequests",
                newName: "IX_PPERequests_RequestedForOrganizationalUnitId");

            migrationBuilder.RenameColumn(
                name: "AppliedMaxQuantityPerRequest",
                table: "PPERequestItems",
                newName: "AppliedMaxQuantityPerCycle");

            migrationBuilder.RenameColumn(
                name: "MaxQuantityPerRequest",
                table: "PPEProducts",
                newName: "DefaultMaxQuantityPerCycle");

            migrationBuilder.RenameColumn(
                name: "MaxQuantityPerRequest",
                table: "OrganizationalUnitPPELimits",
                newName: "MaxQuantityPerCycle");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrganizationalUnitPPELimits_MaxQuantity",
                table: "OrganizationalUnitPPELimits",
                sql: "[MaxQuantityPerCycle] > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_PPERequests_OrganizationalUnits_RequestedForOrganizationalUnitId",
                table: "PPERequests",
                column: "RequestedForOrganizationalUnitId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PPERequests_OrganizationalUnits_RequestedForOrganizationalUnitId",
                table: "PPERequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_OrganizationalUnitPPELimits_MaxQuantity",
                table: "OrganizationalUnitPPELimits");

            migrationBuilder.RenameColumn(
                name: "RequestedForOrganizationalUnitId",
                table: "PPERequests",
                newName: "OrganizationalUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_PPERequests_RequestedForOrganizationalUnitId",
                table: "PPERequests",
                newName: "IX_PPERequests_OrganizationalUnitId");

            migrationBuilder.RenameColumn(
                name: "AppliedMaxQuantityPerCycle",
                table: "PPERequestItems",
                newName: "AppliedMaxQuantityPerRequest");

            migrationBuilder.RenameColumn(
                name: "DefaultMaxQuantityPerCycle",
                table: "PPEProducts",
                newName: "MaxQuantityPerRequest");

            migrationBuilder.RenameColumn(
                name: "MaxQuantityPerCycle",
                table: "OrganizationalUnitPPELimits",
                newName: "MaxQuantityPerRequest");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrganizationalUnitPPELimits_MaxQuantity",
                table: "OrganizationalUnitPPELimits",
                sql: "[MaxQuantityPerRequest] > 0");

            migrationBuilder.AddForeignKey(
                name: "FK_PPERequests_OrganizationalUnits_OrganizationalUnitId",
                table: "PPERequests",
                column: "OrganizationalUnitId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
