namespace Debugging_Doctors.Models.DTO
{
    public class PatientDetailsDto
    {
        public string FullName { get; set; }
        public DateOnly? Dob { get; set; }
        public string Gender { get; set; }
        public string ContactNo { get; set; }
        public string Address { get; set; }
        public string AadhaarNo { get; set; }
    }
}
