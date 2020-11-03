namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {

    /// <summary>
    /// Definiiert mit welchen Einstellungen, der Zugriff auf das ActiveDirectory erfolgt.
    /// </summary>
    public class ActiveDirectoryConfiguration {


        public ActiveDirectoryConfiguration() {
        }

        public ActiveDirectoryConfiguration(string administratorsGroupName, string usersGroupName) {
            AdministratorsGroupName = administratorsGroupName;
            UsersGroupName = usersGroupName;
        }

        /// <summary>
        ///     Legt den Name der Gruppe fest, der ein Nutzer im Active-Directory zugeordnet sein muss, um als Anwender der Rolle
        ///     "Administrator" für queo-boards erkannt zu werden.
        /// </summary>
        public string AdministratorsGroupName {
            get; set;
        }

        /// <summary>
        ///     Legt den Name der Gruppe fest, der ein Nutzer im Active-Directory zugeordnet sein muss, um als Anwender der Rolle
        ///     "Nutzer" für queo-boards erkannt zu werden.
        /// </summary>
        public string UsersGroupName {
            get; set;
        }

    }
}