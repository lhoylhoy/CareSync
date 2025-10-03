using CareSync.Application.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CareSync.Application.Common.Behaviors;

public class ResultWrappingBehavior<TRequest, TResponse>(ILogger<ResultWrappingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only wrap if response type is Result or Result<>
        var responseType = typeof(TResponse);
        var isGenericResult = responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>);
        var isResult = responseType == typeof(Result);

        if (!isResult && !isGenericResult)
            return await next();

        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in handler for {RequestType}", typeof(TRequest).Name);

            if (isGenericResult)
            {
                var failure = responseType.GetMethod("Failure", [typeof(string)])!;
                return (TResponse)failure.Invoke(null, new object[] { ex.Message })!;
            }

            // Non-generic Result
            var failureNonGeneric = typeof(Result).GetMethod(nameof(Result.Failure))!;
            return (TResponse)failureNonGeneric.Invoke(null, new object[] { ex.Message })!;
        }
    }
}
