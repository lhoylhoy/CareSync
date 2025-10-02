using System;
using System.Collections.Generic;
using CareSync.Application.Common.Mapping;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using Xunit;

namespace CareSync.Application.UnitTests.Mapping;

public class MedicalRecordMapperTests
{
    private readonly MedicalRecordMapper _mapper = new();

    [Fact]
    public void Map_Should_Map_Core_Fields()
    {
        var record = BuildSampleRecord();

        var dto = _mapper.Map(record);

        Assert.Equal(record.Id, dto.Id);
        Assert.Equal(record.PatientId, dto.PatientId);
        Assert.Equal(record.DoctorId, dto.DoctorId);
        Assert.Equal(record.ChiefComplaint, dto.ChiefComplaint);
        Assert.Equal(record.IsFinalized, dto.IsFinalized);
        Assert.Equal(record.RecordDate, dto.RecordDate);
    }

    [Fact]
    public void Map_Should_Map_VitalSigns_With_Oxygen_Rounding()
    {
        var record = BuildSampleRecord();
        record.AddVitalSigns(temperature: 37.2m, systolicBp: 120, diastolicBp: 80, heartRate: 70, respiratoryRate: 16, weight: 70m, height: 175m, oxygenSaturation: 97.6m, notes: "stable");

        var dto = _mapper.Map(record);

        Assert.Single(dto.VitalSigns);
        var mapped = dto.VitalSigns[0];
        Assert.Equal((int?)98, mapped.OxygenSaturation); // 97.6 rounds to 98
    }

    [Fact]
    public void Map_Should_Map_Diagnoses_And_Prescriptions()
    {
        var record = BuildSampleRecord();
        record.AddDiagnosis("A00", "Cholera", description: "Infection", isPrimary: true);
        record.AddPrescription("Med", "500mg", "BID", "Take with water", startDate: DateTime.UtcNow, endDate: DateTime.UtcNow.AddDays(7), durationDays: 7);

        var dto = _mapper.Map(record);

        Assert.Single(dto.Diagnoses);
        Assert.Single(dto.Prescriptions);
        Assert.Equal("A00", dto.Diagnoses[0].Code);
        Assert.Equal("Med", dto.Prescriptions[0].MedicationName);
    }

    private static MedicalRecord BuildSampleRecord()
    {
        var record = new MedicalRecord(Guid.NewGuid(), Guid.NewGuid(), "Cough", null, null);
        return record;
    }
}
