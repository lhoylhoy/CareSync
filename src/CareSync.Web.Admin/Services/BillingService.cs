using System.Net.Http.Json;
using System.Text.Json;
using CareSync.Shared.Constants;
using CareSync.Application.DTOs.Billing;
using CareSync.Web.Admin.Services.Contracts;

namespace CareSync.Web.Admin.Services;

public class BillingService : IBillingService
{
    private readonly HttpClient _httpClient;

    public BillingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BillDto>> GetBillsAsync()
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = await _httpClient.GetFromJsonAsync<List<BillDto>>(ApiEndpoints.Billing.Base, jsonOptions);
            return result ?? new List<BillDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching bills: {ex.Message}");
            return new List<BillDto>();
        }
    }

    public async Task<BillDto> GetBillByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<BillDto>(
            ApiEndpoints.Billing.GetById.Replace("{id}", id.ToString())) ?? throw new Exception("Bill not found");
    }

    public async Task<BillDto> CreateBillAsync(CreateBillDto bill)
    {
        var response = await _httpClient.PostAsJsonAsync(ApiEndpoints.Billing.Create, bill);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BillDto>() ?? throw new Exception("Could not create bill");
    }

    public async Task<BillDto> UpdateBillAsync(UpdateBillDto bill)
    {
        var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoints.Billing.Base}/{bill.Id}", bill);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BillDto>() ?? throw new Exception("Could not update bill");
    }

    public async Task DeleteBillAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync(ApiEndpoints.Billing.Delete.Replace("{id}", id.ToString()));
        response.EnsureSuccessStatusCode();
    }

    public async Task<BillDto> UpsertBillAsync(UpsertBillDto bill)
    {
        var response = await _httpClient.PutAsJsonAsync(ApiEndpoints.Billing.Upsert, bill);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BillDto>() ?? throw new Exception("Could not upsert bill");
    }

    // Additional method (not in shared interface) - will be moved to a separate payment service
    public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto payment)
    {
        var response = await _httpClient.PostAsJsonAsync($"{ApiEndpoints.Billing.Base}/payments", payment);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentDto>() ??
               throw new Exception("Could not process payment");
    }
}
