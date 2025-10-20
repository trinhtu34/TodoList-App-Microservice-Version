using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.Models;

namespace TagService.Application.TodoTags.Commands;

public record RemoveTagFromTodoCommand(int TodoId, int TagId, string CognitoSub) : ICommand<Unit>;

public class RemoveTagFromTodoCommandHandler : IRequestHandler<RemoveTagFromTodoCommand, Unit>
{
    private readonly TagServiceDbContext _context;

    public RemoveTagFromTodoCommandHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RemoveTagFromTodoCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.TagId == request.TagId && t.CognitoSub == request.CognitoSub, cancellationToken);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        var todoTag = await _context.TodoTags
            .FirstOrDefaultAsync(tt => tt.TodoId == request.TodoId && tt.TagId == request.TagId, cancellationToken);

        if (todoTag == null)
            throw new KeyNotFoundException("Tag not associated with this todo");

        _context.TodoTags.Remove(todoTag);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
