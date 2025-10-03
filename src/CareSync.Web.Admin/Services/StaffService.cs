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

    public async Task<ApiResponse<PagedResult<StaffDto>>> GetPagedAsync(int page = 1, int pageSize = 10, string? searchTerm = null)
    {
        var all = await GetStaffAsync();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            all = all.Where(s => ($"{s.FirstName} {s.LastName}".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) || (s.EmployeeId?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        }
        var total = all.Count;
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var paged = new PagedResult<StaffDto> { Items = items, PageNumber = page, PageSize = pageSize, TotalCount = total };
        return new ApiResponse<PagedResult<StaffDto>> { Success = true, Data = paged };
    }

    public async Task<ApiResponse<IEnumerable<StaffDto>>> SearchAsync(string searchTerm)
    {
        var all = await GetStaffAsync();
        var filtered = all.Where(s => ($"{s.FirstName} {s.LastName}".Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) || (s.EmployeeId?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        return new ApiResponse<IEnumerable<StaffDto>> { Success = true, Data = filtered.ToList() };
    }
}
