using MediatR;
using PPEInventory.Application.Features.PPERequests.Delivery;

namespace PPEInventory.Application.Features.PPERequests.Commands.Deliver;

public record DeliverPPERequestCommand(
    string Folio,
    string EmployeeNumber)
    : IRequest<DeliverPPERequestResultDto>;