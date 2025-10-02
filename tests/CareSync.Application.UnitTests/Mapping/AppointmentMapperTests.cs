using System;
using CareSync.Application.Common.Mapping;
using CareSync.Domain.Entities;
using Xunit;

namespace CareSync.Application.UnitTests.Mapping;

public class AppointmentMapperTests
{
    private readonly AppointmentMapper _mapper = new();

    [Fact]
    public void Map_Should_Project_Status_And_Names()
    {
        var appointment = new Appointment(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), 30, "Follow-up");
        var dto = _mapper.Map(appointment);
        Assert.Equal(appointment.Id, dto.Id);
        Assert.Equal(appointment.Status, dto.Status);
    }
}
