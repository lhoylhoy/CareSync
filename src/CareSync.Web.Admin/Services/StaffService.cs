using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using CareSync.Application.DTOs.Staff;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class StaffService : IStaffService, ICrudService<StaffDto, CreateStaffDto, UpdateStaffDto>
{
    private readonly HttpClient _httpClient;
    private const string BaseEndpoint = "api/staff";

    public StaffService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StaffDto>> GetStaffAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<StaffDto>>(BaseEndpoint) ?? new List<StaffDto>();
    }

    public async Task<StaffDto> GetStaffByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<StaffDto>($"{BaseEndpoint}/{id}") ?? throw new Exception("Staff not found");
    }

    public async Task<StaffDto> CreateStaffAsync(CreateStaffDto staff)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseEndpoint, staff);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StaffDto>() ?? throw new Exception("Could not create staff");
    }

    public async Task<StaffDto> UpdateStaffAsync(UpdateStaffDto staff)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{staff.Id}", staff);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StaffDto>() ?? throw new Exception("Could not update staff");
    }

    public async Task<StaffDto> UpsertStaffAsync(UpsertStaffDto staff)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/upsert", staff);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StaffDto>() ?? throw new Exception("Could not upsert staff");
    }

    public async Task DeleteStaffAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"{BaseEndpoint}/{id}");
        response.EnsureSuccessStatusCode();
    }

    // ICrudService implementation wrappers
    public async Task<ApiResponse<IEnumerable<StaffDto>>> GetAllAsync()
    {
        var list = await GetStaffAsync();
        return new ApiResponse<IEnumerable<StaffDto>> { Success = true, Data = list };
    }

    public async Task<ApiResponse<StaffDto>> GetByIdAsync(Guid id)
    {
        var dto = await GetStaffByIdAsync(id);
        return new ApiResponse<StaffDto> { Success = true, Data = dto };
    }

    public async Task<ApiResponse<StaffDto>> CreateAsync(CreateStaffDto createDto)
    {
        var dto = await CreateStaffAsync(createDto);
        return new ApiResponse<StaffDto> { Success = true, Data = dto, Message = "Staff created" };
    }

    public async Task<ApiResponse<StaffDto>> UpdateAsync(Guid id, UpdateStaffDto updateDto)
    {
        var dto = await UpdateStaffAsync(updateDto);
        return new ApiResponse<StaffDto> { Success = true, Data = dto, Message = "Staff updated" };
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        await DeleteStaffAsync(id);
        return new ApiResponse<bool> { Success = true, Data = true, Message = "Staff deleted" };
    }

    public async Task<ApiResponse<PagedResult<StaffDto>>> GetPagedAsync(int page = CareSync.Application.Common.PagingDefaults.DefaultPage, int pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize, string? searchTerm = null, IReadOnlyDictionary<string, string?>? filters = null)
    {
        try
        {
            if (page <= 0) page = CareSync.Application.Common.PagingDefaults.DefaultPage;
            if (pageSize <= 0) pageSize = CareSync.Application.Common.PagingDefaults.DefaultPageSize;
            pageSize = Math.Min(pageSize, CareSync.Application.Common.PagingDefaults.MaxPageSize);

            var queryParams = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");
            }

            if (filters != null)
            {
                foreach (var kvp in filters)
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value)) continue;
                    queryParams.Add($"filters[{Uri.EscapeDataString(kvp.Key)}]={Uri.EscapeDataString(kvp.Value!)}");
                }
            }

            var url = $"{BaseEndpoint}?{string.Join("&", queryParams)}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<PagedResult<StaffDto>>();
                if (result is not null)
                {
                    return new ApiResponse<PagedResult<StaffDto>> { Success = true, Data = result };
                }

                return new ApiResponse<PagedResult<StaffDto>>
                {
                    Success = true,
                    Data = new PagedResult<StaffDto>
                    {
                        Items = Array.Empty<StaffDto>(),
                        PageNumber = page,
                        PageSize = pageSize,
                        TotalCount = 0
                    }
                };
            }

            var errorPayload = await response.Content.ReadAsStringAsync();
            return new ApiResponse<PagedResult<StaffDto>>
            {
                Success = false,
                Message = $"Failed to retrieve staff: {response.StatusCode} {errorPayload}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedResult<StaffDto>>
            {
                Success = false,
                Message = $"Error retrieving staff: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<StaffDto>>> SearchAsync(string searchTerm)
    {
        var all = await GetStaffAsync();
        var filtered = all.Where(s => ($"{s.FirstName} {s.LastName}".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) || (s.EmployeeId?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        return new ApiResponse<IEnumerable<StaffDto>> { Success = true, Data = filtered.ToList() };
    }
}
