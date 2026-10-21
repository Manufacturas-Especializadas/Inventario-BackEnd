using MediatR;

namespace PPEInventory.Application.Features.PPERequests.Commands.Reject;

public record RejectPPERequestCommand(string Folio, string RejectionReason)
    : IRequest<RejectPPERequestResultDto>;
