using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Patients;
using CareSync.Domain.Interfaces;
using MediatR;

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

public class GetPatientsPagedQueryHandler(IPatientRepository patientRepository, PatientMapper mapper)
    : IRequestHandler<GetPatientsPagedQuery, Result<CareSync.Application.Common.PagedResult<PatientDto>>>
{
    public async Task<Result<CareSync.Application.Common.PagedResult<PatientDto>>> Handle(GetPatientsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var filters = request.Filters ?? new Dictionary<string, string?>();
        var (items, totalCount) = await patientRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            filters,
            cancellationToken);

        var dtoList = new List<PatientDto>(items.Count);
        foreach (var patient in items)
        {
            var dto = mapper.Map(patient);
            bool hasRelated = await patientRepository.HasRelatedDataAsync(patient.Id);
            dtoList.Add(dto with { HasRelatedData = hasRelated });
        }

        var pagedResult = new CareSync.Application.Common.PagedResult<PatientDto>(
            dtoList,
            totalCount,
            request.Page,
            request.PageSize);

        return Result<CareSync.Application.Common.PagedResult<PatientDto>>.Success(pagedResult);
    }
}
