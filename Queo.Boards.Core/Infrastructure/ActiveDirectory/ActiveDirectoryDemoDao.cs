using System.Collections.Generic;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {
    
    /// <summary>
    /// Diese Klasse dient nur als Fake des AD-Daos für die Demoversion.
    /// </summary>
    public class ActiveDirectoryDemoDao : IActiveDirectoryDao {
        /// <summary>
        /// Ruft eine Eigenschaft eines ActiveDirectory-Nutzers ab.
        /// Existiert der Nutzer nicht oder ist kein Zugriff möglich, wird eine leere Zeichenfolge zurückgegeben.
        /// </summary>
        /// <param name="username">Der Nutzername für den die Eigenschaft abgerufen werden soll.</param>
        /// <param name="property">Welche Eigenschaft des Nutzers soll abgerufen werden?</param>
        /// <returns></returns>
        public string GetProperty(string username, ActiveDirectoryUserProperty property) {
            return "";
        }

        /// <summary>
        /// Ruft bestimmte Informationen über einen Nutzer ab.
        /// </summary>
        /// <param name="username">Der Nutzer für den die Informationen abgerufen werden sollen.</param>
        /// <returns>Können für den Nutzernamen keine Informationen abgerufen werden wird null, andernfalls ein Objekt mit Informationen geliefert.</returns>
        public UserNtInformation FindUserInformation(string username) {
            return new UserNtInformation("Vorname", "Nachname", "Firma", "Abteilung", "e@mail.de", "0123/456789");
        }

        /// <summary>
        /// Ruft ab, ob der angegebene Nutzername vorhanden ist oder nicht.
        /// </summary>
        /// <param name="username">Der zu überprüfende Nutzername</param>
        /// <returns>true wenn der Name gefunden wurde, false wenn nicht</returns>
        public bool IsExistingUsername(string username) {
            return true;
        }

        public bool Authenticate(string username, string password) {
            return true;
        }

        public IList<string> FindUserNamesByGroupName(string activeDirectoryGroupName) {
            return new List<string>();
        }
    }
}