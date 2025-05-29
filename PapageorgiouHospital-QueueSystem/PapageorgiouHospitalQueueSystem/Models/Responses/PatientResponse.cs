namespace PapageorgiouHospitalQueueSystem.Models.Responses
{
    public class PatientResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PatientNumber { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public int Status { get; set; }

        public string? DoctorsOfficeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CalledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
