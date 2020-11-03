using System.Collections.Generic;
using Queo.Boards.Core.Infrastructure.ActiveDirectory;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service, der Methoden bereitstellt, um Nutzer-Informationen aus dem Active Directory abzurufen.
    /// </summary>
    public class ActiveDirectoryService : IActiveDirectoryService {
        /// <summary>
        ///     Konstruktor für Spring.
        /// </summary>
        public ActiveDirectoryService() {
        }

        /// <summary>
        ///     Konstruktor für Testfälle.
        /// </summary>
        /// <param name="activeDirectoryConfiguration"></param>
        /// <param name="activeDirectoryDao"></param>
        public ActiveDirectoryService(ActiveDirectoryConfiguration activeDirectoryConfiguration, IActiveDirectoryDao activeDirectoryDao) {
            ActiveDirectoryConfiguration = activeDirectoryConfiguration;
            ActiveDirectoryDao = activeDirectoryDao;
        }

        /// <summary>
        ///     Legt die Einstellungen fest, mit den auf das ActiveDirectory zugegriffen wird.
        /// </summary>
        public ActiveDirectoryConfiguration ActiveDirectoryConfiguration { private get; set; }

        /// <summary>
        ///     Legt den Dao fest, der auf das AD zugreift.
        /// </summary>
        public IActiveDirectoryDao ActiveDirectoryDao { get; private set; }

        /// <summary>
        ///     Überprüft, ob sich ein Nutzer mit den übergebenen Credentials gegen das Active Directory authentifizieren kann.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password) {
            return ActiveDirectoryDao.Authenticate(username, password);
        }

        /// <summary>
        ///     Ruft eine Liste der Nutzernamen aus dem ActiveyDirectory ab, die als Anwender in der Rolle "Administrator" für
        ///     queo-boards definiert sind.
        /// </summary>
        public IList<string> FindQueoBoardsAdministrators() {
            return ActiveDirectoryDao.FindUserNamesByGroupName(ActiveDirectoryConfiguration.AdministratorsGroupName);
        }

        /// <summary>
        ///     Ruft eine Liste der Nutzernamen aus dem ActiveyDirectory ab, die als Anwender in der Rolle "Nutzer" für queo-boards
        ///     definiert sind.
        /// </summary>
        /// <returns></returns>
        public IList<string> FindQueoBoardsUsers() {
            return ActiveDirectoryDao.FindUserNamesByGroupName(ActiveDirectoryConfiguration.UsersGroupName);
        }

        /// <summary>
        ///   Ruft bestimmte Informationen über einen Nutzer ab.
        /// </summary>
        /// <param name="username"> Der Nutzer für den die Informationen abgerufen werden sollen. </param>
        /// <returns> Können für den Nutzernamen keine Informationen abgerufen werden wird null, andernfalls ein Objekt mit Informationen geliefert. </returns>
        public UserNtInformation FindUserInformation(string username) {
            return ActiveDirectoryDao.FindUserInformation(username);
        }
    }
}