using GroupService.Application.Common;
using GroupService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroupService.Application.Members.Commands;

public record UpdateMemberSettingsCommand(int GroupId, string? Nickname, bool? IsMuted, string UserId) : ICommand<bool>;

public class UpdateMemberSettingsCommandHandler : IRequestHandler<UpdateMemberSettingsCommand, bool>
{
    private readonly GroupServiceDbContext _context;

    public UpdateMemberSettingsCommandHandler(GroupServiceDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateMemberSettingsCommand request, CancellationToken cancellationToken)
    {
        var member = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == request.GroupId && m.UserId == request.UserId && m.IsActive == true, cancellationToken)
            ?? throw new Exception("Not a member");

        if (request.Nickname != null)
            member.Nickname = request.Nickname;

        if (request.IsMuted.HasValue)
            member.IsMuted = request.IsMuted.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
