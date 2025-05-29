using PapageorgiouHospitalQueueSystem.Models;

namespace PapageorgiouHospitalQueueSystem.Services
{
    public interface IDoctorsOfficeService
    {
        Task<IEnumerable<DoctorsOffice>> GetAllAsync();
        Task<DoctorsOffice> CreateAsync(DoctorsOffice office);

    }
}
