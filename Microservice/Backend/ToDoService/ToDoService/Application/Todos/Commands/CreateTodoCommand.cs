using MediatR;
using ToDoService.Application.Common;
using ToDoService.DTOs;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Application.Todos.Commands;

public record CreateTodoCommand(
    string Description,
    DateTime? DueDate,
    int? GroupId,
    string? AssignedTo,
    List<int> TagIds,
    string UserId,
    string Token
) : ICommand<TodoResponse>;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoResponse>
{
    private readonly TodoServiceDbContext _context;
    private readonly ITagServiceClient _tagServiceClient;

    public CreateTodoCommandHandler(
        TodoServiceDbContext context,
        ITagServiceClient tagServiceClient)
    {
        _context = context;
        _tagServiceClient = tagServiceClient;
    }

    public async Task<TodoResponse> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo
        {
            Description = request.Description,
            DueDate = request.DueDate,
            CognitoSub = request.UserId,
            GroupId = request.GroupId,
            AssignedTo = request.AssignedTo,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            IsDone = false
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var tagId in request.TagIds)
        {
            await _tagServiceClient.AddTagToTodo(todo.TodoId, tagId, request.Token);
        }

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
