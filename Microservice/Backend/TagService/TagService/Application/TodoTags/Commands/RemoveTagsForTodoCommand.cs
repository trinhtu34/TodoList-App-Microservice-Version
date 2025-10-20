using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.Models;

namespace TagService.Application.TodoTags.Commands;

public record RemoveTagsForTodoCommand(int TodoId) : ICommand<Unit>;

public class RemoveTagsForTodoCommandHandler : IRequestHandler<RemoveTagsForTodoCommand, Unit>
{
    private readonly TagServiceDbContext _context;

    public RemoveTagsForTodoCommandHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RemoveTagsForTodoCommand request, CancellationToken cancellationToken)
    {
        var todoTags = await _context.TodoTags
            .Where(tt => tt.TodoId == request.TodoId)
            .ToListAsync(cancellationToken);

        if (todoTags.Any())
        {
            _context.TodoTags.RemoveRange(todoTags);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
