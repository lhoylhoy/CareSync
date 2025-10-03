using System.Threading;
using System.Threading.Tasks;
using CareSync.Application.Commands.Patients;
using CareSync.Application.Common.Geographics;
using CareSync.Application.Common.Mapping;
using CareSync.Application.DTOs.Patients;
using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using Moq;
using Xunit;

namespace CareSync.Application.UnitTests.Services.Patients;

public class PatientCommandHandlerTests
{
    private readonly Mock<IPatientRepository> _patientRepo = new();
    private readonly Mock<IPhilippineGeographicDataService> _geo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    public PatientCommandHandlerTests()
    {
        _geo.Setup(g => g.GetProvinceByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string code, CancellationToken _) => new ProvinceDto(code, "ProvName", "Region"));
        _geo.Setup(g => g.GetCityByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string code, CancellationToken _) => new CityDto(code, "CityName", "PCode", "PName", "1000"));
        _geo.Setup(g => g.GetBarangayByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string code, CancellationToken _) => new BarangayDto(code, "Brgy", "CityCode", "CityName", "ProvCode", "ProvName", "1000"));
    }

    [Fact]
    public async Task CreatePatientCommand_Should_ReturnPatientDto()
    {
        // Arrange
        var handler = new CreatePatientCommandHandler(_patientRepo.Object, _geo.Object, new PatientMapper(), _uow.Object);
        var create = new CreatePatientDto(
            FirstName: "John",
            MiddleName: null,
            LastName: "Doe",
            Street: "123 Street",
            ProvinceCode: "0101",
            CityCode: "0101010",
            BarangayCode: "010101001",
            CityZipCode: "1000",
            DateOfBirth: new DateTime(1990, 1, 1),
            Gender: "Male",
            PhoneNumber: "09171234567",
            Email: "john@example.com",
            EmergencyContactName: null,
            EmergencyContactNumber: null,
            PhilHealthNumber: null,
            SssNumber: null,
            Tin: null
        );
        var command = new CreatePatientCommand(create);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("John", result.Value!.FirstName);
        _patientRepo.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
