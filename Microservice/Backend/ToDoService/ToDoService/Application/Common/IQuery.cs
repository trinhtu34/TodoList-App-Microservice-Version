using MediatR;

namespace ToDoService.Application.Common;
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}