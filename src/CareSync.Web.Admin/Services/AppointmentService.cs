using CareSync.Application.DTOs.Appointments;
using System.Net.Http.Json;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class AppointmentService : BaseCrudService<AppointmentDto, CreateAppointmentDto, UpdateAppointmentDto>, IAppointmentService
{
    public AppointmentService(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string BaseEndpoint => "api/appointments";

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

    public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto appointment)
    {
        var create = await CreateAsync(appointment);
        if (!create.Success || create.Data is null) throw new Exception(create.Message ?? "Failed to create appointment");
        return create.Data;
    }

    public async Task UpdateAppointmentAsync(UpdateAppointmentDto appointment)
    {
        var update = await UpdateAsync(appointment.Id, appointment);
        if (!update.Success) throw new Exception(update.Message ?? "Failed to update appointment");
    }

    public async Task<AppointmentDto> UpsertAppointmentAsync(UpsertAppointmentDto appointment)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/upsert", appointment);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AppointmentDto>() ?? throw new Exception("Could not upsert appointment");
    }

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

    public async Task DeleteAppointmentAsync(Guid id)
    {
        var delete = await DeleteAsync(id);
        if (!delete.Success)
            throw new Exception(delete.Message ?? "Failed to delete appointment");
    }
}
