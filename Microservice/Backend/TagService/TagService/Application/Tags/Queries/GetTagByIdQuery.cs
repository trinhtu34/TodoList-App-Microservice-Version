using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.DTOs;
using TagService.Models;

namespace TagService.Application.Tags.Queries;

public record GetTagByIdQuery(int TagId, string CognitoSub) : IQuery<TagResponse>;

public class GetTagByIdQueryHandler : IRequestHandler<GetTagByIdQuery, TagResponse>
{
    private readonly TagServiceDbContext _context;

    public GetTagByIdQueryHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<TagResponse> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .Where(t => t.TagId == request.TagId && t.CognitoSub == request.CognitoSub)
            .Select(t => new TagResponse
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Color = t.Color,
                GroupId = t.GroupId,
                CreatedAt = t.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        return tag;
    }
}
