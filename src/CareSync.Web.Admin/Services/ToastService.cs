using Microsoft.JSInterop;

namespace CareSync.Web.Admin.Services;

public interface IToastService
{
    Task ShowSuccessAsync(string message, string title = "Success");
    Task ShowErrorAsync(string message, string title = "Error");
    Task ShowWarningAsync(string message, string title = "Warning");
    Task ShowInfoAsync(string message, string title = "Info");
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
        var toastScript = $@"
            const toast = document.createElement('div');
            toast.className = 'toast position-fixed top-0 end-0 m-3';
            toast.setAttribute('role', 'alert');
            toast.style.zIndex = '9999';
            toast.innerHTML = `
                <div class='toast-header bg-{type} text-white'>
                    <i class='{icon} me-2'></i>
                    <strong class='me-auto'>{title}</strong>
                    <button type='button' class='btn-close btn-close-white' data-bs-dismiss='toast'></button>
                </div>
                <div class='toast-body'>
                    {message}
                </div>
            `;
            document.body.appendChild(toast);
            const bsToast = new bootstrap.Toast(toast);
            bsToast.show();
            setTimeout(() => toast.remove(), 5000);
        ";

        await _jsRuntime.InvokeVoidAsync("eval", toastScript);
    }
}
