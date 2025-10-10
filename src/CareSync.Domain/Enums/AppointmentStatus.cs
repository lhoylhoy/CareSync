using System.ComponentModel.DataAnnotations;

namespace CareSync.Domain.Enums;

public enum AppointmentStatus
{
    [Display(Name = "Scheduled")]
    Scheduled = 1,

    [Display(Name = "In Progress")]
    InProgress = 2,

    [Display(Name = "Completed")]
    Completed = 3,

    [Display(Name = "Cancelled")]
    Cancelled = 4,

    [Display(Name = "No Show")]
    NoShow = 5
}
