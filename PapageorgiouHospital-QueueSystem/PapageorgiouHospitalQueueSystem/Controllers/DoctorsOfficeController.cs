using Microsoft.AspNetCore.Mvc;
using PapageorgiouHospitalQueueSystem.Models;
using PapageorgiouHospitalQueueSystem.Models.Responses;
using PapageorgiouHospitalQueueSystem.Services;

namespace PapageorgiouHospitalQueueSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsOfficeController : ControllerBase
    {
        private readonly IDoctorsOfficeService _doctorsOfficeService;
        public DoctorsOfficeController(IDoctorsOfficeService doctorsOfficeService)
        {
            _doctorsOfficeService = doctorsOfficeService;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<DoctorsOfficeResponse>> GetAll()
        {
            var offices = await _doctorsOfficeService.GetAllAsync();
            return offices.Select(o => new DoctorsOfficeResponse
            {
                Id = o.Id,
                Name = o.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoctorsOffice office)
        {
            var created = await _doctorsOfficeService.CreateAsync(office);
            return Ok(new DoctorsOfficeResponse
            {
                Id = created.Id,
                Name = created.Name
            });
        }
    }
}
