using Microsoft.AspNetCore.Mvc;
using PapageorgiouHospitalQueueSystem.Models;
using PapageorgiouHospitalQueueSystem.Models.Requests;
using PapageorgiouHospitalQueueSystem.Models.Responses;
using PapageorgiouHospitalQueueSystem.Services;

namespace PapageorgiouHospitalQueueSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // POST api/patients
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            var patient = new Patient
            {
                Name = request.Name,
                PatientNumber = request.PatientNumber,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow,
                Status = 0
            };

            var created = await _patientService.CreatePatientAsync(patient);

            return Ok(new PatientResponse
            {
                Id = created.Id,
                Name = created.Name,
                PatientNumber = created.PatientNumber,
                Comment = created.Comment,
                Status = created.Status,
                CreatedAt = created.CreatedAt,
                CalledAt = created.CalledAt,
                CompletedAt = created.CompletedAt,
                DoctorsOfficeName = created.CurrentDoctorsOffice?.Name
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _patientService.GetByIdAsync(id);
            return patient == null ? NotFound() : Ok(patient);
        }

        // GET api/patients/waiting
        [HttpGet("waiting")]
        public async Task<IEnumerable<PatientResponse>> GetWaitingPatients()
        {
            var patients = await _patientService.GetPatientsByStatusAsync(0);
            return patients.Select(p => new PatientResponse
            {
                Id = p.Id,
                Name = p.Name,
                PatientNumber = p.PatientNumber,
                Comment = p.Comment,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                CalledAt = p.CalledAt,
                CompletedAt = p.CompletedAt,
                DoctorsOfficeName = p.CurrentDoctorsOffice?.Name
            });
        }

        // GET api/patients/current/{doctorsOfficeId}
        [HttpGet("current/{doctorsOfficeId}")]
        public async Task<ActionResult<PatientResponse>> GetCurrentPatient(int doctorsOfficeId)
        {
            var patient = await _patientService.GetCurrentPatientAsync(doctorsOfficeId);
            if (patient == null) return NotFound();

            return Ok(new PatientResponse
            {
                Id = patient.Id,
                Name = patient.Name,
                PatientNumber = patient.PatientNumber,
                Comment = patient.Comment,
                Status = patient.Status,
                CreatedAt = patient.CreatedAt,
                CalledAt = patient.CalledAt,
                CompletedAt = patient.CompletedAt,
                DoctorsOfficeName = patient.CurrentDoctorsOffice?.Name
            });
        }

        // PUT api/patients/{id}/call?doctorsOfficeId=2
        [HttpPut("{id}/call")]
        public async Task<IActionResult> CallPatient(int id, [FromBody] CallPatientRequest request)
        {
            var updated = await _patientService.CallPatientAsync(id, request.DoctorsOfficeId);
            if (updated == null) return NotFound();

            return Ok(new PatientResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                PatientNumber = updated.PatientNumber,
                Comment = updated.Comment,
                Status = updated.Status,
                CreatedAt = updated.CreatedAt,
                CalledAt = updated.CalledAt,
                CompletedAt = updated.CompletedAt,
                DoctorsOfficeName = updated.CurrentDoctorsOffice?.Name
            });
        }

        // PUT api/patients/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompletePatient(int id)
        {
            var updated = await _patientService.CompletePatientAsync(id);
            if (updated == null) return NotFound();

            return Ok(new PatientResponse
            {
                Id = updated.Id,
                Name = updated.Name,
                PatientNumber = updated.PatientNumber,
                Comment = updated.Comment,
                Status = updated.Status,
                CreatedAt = updated.CreatedAt,
                CalledAt = updated.CalledAt,
                CompletedAt = updated.CompletedAt,
                DoctorsOfficeName = updated.CurrentDoctorsOffice?.Name
            });
        }

        // GET api/patients/active-calls
        [HttpGet("active-calls")]
        public async Task<IEnumerable<PatientResponse>> GetActiveCalls()
        {
            var active = await _patientService.GetPatientsByStatusAsync(1);
            return active.Select(p => new PatientResponse
            {
                Id = p.Id,
                Name = p.Name,
                PatientNumber = p.PatientNumber,
                Comment = p.Comment,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                CalledAt = p.CalledAt,
                CompletedAt = p.CompletedAt,
                DoctorsOfficeName = p.CurrentDoctorsOffice?.Name
            });
        }
    }
}
