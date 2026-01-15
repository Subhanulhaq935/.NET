using ExpenseTracker.DTOs.Auth;
using ExpenseTracker.Helpers;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories;

namespace ExpenseTracker.Services;

/// <summary>
/// Service implementation for authentication business logic
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registers a new user with business logic validation
    /// Business Rules:
    /// - Email must be unique
    /// - Password must be at least 6 characters (enforced by DTO)
    /// </summary>
    public async Task<UserDto> RegisterAsync(RegisterRequestDto registerDto)
    {
        // Business rule: Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered");
        }

        // Business logic: Hash password before storing
        var passwordHash = PasswordHelper.HashPassword(registerDto.Password);

        // Create user entity
        var user = new User
        {
            Email = registerDto.Email.ToLowerInvariant(), // Normalize email
            PasswordHash = passwordHash,
            FullName = registerDto.FullName,
            CreatedAt = DateTime.UtcNow
        };

        // Persist user
        var createdUser = await _userRepository.CreateAsync(user);

        // Return user DTO (without password)
        return new UserDto
        {
            Id = createdUser.Id,
            Email = createdUser.Email,
            FullName = createdUser.FullName
        };
    }

    /// <summary>
    /// Authenticates a user
    /// Business Rules:
    /// - User must exist
    /// - Password must match
    /// </summary>
    public async Task<UserDto> LoginAsync(LoginRequestDto loginDto)
    {
        // Business logic: Find user by email
        var user = await _userRepository.GetByEmailAsync(loginDto.Email.ToLowerInvariant());
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Business logic: Verify password
        if (!PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Return user DTO (without password)
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    /// <summary>
    /// Gets user by ID
    /// </summary>
    public async Task<UserDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }
}
