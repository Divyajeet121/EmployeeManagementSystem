using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Exceptions;
using EmployeeManagement.Application.Interfaces;

namespace EmployeeManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new AuthenticationFailedException("Invalid username or password.");
        }

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            ExpiresAt = expiresAt
        };
    }
}
