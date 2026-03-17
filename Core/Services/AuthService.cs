using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServicesAbstractions;
using Shared.DTOs.AuthDto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService(UserManager<ApplicationUser> _userManager, IConfiguration _configuration, IMapper _mapper) : IAuthService
    {
      
        public async Task<ReturnAuthDto> RegisterAsync(RegisterDto model)
        {
            // 1. Check if the email is already registered in the database
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new ReturnAuthDto { Message = "Email is already registered!" };

            // 2. Check if the username is already taken
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new ReturnAuthDto { Message = "Username is already taken!" };

            // 3. Map RegisterDto to ApplicationUser entity using AutoMapper
            var user = _mapper.Map<ApplicationUser>(model);

            // 4. Attempt to create the user and hash the password
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                // Concatenate identity errors if registration fails
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ReturnAuthDto { Message = errors };
            }

            // 5. Generate JWT token for the newly registered user
            var tokenData = await CreateJwtTokenAsync(user);

            return new ReturnAuthDto
            {
                IsAuthenticated = true,
                Message = "User registered successfully",
                Username = user.UserName,
                Token = tokenData.Token,
                ExpiresOn = tokenData.ExpiresOn
            };
        }

        
        /// Validates user credentials and returns a new authentication token.
        
        public async Task<ReturnAuthDto> LoginAsync(LoginDto model)
        {
            // 1. Verify if the user exists by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            // 2. Validate user existence and check password hash
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new ReturnAuthDto { Message = "Invalid Email or Password!" };

            // 3. Generate a fresh JWT token
            var tokenData = await CreateJwtTokenAsync(user);

            return new ReturnAuthDto
            {
                IsAuthenticated = true,
                Username = user.UserName,
                Token = tokenData.Token,
                ExpiresOn = tokenData.ExpiresOn,
                Message = "Login successful"
            };
        }



        public async Task<ReturnAuthDto> GetCurrentUserAsync(string email)
        {
            var user =await _userManager.FindByEmailAsync(email);
            if(user is  null)
                return new ReturnAuthDto { Message = "User not found!" };
            return new ReturnAuthDto
            {
                IsAuthenticated = true,
                Username = user.UserName,
                Message = "User data retrieved successfully"
            };
        }


        public async Task<ReturnAuthDto> UpdateProfileAsync(string currentEmail, UpdateProfileDto model)
        {
            var user = await _userManager.FindByEmailAsync(currentEmail);
            if (user == null) return new ReturnAuthDto { Message = "User not found!" };

            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ReturnAuthDto { Message = errors };
            }

            return new ReturnAuthDto { IsAuthenticated = true, Message = "Profile updated successfully" };
        }

        public async Task<ReturnAuthDto> ChangePasswordAsync(string email, ChangePasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new ReturnAuthDto { Message = "User not found!" };

          
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new ReturnAuthDto { Message = errors };
            }

            return new ReturnAuthDto { IsAuthenticated = true, Message = "Password changed successfully" };
        }

        private async Task<ReturnAuthDto> CreateJwtTokenAsync(ApplicationUser user)
        {
            // Define user-specific information (Claims) to be embedded in the token
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Retrieve JWT settings directly from appsettings.json via IConfiguration
            var key = _configuration["JWT:Key"];
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var durationInDays = double.Parse(_configuration["JWT:DurationInDays"] ?? "30");

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Create the security token descriptor
            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.UtcNow.AddDays(durationInDays),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new ReturnAuthDto
            {
                // Serialize the token to a string format
                Token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor),
                ExpiresOn = tokenDescriptor.ValidTo
            };
        }

        
    }
}