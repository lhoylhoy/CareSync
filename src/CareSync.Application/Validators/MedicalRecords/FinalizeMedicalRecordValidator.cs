using CareSync.Application.Commands.MedicalRecords;
using CareSync.Application.DTOs.MedicalRecords;
using FluentValidation;

namespace CareSync.Application.Validators.MedicalRecords;

public class FinalizeMedicalRecordDtoValidator : AbstractValidator<FinalizeMedicalRecordDto>
{
    public FinalizeMedicalRecordDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FinalNotes).MaximumLength(1000)
            .WithMessage("Final notes cannot exceed 1000 characters");
        RuleFor(x => x.FinalizedBy).MaximumLength(200);
    }
}

public class FinalizeMedicalRecordCommandValidator : AbstractValidator<FinalizeMedicalRecordCommand>
{
    public FinalizeMedicalRecordCommandValidator()
    {
        RuleFor(x => x.MedicalRecordId).NotEmpty();
        RuleFor(x => x.FinalNotes).MaximumLength(1000);
        RuleFor(x => x.FinalizedBy).MaximumLength(200);
    }
}

public class ReopenMedicalRecordCommandValidator : AbstractValidator<ReopenMedicalRecordCommand>
{
    public ReopenMedicalRecordCommandValidator()
    {
        RuleFor(x => x.MedicalRecordId).NotEmpty();
    }
}
