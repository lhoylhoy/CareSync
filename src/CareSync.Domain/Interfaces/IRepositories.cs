using CareSync.Domain.Entities;
using CareSync.Domain.Enums;

namespace CareSync.Domain.Interfaces;

public interface IBillRepository
{
    public Task<Bill?> GetByIdAsync(Guid id);
    public Task<List<Bill>> GetAllAsync();
    public Task<int> GetTotalCountAsync();
    public Task<List<Bill>> GetByPatientIdAsync(Guid patientId);
    public Task<List<Bill>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<List<Bill>> GetOutstandingBillsAsync();
    public Task<List<Bill>> GetOverdueBillsAsync();
    public Task<Bill?> GetByBillNumberAsync(string billNumber);
    public Task<bool> HasRelatedDataAsync(Guid id); // payments or claims
    public Task AddAsync(Bill bill);
    public Task UpdateAsync(Bill bill);
    public Task DeleteAsync(Guid id);
    public Task SaveChangesAsync();
}

public interface IPaymentRepository
{
    public Task<Payment?> GetByIdAsync(Guid id);
    public Task<List<Payment>> GetByBillIdAsync(Guid billId);
    public Task<List<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

    public Task<List<Payment>> GetByPaymentMethodAsync(PaymentMethod method, DateTime? startDate = null,
        DateTime? endDate = null);

    public Task AddAsync(Payment payment);
    public Task UpdateAsync(Payment payment);
    public Task DeleteAsync(Guid id);
}

public interface IInsuranceClaimRepository
{
    public Task<InsuranceClaim?> GetByIdAsync(Guid id);
    public Task<List<InsuranceClaim>> GetByBillIdAsync(Guid billId);
    public Task<List<InsuranceClaim>> GetByPatientInsuranceIdAsync(Guid patientInsuranceId);
    public Task<List<InsuranceClaim>> GetByStatusAsync(ClaimStatus status);
    public Task<List<InsuranceClaim>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<InsuranceClaim?> GetByClaimNumberAsync(string claimNumber);
    public Task AddAsync(InsuranceClaim claim);
    public Task UpdateAsync(InsuranceClaim claim);
    public Task DeleteAsync(Guid id);
}

public interface IStaffRepository
{
    public Task<Staff?> GetByIdAsync(Guid id);
    public Task<List<Staff>> GetAllAsync();
    public Task<List<Staff>> GetByRoleAsync(StaffRole role);
    public Task<List<Staff>> GetByDepartmentAsync(Department department);
    public Task<List<Staff>> GetActiveStaffAsync();
    public Task<Staff?> GetByEmailAsync(string email);
    public Task<Staff?> GetByEmployeeNumberAsync(string employeeNumber);
    // Returns true if the staff member has related domain data that should block hard deletion
    // (future relationships: treatment records, lab orders, authored notes, etc.)
    public Task<bool> HasRelatedDataAsync(Guid id);
    public Task AddAsync(Staff staff);
    public Task UpdateAsync(Staff staff);
    public Task DeleteAsync(Guid id);
    public Task SaveChangesAsync();
}

public interface ITreatmentRepository
{
    public Task<Treatment?> GetByIdAsync(Guid id);
    public Task<List<Treatment>> GetAllAsync();
    public Task<List<Treatment>> GetByCategoryAsync(TreatmentCategory category);
    public Task<List<Treatment>> GetActiveAsync();
    public Task<Treatment?> GetByCodeAsync(string code);
    public Task<List<Treatment>> SearchAsync(string searchTerm);
    public Task AddAsync(Treatment treatment);
    public Task UpdateAsync(Treatment treatment);
    public Task DeleteAsync(Guid id);
}

public interface ITreatmentRecordRepository
{
    public Task<TreatmentRecord?> GetByIdAsync(Guid id);
    public Task<List<TreatmentRecord>> GetByMedicalRecordIdAsync(Guid medicalRecordId);
    public Task<List<TreatmentRecord>> GetByPatientIdAsync(Guid patientId);
    public Task<List<TreatmentRecord>> GetByTreatmentIdAsync(Guid treatmentId);
    public Task<List<TreatmentRecord>> GetByProviderIdAsync(Guid providerId);
    public Task<List<TreatmentRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<List<TreatmentRecord>> GetByStatusAsync(TreatmentStatus status);
    public Task AddAsync(TreatmentRecord treatmentRecord);
    public Task UpdateAsync(TreatmentRecord treatmentRecord);
    public Task DeleteAsync(Guid id);
}

