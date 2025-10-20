using MediatR;
using Microsoft.EntityFrameworkCore;
using TagService.Application.Common;
using TagService.Models;

namespace TagService.Application.Tags.Commands;

public record UpdateTagCommand(int TagId, string? TagName, string? Color, string CognitoSub) 
    : ICommand<Unit>;

public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, Unit>
{
    private readonly TagServiceDbContext _context;

    public UpdateTagCommandHandler(TagServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _context.Tags
            .FirstOrDefaultAsync(t => t.TagId == request.TagId && t.CognitoSub == request.CognitoSub, cancellationToken);

        if (tag == null)
            throw new KeyNotFoundException("Tag not found");

        if (!string.IsNullOrEmpty(request.TagName))
        {
            var exists = await _context.Tags
                .AnyAsync(t => t.TagName == request.TagName 
                            && t.CognitoSub == request.CognitoSub 
                            && t.GroupId == tag.GroupId 
                            && t.TagId != request.TagId, cancellationToken);

            if (exists)
                throw new InvalidOperationException("Tag with this name already exists");

            tag.TagName = request.TagName;
        }

        if (!string.IsNullOrEmpty(request.Color))
            tag.Color = request.Color;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
