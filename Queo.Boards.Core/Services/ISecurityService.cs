namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Interface for a service regarding security related operations.
    /// </summary>
    public interface ISecurityService {
        /// <summary>
        ///     Hashes a password.
        /// </summary>
        /// <param name="password">Clear text password</param>
        /// <returns>The hashed password</returns>
        string HashPassword(string password);
    }
}