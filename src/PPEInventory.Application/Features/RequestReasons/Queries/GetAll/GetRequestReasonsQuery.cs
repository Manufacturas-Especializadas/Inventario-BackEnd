using MediatR;

namespace PPEInventory.Application.Features.RequestReasons.Queries.GetAll;

public record GetRequestReasonsQuery
    : IRequest<IReadOnlyList<RequestReasonDto>>;