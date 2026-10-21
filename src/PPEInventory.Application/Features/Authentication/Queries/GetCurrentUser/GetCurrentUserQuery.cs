using MediatR;

namespace PPEInventory.Application.Features.Authentication.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<CurrentUserDto>;
