using CareSync.Application.DTOs.Patients;
using System.Net.Http.Json;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class PatientService : BaseCrudService<PatientDto, CreatePatientDto, UpdatePatientDto>, IPatientService
{
    public PatientService(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string BaseEndpoint => "api/patients";

    public async Task<List<PatientDto>> GetPatientsAsync()
    {
        var result = await GetAllAsync();
        return result.Data?.ToList() ?? new List<PatientDto>();
    }

    public async Task<PatientDto> GetPatientByIdAsync(Guid id)
    {
        var result = await GetByIdAsync(id);
        if (!result.Success || result.Data is null) throw new Exception("Patient not found");
        return result.Data;
    }

    public async Task<PatientDto> CreatePatientAsync(CreatePatientDto patient)
    {
        var result = await CreateAsync(patient);
        if (!result.Success || result.Data is null) throw new Exception(result.Message ?? "Failed to create patient");
        return result.Data;
    }

    public async Task UpdatePatientAsync(UpdatePatientDto patient)
    {
        var update = await UpdateAsync(patient.Id, patient);
        if (!update.Success) throw new Exception(update.Message ?? "Failed to update patient");
    }

    public async Task<PatientDto> UpsertPatientAsync(UpsertPatientDto patient)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseEndpoint}/upsert", patient);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatientDto>() ?? throw new Exception("Could not upsert patient");
    }

    public async Task DeletePatientAsync(Guid id)
    {
        var delete = await DeleteAsync(id);
        if (!delete.Success) throw new Exception(delete.Message ?? "Failed to delete patient");
    }
}
