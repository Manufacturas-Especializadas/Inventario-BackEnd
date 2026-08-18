using Microsoft.EntityFrameworkCore;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<PPECategory> PPECategories => Set<PPECategory>();

    public DbSet<PPEProduct> PPEProducts => Set<PPEProduct>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<ProductSupplier> ProductSuppliers => Set<ProductSupplier>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();

    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();

    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();

    public DbSet<GoodsReceiptItem> GoodsReceiptItems => Set<GoodsReceiptItem>();

    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();

    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    public DbSet<RequestReason> RequestReasons => Set<RequestReason>();

    public DbSet<PPERequest> PPERequests => Set<PPERequest>();

    public DbSet<PPERequestItem> PPERequestItems => Set<PPERequestItem>();

    public DbSet<InventoryCount> InventoryCounts => Set<InventoryCount>();

    public DbSet<InventoryCountItem> InventoryCountItems => Set<InventoryCountItem>();

    public DbSet<InventoryAdjustment> InventoryAdjustments => Set<InventoryAdjustment>();

    public DbSet<InventoryAdjustmentItem> InventoryAdjustmentItems => Set<InventoryAdjustmentItem>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }




}