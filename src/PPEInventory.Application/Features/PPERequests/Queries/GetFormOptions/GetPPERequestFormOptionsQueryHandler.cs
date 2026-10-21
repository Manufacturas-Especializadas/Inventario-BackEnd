using MediatR;
using PPEInventory.Application.Features.RequestReasons;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.PPERequests.Queries.GetFormOptions;

public class GetPPERequestFormOptionsQueryHandler
    : IRequestHandler<GetPPERequestFormOptionsQuery, PPERequestFormOptionsDto>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IRequestReasonRepository _reasonRepository;

    public GetPPERequestFormOptionsQueryHandler(
        IWarehouseRepository warehouseRepository,
        IRequestReasonRepository reasonRepository)
    {
        _warehouseRepository = warehouseRepository;
        _reasonRepository = reasonRepository;
    }

    public async Task<PPERequestFormOptionsDto> Handle(
        GetPPERequestFormOptionsQuery request,
        CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseRepository.GetPPERequestOptionsAsync(
            cancellationToken);
        var reasons = await _reasonRepository.GetAllAsync(cancellationToken);

        return new PPERequestFormOptionsDto
        {
            Warehouses = warehouses,
            RequestReasons = reasons.Select(x => new RequestReasonDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description
            }).ToArray()
        };
    }
}
