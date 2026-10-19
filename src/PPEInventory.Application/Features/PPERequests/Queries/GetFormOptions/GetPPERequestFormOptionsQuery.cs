using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetFormOptions;

public record GetPPERequestFormOptionsQuery : IRequest<PPERequestFormOptionsDto>;
