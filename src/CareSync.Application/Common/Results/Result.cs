namespace CareSync.Application.Common.Results;

/// <summary>
/// QUICK WIN #3: Result pattern for cleaner error handling
/// Represents the result of an operation with success/failure state
/// Avoids throwing exceptions for business logic failures
/// Enhanced with helper methods
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    protected Result(bool success, string? error)
    {
        IsSuccess = success;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

/// <summary>
/// Result with a return value
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool success, T? value, string? error) : base(success, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static new Result<T> Failure(string error) => new(false, default, error);
    
    /// <summary>
    /// Try to get the value, returns true if successful
    /// </summary>
    public bool TryGetValue(out T? value)
    {
        value = IsSuccess ? Value : default;
        return IsSuccess;
    }
}

/// <summary>
/// QUICK WIN #3: Extension methods for Result pattern
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Map a successful result to another type
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess && result.Value != null
            ? Result<TOut>.Success(mapper(result.Value))
            : Result<TOut>.Failure(result.Error ?? "Mapping failed");
    }
    
    /// <summary>
    /// Execute an action if result is successful
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess && result.Value != null)
            action(result.Value);
        return result;
    }
    
    /// <summary>
    /// Execute an action if result is a failure
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
    {
        if (result.IsFailure && result.Error != null)
            action(result.Error);
        return result;
    }
    
    /// <summary>
    /// Match pattern - execute different actions based on success/failure
    /// </summary>
    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<string, TOut> onFailure)
    {
        return result.IsSuccess && result.Value != null
            ? onSuccess(result.Value)
            : onFailure(result.Error ?? "Operation failed");
    }
}
