using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.InventoryCounts.Queries.GetDrafts;

public class GetDraftInventoryCountsQueryHandler
    : IRequestHandler<
        GetDraftInventoryCountsQuery,
        IReadOnlyList<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repository;

    public GetDraftInventoryCountsQueryHandler(
        IInventoryCountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryCountDto>> Handle(
        GetDraftInventoryCountsQuery request,
        CancellationToken cancellationToken)
    {
        var counts =
            await _repository.GetDraftCountsAsync(
                cancellationToken);

        return counts
            .Select(x => x.ToDto())
            .ToArray();
    }
}