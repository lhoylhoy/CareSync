using CareSync.Domain.Entities;
using CareSync.Domain.Enums;

namespace CareSync.Domain.Extensions;

/// <summary>
/// QUICK WIN #2: Extension methods for cleaner, more readable code
/// These helpers encapsulate common operations and business logic
/// </summary>
public static class PatientExtensions
{
    /// <summary>
    /// Get patient's full address in Philippine format
    /// </summary>
    public static string GetFullAddress(this Patient patient)
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(patient.Street))
            parts.Add(patient.Street);

        parts.Add(patient.BarangayName);
        parts.Add(patient.CityName);
        parts.Add(patient.ProvinceName);

        if (!string.IsNullOrEmpty(patient.CityZipCode))
            parts.Add(patient.CityZipCode);

        return string.Join(", ", parts);
    }

    /// <summary>
    /// Check if patient is a minor (under 18)
    /// </summary>
    public static bool IsMinor(this Patient patient)
    {
        var age = patient.GetAge();
        return age.HasValue && age.Value < 18;
    }

    /// <summary>
    /// Get patient's age in years
    /// </summary>
    public static int? GetAge(this Patient patient)
    {
        if (!patient.DateOfBirth.HasValue)
            return null;

        var today = DateTime.Today;
        var age = today.Year - patient.DateOfBirth.Value.Year;

        // Adjust if birthday hasn't occurred this year
        if (patient.DateOfBirth.Value.Date > today.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// Check if patient has complete contact information
    /// </summary>
    public static bool HasCompleteContactInfo(this Patient patient)
    {
        return !string.IsNullOrEmpty(patient.PhoneNumber?.Number) ||
               !string.IsNullOrEmpty(patient.Email?.Value);
    }

    /// <summary>
    /// Get primary contact method (phone or email)
    /// </summary>
    public static string? GetPrimaryContact(this Patient patient)
    {
        return patient.PhoneNumber?.Number ?? patient.Email?.Value;
    }
}

public static class MedicalRecordExtensions
{
    /// <summary>
    /// Check if medical record is complete enough to finalize
    /// </summary>
    public static bool IsReadyToFinalize(this MedicalRecord record)
    {
        return !string.IsNullOrWhiteSpace(record.ChiefComplaint) &&
               !string.IsNullOrWhiteSpace(record.Assessment) &&
               !string.IsNullOrWhiteSpace(record.TreatmentPlan);
    }

    /// <summary>
    /// Get a concise summary for display in lists
    /// </summary>
    public static string GetSummary(this MedicalRecord record, int maxLength = 100)
    {
        var summary = $"{record.RecordDate:yyyy-MM-dd}: {record.ChiefComplaint}";

        if (summary.Length > maxLength)
            summary = summary.Substring(0, maxLength - 3) + "...";

        return summary;
    }

    /// <summary>
    /// Check if record is recent (within last 30 days)
    /// </summary>
    public static bool IsRecent(this MedicalRecord record)
    {
        return record.RecordDate >= DateTime.UtcNow.AddDays(-30);
    }

    /// <summary>
    /// Get total number of prescriptions
    /// </summary>
    public static int GetPrescriptionCount(this MedicalRecord record)
    {
        return record.Prescriptions.Count;
    }

    /// <summary>
    /// Get total number of diagnoses
    /// </summary>
    public static int GetDiagnosisCount(this MedicalRecord record)
    {
        return record.Diagnoses.Count;
    }
}

public static class AppointmentExtensions
{
    /// <summary>
    /// Check if appointment is upcoming (in the future)
    /// </summary>
    public static bool IsUpcoming(this Appointment appointment)
    {
        return appointment.ScheduledDate > DateTime.Now &&
               appointment.Status == AppointmentStatus.Scheduled;
    }

    /// <summary>
    /// Check if appointment is today
    /// </summary>
    public static bool IsToday(this Appointment appointment)
    {
        return appointment.ScheduledDate.Date == DateTime.Today;
    }

    /// <summary>
    /// Check if patient is late (appointment time has passed but still scheduled)
    /// </summary>
    public static bool IsLate(this Appointment appointment)
    {
        return appointment.ScheduledDate < DateTime.Now &&
               appointment.Status == AppointmentStatus.Scheduled;
    }

    /// <summary>
    /// Check if appointment is in the past
    /// </summary>
    public static bool IsPast(this Appointment appointment)
    {
        return appointment.ScheduledDate < DateTime.Now;
    }

    /// <summary>
    /// Get time until appointment (or time since if past)
    /// </summary>
    public static TimeSpan GetTimeUntil(this Appointment appointment)
    {
        return appointment.ScheduledDate - DateTime.Now;
    }

    /// <summary>
    /// Get formatted time until appointment
    /// </summary>
    public static string GetTimeUntilFormatted(this Appointment appointment)
    {
        var timeUntil = appointment.GetTimeUntil();

        if (timeUntil.TotalMinutes < 0)
            return $"{Math.Abs(timeUntil.TotalMinutes):F0} minutes ago";

        if (timeUntil.TotalHours < 1)
            return $"in {timeUntil.TotalMinutes:F0} minutes";

        if (timeUntil.TotalHours < 24)
            return $"in {timeUntil.TotalHours:F1} hours";

        return $"in {timeUntil.TotalDays:F0} days";
    }

    /// <summary>
    /// Check if appointment can be cancelled (not already completed or cancelled)
    /// </summary>
    public static bool CanBeCancelled(this Appointment appointment)
    {
        return appointment.Status != AppointmentStatus.Completed &&
               appointment.Status != AppointmentStatus.Cancelled &&
               appointment.Status != AppointmentStatus.NoShow;
    }

    /// <summary>
    /// Check if appointment can be checked in (scheduled and today)
    /// </summary>
    public static bool CanBeCheckedIn(this Appointment appointment)
    {
        return appointment.Status == AppointmentStatus.Scheduled &&
               appointment.IsToday();
    }
}

public static class DoctorExtensions
{
    /// <summary>
    /// Get doctor's full name with title
    /// </summary>
    public static string GetFullNameWithTitle(this Doctor doctor)
    {
        return doctor.DisplayName; // Already includes "Dr" prefix
    }

    /// <summary>
    /// Get primary contact method
    /// </summary>
    public static string? GetPrimaryContact(this Doctor doctor)
    {
        return doctor.PhoneNumber?.Number ?? doctor.Email?.Value;
    }
}

public static class BillExtensions
{
    /// <summary>
    /// Check if bill is overdue
    /// </summary>
    public static bool IsOverdue(this Bill bill)
    {
        return bill.DueDate < DateTime.Today &&
               bill.Status == BillStatus.Pending;
    }

    /// <summary>
    /// Get days overdue (0 if not overdue)
    /// </summary>
    public static int GetDaysOverdue(this Bill bill)
    {
        if (!bill.IsOverdue())
            return 0;

        return (DateTime.Today - bill.DueDate).Days;
    }

    /// <summary>
    /// Get remaining balance
    /// </summary>
    public static decimal GetRemainingBalance(this Bill bill)
    {
        return bill.TotalAmount - bill.PaidAmount;
    }

    /// <summary>
    /// Check if bill is fully paid
    /// </summary>
    public static bool IsFullyPaid(this Bill bill)
    {
        return bill.Status == BillStatus.Paid ||
               bill.GetRemainingBalance() <= 0;
    }

    /// <summary>
    /// Get payment progress percentage (0-100)
    /// </summary>
    public static decimal GetPaymentProgress(this Bill bill)
    {
        if (bill.TotalAmount == 0)
            return 100;

        return Math.Min(100, (bill.PaidAmount / bill.TotalAmount) * 100);
    }
}
