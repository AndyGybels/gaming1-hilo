namespace Gaming1.Application.Commands;

public interface ICommandHandler<TRequest, in TResponse>
{
    Task<TRequest> Handle(TResponse response, CancellationToken cancellationToken);
}

public interface ICommandHandler<TRequest>
{
    Task<TRequest> Handle();
}