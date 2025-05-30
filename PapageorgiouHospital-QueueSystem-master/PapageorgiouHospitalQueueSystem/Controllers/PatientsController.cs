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

        private static PatientResponse MapToResponse(Patient p) => new PatientResponse
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
        };

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

            return Ok(MapToResponse(created));
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
            return patients.Select(MapToResponse);
        }

        // GET api/patients/current/{doctorsOfficeId}
        [HttpGet("current/{doctorsOfficeId}")]
        public async Task<ActionResult<PatientResponse>> GetCurrentPatient(int doctorsOfficeId)
        {
            var patient = await _patientService.GetCurrentPatientAsync(doctorsOfficeId);
            if (patient == null) return NotFound();

            return Ok(MapToResponse(patient));
        }

        // PUT api/patients/{id}/call?doctorsOfficeId=2
        [HttpPut("{id}/call")]
        public async Task<IActionResult> CallPatient(int id, [FromBody] CallPatientRequest request)
        {
            var updated = await _patientService.CallPatientAsync(id, request.DoctorsOfficeId);
            if (updated == null) return NotFound();

            return Ok(MapToResponse(updated));
        }

        // PUT api/patients/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompletePatient(int id)
        {
            return await HandlePatientStatusUpdate(_patientService.CompletePatientAsync, id);
        }

        [HttpPut("{id}/release")]
        public async Task<IActionResult> ReleasePatient(int id)
        {
            return await HandlePatientStatusUpdate(_patientService.ReleasePatientAsync, id);
        }

        // GET api/patients/active-calls
        [HttpGet("active-calls")]
        public async Task<IEnumerable<PatientResponse>> GetActiveCalls()
        {
            var active = await _patientService.GetPatientsByStatusAsync(1);
            return active.Select(MapToResponse);
        }

        private async Task<IActionResult> HandlePatientStatusUpdate(Func<int, Task<Patient?>> operation, int patientId)
        {
            var patient = await operation(patientId);
            if (patient == null) return NotFound();
            return Ok(MapToResponse(patient));
        }
    }
}
