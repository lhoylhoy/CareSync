using System.Text.RegularExpressions;
using CareSync.Domain.ValueObjects;

namespace CareSync.Domain.Entities;

public class Patient
{
    private readonly List<string> _allergies = new();
    private readonly List<string> _medicalHistory = new();

    // EF Core requires a parameterless constructor
    private Patient()
    {
        FullName = new FullName("Unknown", "Unknown");
        Email = null; // Nullable field
        PhoneNumber = null; // Nullable field
        Street = "Unknown";
        ProvinceCode = "Unknown";
        ProvinceName = "Unknown";
        CityCode = "Unknown";
        CityName = "Unknown";
        BarangayCode = "Unknown";
        BarangayName = "Unknown";
        Gender = "Unknown";
        DateOfBirth = null; // Nullable field
        IsActive = true;
    }

    public Patient(FullName fullName, Email? email, PhoneNumber? phoneNumber, DateTime? dateOfBirth,
        string gender, string? street, string provinceCode, string provinceName, string cityCode, string cityName,
        string? cityZipCode, string barangayCode, string barangayName)
    {
        if (dateOfBirth.HasValue)
        {
            if (dateOfBirth.Value > DateTime.Today)
                throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));

            if (dateOfBirth.Value < DateTime.Today.AddYears(-150))
                throw new ArgumentException("Date of birth cannot be more than 150 years ago.", nameof(dateOfBirth));
        }

        if (string.IsNullOrWhiteSpace(gender))
            throw new ArgumentException("Gender cannot be null or empty.", nameof(gender));

        if (string.IsNullOrWhiteSpace(provinceName))
            throw new ArgumentException("Province name cannot be null or empty.", nameof(provinceName));

        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("City name cannot be null or empty.", nameof(cityName));

        if (string.IsNullOrWhiteSpace(barangayName))
            throw new ArgumentException("Barangay name cannot be null or empty.", nameof(barangayName));

        if (string.IsNullOrWhiteSpace(barangayCode))
            throw new ArgumentException("Barangay code cannot be empty.", nameof(barangayCode));

        Id = Guid.NewGuid();
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Email = email; // Now nullable
        PhoneNumber = phoneNumber; // Now nullable
        DateOfBirth = dateOfBirth;
        Gender = gender.Trim();
        Street = street?.Trim();
        ProvinceCode = provinceCode.Trim();
        ProvinceName = provinceName.Trim();
        CityCode = cityCode.Trim();
        CityName = cityName.Trim();
        CityZipCode = cityZipCode?.Trim();
        BarangayCode = barangayCode.Trim();
        BarangayName = barangayName.Trim();
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public FullName FullName { get; private set; }
    public string? Street { get; private set; }

    // Store geographic data using API codes - SIMPLE APPROACH
    public string ProvinceCode { get; private set; }
    public string ProvinceName { get; private set; }
    public string CityCode { get; private set; }
    public string CityName { get; private set; }
    public string? CityZipCode { get; private set; }
    public string BarangayCode { get; private set; }
    public string BarangayName { get; private set; }

    public PhoneNumber? PhoneNumber { get; private set; }
    public Email? Email { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public string Gender { get; private set; }
    public bool IsActive { get; private set; }

    // Optional fields
    public string? PhilHealthNumber { get; private set; }
    public string? SssNumber { get; private set; }
    public string? Tin { get; private set; }
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactNumber { get; private set; }
    public string? BloodType { get; private set; }

    public IReadOnlyList<string> Allergies => _allergies.AsReadOnly();
    public IReadOnlyList<string> MedicalHistory => _medicalHistory.AsReadOnly();

    public int? Age => DateOfBirth.HasValue ?
        DateTime.Today.Year - DateOfBirth.Value.Year -
        (DateTime.Today.DayOfYear < DateOfBirth.Value.DayOfYear ? 1 : 0) : null;

    public void UpdateContactInformation(string? street, string provinceCode, string provinceName, string cityCode,
        string cityName, string? cityZipCode, string barangayCode, string barangayName, PhoneNumber? phoneNumber,
        Email? email)
    {
        if (string.IsNullOrWhiteSpace(provinceName))
            throw new ArgumentException("Province name cannot be null or empty.", nameof(provinceName));

        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("City name cannot be null or empty.", nameof(cityName));

        if (string.IsNullOrWhiteSpace(barangayCode))
            throw new ArgumentException("Barangay code cannot be empty.", nameof(barangayCode));

        if (string.IsNullOrWhiteSpace(barangayName))
            throw new ArgumentException("Barangay name cannot be null or empty.", nameof(barangayName));

        Street = street?.Trim();
        ProvinceCode = provinceCode.Trim();
        ProvinceName = provinceName.Trim();
        CityCode = cityCode.Trim();
        CityName = cityName.Trim();
        CityZipCode = cityZipCode?.Trim();
        BarangayCode = barangayCode.Trim();
        BarangayName = barangayName.Trim();
        PhoneNumber = phoneNumber; // Now nullable
        Email = email; // Now nullable
    }

    public void UpdatePersonalInformation(FullName fullName, DateTime? dateOfBirth, string gender)
    {
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        DateOfBirth = dateOfBirth;
        Gender = gender?.Trim() ?? throw new ArgumentNullException(nameof(gender));
    }

    public void SetEmergencyContact(string name, string? phone)
    {
        EmergencyContactName = name?.Trim();
        EmergencyContactNumber = phone?.Trim();
    }

    public void ClearEmergencyContact()
    {
        EmergencyContactName = null;
        EmergencyContactNumber = null;
    }

    public void SetBloodType(string bloodType)
    {
        BloodType = bloodType?.Trim();
    }

    public void ClearBloodType()
    {
        BloodType = null;
    }

    public void SetPhilHealthNumber(string philHealthNumber)
    {
        if (!string.IsNullOrWhiteSpace(philHealthNumber))
        {
            var normalized = Regex.Replace(philHealthNumber, @"[^\d]", "");
            if (normalized.Length != 12)
                throw new ArgumentException("PhilHealth number must be 12 digits.", nameof(philHealthNumber));
            PhilHealthNumber = normalized;
        }
        else
        {
            PhilHealthNumber = null;
        }
    }

    public void SetSssNumber(string sssNumber)
    {
        if (!string.IsNullOrWhiteSpace(sssNumber))
        {
            var normalized = Regex.Replace(sssNumber, @"[^\d]", "");
            if (normalized.Length != 10)
                throw new ArgumentException("SSS number must be 10 digits.", nameof(sssNumber));
            SssNumber = normalized;
        }
        else
        {
            SssNumber = null;
        }
    }

    public void SetTin(string tin)
    {
        if (!string.IsNullOrWhiteSpace(tin))
        {
            var normalized = Regex.Replace(tin, @"[^\d]", "");
            if (normalized.Length != 12)
                throw new ArgumentException("TIN must be 12 digits.", nameof(tin));
            Tin = normalized;
        }
        else
        {
            Tin = null;
        }
    }

    public void AddAllergy(string allergy)
    {
        if (!string.IsNullOrWhiteSpace(allergy) && !_allergies.Contains(allergy.Trim()))
        {
            _allergies.Add(allergy.Trim());
        }
    }

    public void RemoveAllergy(string allergy)
    {
        _allergies.Remove(allergy?.Trim() ?? "");
    }

    public void AddMedicalHistory(string history)
    {
        if (!string.IsNullOrWhiteSpace(history) && !_medicalHistory.Contains(history.Trim()))
        {
            _medicalHistory.Add(history.Trim());
        }
    }

    public void RemoveMedicalHistory(string history)
    {
        _medicalHistory.Remove(history?.Trim() ?? "");
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
