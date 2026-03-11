using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using BCrypt.Net;

namespace Infrastructure.Services
{
    public class BCryptPasswordHasher : IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user, string hashedPassword, string providedPassword)
        {
            if (BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword))
                return PasswordVerificationResult.Success;

            return PasswordVerificationResult.Failed;
        }
    }
}
