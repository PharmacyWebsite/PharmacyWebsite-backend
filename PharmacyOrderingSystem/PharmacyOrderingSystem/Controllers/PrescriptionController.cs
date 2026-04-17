using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyOrderingSystem.DTOs;
using PharmacyOrderingSystem.Services;

namespace PharmacyOrderingSystem.Controllers
{
    [ApiController]
    [Route("api/prescriptions")]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly PrescriptionService _service;

        public PrescriptionController(PrescriptionService service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Upload(int userId, IFormFile file)
        {
            try
            {
                var result = await _service.Upload(userId, file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Upload failed");
            }
        }

        [HttpPut("validate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Validate(int id, UpdatePrescriptionStatusDto dto)
        {
            try
            {
                var result = await _service.Validate(id, dto.Status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Validation failed");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }
    }
}