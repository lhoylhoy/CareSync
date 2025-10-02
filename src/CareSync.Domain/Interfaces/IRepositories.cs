using CareSync.Domain.Entities;
using CareSync.Domain.Enums;

namespace CareSync.Domain.Interfaces;

public interface IBillRepository
{
    Task<Bill?> GetByIdAsync(Guid id);
    Task<List<Bill>> GetAllAsync();
    Task<int> GetTotalCountAsync();
    Task<List<Bill>> GetByPatientIdAsync(Guid patientId);
    Task<List<Bill>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Bill>> GetOutstandingBillsAsync();
    Task<List<Bill>> GetOverdueBillsAsync();
    Task<Bill?> GetByBillNumberAsync(string billNumber);
    Task<bool> HasRelatedDataAsync(Guid id); // payments or claims
    Task AddAsync(Bill bill);
    Task UpdateAsync(Bill bill);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<List<Payment>> GetByBillIdAsync(Guid billId);
    Task<List<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<List<Payment>> GetByPaymentMethodAsync(PaymentMethod method, DateTime? startDate = null,
        DateTime? endDate = null);

    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
    Task DeleteAsync(Guid id);
}

public interface IInsuranceClaimRepository
{
    Task<InsuranceClaim?> GetByIdAsync(Guid id);
    Task<List<InsuranceClaim>> GetByBillIdAsync(Guid billId);
    Task<List<InsuranceClaim>> GetByPatientInsuranceIdAsync(Guid patientInsuranceId);
    Task<List<InsuranceClaim>> GetByStatusAsync(ClaimStatus status);
    Task<List<InsuranceClaim>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<InsuranceClaim?> GetByClaimNumberAsync(string claimNumber);
    Task AddAsync(InsuranceClaim claim);
    Task UpdateAsync(InsuranceClaim claim);
    Task DeleteAsync(Guid id);
}

public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(Guid id);
    Task<List<Staff>> GetAllAsync();
    Task<List<Staff>> GetByRoleAsync(StaffRole role);
    Task<List<Staff>> GetByDepartmentAsync(Department department);
    Task<List<Staff>> GetActiveStaffAsync();
    Task<Staff?> GetByEmailAsync(string email);
    Task<Staff?> GetByEmployeeNumberAsync(string employeeNumber);
    // Returns true if the staff member has related domain data that should block hard deletion
    // (future relationships: treatment records, lab orders, authored notes, etc.)
    Task<bool> HasRelatedDataAsync(Guid id);
    Task AddAsync(Staff staff);
    Task UpdateAsync(Staff staff);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}

public interface ITreatmentRepository
{
    Task<Treatment?> GetByIdAsync(Guid id);
    Task<List<Treatment>> GetAllAsync();
    Task<List<Treatment>> GetByCategoryAsync(TreatmentCategory category);
    Task<List<Treatment>> GetActiveAsync();
    Task<Treatment?> GetByCodeAsync(string code);
    Task<List<Treatment>> SearchAsync(string searchTerm);
    Task AddAsync(Treatment treatment);
    Task UpdateAsync(Treatment treatment);
    Task DeleteAsync(Guid id);
}

public interface ITreatmentRecordRepository
{
    Task<TreatmentRecord?> GetByIdAsync(Guid id);
    Task<List<TreatmentRecord>> GetByMedicalRecordIdAsync(Guid medicalRecordId);
    Task<List<TreatmentRecord>> GetByPatientIdAsync(Guid patientId);
    Task<List<TreatmentRecord>> GetByTreatmentIdAsync(Guid treatmentId);
    Task<List<TreatmentRecord>> GetByProviderIdAsync(Guid providerId);
    Task<List<TreatmentRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<TreatmentRecord>> GetByStatusAsync(TreatmentStatus status);
    Task AddAsync(TreatmentRecord treatmentRecord);
    Task UpdateAsync(TreatmentRecord treatmentRecord);
    Task DeleteAsync(Guid id);
}

