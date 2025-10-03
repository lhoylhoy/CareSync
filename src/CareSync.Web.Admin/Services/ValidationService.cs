using System.Text.RegularExpressions;

namespace CareSync.Web.Admin.Services;

public interface IValidationService
{
    public string? ValidateRequired(string? value, string fieldName);
    public string? ValidateEmail(string? value);
    public string? ValidatePhilippinePhoneNumber(string? value);
    public string? ValidateDateOfBirth(DateTime? dateValue);
    public string? ValidatePhilHealthNumber(string? value);
    public string? ValidateSssNumber(string? value);
    public string? ValidateTinNumber(string? value);
    public string? ValidateLength(string? value, string fieldName, int? minLength = null, int? maxLength = null, int? exactLength = null);
    public string? ValidateNumber(decimal? value, string fieldName, decimal? minValue = null, decimal? maxValue = null);
    public bool IsValidEmail(string? email);
    public bool IsValidPhilippinePhoneNumber(string? phoneNumber);
}

public class ValidationService : IValidationService
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhilippinePhoneRegex = new(@"^(\+?639|09)\d{9}$", RegexOptions.Compiled);
    private static readonly Regex PhilHealthRegex = new(@"^\d{12}$", RegexOptions.Compiled);
    private static readonly Regex SssRegex = new(@"^\d{10}$", RegexOptions.Compiled);
    private static readonly Regex TinRegex = new(@"^\d{9,15}$", RegexOptions.Compiled);

    public string? ValidateRequired(string? value, string fieldName)
    {
        return string.IsNullOrWhiteSpace(value) ? ValidationMessages.Required(fieldName) : null;
    }

    public string? ValidateEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null; // Email is optional; no error when blank

        return IsValidEmail(value) ? null : ValidationMessages.InvalidEmail;
    }

    public string? ValidatePhilippinePhoneNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null; // Optional field validation should be handled separately

        return IsValidPhilippinePhoneNumber(value) ? null : ValidationMessages.InvalidPhilippinePhoneNumber;
    }

    public string? ValidateDateOfBirth(DateTime? dateValue)
    {
        if (!dateValue.HasValue || dateValue.Value == DateTime.MinValue)
            return ValidationMessages.Required("Date of birth");

        if (dateValue.Value > DateTime.Today)
            return ValidationMessages.BirthDateCannotBeFuture;

        if (dateValue.Value < DateTime.Today.AddYears(-150))
            return ValidationMessages.BirthDateTooOld;

        return null;
    }

    public string? ValidatePhilHealthNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null; // Optional field

        return PhilHealthRegex.IsMatch(value) ? null : ValidationMessages.InvalidPhilHealthNumber;
    }

    public string? ValidateSssNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null; // Optional field

        return SssRegex.IsMatch(value) ? null : ValidationMessages.InvalidSssNumber;
    }

    public string? ValidateTinNumber(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null; // Optional field

        return TinRegex.IsMatch(value) ? null : ValidationMessages.InvalidTinNumber;
    }

    public string? ValidateLength(string? value, string fieldName, int? minLength = null, int? maxLength = null, int? exactLength = null)
    {
        if (string.IsNullOrEmpty(value))
            return null; // Length validation only applies to non-empty values

        if (exactLength.HasValue && value.Length != exactLength.Value)
            return ValidationMessages.ExactLength(fieldName, exactLength.Value);

        if (minLength.HasValue && value.Length < minLength.Value)
            return ValidationMessages.TooShort(fieldName, minLength.Value);

        if (maxLength.HasValue && value.Length > maxLength.Value)
            return ValidationMessages.TooLong(fieldName, maxLength.Value);

        return null;
    }

    public string? ValidateNumber(decimal? value, string fieldName, decimal? minValue = null, decimal? maxValue = null)
    {
        if (!value.HasValue)
            return ValidationMessages.Required(fieldName);

        if (minValue.HasValue && value.Value < minValue.Value)
            return ValidationMessages.NumberTooSmall(fieldName, minValue.Value);

        if (maxValue.HasValue && value.Value > maxValue.Value)
            return ValidationMessages.NumberTooLarge(fieldName, maxValue.Value);

        return null;
    }

    public bool IsValidEmail(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    public bool IsValidPhilippinePhoneNumber(string? phoneNumber)
    {
        return !string.IsNullOrWhiteSpace(phoneNumber) && PhilippinePhoneRegex.IsMatch(phoneNumber);
    }
}
