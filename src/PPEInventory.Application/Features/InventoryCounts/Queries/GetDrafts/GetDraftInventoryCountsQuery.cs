using MediatR;

namespace PPEInventory.Application.Features.InventoryCounts.Queries.GetDrafts;

public record GetDraftInventoryCountsQuery()
    : IRequest<IReadOnlyList<InventoryCountDto>>;