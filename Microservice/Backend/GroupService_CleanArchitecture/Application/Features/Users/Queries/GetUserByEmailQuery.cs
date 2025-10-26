using Application.Common;
using Domain.Repositories;
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
