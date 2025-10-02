using System;
using CareSync.Application.Common.Mapping;
using CareSync.Domain.Entities;
using CareSync.Domain.ValueObjects;
using Xunit;

namespace CareSync.Application.UnitTests.Mapping;

public class StaffMapperTests
{
    private readonly StaffMapper _mapper = new();

    [Fact]
    public void Map_Should_Project_Core()
    {
        var staff = new Staff(Guid.NewGuid(), new FullName("Alex", "Mercer"), new Email("alex@example.com"), new PhoneNumber("09172222222"), role: CareSync.Domain.Enums.StaffRole.Nurse, department: "ER", employeeId: "EMP001", hireDate: DateTime.UtcNow.AddDays(-10));
        var dto = _mapper.Map(staff);
        Assert.Equal(staff.Id, dto.Id);
        Assert.Equal(CareSync.Domain.Enums.StaffRole.Nurse, dto.Role);
    }
}
