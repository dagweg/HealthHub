using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
using Microsoft.OpenApi.Extensions;

public static class EntityExtensions
{
  // Maps User entity to UserDto
  public static UserDto ToUserDto(this User user)
  {
    return new UserDto
    {
      UserId = user.UserId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      ProfilePicture = user.ProfilePicture ?? "",
      Gender = user.Gender,
      Address = user.Address,
      DateOfBirth = user.DateOfBirth,
      Phone = user.Phone,
    };
  }

  public static CreatePatientDto ToCreatePatientDto(this RegisterUserDto registerUserDto, User user)
  {
    return new CreatePatientDto
    {
      User = user,
      EmergencyContactName = registerUserDto.EmergencyContactName,
      EmergencyContactPhone = registerUserDto.EmergencyContactPhone,
      MedicalHistory = registerUserDto.MedicalHistory
    };
  }

  public static CreateDoctorDto ToCreateDoctorDto(this RegisterUserDto registerUserDto, User user)
  {
    return new CreateDoctorDto
    {
      Biography = registerUserDto.Biography ?? "None",
      Qualifications = registerUserDto.Qualifications ?? "None",
      User = user
    };
  }

  public static ProfileDto ToProfileDto(this User user)
  {
    return new ProfileDto
    {
      UserId = user.UserId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      ProfilePicture = user.ProfilePicture ?? "",
      Phone = user.Phone,
      Gender = user.Gender,
      DateOfBirth = user.DateOfBirth,
      Address = user.Address,
      Role = user.Role
    };
  }

  // Maps User entity to RegisterUserDto
  public static RegisterUserDto ToRegisterUserDto(this User user, string password)
  {
    return new RegisterUserDto
    {
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      Password = password,
      Phone = user.Phone,
      Gender = user.Gender.ToString(),
      Role = user.Role.ToString(),
      DateOfBirth = user.DateOfBirth.ToString(),
      Address = user.Address
    };
  }

  public static CreateAdminDto ToCreateAdminDto(this RegisterUserDto registerUserDto, User user)
  {
    return new CreateAdminDto { User = user };
  }

  /// <summary>
  /// Make sure to populate User and Doctor Specialities Before calling this Extension Method
  /// </summary>
  /// <param name="d"></param>
  /// <param name="user"></param>
  /// <param name="doctorSpecialities"></param>
  /// <returns></returns>
  public static DoctorDto ToDoctorDto(
    this Doctor d,
    User user,
    ICollection<DoctorSpeciality> doctorSpecialities
  )
  {
    return new DoctorDto
    {
      UserId = user.UserId,
      DoctorId = d.DoctorId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      IsEmailVerified = user.IsEmailVerified,
      Phone = user.Phone,
      Gender = user.Gender,
      DateOfBirth = user.DateOfBirth,
      Address = user.Address,
      Specialities = doctorSpecialities.Select(ds => ds.Speciality.SpecialityName).ToList(),
      Qualifications = d.Qualifications,
      Biography = d.Biography,
      DoctorStatus = d.DoctorStatus,
      ProfilePicture = user.ProfilePicture ?? ""
    };
  }

  public static PatientDto ToPatientDto(this Patient patient, User user)
  {
    return new PatientDto
    {
      UserId = user.UserId,
      PatientId = patient.PatientId,
      FirstName = user.FirstName,
      LastName = user.LastName,
      Email = user.Email,
      IsEmailVerified = user.IsEmailVerified,
      Phone = user.Phone,
      Gender = user.Gender,
      DateOfBirth = user.DateOfBirth,
      Address = user.Address,
      EmergencyContactName = patient.EmergencyContactName ?? "",
      EmergencyContactPhone = patient.EmergencyContactPhone ?? "",
      MedicalHistory = patient.MedicalHistory ?? "",
      ProfilePicture = patient.User.ProfilePicture ?? "",
    };
  }

