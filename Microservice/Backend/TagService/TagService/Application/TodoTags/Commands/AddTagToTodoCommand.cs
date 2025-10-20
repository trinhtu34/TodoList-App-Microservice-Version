using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.Models;

namespace TagService.Application.TodoTags.Commands;

public record AddTagToTodoCommand(int TodoId, int TagId, string CognitoSub) : ICommand<Unit>;

public class AddTagToTodoCommandHandler : IRequestHandler<AddTagToTodoCommand, Unit>
{
    private readonly TagServiceDbContext _context;

    public AddTagToTodoCommandHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(AddTagToTodoCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.TagId == request.TagId && t.CognitoSub == request.CognitoSub, cancellationToken);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        var exists = await _context.TodoTags
            .AnyAsync(tt => tt.TodoId == request.TodoId && tt.TagId == request.TagId, cancellationToken);

        if (exists)
            throw new InvalidOperationException("Tag already added to this todo");

        var todoTag = new TodoTag
        {
            TodoId = request.TodoId,
            TagId = request.TagId,
            CreatedAt = DateTime.UtcNow
        };

        _context.TodoTags.Add(todoTag);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
