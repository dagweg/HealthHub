using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Entities;

public class Appointment
{
  public Guid AppointmentId { get; set; } = Guid.NewGuid();

  [Required]
  public Guid DoctorId { get; set; } // <<FK>>

  [Required]
  public Guid PatientId { get; set; } // <<FK>>

  [Required]
  public DateTime AppointmentDate { get; set; }

  [Required]
  public TimeOnly AppointmentTime { get; set; }

  [Required]
  public TimeSpan AppointmentTimeSpan { get; set; } = TimeSpan.FromMinutes(30);

  [Required]
  public AppointmentType AppointmentType { get; set; }
  public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

  public virtual required Doctor Doctor { get; set; } // <<NAV>>
  public virtual required Patient Patient { get; set; } // <<NAV>>

  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
