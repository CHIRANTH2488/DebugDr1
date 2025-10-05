namespace Debugging_Doctors.Models.DTO
{
    public class ApprovalDto
    {
        public int Id { get; set; }  // DocID or PatientID
        public string Role { get; set; }  // "Doctor" or "Patient"
        public bool IsApproved { get; set; }
    }
}
