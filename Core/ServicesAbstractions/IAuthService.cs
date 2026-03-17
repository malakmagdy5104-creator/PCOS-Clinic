using Shared.DTOs.AuthDto;

namespace ServicesAbstractions
{
    public interface IAuthService
    {
        Task<ReturnAuthDto> RegisterAsync(RegisterDto model);
        Task<ReturnAuthDto> LoginAsync(LoginDto model);
        Task<ReturnAuthDto> GetCurrentUserAsync(string email);
        Task<ReturnAuthDto> UpdateProfileAsync(string currentEmail, UpdateProfileDto model);
        Task<ReturnAuthDto> ChangePasswordAsync(string email, ChangePasswordDto model);
    }
}
