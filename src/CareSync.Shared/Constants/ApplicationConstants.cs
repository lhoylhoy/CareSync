namespace CareSync.Shared.Constants;

/// <summary>
///     API endpoint constants
/// </summary>
public static class ApiEndpoints
{
    public const string BaseUrl = "/api";

    // Patient endpoints
    public static class Patients
    {
        public const string Base = $"{BaseUrl}/patients";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
    }

    // Doctor endpoints
    public static class Doctors
    {
        public const string Base = $"{BaseUrl}/doctors";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
    }

    // Appointment endpoints
    public static class Appointments
    {
        public const string Base = $"{BaseUrl}/appointments";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Upsert = $"{Base}/upsert";
        public const string Cancel = $"{Base}/{{id}}/cancel";
        public const string ByPatient = $"{Base}/patient/{{patientId}}";
        public const string ByDoctor = $"{Base}/doctor/{{doctorId}}";
    }

    // Medical Records endpoints
    public static class MedicalRecords
    {
        public const string Base = $"{BaseUrl}/medicalrecords";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
        public const string ByPatient = $"{Base}/patient/{{patientId}}";
        public const string Finalize = $"{Base}/{{id}}/finalize";
        public const string Reopen = $"{Base}/{{id}}/reopen";
    }

    // Billing endpoints
    public static class Billing
    {
        public const string Base = $"{BaseUrl}/billing";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
    }

    // Geographic endpoints
    public static class Geographic
    {
        public const string Provinces = $"{BaseUrl}/provinces";
        public const string Cities = $"{BaseUrl}/provinces/{{provinceId}}/cities";
        public const string Barangays = $"{BaseUrl}/provinces/{{provinceId}}/cities/{{cityId}}/barangays";
    }
}

/// <summary>
///     Pagination constants
/// </summary>
public static class Pagination
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const int MinPageSize = 1;
    public const int DefaultPage = 1;
}

/// <summary>
///     Application configuration constants
/// </summary>
public static class AppConstants
{
    public const string ApplicationName = "CareSync";
    public const string Version = "1.0.0";

    // Date formats
    public const string DateFormat = "yyyy-MM-dd";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string TimeFormat = "HH:mm";

    // File upload limits
    public const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    public static readonly string[] AllowedImageTypes = { ".jpg", ".jpeg", ".png", ".gif" };
    public static readonly string[] AllowedDocumentTypes = { ".pdf", ".doc", ".docx", ".txt" };
}
