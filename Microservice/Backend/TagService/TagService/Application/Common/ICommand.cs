using MediatR;

namespace TagService.Application.Common;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
