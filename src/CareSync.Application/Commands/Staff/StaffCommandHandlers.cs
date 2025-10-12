using CareSync.Application.Common.Mapping;
using CareSync.Application.Common.Results;
using CareSync.Application.DTOs.Staff;
using CareSync.Domain.Interfaces;
using CareSync.Domain.ValueObjects;
using MediatR;
using StaffEntity = CareSync.Domain.Entities.Staff;

namespace CareSync.Application.Commands.Staff;

public class CreateStaffCommandHandler(IStaffRepository staffRepository, StaffMapper mapper, IUnitOfWork uow)
    : IRequestHandler<CreateStaffCommand, Result<StaffDto>>
{
    public async Task<Result<StaffDto>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = new StaffEntity(
            Guid.NewGuid(),
            new FullName(request.Staff.FirstName, request.Staff.LastName, request.Staff.MiddleName),
            new Email(request.Staff.Email),
            new PhoneNumber(request.Staff.PhoneNumber),
            request.Staff.Role,
            request.Staff.Department,
            request.Staff.EmployeeId,
            request.Staff.HireDate
        );

        if (request.Staff.Salary.HasValue)
            staff.SetSalary(request.Staff.Salary.Value);

        if (!string.IsNullOrEmpty(request.Staff.Notes))
            staff.AddNotes(request.Staff.Notes);

        await staffRepository.AddAsync(staff);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<StaffDto>.Success(mapper.Map(staff));
    }
}

public class UpdateStaffCommandHandler(IStaffRepository staffRepository, StaffMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpdateStaffCommand, Result<StaffDto>>
{
    public async Task<Result<StaffDto>> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var existingStaff = await staffRepository.GetByIdAsync(request.Staff.Id);
        if (existingStaff == null)
            return Result<StaffDto>.Failure($"Staff with ID {request.Staff.Id} not found.");

        // Update staff using available methods
        existingStaff.UpdateContactInformation(new Email(request.Staff.Email), new PhoneNumber(request.Staff.PhoneNumber));
        existingStaff.UpdateRole(request.Staff.Role, request.Staff.Department);

        if (request.Staff.Salary.HasValue)
            existingStaff.SetSalary(request.Staff.Salary.Value);

        if (!string.IsNullOrEmpty(request.Staff.Notes))
            existingStaff.AddNotes(request.Staff.Notes);

        await staffRepository.UpdateAsync(existingStaff);
        await uow.SaveChangesAsync(cancellationToken);

        return Result<StaffDto>.Success(mapper.Map(existingStaff));
    }
}

public class UpsertStaffCommandHandler(IStaffRepository staffRepository, StaffMapper mapper, IUnitOfWork uow)
    : IRequestHandler<UpsertStaffCommand, Result<StaffDto>>
{
    public async Task<Result<StaffDto>> Handle(UpsertStaffCommand request, CancellationToken cancellationToken)
    {
        StaffEntity staff;

        if (request.Staff.Id.HasValue && request.Staff.Id != Guid.Empty)
        {
            // ID provided - this is an UPDATE operation
            var existingStaff = await staffRepository.GetByIdAsync(request.Staff.Id.Value);

            if (existingStaff == null)
                return Result<StaffDto>.Failure($"Staff with ID {request.Staff.Id.Value} not found.");

            // Update existing staff
            existingStaff.UpdateContactInformation(new Email(request.Staff.Email), new PhoneNumber(request.Staff.PhoneNumber));
            existingStaff.UpdateRole(request.Staff.Role, request.Staff.Department);

            if (request.Staff.Salary.HasValue)
                existingStaff.SetSalary(request.Staff.Salary.Value);

            if (!string.IsNullOrEmpty(request.Staff.Notes))
                existingStaff.AddNotes(request.Staff.Notes);

            await staffRepository.UpdateAsync(existingStaff);
            await uow.SaveChangesAsync(cancellationToken);
            staff = existingStaff;
        }
        else
        {
            // No ID provided - this is an INSERT operation
            staff = new StaffEntity(
                Guid.NewGuid(),
                new FullName(request.Staff.FirstName, request.Staff.LastName, request.Staff.MiddleName),
                new Email(request.Staff.Email),
                new PhoneNumber(request.Staff.PhoneNumber),
                request.Staff.Role,
                request.Staff.Department,
                request.Staff.EmployeeId,
                request.Staff.HireDate
            );

            if (request.Staff.Salary.HasValue)
                staff.SetSalary(request.Staff.Salary.Value);

            if (!string.IsNullOrEmpty(request.Staff.Notes))
                staff.AddNotes(request.Staff.Notes);

            await staffRepository.AddAsync(staff);
            await uow.SaveChangesAsync(cancellationToken);
        }

        return Result<StaffDto>.Success(mapper.Map(staff));
    }
}

public class DeleteStaffCommandHandler(IStaffRepository staffRepository, IUnitOfWork uow) : IRequestHandler<DeleteStaffCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
    {
        if (await staffRepository.HasRelatedDataAsync(request.StaffId))
        {
            return Result<Unit>.Failure("Cannot delete staff member with related appointments or medical records. Deactivate instead.");
        }
        await staffRepository.DeleteAsync(request.StaffId);
        await uow.SaveChangesAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
