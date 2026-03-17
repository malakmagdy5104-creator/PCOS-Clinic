using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.DTOs.AuthDto;
using System.Security.Claims;

namespace Presentation.Controller.Account
{
     
        public class AccountController(IAuthService _authService) :BaseController
        {
         
        [HttpPost("register")]
            public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RegisterAsync(model);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                return Ok(result);
            }

            [HttpPost("login")]
            public async Task<IActionResult> LoginAsync([FromBody] LoginDto model)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(model);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                return Ok(result);
            }
        [Authorize] // This attribute ensures that ONLY users with a valid token can enter
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Extract the email claim from the JWT token provided in the request header
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Call the service to get user details
            var result = await _authService.GetCurrentUserAsync(email);

            return Ok(result);
        }
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _authService.UpdateProfileAsync(email, model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _authService.ChangePasswordAsync(email, model);
            return Ok(result);
        }
    }
    }

