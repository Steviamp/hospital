using PapageorgiouHospitalQueueSystem.Models;

namespace PapageorgiouHospitalQueueSystem.Services
{
    public interface IPatientService
    {
        Task<Patient> CreatePatientAsync(Patient patient);
        Task<IEnumerable<Patient>> GetPatientsByStatusAsync(int status);
        Task<Patient?> GetCurrentPatientAsync(int doctorsOfficeId);
        Task<Patient?> CallPatientAsync(int patientId, int doctorsOfficeId);
        Task<Patient?> CompletePatientAsync(int patientId);
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> ReleasePatientAsync(int patientId);

    }
}
