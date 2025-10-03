using System.Collections.Generic;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using MediatR;

namespace CareSync.Application.Queries.MedicalRecords;

public class GetMedicalRecordByIdQueryHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper)
    : IRequestHandler<GetMedicalRecordByIdQuery, Result<MedicalRecordDto>>
{
    public async Task<Result<MedicalRecordDto>> Handle(GetMedicalRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var medicalRecord = await medicalRecordRepository.GetByIdAsync(request.Id);
        if (medicalRecord == null) return Result<MedicalRecordDto>.Failure("Medical record not found");
        var dto = mapper.Map(medicalRecord);
        bool hasRelated = await medicalRecordRepository.HasRelatedDataAsync(medicalRecord.Id);
        dto = dto with { HasRelatedData = hasRelated };
        return Result<MedicalRecordDto>.Success(dto);
    }
}

public class GetMedicalRecordsByPatientQueryHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper)
    : IRequestHandler<GetMedicalRecordsByPatientQuery, Result<IEnumerable<MedicalRecordDto>>>
{
    public async Task<Result<IEnumerable<MedicalRecordDto>>> Handle(GetMedicalRecordsByPatientQuery request,
        CancellationToken cancellationToken)
    {
        var medicalRecords = await medicalRecordRepository.GetByPatientIdAsync(request.PatientId);
        var list = new List<MedicalRecordDto>();
        foreach (var mr in medicalRecords)
        {
            var dto = mapper.Map(mr);
            bool hasRelated = await medicalRecordRepository.HasRelatedDataAsync(mr.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<MedicalRecordDto>>.Success(list);
    }
}

public class GetMedicalRecordsByDoctorQueryHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper)
    : IRequestHandler<GetMedicalRecordsByDoctorQuery, Result<IEnumerable<MedicalRecordDto>>>
{
    public async Task<Result<IEnumerable<MedicalRecordDto>>> Handle(GetMedicalRecordsByDoctorQuery request,
        CancellationToken cancellationToken)
    {
        var medicalRecords = await medicalRecordRepository.GetByDoctorIdAsync(request.DoctorId);
        var list = new List<MedicalRecordDto>();
        foreach (var mr in medicalRecords)
        {
            var dto = mapper.Map(mr);
            bool hasRelated = await medicalRecordRepository.HasRelatedDataAsync(mr.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<MedicalRecordDto>>.Success(list);
    }
}

public class GetMedicalRecordsByDateRangeQueryHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper)
    : IRequestHandler<GetMedicalRecordsByDateRangeQuery, Result<IEnumerable<MedicalRecordDto>>>
{
    public async Task<Result<IEnumerable<MedicalRecordDto>>> Handle(GetMedicalRecordsByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        var medicalRecords = await medicalRecordRepository.GetByDateRangeAsync(request.StartDate, request.EndDate);
        var list = new List<MedicalRecordDto>();
        foreach (var mr in medicalRecords)
        {
            var dto = mapper.Map(mr);
            bool hasRelated = await medicalRecordRepository.HasRelatedDataAsync(mr.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<MedicalRecordDto>>.Success(list);
    }
}

public class GetAllMedicalRecordsQueryHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper)
    : IRequestHandler<GetAllMedicalRecordsQuery, Result<IEnumerable<MedicalRecordDto>>>
{
    public async Task<Result<IEnumerable<MedicalRecordDto>>> Handle(GetAllMedicalRecordsQuery request,
        CancellationToken cancellationToken)
    {
        var medicalRecords = await medicalRecordRepository.GetAllAsync();
        var list = new List<MedicalRecordDto>();
        foreach (var mr in medicalRecords)
        {
            var dto = mapper.Map(mr);
            bool hasRelated = await medicalRecordRepository.HasRelatedDataAsync(mr.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<MedicalRecordDto>>.Success(list);
    }
}

public class GetMedicalRecordsPagedQueryHandler(IMedicalRecordRepository medicalRecordRepository, MedicalRecordMapper mapper)
    : IRequestHandler<GetMedicalRecordsPagedQuery, Result<CareSync.Application.Common.PagedResult<MedicalRecordDto>>>
{
    public async Task<Result<CareSync.Application.Common.PagedResult<MedicalRecordDto>>> Handle(GetMedicalRecordsPagedQuery request,
        CancellationToken cancellationToken)
    {
        var filters = request.Filters ?? new Dictionary<string, string?>();
        var (items, totalCount) = await medicalRecordRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            filters,
            cancellationToken);

        var dtoList = new List<MedicalRecordDto>(items.Count);
        foreach (var record in items)
        {
            var dto = mapper.Map(record);
            var hasRelated = await medicalRecordRepository.HasRelatedDataAsync(record.Id);
            dtoList.Add(dto with { HasRelatedData = hasRelated });
        }

        var pagedResult = new CareSync.Application.Common.PagedResult<MedicalRecordDto>(
            dtoList,
            totalCount,
            request.Page,
            request.PageSize);

        return Result<CareSync.Application.Common.PagedResult<MedicalRecordDto>>.Success(pagedResult);
    }
}
