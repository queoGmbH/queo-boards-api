namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {
    /// <summary>
    ///     Definiert wie und auf welches ActiveDirectory zugegriffen wird.
    /// </summary>
    public class ActiveDirectoryAccessConfiguration {
        public ActiveDirectoryAccessConfiguration() {
        }

        public ActiveDirectoryAccessConfiguration(string serverName, string domainName, string containerName, string accessUserName, string accessPassword) {
            ServerName = serverName;
            DomainName = domainName;
            ContainerName = containerName;
            AccessPassword = accessPassword;
            AccessUserName = accessUserName;
        }

        /// <summary>
        ///     Legt das Kennwort fest, mit dem auf das Active Directory zugegriffen wird.
        ///     Das Kennwort wird nur verwendet, ein Nutzername angegeben ist.
        /// </summary>
        public string AccessPassword { get; set; }

        /// <summary>
        ///     Legt den Namen des Nutzers fest, über dessen Account auf das Active Directory zugegriffen wird.
        ///     Ist der Name leer oder null, wird versucht einen anonymen Zugriff auf das Active Directory herzustellen bzw. im
        ///     Context des Nutzers der die Anwendung ausführt.
        /// </summary>
        public string AccessUserName { get; set; }

        /// <summary>
        ///     Legt den Container fest, der als oberster Einstiegspunkt (Root) für die Abfragen gegen das Active Directory
        ///     verwendet wird.
        ///     Ist der Name leer oder null, wird kein Container verwendet und das tatsächliche Root des Active Directory
        ///     verwendet.
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        ///     Legt den Namen der Domain fest, auf dessen Active Directory zugegriffen werden soll.
        ///     Ist der Name leer oder null, wird versucht die aktuelle Domain zu verwenden.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     Legt den Namen bzw. die Adresse des Servers fest, auf dem die Domain liegt, auf dessen Active Directory zugegriffen
        ///     wird.
        ///     Ist der Name leer oder null, wird localhost verwendet.
        /// </summary>
        public string ServerName { get; set; }
    }
}