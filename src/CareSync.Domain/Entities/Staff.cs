using CareSync.Domain.Enums;
using CareSync.Domain.ValueObjects;

namespace CareSync.Domain.Entities;

public class Staff
{
    private Staff()
    {
        Name = null!;
        Email = null!;
        PhoneNumber = null!;
    }

    public Staff(Guid id, FullName name, Email email, PhoneNumber phoneNumber,
        StaffRole role, Department department, string employeeId, DateTime hireDate)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        Role = role;
        Department = department;
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        HireDate = hireDate;
        IsActive = true;
    }

    public Guid Id { get; private set; }
    public FullName Name { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public StaffRole Role { get; private set; }
    public Department Department { get; private set; }
    public string EmployeeId { get; private set; } = string.Empty;
    public DateTime HireDate { get; }
    public DateTime? TerminationDate { get; private set; }
    public bool IsActive { get; private set; }
    public decimal? Salary { get; private set; }
    public string? Notes { get; private set; }

    public TimeSpan TenureLength => (TerminationDate ?? DateTime.UtcNow) - HireDate;

    public void UpdateContactInformation(Email email, PhoneNumber phoneNumber)
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }

    public void UpdateRole(StaffRole role, Department department)
    {
        Role = role;
        Department = department;
    }

    public void SetSalary(decimal salary)
    {
        if (salary < 0)
            throw new ArgumentException("Salary cannot be negative", nameof(salary));

        Salary = salary;
    }

    public void Terminate(DateTime terminationDate, string? reason = null)
    {
        if (terminationDate < HireDate)
            throw new ArgumentException("Termination date cannot be before hire date", nameof(terminationDate));

        TerminationDate = terminationDate;
        IsActive = false;
        Notes = reason;
    }

    public void Reactivate()
    {
        TerminationDate = null;
        IsActive = true;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }
}
