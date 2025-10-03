using CareSync.Application.Common.Mapping;
using CareSync.Application.DTOs.Staff;
using CareSync.Application.Queries.Staff;
using CareSync.Application.QueryHandlers.Staff;
using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace CareSync.Application.UnitTests.Services.StaffTests;

public class GetStaffByRoleQueryHandlerTests
{
    private readonly StaffMapper _mapper = new();
    private readonly Mock<IStaffRepository> _repo = new();

    private static Staff CreateStaff(StaffRole role, Department department) => new(
        Guid.NewGuid(),
        new FullName("Jane", "Doe"),
        new Email("jane.doe@example.com"),
        new PhoneNumber("09123456789"),
        role,
        department,
        "EMP-123",
        DateTime.UtcNow.AddMonths(-3)
    );

    [Fact]
    public async Task Handle_ReturnsStaffForRole()
    {
        var staffList = new List<Staff>
        {
            CreateStaff(StaffRole.Administrator, Department.Administration),
            CreateStaff(StaffRole.Administrator, Department.Administration)
        };
        _repo.Setup(r => r.GetByRoleAsync(StaffRole.Administrator)).ReturnsAsync(staffList);

        var handler = new GetStaffByRoleQueryHandler(_repo.Object, _mapper);
        var result = await handler.Handle(new GetStaffByRoleQuery(StaffRole.Administrator), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(2);
        result.Value!.All(s => s.Role == StaffRole.Administrator).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsSuccessWithEmpty()
    {
        _repo.Setup(r => r.GetByRoleAsync(StaffRole.Nurse)).ReturnsAsync(new List<Staff>());
        var handler = new GetStaffByRoleQueryHandler(_repo.Object, _mapper);
        var result = await handler.Handle(new GetStaffByRoleQuery(StaffRole.Nurse), CancellationToken.None);
        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().BeEmpty();
    }
}
