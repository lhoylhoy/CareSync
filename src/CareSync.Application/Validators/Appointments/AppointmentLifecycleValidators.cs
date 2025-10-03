using CareSync.Application.Commands.Appointments;
using FluentValidation;

namespace CareSync.Application.Validators.Appointments;

public class StartAppointmentCommandValidator : AbstractValidator<StartAppointmentCommand>
{
    public StartAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
    }
}

public class MarkNoShowAppointmentCommandValidator : AbstractValidator<MarkNoShowAppointmentCommand>
{
    public MarkNoShowAppointmentCommandValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
    }
}
