using MediatR;
using PPEInventory.Application.Interfaces;

namespace PPEInventory.Application.Features.Users.Queries.GetAll;

public class GetUsersQueryHandler
    : IRequestHandler<
        GetUsersQuery,
        IReadOnlyList<UserDto>>
{
    private readonly IUserRepository
        _userRepository;


    public GetUsersQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository =
            userRepository;
    }


    public async Task<IReadOnlyList<UserDto>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users =
            await _userRepository
                .GetAllWithDetailsAsync(
                    cancellationToken);

        return users
            .Select(x => x.ToDto())
            .ToArray();
    }
}