using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PPEInventory.Infrastructure.Persistence;
using PPEInventory.Application.Interfaces;
using PPEInventory.Infrastructure.Repositories;
using PPEInventory.Infrastructure.Services;
using PPEInventory.Infrastructure.Authentication;

namespace PPEInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddScoped<IProductionLineRepository,ProductionLineRepository>();

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IPPECategoryRepository, PPECategoryRepository>();

        services.AddScoped<IPPEProductRepository, PPEProductRepository>();

        services.AddScoped<ISupplierRepository, SupplierRepository>();

        services.AddScoped<IWarehouseRepository, WarehouseRepository>();

        services.AddScoped<IProductSupplierRepository, ProductSupplierRepository>();

        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();

        services.AddScoped<IInventoryRepository, InventoryRepository>();

        services.AddScoped<IRequestReasonRepository, RequestReasonRepository>();

        services.AddScoped<IPPERequestRepository, PPERequestRepository>();

        services.AddScoped<IInventoryCountRepository, InventoryCountRepository>();

        services.AddScoped<IInventoryAdjustmentRepository, InventoryAdjustmentRepository>();

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        return services;
    }
}