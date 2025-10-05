namespace Debugging_Doctors.Models.DTO
{
    public class PrescriptionDto
    {
        public string ChiefComplaints { get; set; }
        public string PastHistory { get; set; }
        public string Examination { get; set; }
        public List<Medication> Medications { get; set; } = new List<Medication>();
        public string Advice { get; set; }
        public string Signature { get; set; }
    }

    public class Medication
    {
        public string Name { get; set; }
        public string Timing { get; set; }  // e.g., "Morning/Afternoon/Night"
        public string BeforeAfterFood { get; set; }
    }
}
