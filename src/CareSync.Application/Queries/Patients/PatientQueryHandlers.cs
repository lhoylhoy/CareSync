using CareSync.Domain.Interfaces;
using CareSync.Application.DTOs.Patients;
using CareSync.Application.Common.Mapping;
using MediatR;
using CareSync.Application.Common.Results;

namespace CareSync.Application.Queries.Patients;

// Query handlers for patient operations
public class GetAllPatientsQueryHandler(IPatientRepository patientRepository, PatientMapper mapper)
    : IRequestHandler<GetAllPatientsQuery, Result<IEnumerable<PatientDto>>>
{
    public async Task<Result<IEnumerable<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await patientRepository.GetAllAsync();
        var list = new List<PatientDto>();
        foreach (var p in patients)
        {
            var dto = mapper.Map(p);
            bool hasRelated = await patientRepository.HasRelatedDataAsync(p.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<PatientDto>>.Success(list);
    }
}

public class GetPatientByIdQueryHandler(IPatientRepository patientRepository, PatientMapper mapper)
    : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(request.PatientId);
        if (patient == null)
            return Result<PatientDto>.Failure("Patient not found");
        var dto = mapper.Map(patient);
        var hasRelated = await patientRepository.HasRelatedDataAsync(patient.Id);
        dto = dto with { HasRelatedData = hasRelated };
        return Result<PatientDto>.Success(dto);
    }
}
