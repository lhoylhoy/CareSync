using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Application.Common.Results;
using MediatR;

namespace CareSync.Application.Queries.MedicalRecords;

public record GetMedicalRecordByIdQuery(Guid Id) : IRequest<Result<MedicalRecordDto>>;

public record GetMedicalRecordsByPatientQuery(Guid PatientId) : IRequest<Result<IEnumerable<MedicalRecordDto>>>;

public record GetMedicalRecordsByDoctorQuery(Guid DoctorId) : IRequest<Result<IEnumerable<MedicalRecordDto>>>;

public record GetMedicalRecordsByDateRangeQuery(DateTime StartDate, DateTime EndDate)
    : IRequest<Result<IEnumerable<MedicalRecordDto>>>;

public record GetAllMedicalRecordsQuery : IRequest<Result<IEnumerable<MedicalRecordDto>>>;
