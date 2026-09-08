using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Entities;

namespace PPEInventory.Application.Features.OrganizationalUnitPPELimits.Commands.Set;

public class SetOrganizationalUnitPPELimitCommandHandler
    : IRequestHandler<
        SetOrganizationalUnitPPELimitCommand,
        OrganizationalUnitPPELimitDto>
{
    private readonly
        IOrganizationalUnitRepository
            _organizationalUnitRepository;

    private readonly
        IOrganizationalUnitPPELimitRepository
            _limitRepository;

    private readonly
        IPPEProductRepository
            _productRepository;

    private readonly
        IDateTimeProvider
            _dateTimeProvider;

    public SetOrganizationalUnitPPELimitCommandHandler(
        IOrganizationalUnitRepository
            organizationalUnitRepository,
        IOrganizationalUnitPPELimitRepository
            limitRepository,
        IPPEProductRepository
            productRepository,
        IDateTimeProvider
            dateTimeProvider)
    {
        _organizationalUnitRepository =
            organizationalUnitRepository;

        _limitRepository =
            limitRepository;

        _productRepository =
            productRepository;

        _dateTimeProvider =
            dateTimeProvider;
    }

    public async Task<
        OrganizationalUnitPPELimitDto>
        Handle(
            SetOrganizationalUnitPPELimitCommand request,
            CancellationToken cancellationToken)
    {
        var unit =
            await _organizationalUnitRepository
                .GetByIdAsync(
                    request.OrganizationalUnitId,
                    cancellationToken);

        if (unit is null)
        {
            throw new NotFoundException(
                $"Organizational unit with id '{request.OrganizationalUnitId}' was not found.");
        }

        var product =
            await _productRepository.GetByIdAsync(
                request.PPEProductId,
                cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(
                $"PPE product with id '{request.PPEProductId}' was not found.");
        }

        if (request.IsActive &&
            !unit.IsActive)
        {
            throw new ConflictException(
                $"Organizational unit '{unit.Name}' is inactive.");
        }

        if (request.IsActive &&
            !product.IsActive)
        {
            throw new ConflictException(
                $"PPE product '{product.Sku}' is inactive.");
        }

        var existing =
            await _limitRepository
                .GetByUnitAndProductAsync(
                    unit.Id,
                    product.Id,
                    cancellationToken);

        var now =
            _dateTimeProvider.UtcNow;

        if (existing is null)
        {
            if (!request.IsActive)
            {
                throw new ConflictException(
                    "The PPE limit does not exist and therefore cannot be deactivated.");
            }

            existing =
                new OrganizationalUnitPPELimit
                {
                    OrganizationalUnitId =
                        unit.Id,

                    PPEProductId =
                        product.Id,

                    MaxQuantityPerCycle =
                        request.MaxQuantityPerCycle,

                    IsActive =
                        true,

                    CreatedAt =
                        now
                };

            await _limitRepository.AddAsync(
                existing,
                cancellationToken);
        }
        else
        {
            existing.MaxQuantityPerCycle =
                request.MaxQuantityPerCycle;

            existing.IsActive =
                request.IsActive;

            existing.UpdatedAt =
                now;
        }

        await _limitRepository.SaveChangesAsync(
            cancellationToken);

        return new OrganizationalUnitPPELimitDto
        {
            Id = existing.Id,

            OrganizationalUnitId =
                unit.Id,

            OrganizationalUnitName =
                unit.Name,

            PPEProductId =
                product.Id,

            Sku =
                product.Sku,

            ProductName =
                product.Name,

            MaxQuantityPerCycle =
                existing.MaxQuantityPerCycle,

            IsActive =
                existing.IsActive,

            CreatedAt =
                existing.CreatedAt,

            UpdatedAt =
                existing.UpdatedAt
        };
    }
}