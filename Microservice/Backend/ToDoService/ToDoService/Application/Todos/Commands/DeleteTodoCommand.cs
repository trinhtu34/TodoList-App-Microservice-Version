using MediatR;
using ToDoService.Application.Common;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Application.Todos.Commands;

public record DeleteTodoCommand(int TodoId, string UserId) : ICommand<bool>;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, bool>
{
    private readonly TodoServiceDbContext _context;
    private readonly ITagServiceClient _tagServiceClient;

    public DeleteTodoCommandHandler(
        TodoServiceDbContext context,
        ITagServiceClient tagServiceClient)
    {
        _context = context;
        _tagServiceClient = tagServiceClient;
    }

    public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FindAsync(request.TodoId);
        if (todo == null)
            throw new KeyNotFoundException("Todo not found");

        if (todo.CognitoSub != request.UserId)
            throw new UnauthorizedAccessException();

        await _tagServiceClient.RemoveTagsForTodo(request.TodoId);

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
