using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.DTOs;
using TagService.Models;

namespace TagService.Application.TodoTags.Queries;

public record GetTagsForTodoQuery(int TodoId, string CognitoSub) : IQuery<List<TagResponse>>;

public class GetTagsForTodoQueryHandler : IRequestHandler<GetTagsForTodoQuery, List<TagResponse>>
{
    private readonly TagServiceDbContext _context;

    public GetTagsForTodoQueryHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<List<TagResponse>> Handle(GetTagsForTodoQuery request, CancellationToken cancellationToken)
    {
        return await _context.TodoTags
            .Where(tt => tt.TodoId == request.TodoId)
            .Include(tt => tt.Tag)
            .Select(tt => new TagResponse
            {
                TagId = tt.Tag.TagId,
                TagName = tt.Tag.TagName,
                Color = tt.Tag.Color,
                GroupId = tt.Tag.GroupId,
                CreatedAt = tt.Tag.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
