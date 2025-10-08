using MediatR;

namespace GroupService.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
