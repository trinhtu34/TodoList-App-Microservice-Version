
using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoService.Application.Common;
using ToDoService.DTOs;
using ToDoService.Models;
using ToDoService.ServiceClients;
namespace ToDoService.Application.Todos.Queries;

public record GetTodoByCognitosubQuery(string CognitoSub) : IQuery<List<TodoResponse>>;

public class GetTodoByCognitosubQueryHandler : IRequestHandler<GetTodoByCognitosubQuery, List<TodoResponse>>
{
    private readonly TodoServiceDbContext _context;
    private readonly ITagServiceClient _tagServiceClient;
    public GetTodoByCognitosubQueryHandler(TodoServiceDbContext context, ITagServiceClient tagServiceClient)
    {
        _context = context;
        _tagServiceClient = tagServiceClient;
    }
    public async Task<List<TodoResponse>> Handle(GetTodoByCognitosubQuery request, CancellationToken cancellationToken)
    {
        return null;
    }
}

