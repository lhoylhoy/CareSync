using System.ComponentModel.DataAnnotations;

namespace CareSync.Domain.Enums;

public enum StaffRole
{
    [Display(Name = "Nurse")]
    Nurse = 1,

    [Display(Name = "Technician")]
    Technician = 2,

    [Display(Name = "Receptionist")]
    Receptionist = 3,

    [Display(Name = "Administrator")]
    Administrator = 4,

    [Display(Name = "Manager")]
    Manager = 5,

    [Display(Name = "Pharmacy Technician")]
    PharmacyTechnician = 6,

    [Display(Name = "Lab Technician")]
    LabTechnician = 7,

    [Display(Name = "Radiology Technician")]
    RadiologyTechnician = 8,

    [Display(Name = "Medical Assistant")]
    MedicalAssistant = 9,

    [Display(Name = "IT Support")]
    ItSupport = 10,

    [Display(Name = "Security")]
    Security = 11,

    [Display(Name = "Maintenance")]
    Maintenance = 12
}