public interface ILabRepository
{
    Task<Lab?> GetByIdAsync(Guid id);
    Task<List<Lab>> GetAllAsync();
    Task<List<Lab>> GetActiveAsync();
    Task<Lab?> GetByCodeAsync(string code);
    Task<List<Lab>> SearchAsync(string searchTerm);
    Task AddAsync(Lab lab);
    Task UpdateAsync(Lab lab);
    Task DeleteAsync(Guid id);
}

public interface ILabTestRepository
{
    Task<LabTest?> GetByIdAsync(Guid id);
    Task<List<LabTest>> GetAllAsync();
    Task<List<LabTest>> GetByCategoryAsync(string category);
    Task<List<LabTest>> GetActiveAsync();
    Task<LabTest?> GetByCodeAsync(string code);
    Task<List<LabTest>> SearchAsync(string searchTerm);
    Task AddAsync(LabTest labTest);
    Task UpdateAsync(LabTest labTest);
    Task DeleteAsync(Guid id);
}

public interface ILabOrderRepository
{
    Task<LabOrder?> GetByIdAsync(Guid id);
    Task<List<LabOrder>> GetByPatientIdAsync(Guid patientId);
    Task<List<LabOrder>> GetByDoctorIdAsync(Guid doctorId);
    Task<List<LabOrder>> GetByLabIdAsync(Guid labId);
    Task<List<LabOrder>> GetByStatusAsync(LabOrderStatus status);
    Task<List<LabOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<LabOrder?> GetByOrderNumberAsync(string orderNumber);
    Task<List<LabOrder>> GetUrgentOrdersAsync();
    Task AddAsync(LabOrder labOrder);
    Task UpdateAsync(LabOrder labOrder);
    Task DeleteAsync(Guid id);
}

public interface ILabResultRepository
{
    Task<LabResult?> GetByIdAsync(Guid id);
    Task<List<LabResult>> GetByLabOrderTestIdAsync(Guid labOrderTestId);
    Task<List<LabResult>> GetByLabOrderIdAsync(Guid labOrderId);
    Task<List<LabResult>> GetByPatientIdAsync(Guid patientId);
    Task<List<LabResult>> GetByStatusAsync(LabResultStatus status);
    Task<List<LabResult>> GetAbnormalResultsAsync();
    Task<List<LabResult>> GetPendingReviewAsync();
    Task<List<LabResult>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddAsync(LabResult labResult);
    Task UpdateAsync(LabResult labResult);
    Task DeleteAsync(Guid id);
}

public interface IInsuranceProviderRepository
{
    Task<InsuranceProvider?> GetByIdAsync(Guid id);
    Task<List<InsuranceProvider>> GetAllAsync();
    Task<List<InsuranceProvider>> GetActiveAsync();
    Task<InsuranceProvider?> GetByCodeAsync(string code);
    Task<List<InsuranceProvider>> SearchAsync(string searchTerm);
    Task AddAsync(InsuranceProvider provider);
    Task UpdateAsync(InsuranceProvider provider);
    Task DeleteAsync(Guid id);
}

public interface IPatientInsuranceRepository
{
    Task<PatientInsurance?> GetByIdAsync(Guid id);
    Task<List<PatientInsurance>> GetByPatientIdAsync(Guid patientId);
    Task<List<PatientInsurance>> GetByInsuranceProviderIdAsync(Guid providerId);
    Task<PatientInsurance?> GetPrimaryInsuranceAsync(Guid patientId);
    Task<PatientInsurance?> GetSecondaryInsuranceAsync(Guid patientId);
    Task<List<PatientInsurance>> GetActiveInsuranceAsync(Guid patientId);
    Task<PatientInsurance?> GetByPolicyNumberAsync(string policyNumber);
    Task AddAsync(PatientInsurance patientInsurance);
    Task UpdateAsync(PatientInsurance patientInsurance);
    Task DeleteAsync(Guid id);
}
