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
    [Authorize(Roles = "Doctor")]
    public class DoctorsController : ControllerBase
    {
        private readonly DebuggingDoctorsContext _context;

        public DoctorsController(DebuggingDoctorsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Doctor>> GetDoctor()
        {
            try
            {
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

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving doctor profile.", Error = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutDoctor(Doctor doctor)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (doctor?.UserId != userId || doctor.DocId <= 0)
                {
                    return BadRequest(new { Message = "Invalid doctor ID or unauthorized access." });
                }

                if (!ModelState.IsValid || !IsValidHpid(doctor.Hpid))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidHpid(doctor.Hpid))
                    {
                        errors.Add("Invalid HPID format. It must be a 14-digit number in the format YYYYMMDDSSNNNN.");
                    }
                    return BadRequest(new { Message = "Invalid doctor data.", Errors = errors });
                }

                if (_context.Doctors.Any(d => d.Hpid == doctor.Hpid && d.DocId != doctor.DocId))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "HPID already exists for another doctor." });
                }

                var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId && d.IsApproved);
                if (existingDoctor == null)
                {
                    return NotFound(new { Message = "Doctor profile not found or not approved." });
                }

                existingDoctor.FullName = doctor.FullName;
                existingDoctor.Specialisation = doctor.Specialisation;
                existingDoctor.Hpid = doctor.Hpid; // Requires approval in production
                existingDoctor.Availability = doctor.Availability;
                existingDoctor.ContactNo = doctor.ContactNo;
                _context.Entry(existingDoctor).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Doctors.Any(d => d.DocId == doctor.DocId))
                    {
                        return NotFound(new { Message = $"Doctor with ID {doctor.DocId} not found." });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating doctor.", Error = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { Message = "Invalid user ID in authentication context." });
                }

                if (doctor.UserId != userId)
                {
                    return BadRequest(new { Message = "Unauthorized: Doctor UserId must match logged-in user." });
                }

                if (!ModelState.IsValid || !IsValidHpid(doctor.Hpid))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidHpid(doctor.Hpid))
                    {
                        errors.Add("Invalid HPID format. It must be a 14-digit number in the format YYYYMMDDSSNNNN.");
                    }
                    return BadRequest(new { Message = "Invalid doctor data.", Errors = errors });
                }

                if (_context.Doctors.Any(d => d.Hpid == doctor.Hpid))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "HPID already exists." });
                }

                if (_context.Doctors.Any(d => d.UserId == userId))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Doctor profile already exists for this user." });
                }

                doctor.IsApproved = false; // Pending approval
                _context.Doctors.Add(doctor);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetDoctor), null, doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating doctor.", Error = ex.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDoctor()
        {
            try
            {
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

                _context.Doctors.Remove(doctor);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting doctor.", Error = ex.Message });
            }
        }

        private bool IsValidHpid(string hpid)
        {
            if (string.IsNullOrEmpty(hpid) || hpid.Length != 14 || !long.TryParse(hpid, out _))
            {
                return false;
            }

            if (!DateTime.TryParseExact(hpid.Substring(0, 8), "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var permitDate))
            {
                return false;
            }

            if (permitDate > DateTime.Today)
            {
                return false;
            }

            var stateCode = hpid.Substring(8, 2);
            var validStateCodes = Enumerable.Range(1, 39).Select(i => i.ToString("D2")).ToHashSet();
            if (!validStateCodes.Contains(stateCode))
            {
                return false;
            }

            var sequentialNumber = int.Parse(hpid.Substring(10, 4));
            return sequentialNumber >= 1 && sequentialNumber <= 9999;
        }
    }
}