namespace CareSync.Web.Admin.Services.Contracts;

using CareSync.Application.DTOs.Patients;
using CareSync.Application.DTOs.Doctors;
using CareSync.Application.DTOs.Appointments;
using CareSync.Application.DTOs.Billing;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Application.DTOs.Staff;
using CareSync.Web.Admin.Services;

public interface IDoctorService : ICrudService<DoctorDto, CreateDoctorDto, UpdateDoctorDto>
{
    Task<List<DoctorDto>> GetDoctorsAsync();
    Task<DoctorDto> GetDoctorByIdAsync(Guid id);
    Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto doctor);
    Task UpdateDoctorAsync(UpdateDoctorDto doctor);
    Task<DoctorDto> UpsertDoctorAsync(UpsertDoctorDto doctor);
    Task DeleteDoctorAsync(Guid id);
}

public interface IPatientService : ICrudService<PatientDto, CreatePatientDto, UpdatePatientDto>
{
    Task<List<PatientDto>> GetPatientsAsync();
    Task<PatientDto> GetPatientByIdAsync(Guid id);
    Task<PatientDto> CreatePatientAsync(CreatePatientDto patient);
    Task UpdatePatientAsync(UpdatePatientDto patient);
    Task<PatientDto> UpsertPatientAsync(UpsertPatientDto patient);
    Task DeletePatientAsync(Guid id);
}

public interface IAppointmentService : ICrudService<AppointmentDto, UpsertAppointmentDto, UpsertAppointmentDto>
{
    Task<List<AppointmentDto>> GetAppointmentsAsync();
    Task<AppointmentDto> GetAppointmentByIdAsync(Guid id);
    Task CancelAppointmentAsync(Guid id, string reason);
    Task<AppointmentDto> CompleteAppointmentAsync(Guid id, string? notes);
    Task DeleteAppointmentAsync(Guid id);
}

public interface IMedicalRecordService : ICrudService<MedicalRecordDto, UpsertMedicalRecordDto, UpsertMedicalRecordDto>
{
    Task<List<MedicalRecordDto>> GetAllMedicalRecordsAsync();
    Task<List<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(Guid patientId);
    Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id);
    Task<MedicalRecordDto> UpsertMedicalRecordAsync(UpsertMedicalRecordDto record);
    Task DeleteMedicalRecordAsync(Guid id);
    Task<MedicalRecordDto> FinalizeMedicalRecordAsync(Guid id, string? finalNotes, string? finalizedBy);
    Task<MedicalRecordDto> ReopenMedicalRecordAsync(Guid id);
}

public interface IBillingService : ICrudService<BillDto, UpsertBillDto, UpsertBillDto>
{
    Task<List<BillDto>> GetBillsAsync();
    Task<BillDto> GetBillByIdAsync(Guid id);
    Task<BillDto> UpsertBillAsync(UpsertBillDto bill);
    Task DeleteBillAsync(Guid id);
}

public interface IStaffService
{
    Task<List<StaffDto>> GetStaffAsync();
    Task<StaffDto> GetStaffByIdAsync(Guid id);
    Task<StaffDto> CreateStaffAsync(CreateStaffDto staff);
    Task<StaffDto> UpdateStaffAsync(UpdateStaffDto staff);
    Task<StaffDto> UpsertStaffAsync(UpsertStaffDto staff);
    Task DeleteStaffAsync(Guid id);
}
