using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PapageorgiouHospitalQueueSystem.Data;
using PapageorgiouHospitalQueueSystem.Hubs;
using PapageorgiouHospitalQueueSystem.Models;

namespace PapageorgiouHospitalQueueSystem.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<QueueHub> _hubContext;


        public PatientService(AppDbContext context, IHubContext<QueueHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

        public async Task<Patient?> CallPatientAsync(int patientId, int doctorsOfficeId)
        {
            var existingActive = await _context.Patients
        .FirstOrDefaultAsync(p => p.CurrentDoctorsOfficeId == doctorsOfficeId && p.Status == 1);

            if (existingActive != null)
                return null;

            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null || patient.Status != 0) return null;

            patient.Status = 1;
            patient.CurrentDoctorsOfficeId = doctorsOfficeId;
            patient.CalledAt = DateTime.UtcNow;

            _context.PatientCalls.Add(new PatientCall
            {
                PatientId = patientId,
                DoctorsOfficeId = doctorsOfficeId,
                CalledAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await BroadcastPatientUpdate(patient);

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
            await BroadcastPatientUpdate(patient);

            return patient;
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }

        public async Task<Patient?> ReleasePatientAsync(int patientId)
        {
            var patient = await _context.Patients.Include(p => p.CurrentDoctorsOffice)
                .FirstOrDefaultAsync(p => p.Id == patientId && p.Status == 1);

            if (patient == null) return null;

            patient.Status = 0;
            patient.CurrentDoctorsOfficeId = null;
            patient.CalledAt = null;
            patient.CompletedAt = null;

            await _context.SaveChangesAsync();
            await BroadcastPatientUpdate(patient);
            return patient;
        }
        private async Task BroadcastPatientUpdate(Patient patient)
        {
            await _hubContext.Clients.All.SendAsync("PatientUpdated", new
            {
                patientId = patient.Id,
                officeId = patient.CurrentDoctorsOfficeId,
                patientNumber = patient.PatientNumber,
                status = patient.Status
            });
        }
    }
}
