namespace CareSync.Web.Admin.Services;

public static class ValidationMessages
{
    // Required field messages
    public static string Required(string fieldName) => $"{fieldName} is required.";

    // Format validation messages
    public const string InvalidEmail = "Please enter a valid email address.";
    public const string InvalidPhoneNumber = "Please enter a valid phone number.";
    public const string InvalidDateFormat = "Please enter a valid date.";

    // Date validation messages
    public const string DateCannotBeFuture = "Date cannot be in the future.";
    public const string DateCannotBePast = "Date cannot be in the past.";
    public const string DateTooOld = "Please enter a valid date.";
    public const string BirthDateCannotBeFuture = "Date of birth cannot be in the future.";
    public const string BirthDateTooOld = "Please enter a valid date of birth.";

    // Length validation messages
    public static string TooShort(string fieldName, int minLength) =>
        $"{fieldName} must be at least {minLength} characters long.";
    public static string TooLong(string fieldName, int maxLength) =>
        $"{fieldName} cannot exceed {maxLength} characters.";
    public static string ExactLength(string fieldName, int length) =>
        $"{fieldName} must be exactly {length} characters long.";

    // Number validation messages
    public static string NumberTooSmall(string fieldName, decimal minValue) =>
        $"{fieldName} must be at least {minValue}.";
    public static string NumberTooLarge(string fieldName, decimal maxValue) =>
        $"{fieldName} cannot exceed {maxValue}.";
    public const string InvalidNumber = "Please enter a valid number.";

    // Selection validation messages
    public static string SelectionRequired(string fieldName) => $"Please select a {fieldName.ToLower()}.";

    // Password validation messages
    public const string PasswordTooWeak = "Password must contain at least 8 characters, including uppercase, lowercase, and a number.";
    public const string PasswordsDoNotMatch = "Passwords do not match.";

    // File validation messages
    public const string FileRequired = "Please select a file.";
    public const string FileTooLarge = "File size exceeds the maximum allowed limit.";
    public static string InvalidFileType(string allowedTypes) =>
        $"Invalid file type. Allowed types: {allowedTypes}.";

    // Philippine-specific validation messages
    public const string InvalidPhilippinePhoneNumber = "Please enter a valid Philippine phone number (e.g., 09123456789 or +639123456789).";
    public const string InvalidPhilHealthNumber = "Please enter a valid PhilHealth number (12 digits).";
    public const string InvalidSssNumber = "Please enter a valid SSS number (10 digits).";
    public const string InvalidTinNumber = "Please enter a valid TIN (9-15 digits).";

    // Address validation messages
    public const string ProvinceRequired = "Province is required.";
    public const string CityRequired = "City/Municipality is required.";
    public const string BarangayRequired = "Barangay is required.";
    public const string ZipCodeRequired = "ZIP Code is required.";

    // Age validation messages
    public static string AgeTooYoung(int minAge) => $"Age must be at least {minAge} years old.";
    public static string AgeTooOld(int maxAge) => $"Age cannot exceed {maxAge} years old.";

    // Appointment validation messages
    public const string AppointmentDateRequired = "Appointment date is required.";
    public const string AppointmentTimeRequired = "Appointment time is required.";
    public const string AppointmentInPast = "Appointment cannot be scheduled in the past.";
    public const string AppointmentTooFarAhead = "Appointment cannot be scheduled more than 6 months in advance.";

    // Medical record validation messages
    public const string DiagnosisRequired = "Diagnosis is required.";
    public const string TreatmentRequired = "Treatment plan is required.";
    public const string VitalSignsInvalid = "Please enter valid vital signs.";

    // Billing validation messages
    public const string AmountRequired = "Amount is required.";
    public const string InvalidAmount = "Please enter a valid amount.";
    public const string AmountMustBePositive = "Amount must be greater than zero.";

    // Generic validation messages
    public const string UnexpectedError = "An unexpected error occurred. Please try again.";
    public const string NetworkError = "Network error. Please check your connection and try again.";
    public const string InvalidInput = "Invalid input. Please check your entry and try again.";

    // Success messages
    public const string SavedSuccessfully = "Saved successfully.";
    public const string UpdatedSuccessfully = "Updated successfully.";
    public const string DeletedSuccessfully = "Deleted successfully.";
    public const string CreatedSuccessfully = "Created successfully.";
}