public interface ILabRepository
{
    public Task<Lab?> GetByIdAsync(Guid id);
    public Task<List<Lab>> GetAllAsync();
    public Task<List<Lab>> GetActiveAsync();
    public Task<Lab?> GetByCodeAsync(string code);
    public Task<List<Lab>> SearchAsync(string searchTerm);
    public Task AddAsync(Lab lab);
    public Task UpdateAsync(Lab lab);
    public Task DeleteAsync(Guid id);
}

public interface ILabTestRepository
{
    public Task<LabTest?> GetByIdAsync(Guid id);
    public Task<List<LabTest>> GetAllAsync();
    public Task<List<LabTest>> GetByCategoryAsync(string category);
    public Task<List<LabTest>> GetActiveAsync();
    public Task<LabTest?> GetByCodeAsync(string code);
    public Task<List<LabTest>> SearchAsync(string searchTerm);
    public Task AddAsync(LabTest labTest);
    public Task UpdateAsync(LabTest labTest);
    public Task DeleteAsync(Guid id);
}

public interface ILabOrderRepository
{
    public Task<LabOrder?> GetByIdAsync(Guid id);
    public Task<List<LabOrder>> GetByPatientIdAsync(Guid patientId);
    public Task<List<LabOrder>> GetByDoctorIdAsync(Guid doctorId);
    public Task<List<LabOrder>> GetByLabIdAsync(Guid labId);
    public Task<List<LabOrder>> GetByStatusAsync(LabOrderStatus status);
    public Task<List<LabOrder>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task<LabOrder?> GetByOrderNumberAsync(string orderNumber);
    public Task<List<LabOrder>> GetUrgentOrdersAsync();
    public Task AddAsync(LabOrder labOrder);
    public Task UpdateAsync(LabOrder labOrder);
    public Task DeleteAsync(Guid id);
}

public interface ILabResultRepository
{
    public Task<LabResult?> GetByIdAsync(Guid id);
    public Task<List<LabResult>> GetByLabOrderTestIdAsync(Guid labOrderTestId);
    public Task<List<LabResult>> GetByLabOrderIdAsync(Guid labOrderId);
    public Task<List<LabResult>> GetByPatientIdAsync(Guid patientId);
    public Task<List<LabResult>> GetByStatusAsync(LabResultStatus status);
    public Task<List<LabResult>> GetAbnormalResultsAsync();
    public Task<List<LabResult>> GetPendingReviewAsync();
    public Task<List<LabResult>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    public Task AddAsync(LabResult labResult);
    public Task UpdateAsync(LabResult labResult);
    public Task DeleteAsync(Guid id);
}

public interface IInsuranceProviderRepository
{
    public Task<InsuranceProvider?> GetByIdAsync(Guid id);
    public Task<List<InsuranceProvider>> GetAllAsync();
    public Task<List<InsuranceProvider>> GetActiveAsync();
    public Task<InsuranceProvider?> GetByCodeAsync(string code);
    public Task<List<InsuranceProvider>> SearchAsync(string searchTerm);
    public Task AddAsync(InsuranceProvider provider);
    public Task UpdateAsync(InsuranceProvider provider);
    public Task DeleteAsync(Guid id);
}

public interface IPatientInsuranceRepository
{
    public Task<PatientInsurance?> GetByIdAsync(Guid id);
    public Task<List<PatientInsurance>> GetByPatientIdAsync(Guid patientId);
    public Task<List<PatientInsurance>> GetByInsuranceProviderIdAsync(Guid providerId);
    public Task<PatientInsurance?> GetPrimaryInsuranceAsync(Guid patientId);
    public Task<PatientInsurance?> GetSecondaryInsuranceAsync(Guid patientId);
    public Task<List<PatientInsurance>> GetActiveInsuranceAsync(Guid patientId);
    public Task<PatientInsurance?> GetByPolicyNumberAsync(string policyNumber);
    public Task AddAsync(PatientInsurance patientInsurance);
    public Task UpdateAsync(PatientInsurance patientInsurance);
    public Task DeleteAsync(Guid id);
}
