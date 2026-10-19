using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Commands.Approve;

public record ApprovePPERequestCommand(string Folio)
    : IRequest<ApprovePPERequestResultDto>;
