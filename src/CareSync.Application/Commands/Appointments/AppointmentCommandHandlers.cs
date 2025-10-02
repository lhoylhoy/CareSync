using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Application.DTOs.Appointments;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using MediatR;

namespace CareSync.Application.Commands.Appointments;

public class CreateAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<CreateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var durationMinutes = (int)(request.Appointment.Duration?.TotalMinutes ?? 30);
        var appointment = new Appointment(
            request.Appointment.PatientId,
            request.Appointment.DoctorId,
            request.Appointment.StartDateTime,
            durationMinutes,
            "Consultation" // Default appointment type
        );

        await appointmentRepository.AddAsync(appointment);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(mapper.Map(appointment));
    }
}

public class UpdateAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpdateAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.Appointment.Id);
        if (appointment == null)
            return Result<AppointmentDto>.Failure("Appointment not found");

        appointment.Reschedule(request.Appointment.StartDateTime);
        appointment.UpdateDuration((int)request.Appointment.Duration.TotalMinutes);
        if (!string.IsNullOrEmpty(request.Appointment.Notes))
            appointment.UpdateNotes(request.Appointment.Notes);

        await uow.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(mapper.Map(appointment));
    }
}

public class UpsertAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpsertAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(UpsertAppointmentCommand request, CancellationToken cancellationToken)
    {
        if (request.Appointment.Id.HasValue)
        {
            // Update existing
            var existingAppointment = await appointmentRepository.GetByIdAsync(request.Appointment.Id.Value);
            if (existingAppointment == null)
                return Result<AppointmentDto>.Failure("Appointment not found");

            existingAppointment.Reschedule(request.Appointment.StartDateTime);
            existingAppointment.UpdateDuration(request.Appointment.DurationMinutes);

            if (!string.IsNullOrEmpty(request.Appointment.AppointmentType))
            {
                // Note: Appointment entity doesn't have a method to update appointment type
                // This would require recreating the appointment or adding a method to the entity
            }

            if (!string.IsNullOrEmpty(request.Appointment.Notes))
                existingAppointment.UpdateNotes(request.Appointment.Notes);

            if (!string.IsNullOrEmpty(request.Appointment.ReasonForVisit))
                existingAppointment.UpdateReasonForVisit(request.Appointment.ReasonForVisit);

            await uow.SaveChangesAsync(cancellationToken);

            return Result<AppointmentDto>.Success(mapper.Map(existingAppointment));
        }

        // Create new
        var appointment = new Appointment(
            request.Appointment.PatientId,
            request.Appointment.DoctorId,
            request.Appointment.StartDateTime,
            request.Appointment.DurationMinutes,
            request.Appointment.AppointmentType ?? "Consultation"
        );

        if (!string.IsNullOrEmpty(request.Appointment.Notes))
            appointment.UpdateNotes(request.Appointment.Notes);

        if (!string.IsNullOrEmpty(request.Appointment.ReasonForVisit))
            appointment.UpdateReasonForVisit(request.Appointment.ReasonForVisit);

        await appointmentRepository.AddAsync(appointment);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(mapper.Map(appointment));
    }
}

public class CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<CancelAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.Appointment.Id);
        if (appointment == null)
            return Result<AppointmentDto>.Failure("Appointment not found");

        appointment.Cancel(request.Appointment.Reason);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(mapper.Map(appointment));
    }
}

public class CompleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<CompleteAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
        if (appointment == null)
            return Result<AppointmentDto>.Failure("Appointment not found");

        appointment.Complete(request.Notes);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<AppointmentDto>.Success(mapper.Map(appointment));
    }
}

public class StartAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<StartAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(StartAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
        if (appointment == null)
            return Result<AppointmentDto>.Failure("Appointment not found");

        try
        {
            appointment.Start();
            await uow.SaveChangesAsync(cancellationToken);
            return Result<AppointmentDto>.Success(mapper.Map(appointment));
        }
        catch (Exception ex)
        {
            return Result<AppointmentDto>.Failure(ex.Message);
        }
    }
}

public class MarkNoShowAppointmentCommandHandler(IAppointmentRepository appointmentRepository, AppointmentMapper mapper, IUnitOfWork uow)
    : IRequestHandler<MarkNoShowAppointmentCommand, Result<AppointmentDto>>
{
    public async Task<Result<AppointmentDto>> Handle(MarkNoShowAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
        if (appointment == null)
            return Result<AppointmentDto>.Failure("Appointment not found");

        try
        {
            appointment.MarkAsNoShow();
            await uow.SaveChangesAsync(cancellationToken);
            return Result<AppointmentDto>.Success(mapper.Map(appointment));
        }
        catch (Exception ex)
        {
            return Result<AppointmentDto>.Failure(ex.Message);
        }
    }
}

public class DeleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IUnitOfWork uow)
    : IRequestHandler<DeleteAppointmentCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
        if (appointment == null)
            return Result<Unit>.Failure("Appointment not found");

        if (await appointmentRepository.HasRelatedDataAsync(request.AppointmentId))
        {
            return Result<Unit>.Failure("Cannot delete appointment with related medical records. Cancel instead.");
        }

        await appointmentRepository.DeleteAsync(appointment.Id);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
