using System.Net.Http.Json;
using CareSync.Shared.Constants;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Shared.Models;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class MedicalRecordService : BaseCrudService<MedicalRecordDto, UpsertMedicalRecordDto, UpsertMedicalRecordDto>, IMedicalRecordService
{
    public MedicalRecordService(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string BaseEndpoint => ApiEndpoints.MedicalRecords.Base;

    public override Task<ApiResponse<MedicalRecordDto>> CreateAsync(UpsertMedicalRecordDto record)
        => SendUpsertAsync(record with { Id = null }, "Medical record created successfully");

    public override Task<ApiResponse<MedicalRecordDto>> UpdateAsync(Guid id, UpsertMedicalRecordDto record)
        => SendUpsertAsync(record with { Id = id }, "Medical record updated successfully");

    public async Task<List<MedicalRecordDto>> GetAllMedicalRecordsAsync()
    {
        var response = await GetAllAsync();
        return response.Data?.ToList() ?? new List<MedicalRecordDto>();
    }

    public async Task<List<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(Guid patientId)
    {
        return await _httpClient.GetFromJsonAsync<List<MedicalRecordDto>>(
                   ApiEndpoints.MedicalRecords.ByPatient.Replace("{patientId}", patientId.ToString()), _jsonOptions)
               ?? new List<MedicalRecordDto>();
    }

    public async Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id)
    {
        var response = await GetByIdAsync(id);
        if (!response.Success || response.Data is null)
        {
            throw new Exception(response.Message ?? "Medical record not found");
        }

        return response.Data;
    }

    public async Task<MedicalRecordDto> UpsertMedicalRecordAsync(UpsertMedicalRecordDto record)
    {
        var result = await SendUpsertAsync(record, "Medical record saved successfully");
        if (!result.Success || result.Data is null)
        {
            throw new Exception(result.Message ?? "Failed to save medical record");
        }

        return result.Data;
    }

    public async Task DeleteMedicalRecordAsync(Guid id)
    {
        var response = await DeleteAsync(id);
        if (!response.Success)
        {
            throw new Exception(response.Message ?? "Failed to delete medical record");
        }
    }

    public async Task<MedicalRecordDto> FinalizeMedicalRecordAsync(Guid id, string? finalNotes, string? finalizedBy)
    {
        var payload = new FinalizeMedicalRecordDto { Id = id, FinalNotes = finalNotes, FinalizedBy = finalizedBy };
        var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.MedicalRecords.Finalize.Replace("{id}", id.ToString()), payload, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MedicalRecordDto>(_jsonOptions) ?? throw new Exception("Could not finalize medical record");
    }

    public async Task<MedicalRecordDto> ReopenMedicalRecordAsync(Guid id)
    {
        var response = await _httpClient.PutAsync(ApiEndpoints.MedicalRecords.Reopen.Replace("{id}", id.ToString()), null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MedicalRecordDto>(_jsonOptions) ?? throw new Exception("Could not reopen medical record");
    }

    private async Task<ApiResponse<MedicalRecordDto>> SendUpsertAsync(UpsertMedicalRecordDto record, string successMessage)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.MedicalRecords.Upsert, record, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<MedicalRecordDto>(_jsonOptions);
                return new ApiResponse<MedicalRecordDto>
                {
                    Success = true,
                    Data = dto,
                    Message = successMessage
                };
            }

            var raw = await response.Content.ReadAsStringAsync();
            return new ApiResponse<MedicalRecordDto>
            {
                Success = false,
                Message = ExtractProblemMessage(raw) ?? $"Failed to save medical record: {(int)response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<MedicalRecordDto>
            {
                Success = false,
                Message = $"Error saving medical record: {ex.Message}"
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
            // ignore parse issues
        }

        return raw;
    }
}
