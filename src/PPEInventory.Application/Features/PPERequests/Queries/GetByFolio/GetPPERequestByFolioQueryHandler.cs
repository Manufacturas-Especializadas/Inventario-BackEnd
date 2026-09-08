using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetByFolio;

public class GetPPERequestByFolioQueryHandler
    : IRequestHandler<
        GetPPERequestByFolioQuery,
        PPERequestDto>
{
    private readonly IPPERequestRepository _repository;

    public GetPPERequestByFolioQueryHandler(
        IPPERequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<PPERequestDto> Handle(
        GetPPERequestByFolioQuery request,
        CancellationToken cancellationToken)
    {
        var folio =
            request.Folio.Trim().ToUpperInvariant();

        var ppeRequest =
            await _repository.GetByFolioAsync(
                folio,
                cancellationToken);

        if (ppeRequest is null)
        {
            throw new NotFoundException(
                $"PPE request '{folio}' was not found.");
        }

        return ppeRequest.ToDto();
    }
}