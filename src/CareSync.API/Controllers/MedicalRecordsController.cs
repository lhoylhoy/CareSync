using CareSync.Application.Commands.MedicalRecords;
using CareSync.Application.DTOs.MedicalRecords;
using CareSync.Application.Queries.MedicalRecords;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicalRecordsController(IMediator mediator) : BaseApiController(mediator)
{

    /// <summary>
    ///     Get all medical records
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetAllMedicalRecords()
    {
        var medicalRecords = await Mediator.Send(new GetAllMedicalRecordsQuery());
        return OkOrProblem(medicalRecords);
    }

    /// <summary>
    ///     Get a medical record by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MedicalRecordDto>> GetMedicalRecord(Guid id)
    {
        var medicalRecord = await Mediator.Send(new GetMedicalRecordByIdQuery(id));
        return OkOrNotFound(medicalRecord);
    }

    /// <summary>
    ///     Get medical records by patient ID
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsByPatient(Guid patientId)
    {
        var medicalRecords = await Mediator.Send(new GetMedicalRecordsByPatientQuery(patientId));
        return OkOrProblem(medicalRecords);
    }

    /// <summary>
    ///     Get medical records by doctor ID
    /// </summary>
    [HttpGet("doctor/{doctorId:guid}")]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetMedicalRecordsByDoctor(Guid doctorId)
    {
        var medicalRecords = await Mediator.Send(new GetMedicalRecordsByDoctorQuery(doctorId));
        return OkOrProblem(medicalRecords);
    }

    /// <summary>
    ///     Search medical records with optional filters
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> SearchMedicalRecords(
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? doctorId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (patientId.HasValue)
        {
            var medicalRecords = await Mediator.Send(new GetMedicalRecordsByPatientQuery(patientId.Value));
            return OkOrProblem(medicalRecords);
        }

        if (doctorId.HasValue)
        {
            var medicalRecords = await Mediator.Send(new GetMedicalRecordsByDoctorQuery(doctorId.Value));
            return OkOrProblem(medicalRecords);
        }

        if (startDate.HasValue && endDate.HasValue)
        {
            var medicalRecords = await Mediator.Send(new GetMedicalRecordsByDateRangeQuery(startDate.Value, endDate.Value));
            return OkOrProblem(medicalRecords);
        }

        // If no filters provided, return all records
        var allRecords = await Mediator.Send(new GetAllMedicalRecordsQuery());
        return OkOrProblem(allRecords);
    }

    /// <summary>
    ///     Create a new medical record
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MedicalRecordDto>> CreateMedicalRecord(
        [FromBody] CreateMedicalRecordDto createMedicalRecordDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var medicalRecord = await Mediator.Send(new CreateMedicalRecordCommand(createMedicalRecordDto));
        return CreatedOrBadRequest(medicalRecord, nameof(GetMedicalRecord), new { id = medicalRecord.Value?.Id });
    }

    /// <summary>
    ///     Delete a medical record
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteMedicalRecord(Guid id)
    {
        var deleted = await Mediator.Send(new DeleteMedicalRecordCommand(id));
        return NoContentOrNotFound(deleted);
    }

    /// <summary>
    ///     Update a medical record by ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<MedicalRecordDto>> UpdateMedicalRecord(Guid id, [FromBody] UpdateMedicalRecordDto updateMedicalRecordDto)
    {
        if (id != updateMedicalRecordDto.Id)
            return BadRequest("ID mismatch between route and body");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var medicalRecord = await Mediator.Send(new UpdateMedicalRecordCommand(updateMedicalRecordDto));
        return UpdatedOrNotFound(medicalRecord);
    }

    /// <summary>
    ///     Upsert a medical record (create or update)
    /// </summary>
    [HttpPut("upsert")]
    public async Task<ActionResult<MedicalRecordDto>> UpsertMedicalRecord([FromBody] UpsertMedicalRecordDto upsertMedicalRecordDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var record = await Mediator.Send(new UpsertMedicalRecordCommand(upsertMedicalRecordDto));
        return UpsertOkOrBadRequest(record);
    }

    /// <summary>
    ///     Finalize a medical record (locks further edits)
    /// </summary>
    [HttpPut("{id:guid}/finalize"), Authorize(Policy = "CanFinalizeMedicalRecord")]
    public async Task<ActionResult<MedicalRecordDto>> FinalizeMedicalRecord(Guid id, [FromBody] FinalizeMedicalRecordDto request)
    {
        if (id != request.Id) return BadRequest("ID mismatch between route and body");
        var result = await Mediator.Send(new FinalizeMedicalRecordCommand(request.Id, request.FinalNotes, request.FinalizedBy));
        return UpdatedOrNotFound(result);
    }

    /// <summary>
    ///     Reopen a finalized medical record (administrative action)
    /// </summary>
    [HttpPut("{id:guid}/reopen"), Authorize(Policy = "CanReopenMedicalRecord")]
    public async Task<ActionResult<MedicalRecordDto>> ReopenMedicalRecord(Guid id)
    {
        var result = await Mediator.Send(new ReopenMedicalRecordCommand(id));
        return UpdatedOrNotFound(result);
    }
}
