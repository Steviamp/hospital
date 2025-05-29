using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Data;
using PapageorgiouHospitalQueueSystem.Models;

namespace PapageorgiouHospitalQueueSystem.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> CreatePatientAsync(Patient patient)
        {
            patient.Status = 0;
            patient.CreatedAt = DateTime.UtcNow;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return patient;
        }

        public async Task<Patient?> GetCurrentPatientAsync(int doctorsOfficeId)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.Status == 1 && p.CurrentDoctorsOfficeId == doctorsOfficeId);
        }
        public async Task<IEnumerable<Patient>> GetPatientsByStatusAsync(int status)
        {
            return await _context.Patients
                .Include(p => p.CurrentDoctorsOffice)
                .Where(p => p.Status == status)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient?> CallPatientAsync(int patientId, int officeId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null || patient.Status == 2) return null;

            patient.Status = 1;
            patient.CurrentDoctorsOfficeId = officeId;
            patient.CalledAt = DateTime.UtcNow;

            _context.PatientCalls.Add(new PatientCall
            {
                PatientId = patientId,
                DoctorsOfficeId = officeId,
                CalledAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync(); 

            return patient;
        }

        public async Task<Patient?> CompletePatientAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.PatientCalls)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null || patient.Status != 1) return null;

            patient.Status = 2;
            patient.CompletedAt = DateTime.UtcNow;

            var latestCall = await _context.PatientCalls
                .Where(pc => pc.PatientId == patientId && pc.CompletedAt == null)
                .OrderByDescending(pc => pc.CalledAt)
                .FirstOrDefaultAsync();

            if (latestCall != null)
                latestCall.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }
    }
}
