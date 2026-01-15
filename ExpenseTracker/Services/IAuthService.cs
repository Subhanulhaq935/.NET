using ExpenseTracker.DTOs.Auth;

namespace ExpenseTracker.Services;

/// <summary>
/// Service interface for authentication business logic
/// </summary>
public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterRequestDto registerDto);
    Task<UserDto> LoginAsync(LoginRequestDto loginDto);
    Task<UserDto?> GetUserByIdAsync(string userId);
}