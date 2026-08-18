using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Suppliers.Queries.GetAll;

public class GetSuppliersQueryHandler
    : IRequestHandler<
        GetSuppliersQuery,
        IReadOnlyList<SupplierDto>>
{
    private readonly ISupplierRepository _repository;

    public GetSuppliersQueryHandler(
        ISupplierRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SupplierDto>> Handle(
        GetSuppliersQuery request,
        CancellationToken cancellationToken)
    {
        var suppliers =
            await _repository.GetAllAsync(
                cancellationToken);

        return suppliers
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                ContactName = x.ContactName,
                Email = x.Email,
                Phone = x.Phone,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }
}