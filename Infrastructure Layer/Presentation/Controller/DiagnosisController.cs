using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controller;
using ServicesAbstractions;
using Shared.DTOs.Diagnosis;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    [Authorize]
    public class DiagnosisController : BaseController
    {
        private readonly IDiagnosisService _diagnosisService;

        public DiagnosisController(IDiagnosisService diagnosisService)
        {
            _diagnosisService = diagnosisService;
        }

        [HttpPost("submit-clinical-data")]
        public async Task<IActionResult> SubmitData([FromBody] ClinicalDataRequestDto model)
        {
            if (model == null)
                return BadRequest("Invalid medical data.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("uid")
                         ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { error = "User identifier not found in token claims." });

            var result = await _diagnosisService.SaveClinicalDataAsync(model, userId);

            if (result)
            {
                return Ok(new
                {
                    Message = "Your clinical data has been saved successfully.",
                    NextStep = "Please upload your ultrasound image for full analysis."
                });
            }

            return BadRequest("Something went wrong while saving your data.");
        }

        [HttpPost("analyze")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Analyze( IFormFile imageFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("uid")
                         ?? User.FindFirstValue("sub");

            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Please upload an ultrasound image.");

            var result = await _diagnosisService.AnalyzePcosAsync(userId, imageFile);

            if (result == null)
                return BadRequest("Please submit your clinical data first.");

            return Ok(result);
        }
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب يوزر id من الـ Token
            var history = await _diagnosisService.GetUserHistoryAsync(userId);
            return Ok(history);
        }
    }
}