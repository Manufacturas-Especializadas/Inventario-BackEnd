using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.InventoryCounts.Queries.GetPendingReview;

public class GetPendingReviewInventoryCountsQueryHandler
    : IRequestHandler<
        GetPendingReviewInventoryCountsQuery,
        IReadOnlyList<InventoryCountDto>>
{
    private readonly IInventoryCountRepository _repository;

    public GetPendingReviewInventoryCountsQueryHandler(
        IInventoryCountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<InventoryCountDto>> Handle(
        GetPendingReviewInventoryCountsQuery request,
        CancellationToken cancellationToken)
    {
        var counts =
            await _repository.GetPendingReviewAsync(
                cancellationToken);

        return counts
            .Select(x => x.ToDto())
            .ToArray();
    }
}