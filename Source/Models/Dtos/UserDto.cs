using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Attributes;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Models.Enums;

namespace HealthHub.Source.Models.Dtos;

public record UserDto
{
  public required Guid UserId { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string Email { get; set; }
  public required string Phone { get; set; }
  public required Gender Gender { get; set; }
  public required DateOnly DateOfBirth { get; set; }
  public required string ProfilePicture { get; set; }
  public required string Address { get; set; }
};

/// <summary>
/// This is what the client sends to register a user
/// </summary>
public record RegisterUserDto
{
  [Required]
  public required string FirstName { get; init; }

  [Required]
  public required string LastName { get; init; }

  [Required]
  [EmailAddress]
  public required string Email { get; init; }

  [Required]
  [Password]
  public required string Password { get; init; }

  [Required]
  [Phone]
  [MinLength(4, ErrorMessage = "The field must be at least 4 characters long.")]
  public required string Phone { get; init; }

  [Required]
  public required string Gender { get; init; }

  [Required]
  public required string DateOfBirth { get; init; }

  [Required]
  public required string Address { get; init; }

  [Required]
  public required string Role { get; init; }

  // If the user is a patient, they may/may-not specify the following
  // Note the following fields MUST be validated in the controller based on the Role field provided as payload
  public string? MedicalHistory { get; set; }
  public string? EmergencyContactName { get; set; }
  public string? EmergencyContactPhone { get; set; }

  // If the user is a doctor, they may/may-not specify the following
  // Note the following fields MUST be validated in the controller based on the Role field provided as payload
  public List<string> Specialities { get; set; } = [];
  public List<AvailabilityDto> Availabilities { get; set; } = [];
  public string? Qualifications { get; set; }
  public string? Biography { get; set; }
  public DoctorStatus DoctorStatus { get; set; } = DoctorStatus.Active;
  public CreateFileDto? Cv { get; set; }
  public required decimal OnlineAppointmentFee { get; init; }
  public required decimal InPersonAppointmentFee { get; init; }
  public List<CreateEducationDto> Educations { get; set; } = [];
  public List<CreateExperienceDto> Experiences { get; set; } = [];
}

/// <summary>
/// These are the fields what a newly created Auth0 User will have
/// </summary>
/// <param name="UserId"></param>
/// <param name="Profile"></param>
/// <param name="EmailVerified"></param>
public record Auth0UserDto(string UserId, string Profile, bool EmailVerified);

/// <summary>
/// Auth0 Login Data Transfer Object. Used for returning Auth0 login information to the client.
/// </summary>
/// <param name="AccessToken"></param>
/// <param name="ExpiresIn"></param>
/// <param name="Auth0ProfileDto"></param>
public record Auth0LoginDto(string AccessToken, int ExpiresIn, Auth0ProfileDto Auth0ProfileDto);

/// <summary>
/// Login User Data Transfer Object. Sent from the client to the server for logging in a user. Used to validate client request upon hitting login endpoint.
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record LoginUserDto([Required] [EmailAddress] string Email, [Required] string Password);
