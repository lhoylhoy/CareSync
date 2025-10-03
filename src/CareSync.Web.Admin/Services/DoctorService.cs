using System.Net.Http.Json;
using CareSync.Application.DTOs.Doctors;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class DoctorService : BaseCrudService<DoctorDto, CreateDoctorDto, UpdateDoctorDto>, IDoctorService
{
    public DoctorService(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string BaseEndpoint => "api/doctors";

    public async Task<List<DoctorDto>> GetDoctorsAsync()
    {
        var result = await GetAllAsync();
        return result.Data?.ToList() ?? new List<DoctorDto>();
    }

    public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (!result.Success || result.Data is null) throw new Exception("Doctor not found");
        return result.Data;
    }

    public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto doctor)
    {
        var result = await CreateAsync(doctor);
        if (!result.Success || result.Data is null) throw new Exception(result.Message ?? "Failed to create doctor");
        return result.Data;
    }

    public async Task UpdateDoctorAsync(UpdateDoctorDto doctor)
    {
        var update = await UpdateAsync(doctor.Id, doctor);
        if (!update.Success) throw new Exception(update.Message ?? "Failed to update doctor");
    }

    public async Task<DoctorDto> UpsertDoctorAsync(UpsertDoctorDto doctor)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/upsert", doctor);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DoctorDto>() ?? throw new Exception("Could not upsert doctor");
    }

    public async Task DeleteDoctorAsync(Guid id)
    {
        var delete = await DeleteAsync(id);
        if (!delete.Success) throw new Exception(delete.Message ?? "Failed to delete doctor");
    }
}
