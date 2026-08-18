using MediatR;

namespace PPEInventory.Application.Features.Suppliers.Queries.GetAll;

public record GetSuppliersQuery
    : IRequest<IReadOnlyList<SupplierDto>>;