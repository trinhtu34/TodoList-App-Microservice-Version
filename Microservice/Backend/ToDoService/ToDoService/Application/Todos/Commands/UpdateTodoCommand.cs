using MediatR;
using ToDoService.Application.Common;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Application.Todos.Commands;

public record UpdateTodoCommand(
    int TodoId,
    string? Description,
    bool? IsDone,
    DateTime? DueDate,
    string? AssignedTo,
    string UserId
) : ICommand<bool>;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, bool>
{
    private readonly TodoServiceDbContext _context;

    public UpdateTodoCommandHandler(TodoServiceDbContext context, IGroupServiceClient_TempUnUse groupServiceClient)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FindAsync(request.TodoId);
        if (todo == null)
            throw new KeyNotFoundException("Todo not found");

        else if (todo.CognitoSub != request.UserId)
        {
            throw new UnauthorizedAccessException();
        }

        if (request.Description != null) todo.Description = request.Description;
        if (request.IsDone.HasValue) todo.IsDone = request.IsDone.Value;
        if (request.DueDate.HasValue) todo.DueDate = request.DueDate.Value;
        if (request.AssignedTo != null) todo.AssignedTo = request.AssignedTo;

        todo.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
