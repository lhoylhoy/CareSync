using System;
using CareSync.Application.Common.Mapping;
using CareSync.Domain.Entities;
using CareSync.Domain.ValueObjects;
using Xunit;

namespace CareSync.Application.UnitTests.Mapping;

public class PatientMapperTests
{
    private readonly PatientMapper _mapper = new();

    [Fact]
    public void Map_Should_Project_Core_Fields()
    {
        var patient = new Patient(
            new FullName("Jane", "Doe", "M"),
            new Email("jane@example.com"),
            new PhoneNumber("09170000000"),
            new DateTime(1992, 5, 1),
            "Female",
            street: "123 Road",
            provinceCode: "01",
            provinceName: "Prov",
            cityCode: "0101",
            cityName: "City",
            cityZipCode: "1000",
            barangayCode: "010101",
            barangayName: "Barangay");

        var dto = _mapper.Map(patient);

        Assert.Equal(patient.Id, dto.Id);
        Assert.Equal("Jane", dto.FirstName);
        Assert.Equal("Prov", dto.ProvinceName);
    }
}
