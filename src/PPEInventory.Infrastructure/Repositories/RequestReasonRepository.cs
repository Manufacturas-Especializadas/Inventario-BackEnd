using Microsoft.EntityFrameworkCore;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;
using PPEInventory.Infrastructure.Persistence;

namespace PPEInventory.Infrastructure.Repositories;

public class RequestReasonRepository
    : IRequestReasonRepository
{
    private readonly ApplicationDbContext _context;

    public RequestReasonRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RequestReason>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.RequestReasons
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<RequestReason?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return _context.RequestReasons
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }
}
