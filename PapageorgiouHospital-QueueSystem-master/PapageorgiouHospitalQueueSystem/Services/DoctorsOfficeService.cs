using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Data;
using PapageorgiouHospitalQueueSystem.Models;

namespace PapageorgiouHospitalQueueSystem.Services
{
    public class DoctorsOfficeService : IDoctorsOfficeService
    {
        private readonly AppDbContext _context;

        public DoctorsOfficeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorsOffice>> GetAllAsync()
        {
            return await _context.DoctorsOffices.ToListAsync();
        }

        public async Task<DoctorsOffice> CreateAsync(DoctorsOffice office)
        {
            _context.DoctorsOffices.Add(office);
            await _context.SaveChangesAsync();
            return office;
        }

    }
}
