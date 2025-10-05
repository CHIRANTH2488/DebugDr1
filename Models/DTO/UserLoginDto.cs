using System.ComponentModel.DataAnnotations;

namespace Debugging_Doctors.Models.DTO
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PswdHash { get; set; }
    }
}
