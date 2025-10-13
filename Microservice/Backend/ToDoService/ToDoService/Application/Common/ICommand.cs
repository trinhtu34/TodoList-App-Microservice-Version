using MediatR;


namespace ToDoService.Application.Common;
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
