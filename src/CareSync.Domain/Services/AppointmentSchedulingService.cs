using CareSync.Domain.Entities;
using CareSync.Domain.Enums;
using CareSync.Domain.Interfaces;

namespace CareSync.Domain.Services;

public class AppointmentSchedulingService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;

    public AppointmentSchedulingService(IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<bool> IsDoctorAvailableAsync(Guid doctorId, DateTime startTime, TimeSpan duration)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor is null || !doctor.IsActive)
            return false;

        var endTime = startTime.Add(duration);
        var doctorAppointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);

        var scheduledAppointments = doctorAppointments
            .Where(a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.InProgress)
            .Where(a => a.ScheduledDate.Date == startTime.Date);

        foreach (var appointment in scheduledAppointments)
        {
            var appointmentEnd = appointment.EndTime;

            // Check for overlap
            if (startTime < appointmentEnd && endTime > appointment.ScheduledDate)
                return false;
        }

        return IsWithinWorkingHours(startTime, duration);
    }

    public async Task<Appointment> ScheduleAppointmentAsync(Guid patientId, Guid doctorId, DateTime scheduledDate,
        TimeSpan? duration = null)
    {
        var appointmentDuration = duration ?? TimeSpan.FromMinutes(30);

        if (!await IsDoctorAvailableAsync(doctorId, scheduledDate, appointmentDuration))
            throw new InvalidOperationException("Doctor is not available at the requested time.");

        var appointment = new Appointment(Guid.NewGuid(), patientId, doctorId, scheduledDate, appointmentDuration);
        await _appointmentRepository.AddAsync(appointment);

        return appointment;
    }

    public async Task<bool> CanRescheduleAsync(Guid appointmentId, DateTime newScheduledDate)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
        if (appointment is null)
            return false;

        if (!appointment.CanBeModified)
            return false;

        return await IsDoctorAvailableAsync(appointment.DoctorId, newScheduledDate, appointment.Duration);
    }

    private static bool IsWithinWorkingHours(DateTime startTime, TimeSpan duration)
    {
        var endTime = startTime.Add(duration);
        var workingHoursStart = new TimeSpan(8, 0, 0); // 8 AM
        var workingHoursEnd = new TimeSpan(17, 0, 0); // 5 PM

        // Check if it's a weekend
        if (startTime.DayOfWeek == DayOfWeek.Saturday || startTime.DayOfWeek == DayOfWeek.Sunday)
            return false;

        // Check if within working hours
        return startTime.TimeOfDay >= workingHoursStart &&
               endTime.TimeOfDay <= workingHoursEnd;
    }
}
