using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {
    /// <summary>
    ///     Klasse die den Zugriff auf das Active Directory erlaubt.
    /// </summary>
    public class ActiveDirectoryDao : IActiveDirectoryDao {
        public ActiveDirectoryDao() {
        }

        public ActiveDirectoryDao(ActiveDirectoryAccessConfiguration activeDirectoryAccessConfiguration) {
            ActiveDirectoryAccessConfiguration = activeDirectoryAccessConfiguration;
        }

        /// <summary>
        ///     Legt die Konfiguration fest, mit der auf das Active Directory zugegriffen wird.
        /// </summary>
        public ActiveDirectoryAccessConfiguration ActiveDirectoryAccessConfiguration { private get; set; }

        public static PrincipalContext GetPrincipalContext(ActiveDirectoryAccessConfiguration config) {

            PrincipalContext principalContext = null;
            if (!string.IsNullOrWhiteSpace(config.ServerName)) {
                if (string.IsNullOrWhiteSpace(config.AccessUserName) || string.IsNullOrWhiteSpace(config.AccessPassword)) {
                    principalContext = new PrincipalContext(ContextType.Domain, config.ServerName);
                } else {
                    principalContext = new PrincipalContext(ContextType.Domain, config.ServerName, config.AccessUserName, config.AccessPassword);
                }

            } else {
                if (!string.IsNullOrWhiteSpace(config.ContainerName)) {
                    if (string.IsNullOrWhiteSpace(config.AccessUserName) || string.IsNullOrWhiteSpace(config.AccessPassword)) {
                        principalContext = new PrincipalContext(ContextType.Domain, config.DomainName, config.ContainerName);
                    } else {
                        principalContext = new PrincipalContext(ContextType.Domain, config.DomainName, config.ContainerName, config.AccessUserName, config.AccessPassword);
                    }
                } else {
                    if (string.IsNullOrWhiteSpace(config.AccessUserName) || string.IsNullOrWhiteSpace(config.AccessPassword)) {
                        principalContext = new PrincipalContext(ContextType.Domain, config.DomainName);
                    } else {
                        principalContext = new PrincipalContext(ContextType.Domain, config.DomainName, config.AccessUserName, config.AccessPassword);
                    }

                }

            }
            
            return principalContext;
        }

        public bool Authenticate(string username, string password) {
            if (!IsExistingUsername(username)) {
                /*Den Nutzer gibt es nicht.*/
                return false;
            }

            try {
                PrincipalContext principalContext = GetPrincipalContext();
                return principalContext.ValidateCredentials(username, password);
            } catch (Exception e) {
                return false;
            }
        }

        /// <summary>
        ///     Ruft bestimmte Informationen über einen Nutzer ab.
        /// </summary>
        /// <param name="username">Der Nutzer für den die Informationen abgerufen werden sollen.</param>
        /// <returns></returns>
        public UserNtInformation FindUserInformation(string username) {
            try {
                /*Verbindung zum AD herstellen.*/
                using (PrincipalContext context = GetPrincipalContext()) {
                    /*Nach der Gruppe suchen*/
                    using (UserPrincipal user = UserPrincipal.FindByIdentity(context, username)) {
                        if (user != null) {
                            return new UserNtInformation(user.GivenName, user.Surname, GetPropertyOrEmpty(user, "company"), GetPropertyOrEmpty(user, "department"), user.EmailAddress, user.VoiceTelephoneNumber);
                        } else {
                            return null;
                        }
                    }
                }
            } catch (Exception) {
                return null;
            }
        }

        public IList<string> FindUserNamesByGroupName(string activeDirectoryGroupName) {
            Require.NotNullOrWhiteSpace(activeDirectoryGroupName, "activeDirectoryGroupName");

            /*Verbindung zum AD herstellen.*/
            using (PrincipalContext context = GetPrincipalContext(ActiveDirectoryAccessConfiguration)) {
                /*Nach der Gruppe suchen*/
                using (GroupPrincipal group = GroupPrincipal.FindByIdentity(context, activeDirectoryGroupName)) {
                    List<string> findUserNamesByGroupName = new List<string>();
                    if (group != null) {
                        PrincipalSearchResult<Principal> users = group.GetMembers(true);
                        foreach (UserPrincipal user in users) {
                            findUserNamesByGroupName.Add(user.SamAccountName);
                        }
                    }

                    return findUserNamesByGroupName;
                }
            }
        }

        /// <summary>
        ///     Ruft ab, ob der angegebene Nutzername vorhanden ist oder nicht.
        /// </summary>
        /// <param name="username">Der zu überprüfende Nutzername</param>
        /// <returns>true wenn der Name gefunden wurde, false wenn nicht</returns>
        public bool IsExistingUsername(string username) {
            /*Verbindung zum AD herstellen.*/
            using (PrincipalContext context = GetPrincipalContext()) {
                /*Nach der Gruppe suchen*/
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, username)) {
                    return user != null;
                }
            }
        }

        private static string GetPropertyOrEmpty(Principal principal, string propertyName) {
            if (principal == null) {
                return string.Empty;
            }
            DirectoryEntry directoryEntry = principal.GetUnderlyingObject() as DirectoryEntry;
            if (directoryEntry == null) {
                return string.Empty;
            }

            if (directoryEntry.Properties.Contains(propertyName)) {
                return directoryEntry.Properties[propertyName].Value.ToString();
            } else {
                return string.Empty;
            }
        }

        private PrincipalContext GetPrincipalContext() {
            return GetPrincipalContext(ActiveDirectoryAccessConfiguration);
        }
    }
}