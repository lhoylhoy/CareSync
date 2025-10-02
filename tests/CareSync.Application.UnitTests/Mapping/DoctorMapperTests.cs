using System;
using CareSync.Application.Common.Mapping;
using CareSync.Domain.Entities;
using CareSync.Domain.ValueObjects;
using Xunit;

namespace CareSync.Application.UnitTests.Mapping;

public class DoctorMapperTests
{
    private readonly DoctorMapper _mapper = new();

    [Fact]
    public void Map_Should_Set_DisplayName()
    {
        var doctor = new Doctor(Guid.NewGuid(), new FullName("Gregory", "House"), "Diagnostics", new PhoneNumber("09171111111"), new Email("house@example.com"));
        var dto = _mapper.Map(doctor);
        Assert.Equal(doctor.DisplayName, dto.DisplayName);
        Assert.Equal("Diagnostics", dto.Specialty);
    }
}
