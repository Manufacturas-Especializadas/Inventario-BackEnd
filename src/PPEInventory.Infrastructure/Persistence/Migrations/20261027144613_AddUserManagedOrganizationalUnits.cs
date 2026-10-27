using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserManagedOrganizationalUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserManagedOrganizationalUnits",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrganizationalUnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserManagedOrganizationalUnits", x => new { x.UserId, x.OrganizationalUnitId });
                    table.ForeignKey(
                        name: "FK_UserManagedOrganizationalUnits_OrganizationalUnits_OrganizationalUnitId",
                        column: x => x.OrganizationalUnitId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserManagedOrganizationalUnits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserManagedOrganizationalUnits_OrganizationalUnitId",
                table: "UserManagedOrganizationalUnits",
                column: "OrganizationalUnitId");

            // Preserve existing Production scope, including inactive accounts/units for later reactivation.
            // Runtime checks still require an active user, employee and managed unit.
            migrationBuilder.Sql("""
                INSERT INTO [UserManagedOrganizationalUnits] ([UserId], [OrganizationalUnitId])
                SELECT u.[Id], e.[OrganizationalUnitId]
                FROM [Users] u
                INNER JOIN [Employees] e ON e.[Id] = u.[EmployeeId]
                INNER JOIN [OrganizationalUnits] ou ON ou.[Id] = e.[OrganizationalUnitId]
                WHERE e.[OrganizationalUnitId] IS NOT NULL
                  AND EXISTS (
                      SELECT 1 FROM [UserRoles] ur
                      INNER JOIN [Roles] r ON r.[Id] = ur.[RoleId]
                      WHERE ur.[UserId] = u.[Id] AND r.[Name] = 'Production'
                  )
                  AND NOT EXISTS (
                      SELECT 1 FROM [UserManagedOrganizationalUnits] managed
                      WHERE managed.[UserId] = u.[Id]
                        AND managed.[OrganizationalUnitId] = e.[OrganizationalUnitId]
                  );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserManagedOrganizationalUnits");
        }
    }
}
