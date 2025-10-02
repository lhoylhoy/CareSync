using System.Net.Http.Json;
using CareSync.Shared.Constants;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class MedicalRecordService : IMedicalRecordService
{
    private readonly HttpClient _httpClient;

    public MedicalRecordService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(Guid patientId)
    {
        return await _httpClient.GetFromJsonAsync<List<MedicalRecordDto>>(
                   ApiEndpoints.MedicalRecords.ByPatient.Replace("{patientId}", patientId.ToString())) ??
               new List<MedicalRecordDto>();
    }

    public async Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<MedicalRecordDto>(
                   ApiEndpoints.MedicalRecords.GetById.Replace("{id}", id.ToString())) ??
               throw new Exception("Medical record not found");
    }

    public async Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto record)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.MedicalRecords.Create, record);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MedicalRecordDto>() ??
               throw new Exception("Could not create medical record");
    }

    public async Task UpdateMedicalRecordAsync(UpdateMedicalRecordDto record)
    {
        var response =
            await _httpClient.PutAsJsonAsync(ApiEndpoints.MedicalRecords.Update.Replace("{id}", record.Id.ToString()),
                record);
        response.EnsureSuccessStatusCode();
    }

    public async Task<MedicalRecordDto> UpsertMedicalRecordAsync(UpsertMedicalRecordDto record)
    {
        var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.MedicalRecords.Upsert, record);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MedicalRecordDto>() ?? throw new Exception("Could not upsert medical record");
    }

    public async Task DeleteMedicalRecordAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(ApiEndpoints.MedicalRecords.Delete.Replace("{id}", id.ToString()));
        response.EnsureSuccessStatusCode();
    }

    // Added: Fetch all medical records
    public async Task<List<MedicalRecordDto>> GetAllMedicalRecordsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<MedicalRecordDto>>(ApiEndpoints.MedicalRecords.Base) ?? new List<MedicalRecordDto>();
    }
}
