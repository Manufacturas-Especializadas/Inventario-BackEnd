using System.Text.Json;
using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;
using PPEInventory.Domain.Constants;
using PPEInventory.Domain.Entities;
using PPEInventory.Domain.Enums;

namespace PPEInventory.Application.Features.PPERequests.Commands.Reject;

public class RejectPPERequestCommandHandler
    : IRequestHandler<RejectPPERequestCommand, RejectPPERequestResultDto>
{
    private readonly IPPERequestRepository _requestRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RejectPPERequestCommandHandler(
        IPPERequestRepository requestRepository,
        IAuditLogRepository auditLogRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _requestRepository = requestRepository;
        _auditLogRepository = auditLogRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RejectPPERequestResultDto> Handle(
        RejectPPERequestCommand command,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Authenticated user was not found.");
        var folio = command.Folio.Trim().ToUpperInvariant();
        var rejectionReason = command.RejectionReason.Trim();

        await using var transaction = await _unitOfWork.BeginSerializableTransactionAsync(cancellationToken);
        try
        {
            var ppeRequest = await _requestRepository.GetByFolioForUpdateAsync(folio, cancellationToken)
                ?? throw new NotFoundException($"PPE request '{folio}' was not found.");
            if (ppeRequest.Status != PPERequestStatus.PendingApproval)
                throw new ConflictException($"PPE request '{folio}' cannot be rejected because its current status is '{ppeRequest.Status}'.");
            if (ppeRequest.RequestReason is null ||
                ppeRequest.RequestReason.Code is not
                    (RequestReasonCodes.Damage or RequestReasonCodes.Lost or RequestReasonCodes.Other))
                throw new ConflictException("Only exceptional PPE requests can be rejected.");

            var oldValues = JsonSerializer.Serialize(new
            {
                Status = ppeRequest.Status.ToString(),
                ppeRequest.ApprovedByUserId,
                ppeRequest.ApprovedAt,
                ppeRequest.RejectedByUserId,
                ppeRequest.RejectedAt,
                ppeRequest.RejectionReason
            });
            var now = _dateTimeProvider.UtcNow;
            ppeRequest.Status = PPERequestStatus.Rejected;
            ppeRequest.RejectedByUserId = userId;
            ppeRequest.RejectedAt = now;
            ppeRequest.RejectionReason = rejectionReason;
            ppeRequest.ApprovedByUserId = null;
            ppeRequest.ApprovedAt = null;

            await _auditLogRepository.AddAsync(new AuditLog
            {
                EntityName = nameof(PPERequest),
                EntityId = ppeRequest.Id.ToString(),
                Action = AuditActions.PPERequestRejected,
                Description = $"PPE request {ppeRequest.Folio} rejected.",
                OldValuesJson = oldValues,
                NewValuesJson = JsonSerializer.Serialize(new
                {
                    Status = ppeRequest.Status.ToString(),
                    ppeRequest.ApprovedByUserId,
                    ppeRequest.ApprovedAt,
                    ppeRequest.RejectedByUserId,
                    ppeRequest.RejectedAt,
                    ppeRequest.RejectionReason
                }),
                PerformedByUserId = userId,
                CreatedAt = now
            }, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new RejectPPERequestResultDto
            {
                Id = ppeRequest.Id,
                Folio = ppeRequest.Folio,
                Status = ppeRequest.Status,
                RejectedByUserId = userId,
                RejectedAt = now,
                RejectionReason = rejectionReason
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
