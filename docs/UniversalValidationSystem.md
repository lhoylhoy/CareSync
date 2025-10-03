# Universal Validation System

## Overview
The CareSync application now includes a comprehensive universal validation system that provides consistent, human-readable validation messages across all forms and components.

## Components

### 1. ValidationMessages.cs
Central repository for all validation messages used throughout the application.

**Key Features:**
- Consistent messaging across the entire application
- Philippine-specific validation messages
- Parameterized messages for dynamic content
- Professional, user-friendly language

### 2. ValidationService.cs
Service class that handles all validation logic and returns appropriate error messages.

**Key Methods:**
- `ValidateRequired(value, fieldName)` - Required field validation
- `ValidateEmail(value)` - Email format validation
- `ValidatePhilippinePhoneNumber(value)` - Philippine phone number format
- `ValidateDateOfBirth(dateValue)` - Date of birth validation with age limits
- `ValidatePhilHealthNumber(value)` - 12-digit PhilHealth number
- `ValidateSssNumber(value)` - 10-digit SSS number
- `ValidateTinNumber(value)` - 9-15 digit TIN
- `ValidateLength(value, fieldName, min, max, exact)` - String length validation
- `ValidateNumber(value, fieldName, min, max)` - Number range validation

## Usage Examples

### Basic Required Field Validation
```csharp
var error = ValidationService.ValidateRequired(Model.FirstName, "First name");
if (error != null)
{
    ValidationErrors["FirstName"] = error;
}
```

### Email Validation
```csharp
var error = ValidationService.ValidateEmail(Model.Email);
if (error != null)
{
    ValidationErrors["Email"] = error;
}
```

### Philippine Phone Number Validation
```csharp
var error = ValidationService.ValidatePhilippinePhoneNumber(Model.PhoneNumber);
if (error != null)
{
    ValidationErrors["PhoneNumber"] = error;
}
```

### Optional Field Validation (PhilHealth, SSS, TIN)
```csharp
// Only validates if value is not empty
var error = ValidationService.ValidatePhilHealthNumber(Model.PhilHealthNumber);
if (error != null)
{
    ValidationErrors["PhilHealthNumber"] = error;
}
```

## Validation Messages

### Common Messages
- **Required Fields:** "{Field name} is required."
- **Invalid Email:** "Please enter a valid email address."
- **Invalid Phone:** "Please enter a valid Philippine phone number (e.g., 09123456789 or +639123456789)."

### Date Validation
- **Date of Birth Required:** "Date of birth is required."
- **Future Date:** "Date of birth cannot be in the future."
- **Too Old:** "Please enter a valid date of birth."

### Philippine-Specific Validation
- **PhilHealth:** "Please enter a valid PhilHealth number (12 digits)."
- **SSS:** "Please enter a valid SSS number (10 digits)."
- **TIN:** "Please enter a valid TIN (9-15 digits)."

### Address Validation
- **Province:** "Province is required."
- **City:** "City/Municipality is required."
- **Barangay:** "Barangay is required."
- **ZIP Code:** "ZIP Code is required."

## Implementation in Forms

### 1. Inject the ValidationService
```razor
@inject IValidationService ValidationService
@using CareSync.Web.Admin.Services
```

### 2. Create Validation Methods
```csharp
private void ValidateField(string fieldName, string? value, string displayName)
{
    TouchedFields.Add(fieldName);

    var error = ValidationService.ValidateRequired(value, displayName);
    if (error != null)
    {
        ValidationErrors[fieldName] = error;
    }
    else
    {
        ValidationErrors.Remove(fieldName);
    }
    StateHasChanged();
}
```

### 3. Handle Input Events
```csharp
private void OnFieldChanged(ChangeEventArgs e)
{
    var value = e.Value?.ToString() ?? "";
    Model.FieldName = value;
    ValidateField("FieldName", value, "Field Display Name");
}
```

### 4. Display Validation Errors
```razor
@if (ValidationErrors.TryGetValue("FieldName", out var fieldError))
{
    <div class="invalid-feedback d-block">@fieldError</div>
}
```

## Benefits

1. **Consistency:** All validation messages follow the same format and tone
2. **Maintainability:** Central location for all validation logic and messages
3. **Reusability:** Validation service can be used across all forms and components
4. **Philippine Context:** Built-in support for Philippine-specific validations
5. **User Experience:** Professional, clear, and helpful error messages
6. **Performance:** Compiled regex patterns for optimal validation speed
7. **Extensibility:** Easy to add new validation rules and messages

## Best Practices

1. Always use the ValidationService for consistency
2. Mark fields as "touched" before showing validation errors
3. Clear validation errors when fields become valid
4. Use appropriate field names in validation messages
5. Handle both required and optional field validations appropriately
6. Test validation with edge cases and invalid inputs

## Future Enhancements

- Add client-side validation attributes
- Implement async validation for unique field checks
- Add conditional validation based on other field values
- Create validation groups for complex forms
- Add localization support for multiple languages
