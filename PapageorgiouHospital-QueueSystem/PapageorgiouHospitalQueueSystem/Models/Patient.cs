namespace PapageorgiouHospitalQueueSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string? PatientNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string? Comment { get; set; }
        public int Status { get; set; } // 0 = waiting, 1 = called, 2 = completed

        public int? CurrentDoctorsOfficeId { get; set; }
        public DoctorsOffice? CurrentDoctorsOffice { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? CalledAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<PatientCall>? PatientCalls { get; set; }
    }
}
