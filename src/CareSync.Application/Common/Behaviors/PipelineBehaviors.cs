using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CareSync.Application.Common.Behaviors;

/// <summary>
///     Pipeline behavior that validates requests using FluentValidation
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults =
            await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}

/// <summary>
///     Pipeline behavior for logging and exception handling
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogDebug("Handling {RequestName}", requestName);

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            if (ex is ValidationException)
            {
                // Don't log validation failures as errors (they are expected client issues)
                _logger.LogWarning(ex, "Validation failure in {RequestName}", requestName);
            }
            else
            {
                _logger.LogError(ex, "Error handling {RequestName}", requestName);
            }
            throw;
        }
    }
}
