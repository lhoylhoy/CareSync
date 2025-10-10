using System.ComponentModel.DataAnnotations;

namespace CareSync.Domain.Enums;

/// <summary>
/// Common doctor specializations used by the admin UI.
/// Keep values concise and use PascalCase. The DB still stores specialty as string;
/// the enum is a code-side representation to drive UI choices.
/// </summary>
public enum Specialization
{
    [Display(Name = "General Practice")]
    GeneralPractice,

    [Display(Name = "Family Medicine")]
    FamilyMedicine,

    [Display(Name = "Internal Medicine")]
    InternalMedicine,

    Pediatrics,

    [Display(Name = "Obstetrics & Gynaecology")]
    ObstetricsGynaecology,

    Surgery,
    Cardiology,
    Dermatology,
    Neurology,

    [Display(Name = "Orthopedics")]
    Orthopedics,

    Psychiatry,
    Radiology,

    [Display(Name = "Emergency Medicine")]
    EmergencyMedicine,

    Ophthalmology,

    [Display(Name = "Otolaryngology (ENT)")]
    Otolaryngology,

    Dentistry
}
