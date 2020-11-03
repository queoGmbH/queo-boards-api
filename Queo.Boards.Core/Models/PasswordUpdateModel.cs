namespace Queo.Boards.Core.Models {
    /// <summary>
    /// Model zum Aktualisieren des Passworts vom Nutzer
    /// </summary>
    public class PasswordUpdateModel {
        /// <summary>
        /// Das alte Passwort.
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// Das neue Passwort.
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Die Wiederholung vom neuen Passwort.
        /// </summary>
        public string NewPasswordConfirmation { get; set; }

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public PasswordUpdateModel() {
        }

    }
}