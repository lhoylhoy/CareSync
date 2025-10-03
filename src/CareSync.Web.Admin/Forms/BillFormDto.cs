namespace CareSync.Web.Admin.Forms;

using CareSync.Application.DTOs.Billing;

public class BillFormDto
{
    public Guid? Id { get; set; }
    public string PatientIdString { get; set; } = string.Empty;
    public Guid PatientId => Guid.TryParse(PatientIdString, out var g) ? g : Guid.Empty;
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);
    public decimal TaxRate { get; set; } = 0.12m;
    public decimal DiscountAmount { get; set; }
    public string? Notes { get; set; }
    public CreateBillDto ToCreateDto() => new() { PatientId = PatientId, DueDate = DueDate, TaxRate = TaxRate, DiscountAmount = DiscountAmount, Notes = Notes };
    public UpsertBillDto ToUpsertDto() => new(Id, PatientId, DueDate, TaxRate, DiscountAmount, Notes);
    public static BillFormDto FromDto(BillDto dto) => new()
    {
        Id = dto.Id,
        PatientIdString = dto.PatientId.ToString(),
        DueDate = dto.DueDate,
        TaxRate = dto.TaxRate,
        DiscountAmount = dto.DiscountAmount,
        Notes = dto.Notes
    };
}
