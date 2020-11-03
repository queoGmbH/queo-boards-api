using System.Collections.Generic;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {
    /// <summary>
    ///   Schnittstelle f�r einen Dao, der Methoden zum Zugriff auf das Active Directory bietet.
    /// </summary>
    public interface IActiveDirectoryDao {

        /// <summary>
        ///   Ruft ab, ob der angegebene Nutzername vorhanden ist oder nicht.
        /// </summary>
        /// <param name="username"> Der zu �berpr�fende Nutzername </param>
        /// <returns> true wenn der Name gefunden wurde, false wenn nicht </returns>
        bool IsExistingUsername(string username);

        /// <summary>
        ///   Ruft bestimmte Informationen �ber einen Nutzer ab.
        /// </summary>
        /// <param name="username"> Der Nutzer f�r den die Informationen abgerufen werden sollen. </param>
        /// <returns> K�nnen f�r den Nutzernamen keine Informationen abgerufen werden wird null, andernfalls ein Objekt mit Informationen geliefert. </returns>
        UserNtInformation FindUserInformation(string username);

        /// <summary>
        /// �berpr�ft, ob die Authentifizierung mit den �bergebenen Anmeldedaten m�glich ist.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Authenticate(string username, string password);

        /// <summary>
        /// Sucht nach den Namen der Nutzer, die in einer bestimmten Gruppe sind.
        /// </summary>
        /// <param name="activeDirectoryGroupName"></param>
        /// <returns></returns>
        IList<string> FindUserNamesByGroupName(string activeDirectoryGroupName);
    }
}