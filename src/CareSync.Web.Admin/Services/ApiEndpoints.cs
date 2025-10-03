namespace CareSync.Web.Admin.Services;

/// <summary>
///     API endpoint constants used by the admin web client.
/// </summary>
public static class ApiEndpoints
{
    public const string BaseUrl = "/api";

    public static class Patients
    {
        public const string Base = $"{BaseUrl}/patients";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Doctors
    {
        public const string Base = $"{BaseUrl}/doctors";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Update = $"{Base}/{{id}}";
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
    }

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

    public static class Billing
    {
        public const string Base = $"{BaseUrl}/billing";
        public const string GetById = $"{Base}/{{id}}";
        public const string Create = Base;
        public const string Upsert = $"{Base}/upsert";
        public const string Delete = $"{Base}/{{id}}";
    }

    public static class Geographic
    {
        public const string Provinces = $"{BaseUrl}/provinces";
        public const string Cities = $"{BaseUrl}/provinces/{{provinceId}}/cities";
        public const string Barangays = $"{BaseUrl}/provinces/{{provinceId}}/cities/{{cityId}}/barangays";
    }
}
