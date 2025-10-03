namespace CareSync.Domain.Enums;

/// <summary>
/// Common doctor specializations used by the admin UI.
/// Keep values concise and use PascalCase. The DB still stores specialty as string;
/// the enum is a code-side representation to drive UI choices.
/// </summary>
public enum Specialization
{
    GeneralPractice,
    FamilyMedicine,
    InternalMedicine,
    Pediatrics,
    ObstetricsGynaecology,
    Surgery,
    Cardiology,
    Dermatology,
    Neurology,
    Orthopedics,
    Psychiatry,
    Radiology,
    EmergencyMedicine,
    Ophthalmology,
    Otolaryngology,
    Dentistry
}
