using MediatR;

namespace GroupService.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
