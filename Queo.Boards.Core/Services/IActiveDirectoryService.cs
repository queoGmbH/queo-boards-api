using System.Collections.Generic;
using Queo.Boards.Core.Infrastructure.ActiveDirectory;

namespace Queo.Boards.Core.Services {
    /// <summary>
    /// Schnittstelle, die einen Service beschreibt, der Methoden bereitstellt, um Nutzer-Informationen aus dem Active Directory abzurufen.
    /// </summary>
    public interface IActiveDirectoryService {

        /// <summary>
        /// Ruft eine Liste der Nutzernamen aus dem ActiveyDirectory ab, die als Anwender in der Rolle "Nutzer" für queo-boards definiert sind.
        /// </summary>
        /// <returns></returns>
        IList<string> FindQueoBoardsUsers();

        /// <summary>
        /// Ruft eine Liste der Nutzernamen aus dem ActiveyDirectory ab, die als Anwender in der Rolle "Administrator" für queo-boards definiert sind.
        /// </summary>
        IList<string> FindQueoBoardsAdministrators();

        /// <summary>
        /// Überprüft, ob sich ein Nutzer mit den übergebenen Credentials gegen das Active Directory authentifizieren kann.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool Authenticate(string username, string password);

        /// <summary>
        ///   Ruft bestimmte Informationen über einen Nutzer ab.
        /// </summary>
        /// <param name="username"> Der Nutzer für den die Informationen abgerufen werden sollen. </param>
        /// <returns> Können für den Nutzernamen keine Informationen abgerufen werden wird null, andernfalls ein Objekt mit Informationen geliefert. </returns>
        UserNtInformation FindUserInformation(string username);
    }
}