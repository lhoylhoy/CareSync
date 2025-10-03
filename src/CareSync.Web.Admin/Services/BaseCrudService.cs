using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace CareSync.Web.Admin.Services;

public abstract class BaseCrudService<TDto, TCreateDto, TUpdateDto> : ICrudService<TDto, TCreateDto, TUpdateDto>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly HttpClient _httpClient;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected BaseCrudService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    protected abstract string BaseEndpoint { get; }

    public virtual async Task<ApiResponse<IEnumerable<TDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(BaseEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<IEnumerable<TDto>>(_jsonOptions);
                return new ApiResponse<IEnumerable<TDto>> { Success = true, Data = items ?? new List<TDto>() };
            }

            return new ApiResponse<IEnumerable<TDto>>
            {
                Success = false,
                Message = $"Failed to retrieve items. Status: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<TDto>>
            {
                Success = false,
                Message = $"Error retrieving items: {ex.Message}"
            };
        }
    }

    public virtual async Task<ApiResponse<TDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var item = await response.Content.ReadFromJsonAsync<TDto>(_jsonOptions);
                return new ApiResponse<TDto> { Success = true, Data = item };
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new ApiResponse<TDto> { Success = false, Message = "Item not found" };

            return new ApiResponse<TDto>
            {
                Success = false,
                Message = $"Failed to retrieve item. Status: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TDto> { Success = false, Message = $"Error retrieving item: {ex.Message}" };
        }
    }

    public virtual async Task<ApiResponse<TDto>> CreateAsync(TCreateDto createDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, createDto, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var createdItem = await response.Content.ReadFromJsonAsync<TDto>(_jsonOptions);
                return new ApiResponse<TDto>
                {
                    Success = true,
                    Data = createdItem,
                    Message = "Item created successfully"
                };
            }

            // Parse ProblemDetails / ValidationProblemDetails for user-friendly message
            var raw = await response.Content.ReadAsStringAsync();
            var friendly = TryExtractProblemDetailsMessage(raw, out var combinedMessage)
                ? combinedMessage
                : $"Failed to create item: {raw}";
            return new ApiResponse<TDto> { Success = false, Message = friendly };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TDto> { Success = false, Message = $"Error creating item: {ex.Message}" };
        }
    }

    public virtual async Task<ApiResponse<TDto>> UpdateAsync(Guid id, TUpdateDto updateDto)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}", updateDto, _jsonOptions);

            if (response.IsSuccessStatusCode)
            {
                var updatedItem = await response.Content.ReadFromJsonAsync<TDto>(_jsonOptions);
                return new ApiResponse<TDto>
                {
                    Success = true,
                    Data = updatedItem,
                    Message = "Item updated successfully"
                };
            }

            var raw = await response.Content.ReadAsStringAsync();
            var friendly = TryExtractProblemDetailsMessage(raw, out var combinedMessage)
                ? combinedMessage
                : $"Failed to update item: {raw}";
            return new ApiResponse<TDto> { Success = false, Message = friendly };
        }
        catch (Exception ex)
        {
            return new ApiResponse<TDto> { Success = false, Message = $"Error updating item: {ex.Message}" };
        }
    }

    public virtual async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");

            if (response.IsSuccessStatusCode)
                return new ApiResponse<bool> { Success = true, Data = true, Message = "Item deleted successfully" };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new ApiResponse<bool> { Success = false, Message = "Item not found" };

            var raw = await response.Content.ReadAsStringAsync();
            var friendly = TryExtractProblemDetailsMessage(raw, out var combinedMessage)
                ? combinedMessage
                : $"Failed to delete item: {raw}";
            return new ApiResponse<bool> { Success = false, Message = friendly };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool> { Success = false, Message = $"Error deleting item: {ex.Message}" };
        }
    }

    private bool TryExtractProblemDetailsMessage(string rawJson, out string message)
    {
        message = string.Empty;
        if (string.IsNullOrWhiteSpace(rawJson)) return false;
        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            var root = doc.RootElement;
            // Basic ProblemDetails shape
            if (root.TryGetProperty("title", out var titleEl) && root.TryGetProperty("status", out _))
            {
                // ValidationProblemDetails has an "errors" object
                if (root.TryGetProperty("errors", out var errorsEl) && errorsEl.ValueKind == JsonValueKind.Object)
                {
                    var fieldErrors = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in errorsEl.EnumerateObject())
                    {
                        if (prop.Value.ValueKind != JsonValueKind.Array) continue;
                        var field = prop.Name;
                        field = HumanizeFieldName(field);
                        if (!fieldErrors.TryGetValue(field, out var set))
                        {
                            set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            fieldErrors[field] = set;
                        }
                        foreach (var v in prop.Value.EnumerateArray())
                        {
                            var rawMsg = v.GetString();
                            if (string.IsNullOrWhiteSpace(rawMsg)) continue;
                            var clean = CleanValidationMessage(rawMsg);
                            set.Add(clean);
                        }
                    }
                    if (fieldErrors.Count > 0)
                    {
                        // Build multi-line message for UI clarity
                        var lines = fieldErrors
                            .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")
                            .ToList();
                        message = string.Join("\n", lines);
                        return true;
                    }
                }
                // Fallback to title + detail
                var detail = root.TryGetProperty("detail", out var dEl) ? dEl.GetString() : null;
                message = detail?.Length > 0 ? detail : titleEl.GetString() ?? "Request failed";
                return true;
            }
        }
        catch
        {
            // Ignore parse issues
        }
        return false;
    }

    private static string CleanValidationMessage(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg)) return msg;
        // Remove enclosing quotes produced by FluentValidation default messages
        msg = msg.Trim();
        if (msg.StartsWith("'") && msg.Contains("' must not be empty"))
        {
            // Turn "'Gender' must not be empty." into "must not be empty"
            var idx = msg.IndexOf("' must not be empty", StringComparison.OrdinalIgnoreCase);
            if (idx > 0)
            {
                var suffix = msg[(idx + 2)..]; // skip leading quote space
                // produce concise
                return suffix.Trim();
            }
        }
        // Remove leading field repetition if any (e.g., Gender: Gender must be ...)
        var parts = msg.Split(':', 2);
        if (parts.Length == 2 && parts[1].TrimStart().StartsWith(parts[0], StringComparison.OrdinalIgnoreCase))
        {
            msg = parts[1].Trim();
        }
        // Remove repeated field name inside quotes
        return msg.Replace("'", string.Empty).Trim();
    }

    private static string HumanizeFieldName(string field)
    {
        return field switch
        {
            "firstName" => "First Name",
            "middleName" => "Middle Name",
            "lastName" => "Last Name",
            "provinceCode" => "Province",
            "cityCode" => "City / Municipality",
            "barangayCode" => "Barangay",
            "cityZipCode" => "Zip Code",
            "gender" => "Gender",
            _ => ToSpaced(field)
        };
    }

    private static string ToSpaced(string field)
    {
        if (string.IsNullOrWhiteSpace(field)) return field;
        var chars = new List<char>(field.Length + 5);
        for (int i = 0; i < field.Length; i++)
        {
            var c = field[i];
            if (i > 0 && char.IsUpper(c) && char.IsLower(field[i - 1]))
                chars.Add(' ');
            chars.Add(c);
        }
        // capitalize first
        if (chars.Count > 0) chars[0] = char.ToUpperInvariant(chars[0]);
        return new string(chars.ToArray());
    }

    public virtual async Task<ApiResponse<PagedResult<TDto>>> GetPagedAsync(int page = 1, int pageSize = 10,
        string? searchTerm = null)
    {
        try
        {
            var queryParams = new List<string>();
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            if (!string.IsNullOrWhiteSpace(searchTerm)) queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");

            var url = $"{BaseEndpoint}?{string.Join("&", queryParams)}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<TDto>>(_jsonOptions);
                return new ApiResponse<PagedResult<TDto>> { Success = true, Data = result ?? new PagedResult<TDto>() };
            }

            return new ApiResponse<PagedResult<TDto>>
            {
                Success = false,
                Message = $"Failed to retrieve paged items. Status: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedResult<TDto>>
            {
                Success = false,
                Message = $"Error retrieving paged items: {ex.Message}"
            };
        }
    }

    public virtual async Task<ApiResponse<IEnumerable<TDto>>> SearchAsync(string searchTerm)
    {
        try
        {
            var url = $"{BaseEndpoint}/search?term={Uri.EscapeDataString(searchTerm)}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<IEnumerable<TDto>>(_jsonOptions);
                return new ApiResponse<IEnumerable<TDto>> { Success = true, Data = items ?? new List<TDto>() };
            }

            return new ApiResponse<IEnumerable<TDto>>
            {
                Success = false,
                Message = $"Failed to search items. Status: {response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<TDto>>
            {
                Success = false,
                Message = $"Error searching items: {ex.Message}"
            };
        }
    }
}
