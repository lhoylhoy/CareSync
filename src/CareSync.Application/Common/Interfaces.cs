using MediatR;

namespace CareSync.Application.Common;

/// <summary>
///     Base interface for all application commands that modify state
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
///     Base interface for commands that return a result
/// </summary>
/// <typeparam name="TResult">The type of result returned</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>
{
}

/// <summary>
///     Base interface for all application queries
/// </summary>
/// <typeparam name="TResult">The type of result returned</typeparam>
public interface IQuery<out TResult> : IRequest<TResult>
{
}
