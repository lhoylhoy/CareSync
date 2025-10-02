namespace CareSync.Shared.Models;

/// <summary>
///     Generic paged response model for API results
/// </summary>
/// <typeparam name="T">The type of items in the response</typeparam>
public class PagedResponse<T> where T : class
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
///     API response wrapper for consistent response format
/// </summary>
/// <typeparam name="T">The type of data in the response</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResponse<T> { Success = true, Data = data, Message = message };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T> { Success = false, Message = message, Errors = errors ?? new List<string>() };
    }
}

/// <summary>
///     Standard error response model
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public List<string> Details { get; set; } = new();
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
