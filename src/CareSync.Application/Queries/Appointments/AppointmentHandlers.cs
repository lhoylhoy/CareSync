using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Appointments;
using CareSync.Domain.Interfaces;
using MediatR;

namespace CareSync.Application.Queries.Appointments;

public class GetAllAppointmentsHandler : IRequestHandler<GetAllAppointmentsQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppointmentMapper _mapper;

    public GetAllAppointmentsHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAllAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var list = new List<AppointmentDto>();
        foreach (var a in appointments)
        {
            var dto = _mapper.Map(a);
            bool hasRelated = await _appointmentRepository.HasRelatedDataAsync(a.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<AppointmentDto>>.Success(list);
    }
}

public class GetAppointmentByIdHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppointmentMapper _mapper;

    public GetAppointmentByIdHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(request.Id);
        if (appointment == null)
            return Result<AppointmentDto>.Failure("Appointment not found");
        var dto = _mapper.Map(appointment);
        bool hasRelated = await _appointmentRepository.HasRelatedDataAsync(appointment.Id);
        dto = dto with { HasRelatedData = hasRelated };
        return Result<AppointmentDto>.Success(dto);
    }
}

public class
    GetAppointmentsByPatientHandler : IRequestHandler<GetAppointmentsByPatientQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppointmentMapper _mapper;

    public GetAppointmentsByPatientHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAppointmentsByPatientQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var patientAppointments = appointments.Where(a => a.PatientId == request.PatientId);
        var list = new List<AppointmentDto>();
        foreach (var a in patientAppointments)
        {
            var dto = _mapper.Map(a);
            bool hasRelated = await _appointmentRepository.HasRelatedDataAsync(a.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<AppointmentDto>>.Success(list);
    }
}

public class GetAppointmentsByDoctorHandler : IRequestHandler<GetAppointmentsByDoctorQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppointmentMapper _mapper;

    public GetAppointmentsByDoctorHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAppointmentsByDoctorQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var doctorAppointments = appointments.Where(a => a.DoctorId == request.DoctorId);
        var list = new List<AppointmentDto>();
        foreach (var a in doctorAppointments)
        {
            var dto = _mapper.Map(a);
            bool hasRelated = await _appointmentRepository.HasRelatedDataAsync(a.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<AppointmentDto>>.Success(list);
    }
}

public class
    GetAppointmentsByDateRangeHandler : IRequestHandler<GetAppointmentsByDateRangeQuery, Result<IEnumerable<AppointmentDto>>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly AppointmentMapper _mapper;

    public GetAppointmentsByDateRangeHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AppointmentDto>>> Handle(GetAppointmentsByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var dateRangeAppointments =
            appointments.Where(a => a.ScheduledDate >= request.StartDate && a.ScheduledDate <= request.EndDate);
        var list = new List<AppointmentDto>();
        foreach (var a in dateRangeAppointments)
        {
            var dto = _mapper.Map(a);
            bool hasRelated = await _appointmentRepository.HasRelatedDataAsync(a.Id);
            dto = dto with { HasRelatedData = hasRelated };
            list.Add(dto);
        }
        return Result<IEnumerable<AppointmentDto>>.Success(list);
    }
}
