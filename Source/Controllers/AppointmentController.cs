using FluentValidation;
using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Constants;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Responses;
using HealthHub.Source.Services;
using HealthHub.Source.Validation.AppointmentValidation;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Source.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentController(
  AppointmentService appointmentService,
  ILogger<AppointmentController> logger,
  IValidator<CreateAppointmentDto> createAppointmentDtoValidator
) : ControllerBase
{
  /// <summary>
  /// Allows booking of appointment for patients and doctors
  /// </summary>
  /// <param name="createAppointmentDto"></param>
  /// <returns></returns>
  [HttpPost("book")]
  public async Task<IActionResult> CreateAppointment(
    [FromBody] CreateAppointmentDto createAppointmentDto
  )
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var fluentValidation = createAppointmentDtoValidator.Validate(createAppointmentDto);

      if (!fluentValidation.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.FluentValidationErrors] =
          fluentValidation.ToFluentValidationErrorResult();
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }

      var response = await appointmentService.CreateAppointmentAsync(createAppointmentDto);
      if (!response.Success)
        throw new BadHttpRequestException(response.Message!);
      return Ok(response);
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  /// <summary>
  /// Gets all appointments from the database
  /// </summary>
  /// <returns></returns>
  [HttpGet("all")]
  public async Task<IActionResult> GetAllAppointments()
  {
    try
    {
      var response = await appointmentService.GetAllAppointmentsAsync();
      if (!response.Success)
        throw new Exception(response.Message);

      return Ok(response);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Error occured trying to get all appointments {ex}");
      throw;
    }
  }

  /// <summary>
  /// Gets all appointments for a specific doctor
  /// </summary>
  /// <returns></returns>
  [HttpGet("doctor/{doctorId}")]
  public async Task<IActionResult> GetDoctorAppointments([FromRoute] Guid doctorId)
  {
    try
    {
      var response = await appointmentService.GetDoctorAppointmentsAsync(doctorId);
      if (!response.Success)
        throw new Exception(response.Message);
      return StatusCode(response.StatusCode, response);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to get patient appointments {ex}");
      throw new Exception("An error occured while trying to get doctor appointments", ex);
    }
  }

  /// <summary>
  /// Gets all appointments for a specific patient
  /// </summary>
  /// <returns></returns>
  [HttpGet("patient/{patientId}")]
  public async Task<IActionResult> GetPatientAppointments([FromRoute] Guid patientId)
  {
    try
    {
      var response = await appointmentService.GetPatientAppointmentsAsync(patientId);
      if (!response.Success)
        throw new Exception(response.Message);
      return StatusCode(response.StatusCode, response);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"An error occured while trying to get patient appointments {ex}");
      throw new Exception("An error occured while trying to get patient appointments", ex);
    }
  }

  /// <summary>
  /// Deletes an appointment from the database
  /// </summary>
  /// <param name="appointmentId"></param>
  /// <returns></returns>
  [HttpDelete("{appointmentId}")]
  public async Task<IActionResult> DeleteAppointment([FromRoute] Guid appointmentId)
  {
    try
    {
      var response = await appointmentService.DeleteAppointmentAsync(appointmentId);
      if (!response.Success)
        throw new Exception(response.Message);

      return StatusCode(response.StatusCode, response);
    }
    catch (System.Exception ex)
    {
      logger.LogError($"Error occured trying to delete appointment {ex}");
      throw;
    }
  }
}
