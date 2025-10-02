using System.Reflection;
using CareSync.Application.Commands.Appointments;
using CareSync.Application.Common.Mapping;
using CareSync.Application.DTOs.Appointments;
using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace CareSync.Application.UnitTests.Services.Appointments;

public class UpsertAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
    private readonly Mock<IPatientRepository> _patientRepository = new();
    private readonly Mock<IDoctorRepository> _doctorRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AppointmentMapper _mapper = new();

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDoctorMissing()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _patientRepository.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(CreatePatient(patientId));
        _doctorRepository.Setup(r => r.GetByIdAsync(doctorId))
            .ReturnsAsync((Doctor?)null);

        var handler = CreateHandler();
        var dto = CreateUpsertDto(patientId, doctorId);

        // Act
        var result = await handler.Handle(new UpsertAppointmentCommand(dto), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Doctor not found or inactive.");
        _appointmentRepository.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDoctorInactive()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _patientRepository.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync(CreatePatient(patientId));
        _doctorRepository.Setup(r => r.GetByIdAsync(doctorId))
            .ReturnsAsync(CreateDoctor(doctorId, isActive: false));

        var handler = CreateHandler();
        var dto = CreateUpsertDto(patientId, doctorId);

        // Act
        var result = await handler.Handle(new UpsertAppointmentCommand(dto), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Doctor not found or inactive.");
        _appointmentRepository.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenPatientMissing()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _patientRepository.Setup(r => r.GetByIdAsync(patientId))
            .ReturnsAsync((Patient?)null);

        var handler = CreateHandler();
        var dto = CreateUpsertDto(patientId, doctorId);

        // Act
        var result = await handler.Handle(new UpsertAppointmentCommand(dto), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Patient not found or inactive.");
        _doctorRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _appointmentRepository.Verify(r => r.AddAsync(It.IsAny<Appointment>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private UpsertAppointmentCommandHandler CreateHandler() => new(
        _appointmentRepository.Object,
        _mapper,
        _unitOfWork.Object,
        _patientRepository.Object,
        _doctorRepository.Object);

    private static UpsertAppointmentDto CreateUpsertDto(Guid patientId, Guid doctorId) => new(
        Id: null,
        PatientId: patientId,
        DoctorId: doctorId,
        StartDateTime: DateTime.UtcNow.AddHours(1),
        DurationMinutes: 30,
        AppointmentType: "Consultation",
        Notes: null,
        ReasonForVisit: null);

    private static Patient CreatePatient(Guid patientId)
    {
        var patient = new Patient(
            new FullName("Jane", "Doe"),
            new Email("jane.doe@example.com"),
            new PhoneNumber("09171234567"),
            DateTime.UtcNow.AddYears(-30),
            "Female",
            "123 Main St",
            "01",
            "Metro Province",
            "0101",
            "Metro City",
            "1000",
            "010101",
            "Central Barangay");

        SetPrivateProperty(patient, nameof(Patient.Id), patientId);
        return patient;
    }

    private static Doctor CreateDoctor(Guid doctorId, bool isActive = true)
    {
        var doctor = new Doctor(
            doctorId,
            new FullName("Gregory", "House"),
            "Diagnostics",
            new PhoneNumber("09171234568"),
            new Email("doctor.house@example.com"));

        if (!isActive)
        {
            doctor.Deactivate();
        }

        return doctor;
    }

    private static void SetPrivateProperty<T>(T target, string propertyName, object value)
    {
        typeof(T)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
            .SetValue(target, value);
    }
}
