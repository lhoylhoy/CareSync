using System.Net.Http.Json;
using CareSync.Application.DTOs.Billing;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class BillingService : BaseCrudService<BillDto, UpsertBillDto, UpsertBillDto>, IBillingService
{
    public BillingService(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string BaseEndpoint => ApiEndpoints.Billing.Base;

    public override Task<ApiResponse<BillDto>> CreateAsync(UpsertBillDto bill)
        => SendUpsertAsync(bill with { Id = null }, "Invoice created successfully");

    public override Task<ApiResponse<BillDto>> UpdateAsync(Guid id, UpsertBillDto bill)
        => SendUpsertAsync(bill with { Id = id }, "Invoice updated successfully");

    public async Task<List<BillDto>> GetBillsAsync()
    {
        var response = await GetAllAsync();
        return response.Data?.ToList() ?? new List<BillDto>();
    }

    public async Task<BillDto> GetBillByIdAsync(Guid id)
    {
        var response = await GetByIdAsync(id);
        if (!response.Success || response.Data is null)
        {
            throw new Exception(response.Message ?? "Bill not found");
        }

        return response.Data;
    }

    public async Task<BillDto> UpsertBillAsync(UpsertBillDto bill)
    {
        var response = await SendUpsertAsync(bill, "Invoice saved successfully");
        if (!response.Success || response.Data is null)
        {
            throw new Exception(response.Message ?? "Could not save bill");
        }

        return response.Data;
    }

    public async Task DeleteBillAsync(Guid id)
    {
        var response = await DeleteAsync(id);
        if (!response.Success)
        {
            throw new Exception(response.Message ?? "Failed to delete bill");
        }
    }

    // Additional method (not in shared interface) - will be moved to a separate payment service
    public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto payment)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Billing.Base}/payments", payment, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentDto>(_jsonOptions) ??
               throw new Exception("Could not process payment");
    }

    private async Task<ApiResponse<BillDto>> SendUpsertAsync(UpsertBillDto bill, string successMessage)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.Billing.Upsert, bill, _jsonOptions);
            if (response.IsSuccessStatusCode)
            {
                var dto = await response.Content.ReadFromJsonAsync<BillDto>(_jsonOptions);
                return new ApiResponse<BillDto>
                {
                    Success = true,
                    Data = dto,
                    Message = successMessage
                };
            }

            var raw = await response.Content.ReadAsStringAsync();
            return new ApiResponse<BillDto>
            {
                Success = false,
                Message = ExtractProblemMessage(raw) ?? $"Failed to save invoice: {(int)response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<BillDto>
            {
                Success = false,
                Message = $"Error saving invoice: {ex.Message}"
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
