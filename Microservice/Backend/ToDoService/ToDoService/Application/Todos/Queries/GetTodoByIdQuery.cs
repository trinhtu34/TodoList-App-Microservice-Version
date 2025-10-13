using MediatR;
using ToDoService.Application.Common;
using ToDoService.DTOs;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Application.Todos.Queries;

public record GetTodoByIdQuery(int TodoId, string UserId, string Token) : IQuery<TodoResponse>;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoResponse>
{
    private readonly TodoServiceDbContext _context;
    private readonly ITagServiceClient _tagServiceClient;

    public GetTodoByIdQueryHandler(
        TodoServiceDbContext context,
        ITagServiceClient tagServiceClient)
    {
        _context = context;
        _tagServiceClient = tagServiceClient;
    }

    public async Task<TodoResponse> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FindAsync(request.TodoId);
        if (todo == null)
            throw new KeyNotFoundException("Todo not found");

        else if (todo.CognitoSub != request.UserId)
        {
            throw new UnauthorizedAccessException();
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
