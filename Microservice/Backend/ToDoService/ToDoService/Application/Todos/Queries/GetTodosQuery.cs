using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoService.Application.Common;
using ToDoService.DTOs;
using ToDoService.Models;
using ToDoService.ServiceClients;

namespace ToDoService.Application.Todos.Queries;

public record GetTodosQuery(string UserId, int? GroupId, string Token) : IQuery<List<TodoResponse>>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, List<TodoResponse>>
{
    private readonly TodoServiceDbContext _context;
    private readonly ITagServiceClient _tagServiceClient;

    public GetTodosQueryHandler(
        TodoServiceDbContext context,
        ITagServiceClient tagServiceClient)
    {
        _context = context;
        _tagServiceClient = tagServiceClient;
    }

    public async Task<List<TodoResponse>> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Todos.AsQueryable();

        if (request.GroupId.HasValue)
        {
            query = query.Where(t => t.GroupId == request.GroupId.Value);
        }
        else
        {
            query = query.Where(t => t.CognitoSub == request.UserId && t.GroupId == null);
        }

        var todos = await query.ToListAsync(cancellationToken);

        var responses = new List<TodoResponse>();
        foreach (var todo in todos)
        {
            var tags = await _tagServiceClient.GetTagsForTodo(todo.TodoId, request.Token);
            responses.Add(new TodoResponse
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
            });
        }

        return responses;
    }
}
