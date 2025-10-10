using System.Net.Http.Json;
using CareSync.Application.DTOs.Patients;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

// Updated to use consolidated PatientDto for all operations
public class PatientService : BaseCrudService<PatientDto, PatientDto, PatientDto>, IPatientService
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

    public async Task<PatientDto> CreatePatientAsync(PatientDto patient)
    {
        var result = await CreateAsync(patient);
        if (!result.Success || result.Data is null) throw new Exception(result.Message ?? "Failed to create patient");
        return result.Data;
    }

    public async Task UpdatePatientAsync(PatientDto patient)
    {
        if (!patient.Id.HasValue) throw new ArgumentException("Patient Id is required for update");
        var update = await UpdateAsync(patient.Id.Value, patient);
        if (!update.Success) throw new Exception(update.Message ?? "Failed to update patient");
    }

    public async Task<PatientDto> UpsertPatientAsync(PatientDto patient)
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
