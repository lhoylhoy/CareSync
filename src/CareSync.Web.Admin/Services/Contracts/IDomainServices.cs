namespace CareSync.Web.Admin.Services.Contracts;

using CareSync.Application.DTOs.Appointments;
using CareSync.Application.DTOs.Billing;
using CareSync.Application.DTOs.Doctors;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Application.DTOs.Patients;
using CareSync.Application.DTOs.Staff;
using CareSync.Web.Admin.Services;

public interface IDoctorService : ICrudService<DoctorDto, CreateDoctorDto, UpdateDoctorDto>
{
    public Task<List<DoctorDto>> GetDoctorsAsync();
    public Task<DoctorDto> GetDoctorByIdAsync(Guid id);
    public Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto doctor);
    public Task UpdateDoctorAsync(UpdateDoctorDto doctor);
    public Task<DoctorDto> UpsertDoctorAsync(UpsertDoctorDto doctor);
    public Task DeleteDoctorAsync(Guid id);
}

public interface IPatientService : ICrudService<PatientDto, CreatePatientDto, UpdatePatientDto>
{
    public Task<List<PatientDto>> GetPatientsAsync();
    public Task<PatientDto> GetPatientByIdAsync(Guid id);
    public Task<PatientDto> CreatePatientAsync(CreatePatientDto patient);
    public Task UpdatePatientAsync(UpdatePatientDto patient);
    public Task<PatientDto> UpsertPatientAsync(UpsertPatientDto patient);
    public Task DeletePatientAsync(Guid id);
}

public interface IAppointmentService : ICrudService<AppointmentDto, UpsertAppointmentDto, UpsertAppointmentDto>
{
    public Task<List<AppointmentDto>> GetAppointmentsAsync();
    public Task<AppointmentDto> GetAppointmentByIdAsync(Guid id);
    public Task CancelAppointmentAsync(Guid id, string reason);
    public Task<AppointmentDto> CompleteAppointmentAsync(Guid id, string? notes);
    public Task DeleteAppointmentAsync(Guid id);
}

public interface IMedicalRecordService : ICrudService<MedicalRecordDto, UpsertMedicalRecordDto, UpsertMedicalRecordDto>
{
    public Task<List<MedicalRecordDto>> GetAllMedicalRecordsAsync();
    public Task<List<MedicalRecordDto>> GetMedicalRecordsByPatientAsync(Guid patientId);
    public Task<MedicalRecordDto> GetMedicalRecordByIdAsync(Guid id);
    public Task<MedicalRecordDto> UpsertMedicalRecordAsync(UpsertMedicalRecordDto record);
    public Task DeleteMedicalRecordAsync(Guid id);
    public Task<MedicalRecordDto> FinalizeMedicalRecordAsync(Guid id, string? finalNotes, string? finalizedBy);
    public Task<MedicalRecordDto> ReopenMedicalRecordAsync(Guid id);
}

public interface IBillingService : ICrudService<BillDto, UpsertBillDto, UpsertBillDto>
{
    public Task<List<BillDto>> GetBillsAsync();
    public Task<BillDto> GetBillByIdAsync(Guid id);
    public Task<BillDto> UpsertBillAsync(UpsertBillDto bill);
    public Task DeleteBillAsync(Guid id);
}

public interface IStaffService
{
    public Task<List<StaffDto>> GetStaffAsync();
    public Task<StaffDto> GetStaffByIdAsync(Guid id);
    public Task<StaffDto> CreateStaffAsync(CreateStaffDto staff);
    public Task<StaffDto> UpdateStaffAsync(UpdateStaffDto staff);
    public Task<StaffDto> UpsertStaffAsync(UpsertStaffDto staff);
    public Task DeleteStaffAsync(Guid id);
}
