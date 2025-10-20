using MediatR;

namespace TagService.Application.Common;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
