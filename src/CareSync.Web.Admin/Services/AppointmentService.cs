using System.Net.Http.Json;
using CareSync.Application.DTOs.Appointments;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class AppointmentService : BaseCrudService<AppointmentDto, UpsertAppointmentDto, UpsertAppointmentDto>, IAppointmentService
{
    public AppointmentService(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string BaseEndpoint => "api/appointments";

    public override Task<ApiResponse<AppointmentDto>> CreateAsync(UpsertAppointmentDto appointment)
        => SendUpsertAsync(appointment with { Id = null }, "Appointment scheduled successfully");

    public override Task<ApiResponse<AppointmentDto>> UpdateAsync(Guid id, UpsertAppointmentDto appointment)
        => SendUpsertAsync(appointment with { Id = id }, "Appointment updated successfully");

    public async Task CancelAppointmentAsync(Guid id, string reason)
    {
        var dto = new CancelAppointmentDto(id, reason);
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}/cancel", dto);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AppointmentDto> CompleteAppointmentAsync(Guid id, string? notes)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/{id}/complete", new { Notes = notes });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AppointmentDto>() ?? throw new Exception("Could not complete appointment");
    }

    public async Task<List<AppointmentDto>> GetAppointmentsAsync()
    {
        var result = await GetAllAsync();
        return result.Data?.ToList() ?? new List<AppointmentDto>();
    }

    public async Task<AppointmentDto> GetAppointmentByIdAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (!result.Success || result.Data is null) throw new Exception("Appointment not found");
        return result.Data;
    }

    public async Task DeleteAppointmentAsync(Guid id)
    {
        var delete = await DeleteAsync(id);
        if (!delete.Success)
            throw new Exception(delete.Message ?? "Failed to delete appointment");
    }

    private async Task<ApiResponse<AppointmentDto>> SendUpsertAsync(UpsertAppointmentDto appointment, string successMessage)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/upsert", appointment, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<AppointmentDto>(_jsonOptions);
                return new ApiResponse<AppointmentDto>
                {
                    Success = true,
                    Data = dto,
                    Message = successMessage
                };
            }

            var raw = await response.Content.ReadAsStringAsync();
            return new ApiResponse<AppointmentDto>
            {
                Success = false,
                Message = ExtractProblemMessage(raw) ?? $"Failed to save appointment: {(int)response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<AppointmentDto>
            {
                Success = false,
                Message = $"Error saving appointment: {ex.Message}"
            };
        }
    }

    private static string? ExtractProblemMessage(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(raw);
            var root = doc.RootElement;
            if (root.TryGetProperty("detail", out var detail) && detail.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                return detail.GetString();
            }
            if (root.TryGetProperty("title", out var title) && title.ValueKind == System.Text.Json.JsonValueKind.String)
            {
                return title.GetString();
            }
        }
        catch
        {
            // Ignore parsing issues and fall back to raw payload
        }

        return raw;
    }
}
