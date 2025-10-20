using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.DTOs;
using TagService.Models;

namespace TagService.Application.Tags.Commands;

public record CreateTagCommand(string TagName, string? Color, int? GroupId, string CognitoSub) 
    : ICommand<TagResponse>;

public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagResponse>
{
    private readonly TagServiceDbContext _context;

    public CreateTagCommandHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<TagResponse> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Tags
            .AnyAsync(t => t.TagName == request.TagName 
                        && t.CognitoSub == request.CognitoSub 
                        && t.GroupId == request.GroupId, cancellationToken);

        if (exists)
            throw new InvalidOperationException("Tag with this name already exists");

        var tag = new Tag
        {
            TagName = request.TagName,
            Color = request.Color ?? "#808080",
            CognitoSub = request.CognitoSub,
            GroupId = request.GroupId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tags.Add(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return new TagResponse
        {
            TagId = tag.TagId,
            TagName = tag.TagName,
            Color = tag.Color,
            GroupId = tag.GroupId,
            CreatedAt = tag.CreatedAt
        };
    }
}
