using MediatR;
using PPEInventory.Application.Common.Exceptions;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Users.Queries.GetById;

public class GetUserByIdQueryHandler
    : IRequestHandler<
        GetUserByIdQuery,
        UserDto>
{
    private readonly IUserRepository
        _userRepository;


    public GetUserByIdQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository =
            userRepository;
    }


    public async Task<UserDto> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository
                .GetByIdWithDetailsAsync(
                    request.Id,
                    cancellationToken);


        if (user is null)
        {
            throw new NotFoundException(
                $"User with id '{request.Id}' was not found.");
        }


        return user.ToDto();
    }
}