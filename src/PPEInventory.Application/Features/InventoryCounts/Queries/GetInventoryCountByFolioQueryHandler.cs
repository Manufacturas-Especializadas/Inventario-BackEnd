using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.InventoryCounts.Queries.GetByFolio;

public class GetInventoryCountByFolioQueryHandler
    : IRequestHandler<
        GetInventoryCountByFolioQuery,
        InventoryCountDto>
{
    private readonly IInventoryCountRepository _repository;

    public GetInventoryCountByFolioQueryHandler(
        IInventoryCountRepository repository)
    {
        _repository = repository;
    }

    public async Task<InventoryCountDto> Handle(
        GetInventoryCountByFolioQuery request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.Folio.Trim().ToUpperInvariant();

        var count =
            await _repository.GetByFolioAsync(
                folio,
                cancellationToken);

        if (count is null)
        {
            throw new NotFoundException(
                $"Inventory count '{folio}' was not found.");
        }

        return count.ToDto();
    }
}