using CareSync.Domain.Entities;

namespace CareSync.Domain.Interfaces;

/// <summary>
///     Repository interface for MedicalRecord entity
/// </summary>
public interface IMedicalRecordRepository
{
    /// <summary>
    ///     Gets a medical record by ID
    /// </summary>
    /// <param name="id">The medical record ID</param>
    /// <returns>The medical record if found, null otherwise</returns>
    Task<MedicalRecord?> GetByIdAsync(Guid id);

    /// <summary>
    ///     Gets a medical record by ID with all related data (patient, doctor, vital signs, diagnoses, prescriptions)
    /// </summary>
    /// <param name="id">The medical record ID</param>
    /// <returns>The medical record with details if found, null otherwise</returns>
    Task<MedicalRecord?> GetByIdWithDetailsAsync(Guid id);

    /// <summary>
    ///     Gets all medical records for a specific patient
    /// </summary>
    /// <param name="patientId">The patient ID</param>
    /// <returns>List of medical records for the patient</returns>
    Task<List<MedicalRecord>> GetByPatientIdAsync(Guid patientId);

    /// <summary>
    ///     Gets all medical records for a specific doctor
    /// </summary>
    /// <param name="doctorId">The doctor ID</param>
    /// <returns>List of medical records for the doctor</returns>
    Task<List<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId);

    /// <summary>
    ///     Gets all medical records for a specific appointment
    /// </summary>
    /// <param name="appointmentId">The appointment ID</param>
    /// <returns>List of medical records for the appointment</returns>
    Task<List<MedicalRecord>> GetByAppointmentIdAsync(Guid appointmentId);

    /// <summary>
    ///     Gets all medical records in the system
    /// </summary>
    /// <returns>List of all medical records</returns>
    Task<IEnumerable<MedicalRecord>> GetAllAsync();

    /// <summary>
    ///     Searches medical records based on various criteria
    /// </summary>
    /// <param name="patientId">Optional patient ID filter</param>
    /// <param name="doctorId">Optional doctor ID filter</param>
    /// <param name="fromDate">Optional start date filter</param>
    /// <param name="toDate">Optional end date filter</param>
    /// <param name="isFinalized">Optional finalization status filter</param>
    /// <returns>List of medical records matching the criteria</returns>
    Task<IEnumerable<MedicalRecord>> SearchAsync(
        Guid? patientId = null,
        Guid? doctorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        bool? isFinalized = null);

    /// <summary>
    ///     Gets medical records created within a date range
    /// </summary>
    /// <param name="fromDate">Start date</param>
    /// <param name="toDate">End date</param>
    /// <returns>List of medical records within the date range</returns>
    Task<List<MedicalRecord>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    ///     Gets medical records by diagnosis code
    /// </summary>
    /// <param name="diagnosisCode">The diagnosis code</param>
    /// <returns>List of medical records with the specified diagnosis</returns>
    Task<List<MedicalRecord>> GetByDiagnosisCodeAsync(string diagnosisCode);

    /// <summary>
    ///     Checks if a medical record exists with the specified appointment
    /// </summary>
    /// <param name="appointmentId">The appointment ID</param>
    /// <returns>True if a medical record exists for the appointment</returns>
    Task<bool> ExistsForAppointmentAsync(Guid appointmentId);

    /// <summary>
    ///     Adds a new medical record
    /// </summary>
    /// <param name="medicalRecord">The medical record to add</param>
    Task AddAsync(MedicalRecord medicalRecord);

    /// <summary>
    ///     Updates an existing medical record
    /// </summary>
    /// <param name="medicalRecord">The medical record to update</param>
    Task UpdateAsync(MedicalRecord medicalRecord);

    /// <summary>
    ///     Deletes a medical record
    /// </summary>
    /// <param name="id">The medical record ID to delete</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    ///     Saves all changes to the database
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    ///     Indicates whether the medical record has related clinical data (diagnoses, prescriptions, vitals, treatments) or is finalized.
    ///     Used to disable deletion in UI and guard destructive commands.
    /// </summary>
    Task<bool> HasRelatedDataAsync(Guid id);
}
