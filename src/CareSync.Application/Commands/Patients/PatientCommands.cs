using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Patients;
using FluentValidation;
using MediatR;

namespace CareSync.Application.Commands.Patients;

public record CreatePatientCommand(CreatePatientDto Patient) : IRequest<Result<PatientDto>>;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        RuleFor(c => c.Patient.FirstName).NotEmpty();
        RuleFor(c => c.Patient.LastName).NotEmpty();
        RuleFor(c => c.Patient.Gender).NotEmpty();
        RuleFor(c => c.Patient.ProvinceCode).NotEmpty();
        RuleFor(c => c.Patient.CityCode).NotEmpty();
        RuleFor(c => c.Patient.BarangayCode).NotEmpty();
        RuleFor(c => c.Patient.CityZipCode).NotEmpty();

        RuleFor(c => c.Patient.Gender)
            .Must(g => new[] { "Male", "Female", "M", "F", "Other" }.Contains(g, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Gender must be Male, Female, M, F, or Other");
    }
}

public record UpdatePatientCommand(UpdatePatientDto Patient) : IRequest<Result<PatientDto>>;

public record UpsertPatientCommand(UpsertPatientDto Patient) : IRequest<Result<PatientDto>>;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(c => c.Patient.Id).NotEmpty();
        RuleFor(c => c.Patient.FirstName).NotEmpty();
        RuleFor(c => c.Patient.LastName).NotEmpty();
        RuleFor(c => c.Patient.Gender).NotEmpty();
        RuleFor(c => c.Patient.ProvinceCode).NotEmpty();
        RuleFor(c => c.Patient.CityCode).NotEmpty();
        RuleFor(c => c.Patient.BarangayCode).NotEmpty();
        RuleFor(c => c.Patient.CityZipCode).NotEmpty();
        RuleFor(c => c.Patient.Gender)
            .Must(g => new[] { "Male", "Female", "M", "F", "Other" }.Contains(g, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Gender must be Male, Female, M, F, or Other");
    }
}

public class UpsertPatientCommandValidator : AbstractValidator<UpsertPatientCommand>
{
    public UpsertPatientCommandValidator()
    {
        // If Id present, must not be empty Guid
        RuleFor(c => c.Patient.Id)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Id, if provided, must be a non-empty GUID");

        RuleFor(c => c.Patient.FirstName).NotEmpty();
        RuleFor(c => c.Patient.LastName).NotEmpty();
        RuleFor(c => c.Patient.Gender).NotEmpty();
        RuleFor(c => c.Patient.ProvinceCode).NotEmpty();
        RuleFor(c => c.Patient.CityCode).NotEmpty();
        RuleFor(c => c.Patient.BarangayCode).NotEmpty();
        RuleFor(c => c.Patient.CityZipCode).NotEmpty();
        RuleFor(c => c.Patient.Gender)
            .Must(g => new[] { "Male", "Female", "M", "F", "Other" }.Contains(g, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Gender must be Male, Female, M, F, or Other");
    }
}

public record DeletePatientCommand(Guid PatientId) : IRequest<Result>;
