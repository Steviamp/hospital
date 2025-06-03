namespace PapageorgiouHospitalQueueSystem.Models
{
    public class PatientCall
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int DoctorsOfficeId { get; set; }
        public DoctorsOffice? DoctorsOffice { get; set; }

        public DateTime CalledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
