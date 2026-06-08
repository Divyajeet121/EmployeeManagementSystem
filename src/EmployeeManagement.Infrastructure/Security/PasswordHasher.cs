using EmployeeManagement.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _identityHasher = new();

    public string Hash(string password)
    {
        return _identityHasher.HashPassword(new object(), password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var result = _identityHasher.VerifyHashedPassword(new object(), passwordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
