using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.DTOs;
using TagService.Models;

namespace TagService.Application.Tags.Queries;

public record GetTagsQuery(string CognitoSub, int? GroupId) : IQuery<List<TagResponse>>;

public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, List<TagResponse>>
{
    private readonly TagServiceDbContext _context;

    public GetTagsQueryHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<List<TagResponse>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tags.Where(t => t.CognitoSub == request.CognitoSub);

        if (request.GroupId.HasValue)
            query = query.Where(t => t.GroupId == request.GroupId.Value);

        return await query
            .Select(t => new TagResponse
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Color = t.Color,
                GroupId = t.GroupId,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
