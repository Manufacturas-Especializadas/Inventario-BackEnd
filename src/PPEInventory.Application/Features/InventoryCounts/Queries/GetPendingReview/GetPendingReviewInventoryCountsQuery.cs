using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Queries.GetPendingReview;

public record GetPendingReviewInventoryCountsQuery
    : IRequest<IReadOnlyList<InventoryCountDto>>;