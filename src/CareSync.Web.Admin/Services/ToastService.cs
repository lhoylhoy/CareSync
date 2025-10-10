using Microsoft.JSInterop;

namespace CareSync.Web.Admin.Services;

public interface IToastService
{
    public Task ShowSuccessAsync(string message, string title = "Success");
    public Task ShowErrorAsync(string message, string title = "Error");
    public Task ShowWarningAsync(string message, string title = "Warning");
    public Task ShowInfoAsync(string message, string title = "Info");
}

public class ToastService : IToastService
{
    private readonly IJSRuntime _jsRuntime;

    public ToastService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task ShowSuccessAsync(string message, string title = "Success")
    {
        await ShowToastAsync(message, title, "success", "fas fa-check-circle");
    }

    public async Task ShowErrorAsync(string message, string title = "Error")
    {
        await ShowToastAsync(message, title, "danger", "fas fa-exclamation-triangle");
    }

    public async Task ShowWarningAsync(string message, string title = "Warning")
    {
        await ShowToastAsync(message, title, "warning", "fas fa-exclamation-circle");
    }

    public async Task ShowInfoAsync(string message, string title = "Info")
    {
        await ShowToastAsync(message, title, "info", "fas fa-info-circle");
    }

    private async Task ShowToastAsync(string message, string title, string type, string icon)
    {
        // Use a small, safe JS helper instead of `eval` to avoid runtime parsing issues and CSP problems.
        try
        {
            await _jsRuntime.InvokeVoidAsync("careSync.toast.show", message ?? string.Empty, title ?? string.Empty, type ?? "info", icon ?? "");
        }
        catch (JSException jsEx)
        {
            // Avoid crashing the rendering pipeline if the JS helper is not available yet.
            // Log to console/server output for diagnostics and continue gracefully.
            Console.WriteLine($"Toast JS interop failed: {jsEx.Message}");
        }
    }
}
