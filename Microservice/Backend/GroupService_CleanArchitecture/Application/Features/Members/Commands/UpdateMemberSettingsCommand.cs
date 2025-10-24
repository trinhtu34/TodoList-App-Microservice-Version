using Application.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Members.Commands;

public record UpdateMemberSettingsCommand(int GroupId, string? Nickname, bool? IsMuted, string UserId) : ICommand;

public class UpdateMemberSettingsCommandHandler : IRequestHandler<UpdateMemberSettingsCommand>
{
    private readonly IGroupMemberRepository _memberRepository;

    public UpdateMemberSettingsCommandHandler(IGroupMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task Handle(UpdateMemberSettingsCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.GroupId, request.UserId, cancellationToken);
        
        if (member == null || !member.IsActive)
            throw new UnauthorizedAccessException("Not a member");

        if (request.Nickname != null)
            member.Nickname = request.Nickname;

        if (request.IsMuted.HasValue)
            member.IsMuted = request.IsMuted.Value;

        await _memberRepository.UpdateAsync(member, cancellationToken);
    }
}
