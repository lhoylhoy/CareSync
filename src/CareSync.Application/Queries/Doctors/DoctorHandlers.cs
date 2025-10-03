using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Doctors;
using CareSync.Domain.Interfaces;
using MediatR;

namespace CareSync.Application.Queries.Doctors;

public class GetDoctorByIdHandler : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly DoctorMapper _mapper;

    public GetDoctorByIdHandler(IDoctorRepository doctorRepository, DoctorMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetByIdAsync(request.DoctorId);

        if (doctor == null)
            return Result<DoctorDto>.Failure("Doctor not found");

        var dto = _mapper.Map(doctor);
        var hasRelated = await _doctorRepository.HasRelatedDataAsync(doctor.Id);
        dto = dto with { HasRelatedData = hasRelated };
        return Result<DoctorDto>.Success(dto);
    }
}

public class GetAllDoctorsHandler : IRequestHandler<GetAllDoctorsQuery, Result<IEnumerable<DoctorDto>>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly DoctorMapper _mapper;

    public GetAllDoctorsHandler(IDoctorRepository doctorRepository, DoctorMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<DoctorDto>>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _doctorRepository.GetAllAsync();
        var list = new List<DoctorDto>();
        foreach (var d in doctors)
        {
            var dto = _mapper.Map(d);
            var hasRelated = await _doctorRepository.HasRelatedDataAsync(d.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<DoctorDto>>.Success(list);
    }
}
