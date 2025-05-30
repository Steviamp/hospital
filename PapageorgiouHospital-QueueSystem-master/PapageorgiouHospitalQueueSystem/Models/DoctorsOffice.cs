namespace PapageorgiouHospitalQueueSystem.Models
{
    public class DoctorsOffice
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<PatientCall>? PatientCalls { get; set; }
    }
}
