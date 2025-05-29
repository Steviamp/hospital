namespace PapageorgiouHospitalQueueSystem.Models.Requests
{
    public class CreatePatientRequest
    {
        public string Name { get; set; } = string.Empty;
        public string PatientNumber { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}
