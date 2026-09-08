using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PPEInventory.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationalStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationalUnitId",
                table: "PPERequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppliedMaxQuantityPerRequest",
                table: "PPERequestItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganizationalUnitId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganizationalUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalUnits_OrganizationalUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalUnitPPELimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationalUnitId = table.Column<int>(type: "int", nullable: false),
                    PPEProductId = table.Column<int>(type: "int", nullable: false),
                    MaxQuantityPerRequest = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalUnitPPELimits", x => x.Id);
                    table.CheckConstraint("CK_OrganizationalUnitPPELimits_MaxQuantity", "[MaxQuantityPerRequest] > 0");
                    table.ForeignKey(
                        name: "FK_OrganizationalUnitPPELimits_OrganizationalUnits_OrganizationalUnitId",
                        column: x => x.OrganizationalUnitId,
                        principalTable: "OrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationalUnitPPELimits_PPEProducts_PPEProductId",
                        column: x => x.PPEProductId,
                        principalTable: "PPEProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
    """
    /* =========================================================
       1. Departments -> OrganizationalUnits
       ========================================================= */
    INSERT INTO OrganizationalUnits
    (
        Name,
        Description,
        Type,
        ParentId,
        IsActive,
        CreatedAt,
        UpdatedAt
    )
    SELECT
        d.Name,
        d.Description,
        'Department',
        NULL,
        d.IsActive,
        d.CreatedAt,
        d.UpdatedAt
    FROM Departments d
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM OrganizationalUnits ou
        WHERE ou.ParentId IS NULL
          AND ou.Type = 'Department'
          AND ou.Name = d.Name
    );


    /* =========================================================
       2. ProductionLines -> OrganizationalUnits
       ========================================================= */
    INSERT INTO OrganizationalUnits
    (
        Name,
        Description,
        Type,
        ParentId,
        IsActive,
        CreatedAt,
        UpdatedAt
    )
    SELECT
        pl.Name,
        pl.Description,
        'Line',
        departmentUnit.Id,
        pl.IsActive,
        pl.CreatedAt,
        pl.UpdatedAt
    FROM ProductionLines pl
    INNER JOIN Departments d
        ON d.Id = pl.DepartmentId
    INNER JOIN OrganizationalUnits departmentUnit
        ON departmentUnit.ParentId IS NULL
       AND departmentUnit.Type = 'Department'
       AND departmentUnit.Name = d.Name
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM OrganizationalUnits lineUnit
        WHERE lineUnit.ParentId = departmentUnit.Id
          AND lineUnit.Type = 'Line'
          AND lineUnit.Name = pl.Name
    );


    /* =========================================================
       3. Employees -> OrganizationalUnitId
       Si tiene línea, usa Línea.
       Si no tiene línea, usa Departamento.
       ========================================================= */
    UPDATE e
    SET OrganizationalUnitId =
        COALESCE(
            lineUnit.Id,
            departmentUnit.Id
        )
    FROM Employees e
    INNER JOIN Departments d
        ON d.Id = e.DepartmentId
    INNER JOIN OrganizationalUnits departmentUnit
        ON departmentUnit.ParentId IS NULL
       AND departmentUnit.Type = 'Department'
       AND departmentUnit.Name = d.Name
    LEFT JOIN ProductionLines pl
        ON pl.Id = e.LineId
    LEFT JOIN OrganizationalUnits lineUnit
        ON lineUnit.ParentId = departmentUnit.Id
       AND lineUnit.Type = 'Line'
       AND lineUnit.Name = pl.Name
    WHERE e.OrganizationalUnitId IS NULL;


    /* =========================================================
       4. Snapshot organizacional para solicitudes existentes
       ========================================================= */
    UPDATE r
    SET OrganizationalUnitId =
        e.OrganizationalUnitId
    FROM PPERequests r
    INNER JOIN Employees e
        ON e.Id = r.EmployeeId
    WHERE r.OrganizationalUnitId IS NULL;


    /* =========================================================
       5. Snapshot del límite utilizado en solicitudes anteriores.

       Antes de este refactor, el único límite existente era
       PPEProducts.MaxQuantityPerRequest, así que ese es el
       máximo históricamente aplicable.
       ========================================================= */
    UPDATE item
    SET AppliedMaxQuantityPerRequest =
        product.MaxQuantityPerRequest
    FROM PPERequestItems item
    INNER JOIN PPEProducts product
        ON product.Id = item.PPEProductId
    WHERE item.AppliedMaxQuantityPerRequest IS NULL;
    """
);


            migrationBuilder.CreateIndex(
                name: "IX_PPERequests_OrganizationalUnitId",
                table: "PPERequests",
                column: "OrganizationalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_OrganizationalUnitId",
                table: "Employees",
                column: "OrganizationalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnitPPELimits_OrganizationalUnitId_PPEProductId",
                table: "OrganizationalUnitPPELimits",
                columns: new[] { "OrganizationalUnitId", "PPEProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnitPPELimits_PPEProductId",
                table: "OrganizationalUnitPPELimits",
                column: "PPEProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnits_ParentId_Name",
                table: "OrganizationalUnits",
                columns: new[] { "ParentId", "Name" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_OrganizationalUnits_OrganizationalUnitId",
                table: "Employees",
                column: "OrganizationalUnitId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PPERequests_OrganizationalUnits_OrganizationalUnitId",
                table: "PPERequests",
                column: "OrganizationalUnitId",
                principalTable: "OrganizationalUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_OrganizationalUnits_OrganizationalUnitId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PPERequests_OrganizationalUnits_OrganizationalUnitId",
                table: "PPERequests");

            migrationBuilder.DropTable(
                name: "OrganizationalUnitPPELimits");

            migrationBuilder.DropTable(
                name: "OrganizationalUnits");

            migrationBuilder.DropIndex(
                name: "IX_PPERequests_OrganizationalUnitId",
                table: "PPERequests");

            migrationBuilder.DropIndex(
                name: "IX_Employees_OrganizationalUnitId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "OrganizationalUnitId",
                table: "PPERequests");

            migrationBuilder.DropColumn(
                name: "AppliedMaxQuantityPerRequest",
                table: "PPERequestItems");

            migrationBuilder.DropColumn(
                name: "OrganizationalUnitId",
                table: "Employees");
        }
    }
}
