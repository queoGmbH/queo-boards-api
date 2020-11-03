namespace Queo.Boards.Core.Models {
    /// <summary>
    /// Model zum Aktualisieren eine Nutzer-Passworts
    /// </summary>
    public class UserUpdatePasswordModel {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="newPassword"></param>
        public UserUpdatePasswordModel(string newPassword) {
            NewPassword = newPassword;
        }

        /// <summary>
        /// Das neue Nutzer-Passwort
        /// </summary>
        public string NewPassword { get; }
    }
}