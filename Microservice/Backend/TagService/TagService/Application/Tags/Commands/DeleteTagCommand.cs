using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.Models;

namespace TagService.Application.Tags.Commands;

public record DeleteTagCommand(int TagId, string CognitoSub) : ICommand<Unit>;

public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Unit>
{
    private readonly TagServiceDbContext _context;

    public DeleteTagCommandHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .Include(t => t.TodoTags)
            .FirstOrDefaultAsync(t => t.TagId == request.TagId && t.CognitoSub == request.CognitoSub, cancellationToken);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        if (tag.TodoTags.Any())
            _context.TodoTags.RemoveRange(tag.TodoTags);

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
