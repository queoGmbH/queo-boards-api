using Microsoft.AspNet.Identity;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service regarding security operations.
    /// </summary>
    public class SecurityService : ISecurityService {
        /// <summary>
        ///     Hashes a password.
        /// </summary>
        /// <param name="password">Clear text password</param>
        /// <returns>The hashed password</returns>
        public string HashPassword(string password) {
            return new PasswordHasher().HashPassword(password);
        }
    }
}