namespace Debugging_Doctors.Models.DTO
{
    public class AppointmentDetailDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string DoctorSpecialisation { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Medicines { get; set; }
        public string InvoiceStatus { get; set; }
        public decimal? InvoiceAmount { get; set; }
    }
}
