# Reusable UI Components Guide

This document provides guidance on using the standardized UI components throughout the CareSync Web Admin application.

## ConfirmationDialog Component

A reusable modal dialog for user confirmations with customizable styling and messaging.

### Usage

```razor
@using CareSync.Web.Admin.Components

<!-- Add the component reference -->
<ConfirmationDialog @ref="confirmationDialog"
                   Type="ConfirmationDialogType.Danger"
                   Title="Confirm Action"
                   Message="Are you sure you want to perform this action?"
                   ConfirmText="Yes, Continue"
                   OnConfirm="HandleConfirmation" />
```

### Code-behind

```csharp
private ConfirmationDialog confirmationDialog = null!;

private async Task ShowConfirmation()
{
    await confirmationDialog.ShowAsync();
}

private async Task HandleConfirmation()
{
    // Your confirmation logic here
    Console.WriteLine("User confirmed the action");
}
```

### Parameters

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `DialogType` | `ConfirmationDialogType` | The dialog type (Danger, Warning, Info, Success) | `Danger` |
| `Title` | `string` | The dialog title | `"Confirm Action"` |
| `Message` | `string` | The dialog message | `"Are you sure?"` |
| `ConfirmText` | `string` | The confirm button text | `"Confirm"` |
| `CancelText` | `string` | The cancel button text | `"Cancel"` |
| `OnConfirm` | `EventCallback` | Callback when user confirms | - |

### Dialog Types

- **Danger**: Red styling for destructive actions (delete, remove)
- **Warning**: Yellow styling for cautionary actions
- **Info**: Blue styling for informational confirmations
- **Success**: Green styling for positive confirmations

### Important: Hide Dialogs After Confirmation

Always hide the confirmation dialog after the user confirms an action:

```csharp
private async Task HandleDelete()
{
    try
    {
        await MyService.DeleteItemAsync(itemId);
        await LoadItems();
        await ToastService.ShowSuccessAsync("Item deleted successfully.");
        await confirmationDialog.HideAsync(); // Always hide the dialog after successful operation
    }
    catch
    {
        await ToastService.ShowErrorAsync("Failed to delete item. Please try again.");
        // Note: Don't hide dialog on error - let user retry or cancel
    }
}
```## ToastService

A service for displaying toast notifications with different types and styling.

### Usage

First, inject the service in your component:

```razor
@inject IToastService ToastService
```

### Methods

```csharp
// Success toast
await ToastService.ShowSuccessAsync("Operation completed successfully!");

// Error toast
await ToastService.ShowErrorAsync("An error occurred while processing your request.");

// Warning toast
await ToastService.ShowWarningAsync("Please review your input before proceeding.");

// Info toast
await ToastService.ShowInfoAsync("Here's some helpful information.");

// With custom titles
await ToastService.ShowSuccessAsync("Data saved successfully!", "Success");
await ToastService.ShowErrorAsync("Failed to connect to server.", "Connection Error");
```

### Features

- **Auto-dismiss**: Toasts automatically disappear after 5 seconds
- **Positioning**: Displays in the top-right corner
- **Stacking**: Multiple toasts stack vertically
- **Responsive**: Works on all screen sizes
- **Accessible**: Includes proper ARIA attributes

## Implementation Guidelines

### 1. Consistency
- Always use `ConfirmationDialog` for user confirmations instead of JavaScript alerts
- Use `ToastService` for feedback messages instead of custom toast implementations
- Follow the established color scheme and styling

### 2. Accessibility
- Components include proper ARIA attributes
- Keyboard navigation is supported
- Screen reader friendly

### 3. Performance
- Components are lightweight and optimized
- Minimal JavaScript interop usage
- No external dependencies beyond Bootstrap 5

### 4. Customization
- Use the provided parameters to customize behavior
- Extend dialog types if needed
- Maintain consistent styling across the application

## Migration from Custom Implementations

### Before (Custom Modal)
```razor
<!-- Custom delete modal -->
<div class="modal fade" id="deleteModal">
    <!-- Custom modal structure -->
</div>

<script>
    function showDeleteModal() {
        new bootstrap.Modal(document.getElementById('deleteModal')).show();
    }
</script>
```

### After (Reusable Component)
```razor
<ConfirmationDialog @ref="confirmDialog"
                   Type="ConfirmationDialogType.Danger"
                   Title="Confirm Deletion"
                   Message="Are you sure you want to delete this item?"
                   OnConfirm="ConfirmDelete" />
```

### Benefits of Migration
- ✅ Consistent UI/UX across the application
- ✅ Reduced code duplication
- ✅ Better maintainability
- ✅ Improved accessibility
- ✅ Type-safe parameters
- ✅ No custom JavaScript required
- ✅ Responsive design
- ✅ Built-in animations and styling

## Best Practices

1. **Always use the reusable components** instead of creating custom modals or alerts
2. **Choose appropriate dialog types** based on the action severity
3. **Provide clear and descriptive messages** for better user experience
4. **Use ToastService for feedback** after successful or failed operations
5. **Test with keyboard navigation** to ensure accessibility
6. **Keep messages concise** but informative
