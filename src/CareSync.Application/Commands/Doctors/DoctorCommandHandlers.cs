using CareSync.Domain.Entities;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using CareSync.Application.DTOs.Doctors;
using MediatR;
using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;

namespace CareSync.Application.Commands.Doctors;

public class CreateDoctorCommandHandler(IDoctorRepository doctorRepository, DoctorMapper mapper, IUnitOfWork uow)
    : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = new Doctor(
            Guid.NewGuid(),
            new FullName(request.Doctor.FirstName, request.Doctor.LastName, request.Doctor.MiddleName),
            request.Doctor.Specialty,
            new PhoneNumber(request.Doctor.PhoneNumber),
            new Email(request.Doctor.Email)
        );

        await doctorRepository.AddAsync(doctor);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<DoctorDto>.Success(mapper.Map(doctor));
    }
}

public class UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, DoctorMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpdateDoctorCommand, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var existingDoctor = await doctorRepository.GetByIdAsync(request.Doctor.Id);
        if (existingDoctor == null)
            return Result<DoctorDto>.Failure($"Doctor with ID {request.Doctor.Id} not found");

        existingDoctor.UpdateProfessionalInformation(
            new FullName(request.Doctor.FirstName, request.Doctor.LastName, request.Doctor.MiddleName),
            request.Doctor.Specialty
        );

        existingDoctor.UpdateContactInformation(
            new PhoneNumber(request.Doctor.PhoneNumber),
            new Email(request.Doctor.Email)
        );

        await doctorRepository.UpdateAsync(existingDoctor);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<DoctorDto>.Success(mapper.Map(existingDoctor));
    }
}

public class UpsertDoctorCommandHandler(IDoctorRepository doctorRepository, DoctorMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpsertDoctorCommand, Result<DoctorDto>>
{
    public async Task<Result<DoctorDto>> Handle(UpsertDoctorCommand request, CancellationToken cancellationToken)
    {
        Doctor doctor;

        if (request.Doctor.Id.HasValue && request.Doctor.Id != Guid.Empty)
        {
            // ID provided - this is an UPDATE operation
            var existingDoctor = await doctorRepository.GetByIdAsync(request.Doctor.Id.Value);

            if (existingDoctor == null)
                return Result<DoctorDto>.Failure($"Doctor with ID {request.Doctor.Id.Value} not found.");

            // Update existing doctor
            existingDoctor.UpdateProfessionalInformation(
                new FullName(request.Doctor.FirstName, request.Doctor.LastName, request.Doctor.MiddleName),
                request.Doctor.Specialty
            );

            existingDoctor.UpdateContactInformation(
                new PhoneNumber(request.Doctor.PhoneNumber),
                new Email(request.Doctor.Email)
            );

            await doctorRepository.UpdateAsync(existingDoctor);
            doctor = existingDoctor;
        }
        else
        {
            // No ID provided - this is an INSERT operation
            doctor = new Doctor(
                Guid.NewGuid(),
                new FullName(request.Doctor.FirstName, request.Doctor.LastName, request.Doctor.MiddleName),
                request.Doctor.Specialty,
                new PhoneNumber(request.Doctor.PhoneNumber),
                new Email(request.Doctor.Email)
            );

            await doctorRepository.AddAsync(doctor);
        }
        await uow.SaveChangesAsync(cancellationToken);
        return Result<DoctorDto>.Success(mapper.Map(doctor));
    }
}

public class DeleteDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork uow) : IRequestHandler<DeleteDoctorCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        if (await doctorRepository.HasRelatedDataAsync(request.DoctorId))
            return Result<Unit>.Failure("Cannot deactivate doctor: related appointments or medical records exist.");
        await doctorRepository.DeleteAsync(request.DoctorId);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
