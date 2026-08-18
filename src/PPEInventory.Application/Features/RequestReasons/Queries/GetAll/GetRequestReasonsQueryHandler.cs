using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.RequestReasons.Queries.GetAll;

public class GetRequestReasonsQueryHandler
    : IRequestHandler<
        GetRequestReasonsQuery,
        IReadOnlyList<RequestReasonDto>>
{
    private readonly IRequestReasonRepository _repository;

    public GetRequestReasonsQueryHandler(
        IRequestReasonRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RequestReasonDto>> Handle(
        GetRequestReasonsQuery request,
        CancellationToken cancellationToken)
    {
        var reasons =
            await _repository.GetAllAsync(
                cancellationToken);

        return reasons
            .Select(x => new RequestReasonDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            })
            .ToArray();
    }
}