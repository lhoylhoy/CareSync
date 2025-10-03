using System.Collections.Generic;

namespace CareSync.Web.Admin.Services;

public interface ICrudService<TDto, TCreateDto, TUpdateDto>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    public Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync();
    public Task<ApiResponse<TDto>> GetByIdAsync(Guid id);
    public Task<ApiResponse<TDto>> CreateAsync(TCreateDto createDto);
    public Task<ApiResponse<TDto>> UpdateAsync(Guid id, TUpdateDto updateDto);
    public Task<ApiResponse<bool>> DeleteAsync(Guid id);

    // Optional: Search and pagination
    public Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync(int page = 1, int pageSize = 10, string? searchTerm = null, IReadOnlyDictionary<string, string?>? filters = null);
    public Task<ApiResponse<IEnumerable<TDto>>> SearchAsync(string searchTerm);
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
