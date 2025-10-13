using MediatR;
using ToDoService.Application.Common;
using ToDoService.DTOs;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Application.Todos.Commands;

public record UpdateTodoCommand(
    int TodoId,
    string? Description,
    bool? IsDone,
    DateTime? DueDate,
    string? AssignedTo,
    string UserId,
    string Token
) : ICommand<TodoResponse>;

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, TodoResponse>
{
    private readonly TodoServiceDbContext _context;
    private readonly ITagServiceClient _tagServiceClient;

    public UpdateTodoCommandHandler(
        TodoServiceDbContext context,
        ITagServiceClient tagServiceClient)
    {
        _context = context;
        _tagServiceClient = tagServiceClient;
    }

    public async Task<TodoResponse> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FindAsync(request.TodoId);
        if (todo == null)
            throw new KeyNotFoundException("Todo not found");

        if (todo.CognitoSub != request.UserId)
            throw new UnauthorizedAccessException();

        if (request.Description != null) todo.Description = request.Description;
        if (request.IsDone.HasValue) todo.IsDone = request.IsDone.Value;
        if (request.DueDate.HasValue) todo.DueDate = request.DueDate.Value;
        if (request.AssignedTo != null) todo.AssignedTo = request.AssignedTo;

        todo.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var tags = await _tagServiceClient.GetTagsForTodo(todo.TodoId, request.Token);

        return new TodoResponse
        {
            TodoId = todo.TodoId,
            Description = todo.Description,
            IsDone = todo.IsDone,
            DueDate = todo.DueDate,
            CreateAt = todo.CreateAt,
            UpdateAt = todo.UpdateAt,
            GroupId = todo.GroupId,
            AssignedTo = todo.AssignedTo,
            Tags = tags.Select(t => new TagResponse
            {
                TagId = t.TagId,
                TagName = t.TagName,
                Color = t.Color
            }).ToList()
        };
    }
}
