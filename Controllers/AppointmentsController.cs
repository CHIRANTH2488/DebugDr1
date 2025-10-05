using Debugging_Doctors.Models;
using Debugging_Doctors.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly DebuggingDoctorsContext _context;

        public AppointmentsController(DebuggingDoctorsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Authorize(Roles = "Doctor,Patient,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (role != "Admin" && role != "Doctor" && role != "Patient")
                {
                    return Forbid();
                }

                IQueryable<Appointment> query;
                if (role == "Admin")
                {
                    query = _context.Appointments;
                }
                else if (role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = "Doctor profile not found or not approved." });
                    }
                    query = _context.Appointments.Where(a => a.DoctorId == doctor.DocId && a.IsApproved);
                }
                else // Patient
                {
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId && p.IsApproved);
                    if (patient == null)
                    {
                        return NotFound(new { Message = "Patient profile not found or not approved." });
                    }
                    query = _context.Appointments
                        .Where(a => a.PatientId == patient.PatientId)
                        .Select(a => new Appointment
                        {
                            AppointmentId = a.AppointmentId,
                            DoctorId = a.DoctorId,
                            AppointmentDate = a.AppointmentDate,
                            AppointmentStatus = a.AppointmentStatus,
                            Symptoms = a.Symptoms,
                            Diagnosis = a.Diagnosis,
                            Medicines = a.Medicines,
                            InvoiceStatus = a.InvoiceStatus,
                            InvoiceAmount = a.InvoiceAmount,
                            IsApproved = a.IsApproved
                        });
                }

                var appointments = await query.ToListAsync();
                if (!appointments.Any())
                {
                    return NotFound(new { Message = "No appointments found." });
                }

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving appointments.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Doctor,Patient,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid appointment ID." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (role != "Admin" && role != "Doctor" && role != "Patient")
                {
                    return Forbid();
                }

                Appointment appointment;
                if (role == "Admin")
                {
                    appointment = await _context.Appointments.FindAsync(id);
                }
                else if (role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = "Doctor profile not found or not approved." });
                    }
                    appointment = await _context.Appointments
                        .FirstOrDefaultAsync(a => a.AppointmentId == id && a.DoctorId == doctor.DocId && a.IsApproved);
                }
                else // Patient
                {
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId && p.IsApproved);
                    if (patient == null)
                    {
                        return NotFound(new { Message = "Patient profile not found or not approved." });
                    }
                    appointment = await _context.Appointments
                        .Where(a => a.AppointmentId == id && a.PatientId == patient.PatientId)
                        .Select(a => new Appointment
                        {
                            AppointmentId = a.AppointmentId,
                            DoctorId = a.DoctorId,
                            AppointmentDate = a.AppointmentDate,
                            AppointmentStatus = a.AppointmentStatus,
                            Symptoms = a.Symptoms,
                            Diagnosis = a.Diagnosis,
                            Medicines = a.Medicines,
                            InvoiceStatus = a.InvoiceStatus,
                            InvoiceAmount = a.InvoiceAmount,
                            IsApproved = a.IsApproved
                        })
                        .FirstOrDefaultAsync();
                }

                if (appointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {id} not found or not authorized." });
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving appointment.", Error = ex.Message });
            }
        }

        [HttpGet("{id}/PatientData")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientDataForApprovedAppointment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid appointment ID." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                if (doctor == null)
                {
                    return NotFound(new { Message = "Doctor profile not found or not approved." });
                }

                var parameters = new[]
                {
                    new SqlParameter("@AppointmentId", id),
                    new SqlParameter("@UserId", doctor.DocId),
                    new SqlParameter("@UserRole", "Doctor")
                };

                var result = await _context.Database
                    .SqlQueryRaw<dynamic>("EXEC GetPatientDataForApprovedAppointment @AppointmentId, @UserId, @UserRole", parameters)
                    .ToListAsync();

                if (result == null || !result.Any())
                {
                    return NotFound(new { Message = "No patient data found for the specified appointment." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving patient data.", Error = ex.Message });
            }
        }

        [HttpPost("Book")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Appointment>> BookAppointment(Appointment appointment)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId && p.IsApproved);
                if (patient == null)
                {
                    return NotFound(new { Message = "Patient profile not found or not approved." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { Message = "Invalid appointment data.", Errors = errors });
                }

                // Auto-assign doctor based on specialization and availability
                var doctors = await _context.Doctors
                    .Where(d => d.Specialisation == appointment.Symptoms && d.IsApproved) // Assume Symptoms holds specialization for simplicity
                    .ToListAsync();
                var availableDoctor = doctors.FirstOrDefault(d => IsAvailable(d.Availability, appointment.AppointmentDate));
                if (availableDoctor == null)
                {
                    return BadRequest(new { Message = "No available doctor found." });
                }

                appointment.DoctorId = availableDoctor.DocId;
                appointment.PatientId = patient.PatientId;
                appointment.IsApproved = false; // Pending approval
                appointment.AppointmentStatus = "Scheduled";
                appointment.InvoiceStatus = "Pending";

                _context.Appointments.Add(appointment);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error booking appointment.", Error = ex.Message });
            }
        }

        [HttpPut("Cancel/{id}")]
        [Authorize(Roles = "Patient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid appointment ID." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId && p.IsApproved);
                if (patient == null)
                {
                    return NotFound(new { Message = "Patient profile not found or not approved." });
                }

                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == patient.PatientId && a.AppointmentStatus == "Scheduled");
                if (appointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {id} not found, not scheduled, or not associated with this patient." });
                }

                appointment.AppointmentStatus = "Cancelled";
                appointment.InvoiceStatus = "Cancelled";
                _context.Entry(appointment).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error cancelling appointment.", Error = ex.Message });
            }
        }

        [HttpPost("Prescription/{id}")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubmitPrescription(int id, [FromBody] PrescriptionDto prescription)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid appointment ID." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                if (doctor == null)
                {
                    return NotFound(new { Message = "Doctor profile not found or not approved." });
                }

                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == id && a.DoctorId == doctor.DocId && a.IsApproved);
                if (appointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {id} not found or not authorized." });
                }

                appointment.Medicines = System.Text.Json.JsonSerializer.Serialize(prescription);
                appointment.Diagnosis = prescription.ChiefComplaints; // Or separate field
                appointment.AppointmentStatus = "Completed";
                _context.Entry(appointment).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error submitting prescription.", Error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Doctor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (role != "Admin" && role != "Doctor")
                {
                    return Forbid();
                }

                if (role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = "Doctor profile not found or not approved." });
                    }
                    if (appointment.DoctorId != doctor.DocId)
                    {
                        return BadRequest(new { Message = "Unauthorized: Appointment must be assigned to the logged-in doctor." });
                    }
                    if (!IsAvailable(doctor.Availability, appointment.AppointmentDate))
                    {
                        return BadRequest(new { Message = "Doctor not available at the specified time." });
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { Message = "Invalid appointment data.", Errors = errors });
                }

                appointment.IsApproved = role == "Admin"; // Admin approves instantly, Doctor needs approval
                _context.Appointments.Add(appointment);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating appointment.", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            try
            {
                if (id <= 0 || id != appointment?.AppointmentId)
                {
                    return BadRequest(new { Message = "Invalid appointment ID or mismatch with request body." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (role != "Admin" && role != "Doctor")
                {
                    return Forbid();
                }

                Appointment existingAppointment;
                if (role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = "Doctor profile not found or not approved." });
                    }
                    existingAppointment = await _context.Appointments
                        .FirstOrDefaultAsync(a => a.AppointmentId == id && a.DoctorId == doctor.DocId && a.IsApproved);
                    if (existingAppointment == null)
                    {
                        return NotFound(new { Message = $"Appointment with ID {id} not found or not authorized." });
                    }
                }
                else // Admin
                {
                    existingAppointment = await _context.Appointments.FindAsync(id);
                    if (existingAppointment == null)
                    {
                        return NotFound(new { Message = $"Appointment with ID {id} not found." });
                    }
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { Message = "Invalid appointment data.", Errors = errors });
                }

                _context.Entry(existingAppointment).CurrentValues.SetValues(appointment);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Appointments.Any(a => a.AppointmentId == id))
                    {
                        return NotFound(new { Message = $"Appointment with ID {id} not found." });
                    }
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Concurrency error." });
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating appointment.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Doctor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid appointment ID." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (role != "Admin" && role != "Doctor")
                {
                    return Forbid();
                }

                Appointment appointment;
                if (role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = "Doctor profile not found or not approved." });
                    }
                    appointment = await _context.Appointments
                        .FirstOrDefaultAsync(a => a.AppointmentId == id && a.DoctorId == doctor.DocId && a.IsApproved);
                }
                else // Admin
                {
                    appointment = await _context.Appointments.FindAsync(id);
                }

                if (appointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {id} not found or not authorized." });
                }

                _context.Appointments.Remove(appointment);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting appointment.", Error = ex.Message });
            }
        }

        private bool IsAvailable(string availability, DateTime date)
        {
            // Parse availability string, e.g., "Mon-Fri 9-5"
            // Simple implementation for demo
            return true; // Replace with actual logic
        }
    }
}