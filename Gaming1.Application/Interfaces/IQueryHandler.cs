namespace Gaming1.Application.Commands;

public interface IQueryHandler<TQuery, TRespone>
{
    Task<TRespone> Handle(TQuery query, CancellationToken cancellationToken);
}