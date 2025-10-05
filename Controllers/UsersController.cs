using Debugging_Doctors.Models;
using Debugging_Doctors.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly DebuggingDoctorsContext _context;

        public UsersController(DebuggingDoctorsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region User CRUD
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                if (!users.Any())
                {
                    return NotFound(new { Message = "No users found." });
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving users.", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid user ID." });
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving user.", Error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            try
            {
                if (id <= 0 || id != user?.UserId)
                {
                    return BadRequest(new { Message = "Invalid user ID or mismatch with request body." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { Message = "Invalid user data.", Errors = errors });
                }

                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(u => u.UserId == id))
                    {
                        return NotFound(new { Message = $"User with ID {id} not found." });
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error updating user.", Error = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new { Message = "Invalid user data.", Errors = errors });
                }

                _context.Users.Add(user);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating user.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid user ID." });
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { Message = $"User with ID {id} not found." });
                }

                _context.Users.Remove(user);
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error deleting user.", Error = ex.Message });
            }
        }
        #endregion

        #region Doctor CRUD
        [HttpGet("Doctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            try
            {
                var doctors = await _context.Doctors.Where(d => d.IsApproved).ToListAsync();
                if (!doctors.Any())
                {
                    return NotFound(new { Message = "No approved doctors found." });
                }
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving doctors.", Error = ex.Message });
            }
        }

        [HttpGet("Doctors/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid doctor ID." });
                }

                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DocId == id && d.IsApproved);
                if (doctor == null)
                {
                    return NotFound(new { Message = $"Doctor with ID {id} not found or not approved." });
                }

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving doctor.", Error = ex.Message });
            }
        }

        [HttpPut("Doctors/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            try
            {
                if (id <= 0 || id != doctor?.DocId)
                {
                    return BadRequest(new { Message = "Invalid doctor ID or mismatch with request body." });
                }

                if (!ModelState.IsValid || !IsValidHpid(doctor.Hpid))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidHpid(doctor.Hpid))
                    {
                        errors.Add("Invalid Hpid format. It must be a 14-digit number in the format YYYYMMDDSSNNNN.");
                    }
                    return BadRequest(new { Message = "Invalid doctor data.", Errors = errors });
                }

                if (_context.Doctors.Any(d => d.Hpid == doctor.Hpid && d.DocId != id))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Hpid already exists for another doctor." });
                }

                _context.Entry(doctor).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Doctors.Any(d => d.DocId == id))
                    {
                        return NotFound(new { Message = $"Doctor with ID {id} not found." });
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

        [HttpPost("Doctors")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            try
            {
                if (!ModelState.IsValid || !IsValidHpid(doctor.Hpid))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidHpid(doctor.Hpid))
                    {
                        errors.Add("Invalid Hpid format. It must be a 14-digit number in the format YYYYMMDDSSNNNN.");
                    }
                    return BadRequest(new { Message = "Invalid doctor data.", Errors = errors });
                }

                if (_context.Doctors.Any(d => d.Hpid == doctor.Hpid))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "Hpid already exists." });
                }

                doctor.IsApproved = true; // Admin-created doctors are auto-approved
                _context.Doctors.Add(doctor);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.DocId }, doctor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating doctor.", Error = ex.Message });
            }
        }

        [HttpDelete("Doctors/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid doctor ID." });
                }

                var doctor = await _context.Doctors.FindAsync(id);
                if (doctor == null)
                {
                    return NotFound(new { Message = $"Doctor with ID {id} not found." });
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
        #endregion

        #region Patient CRUD
        [HttpGet("Patients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            try
            {
                var patients = await _context.Patients.Where(p => p.IsApproved).ToListAsync();
                if (!patients.Any())
                {
                    return NotFound(new { Message = "No approved patients found." });
                }
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving patients.", Error = ex.Message });
            }
        }

        [HttpGet("Patients/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid patient ID." });
                }

                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && p.IsApproved);
                if (patient == null)
                {
                    return NotFound(new { Message = $"Patient with ID {id} not found or not approved." });
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving patient.", Error = ex.Message });
            }
        }

        [HttpPut("Patients/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            try
            {
                if (id <= 0 || id != patient?.PatientId)
                {
                    return BadRequest(new { Message = "Invalid patient ID or mismatch with request body." });
                }

                if (!ModelState.IsValid || !IsValidAadhaarNo(patient.AadhaarNo))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidAadhaarNo(patient.AadhaarNo))
                    {
                        errors.Add("Invalid AadhaarNo. It must be a 12-digit numeric string starting with 2-9.");
                    }
                    return BadRequest(new { Message = "Invalid patient data.", Errors = errors });
                }

                if (_context.Patients.Any(p => p.AadhaarNo == patient.AadhaarNo && p.PatientId != id))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "AadhaarNo already exists for another patient." });
                }

                _context.Entry(patient).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Patients.Any(p => p.PatientId == id))
                    {
                        return NotFound(new { Message = $"Patient with ID {id} not found." });
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

        [HttpPost("Patients")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            try
            {
                if (!ModelState.IsValid || !IsValidAadhaarNo(patient.AadhaarNo))
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    if (!IsValidAadhaarNo(patient.AadhaarNo))
                    {
                        errors.Add("Invalid AadhaarNo. It must be a 12-digit numeric string starting with 2-9.");
                    }
                    return BadRequest(new { Message = "Invalid patient data.", Errors = errors });
                }

                if (_context.Patients.Any(p => p.AadhaarNo == patient.AadhaarNo))
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { Message = "AadhaarNo already exists." });
                }

                patient.IsApproved = true; // Admin-created patients are auto-approved
                _context.Patients.Add(patient);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Database error.", Error = ex.InnerException?.Message ?? ex.Message });
                }

                return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating patient.", Error = ex.Message });
            }
        }

        [HttpDelete("Patients/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid patient ID." });
                }

                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    return NotFound(new { Message = $"Patient with ID {id} not found." });
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
        #endregion

        #region Approval Endpoints
        [HttpGet("PendingDoctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetPendingDoctors()
        {
            try
            {
                var doctors = await _context.Doctors.Where(d => !d.IsApproved).ToListAsync();
                if (!doctors.Any())
                {
                    return NotFound(new { Message = "No pending doctors found." });
                }
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving pending doctors.", Error = ex.Message });
            }
        }

        [HttpGet("PendingPatients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPendingPatients()
        {
            try
            {
                var patients = await _context.Patients.Where(p => !p.IsApproved).ToListAsync();
                if (!patients.Any())
                {
                    return NotFound(new { Message = "No pending patients found." });
                }
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error retrieving pending patients.", Error = ex.Message });
            }
        }

        [HttpPut("Approve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Approve(ApprovalDto dto)
        {
            try
            {
                if (dto == null || dto.Id <= 0 || (dto.Role != "Doctor" && dto.Role != "Patient"))
                {
                    return BadRequest(new { Message = "Invalid approval data or role." });
                }

                if (dto.Role == "Doctor")
                {
                    var doctor = await _context.Doctors.FindAsync(dto.Id);
                    if (doctor == null)
                    {
                        return NotFound(new { Message = $"Doctor with ID {dto.Id} not found." });
                    }
                    if (dto.IsApproved)
                    {
                        doctor.IsApproved = true;
                        _context.Entry(doctor).State = EntityState.Modified;
                    }
                    else
                    {
                        _context.Doctors.Remove(doctor); // Decline = delete
                    }
                }
                else if (dto.Role == "Patient")
                {
                    var patient = await _context.Patients.FindAsync(dto.Id);
                    if (patient == null)
                    {
                        return NotFound(new { Message = $"Patient with ID {dto.Id} not found." });
                    }
                    if (dto.IsApproved)
                    {
                        patient.IsApproved = true;
                        _context.Entry(patient).State = EntityState.Modified;
                    }
                    else
                    {
                        _context.Patients.Remove(patient); // Decline = delete
                    }
                }

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
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error processing approval.", Error = ex.Message });
            }
        }
        #endregion

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

        private bool IsValidAadhaarNo(string aadhaarNo)
        {
            if (string.IsNullOrEmpty(aadhaarNo) || aadhaarNo.Length != 12 || !long.TryParse(aadhaarNo, out _))
            {
                return false;
            }
            return !aadhaarNo.StartsWith("0") && !aadhaarNo.StartsWith("1");
        }
    }
}