  /// <summary>
  /// Maps Appointment entity to AppointmentDto
  /// </summary>
  /// <param name="appointment">appointment</param>
  /// <param name="doctor"></param>
  /// <param name="patient"></param>
  /// <param name="doctorUser"></param>
  /// <param name="patientUser"></param>
  /// <param name="user"></param>
  /// <param name="doctorSpecialities"></param>
  /// <returns></returns>
  public static AppointmentDto ToAppointmentDto(
    this Appointment appointment,
    Doctor doctor,
    Patient patient,
    User doctorUser,
    User patientUser,
    ICollection<DoctorSpeciality> doctorSpecialities
  )
  {
    return new AppointmentDto
    {
      AppointmentId = appointment.AppointmentId,
      Doctor = doctor.ToDoctorDto(doctorUser, doctorSpecialities),
      Patient = patient.ToPatientDto(patientUser),
      AppointmentDate = appointment.AppointmentDate,
      AppointmentTime = appointment.AppointmentTime,
      AppointmentType = appointment.AppointmentType,
    };
  }

  public static AppointmentDto ToAppointmentDto(
    this Appointment appointment,
    Patient patient,
    User patientUser
  )
  {
    return new AppointmentDto
    {
      AppointmentId = appointment.AppointmentId,
      Patient = patient.ToPatientDto(patientUser),
      AppointmentDate = appointment.AppointmentDate,
      AppointmentTime = appointment.AppointmentTime,
      AppointmentType = appointment.AppointmentType,
    };
  }

  public static AppointmentDto ToAppointmentDto(
    this Appointment appointment,
    Doctor doctor,
    User doctorUser,
    ICollection<DoctorSpeciality> doctorSpecialities
  )
  {
    return new AppointmentDto
    {
      AppointmentId = appointment.AppointmentId,
      Doctor = doctor.ToDoctorDto(doctorUser, doctorSpecialities),
      AppointmentDate = appointment.AppointmentDate,
      AppointmentTime = appointment.AppointmentTime,
      AppointmentType = appointment.AppointmentType,
    };
  }

  public static CreateDoctorSpecialityDto ToCreateDoctorSpecialityDto(
    this Speciality speciality,
    Doctor doctor
  )
  {
    return new CreateDoctorSpecialityDto { Doctor = doctor, Speciality = speciality };
  }

  public static DoctorProfileDto ToDoctorProfileDto(
    this Doctor doctor,
    User user,
    ICollection<DoctorAvailability> doctorAvailability,
    ICollection<Speciality> specialities
  )
  {
    return new DoctorProfileDto
    {
      UserId = user.UserId,
      Address = user.Address,
      Availabilities = doctorAvailability.Select(da => da.ToAvailabilityDto()).ToList(),
      Biography = doctor.Biography,
      DateOfBirth = user.DateOfBirth,
      DoctorStatus = doctor.DoctorStatus,
      Email = user.Email,
      FirstName = user.FirstName,
      Gender = user.Gender,
      LastName = user.LastName,
      Phone = user.Phone,
      ProfilePicture = user.ProfilePicture ?? "",
      Qualifications = doctor.Qualifications,
      Role = user.Role,
      Specialities = specialities.Select(s => s.ToSpecialityDto()).ToList()
    };
  }

  public static PatientProfileDto ToPatientProfileDto(this Patient patient, User user)
  {
    return new PatientProfileDto
    {
      UserId = user.UserId,
      Address = user.Address,
      DateOfBirth = user.DateOfBirth,
      Email = user.Email,
      FirstName = user.FirstName,
      Gender = user.Gender,
      LastName = user.LastName,
      Phone = user.Phone,
      ProfilePicture = user.ProfilePicture ?? "",
      Role = user.Role,
      EmergencyContactName = patient.EmergencyContactName ?? "",
      EmergencyContactPhone = patient.EmergencyContactPhone ?? "",
      MedicalHistory = patient.MedicalHistory ?? ""
    };
  }

  public static AvailabilityDto ToAvailabilityDto(this DoctorAvailability doctorAvailability)
  {
    return new AvailabilityDto(
      doctorAvailability.AvailableDay.GetDisplayName(),
      doctorAvailability.StartTime.ToString(),
      doctorAvailability.EndTime.ToString()
    );
  }

  public static string ToSpecialityDto(this Speciality speciality)
  {
    return speciality.SpecialityName;
  }
}
