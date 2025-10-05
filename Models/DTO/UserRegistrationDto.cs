using System.ComponentModel.DataAnnotations;

namespace Debugging_Doctors.Models.DTO
{
    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PswdHash { get; set; }

        [Required]
        public string Role { get; set; }

        // Additional fields for Doctor/Patient
        public string FullName { get; set; }
        public string Specialisation { get; set; } // For Doctor
        public string Hpid { get; set; } // For Doctor
        public string Availability { get; set; } // For Doctor
        public string ContactNo { get; set; } // For both
        public DateOnly? Dob { get; set; } // For Patient
        public string Gender { get; set; } // For Patient
        public string Address { get; set; } // For Patient
        public string AadhaarNo { get; set; } // For Patient
    }
}
