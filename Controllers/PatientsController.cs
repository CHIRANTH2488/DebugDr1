using Debugging_Doctors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientsController : ControllerBase
    {
        private readonly DebuggingDoctorsContext _context;

        public PatientsController(DebuggingDoctorsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Patient>> GetPatient()
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

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving patient profile.", Error = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPatient(Patient patient)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (patient?.UserId != userId || patient.PatientId <= 0)
                {
                    return BadRequest(new { Message = "Invalid patient ID or unauthorized access." });
                }

                if (!ModelState.IsValid || !IsValidAadhaar_no(patient.AadhaarNo))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidAadhaar_no(patient.AadhaarNo))
                    {
                        errors.Add("Invalid Aadhaar number. It must be a 12-digit numeric string starting with 2-9.");
                    }
                    return BadRequest(new { Message = "Invalid patient data.", Errors = errors });
                }

                if (_context.Patients.Any(p => p.AadhaarNo == patient.AadhaarNo && p.PatientId != patient.PatientId))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Aadhaar number already exists for another patient." });
                }

                var existingPatient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId && p.IsApproved);
                if (existingPatient == null)
                {
                    return NotFound(new { Message = "Patient profile not found or not approved." });
                }

                existingPatient.ContactNo = patient.ContactNo;
                existingPatient.Address = patient.Address; // Only editable fields
                _context.Entry(existingPatient).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Patients.Any(p => p.PatientId == patient.PatientId))
                    {
                        return NotFound(new { Message = $"Patient with ID {patient.PatientId} not found." });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating patient.", Error = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (patient.UserId != userId)
                {
                    return BadRequest(new { Message = "Unauthorized: Patient UserId must match logged-in user." });
                }

                if (!ModelState.IsValid || !IsValidAadhaar_no(patient.AadhaarNo))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidAadhaar_no(patient.AadhaarNo))
                    {
                        errors.Add("Invalid Aadhaar number. It must be a 12-digit numeric string starting with 2-9.");
                    }
                    return BadRequest(new { Message = "Invalid patient data.", Errors = errors });
                }

                if (_context.Patients.Any(p => p.AadhaarNo == patient.AadhaarNo))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Aadhaar number already exists." });
                }

                if (_context.Patients.Any(p => p.UserId == userId))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Patient profile already exists for this user." });
                }

                patient.IsApproved = false; // Pending approval
                _context.Patients.Add(patient);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetPatient), null, patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating patient.", Error = ex.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatient()
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

                _context.Patients.Remove(patient);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting patient.", Error = ex.Message });
            }
        }

        [HttpGet("Appointments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetPatientAppointments()
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

                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == patient.PatientId)
                    .Select(a => new
                    {
                        a.AppointmentId,
                        a.DoctorId,
                        a.AppointmentDate,
                        a.AppointmentStatus,
                        a.Symptoms,
                        a.Diagnosis,
                        a.Medicines,
                        a.InvoiceStatus,
                        a.InvoiceAmount,
                        a.IsApproved
                    })
                    .ToListAsync();

                if (!appointments.Any())
                {
                    return NotFound(new { Message = "No appointments found for this patient." });
                }

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving appointments.", Error = ex.Message });
            }
        }

        [HttpGet("Appointments/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Appointment>> GetPatientAppointment(int id)
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
                    .Where(a => a.AppointmentId == id && a.PatientId == patient.PatientId)
                    .Select(a => new
                    {
                        a.AppointmentId,
                        a.DoctorId,
                        a.AppointmentDate,
                        a.AppointmentStatus,
                        a.Symptoms,
                        a.Diagnosis,
                        a.Medicines,
                        a.InvoiceStatus,
                        a.InvoiceAmount,
                        a.IsApproved
                    })
                    .FirstOrDefaultAsync();

                if (appointment == null)
                {
                    return NotFound(new { Message = $"Appointment with ID {id} not found or not associated with this patient." });
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving appointment.", Error = ex.Message });
            }
        }

        private bool IsValidAadhaar_no(string aadhaar_no)
        {
            if (string.IsNullOrEmpty(aadhaar_no) || aadhaar_no.Length != 12 || !long.TryParse(aadhaar_no, out _))
            {
                return false;
            }
            return !aadhaar_no.StartsWith("0") && !aadhaar_no.StartsWith("1");
        }
    }
}