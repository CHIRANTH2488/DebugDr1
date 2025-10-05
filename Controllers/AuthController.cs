using Debugging_Doctors.Models;
using Debugging_Doctors.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hospital_Management_System.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DebuggingDoctorsContext _context;
        private readonly IConfiguration _config;

        public AuthController(DebuggingDoctorsContext context, IConfiguration config)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register(UserRegistrationDto dto)
        {
            try
            {
                if (_context.Users.Any(u => u.Email == dto.Email))
                    return BadRequest(new { Message = "Email already exists." });

                var user = new User
                {
                    Email = dto.Email,
                    PswdHash = dto.PswdHash, // In production, hash the password using BCrypt or similar!
                    Role = dto.Role,
                    CreatedAt = DateTime.Now
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (dto.Role == "Doctor")
                {
                    if (!IsValidHpid(dto.Hpid))
                        return BadRequest(new { Message = "Invalid Hpid format. It must be a 14-digit number in the format YYYYMMDDSSNNNN." });

                    if (_context.Doctors.Any(d => d.Hpid == dto.Hpid))
                        return StatusCode(StatusCodes.Status409Conflict, new { Message = "Hpid already exists." });

                    var doctor = new Doctor
                    {
                        UserId = user.UserId,
                        FullName = dto.FullName,
                        Specialisation = dto.Specialisation,
                        Hpid = dto.Hpid,
                        Availability = dto.Availability,
                        ContactNo = dto.ContactNo,
                        IsApproved = false
                    };
                    _context.Doctors.Add(doctor);
                }
                else if (dto.Role == "Patient")
                {
                    if (!IsValidAadhaarNo(dto.AadhaarNo))
                        return BadRequest(new { Message = "Invalid AadhaarNo. It must be a 12-digit numeric string starting with 2-9." });

                    if (_context.Patients.Any(p => p.AadhaarNo == dto.AadhaarNo))
                        return StatusCode(StatusCodes.Status409Conflict, new { Message = "AadhaarNo already exists." });

                    var patient = new Patient
                    {
                        UserId = user.UserId,
                        FullName = dto.FullName,
                        Dob = dto.Dob,
                        Gender = dto.Gender,
                        ContactNo = dto.ContactNo,
                        Address = dto.Address,
                        AadhaarNo = dto.AadhaarNo,
                        IsApproved = false
                    };
                    _context.Patients.Add(patient);
                }
                else
                {
                    return BadRequest(new { Message = "Invalid role. Must be 'Doctor' or 'Patient'." });
                }

                await _context.SaveChangesAsync();
                return Ok(new { Message = "Registration sent for approval." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error during registration.", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email && u.PswdHash == dto.PswdHash); // Hash in production!
                if (user == null) return Unauthorized(new { Message = "Invalid credentials." });

                bool isApproved = false;
                if (user.Role == "Doctor")
                {
                    var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == user.UserId);
                    isApproved = doctor?.IsApproved ?? false;
                }
                else if (user.Role == "Patient")
                {
                    var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == user.UserId);
                    isApproved = patient?.IsApproved ?? false;
                }
                else if (user.Role == "Admin")
                {
                    isApproved = true; // Admins are always approved
                }

                if (!isApproved) return Unauthorized(new { Message = "Account not approved." });

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error during login.", Error = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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