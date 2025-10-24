using Application.Common;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUserByEmailQuery(string Email) : IQuery<string>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, string>
{
    private readonly IUserService _userService;

    public GetUserByEmailQueryHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<string> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return await _userService.GetUserByEmailAsync(request.Email, cancellationToken);
    }
}

public interface IUserService
{
    Task<string> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
