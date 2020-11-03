using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Dtos {

    /// <summary>
    /// Enthält die Zusammenfassung der Synchronisation von Nutzer mit dem ActiveDirectory.
    /// </summary>
    public class ActiveDirectorySynchronisationSummary {

        public ActiveDirectorySynchronisationSummary(IList<User> createdUsers, IList<User> updatedUsers, IList<User> deletedUsers) {
            CreatedUsers = createdUsers;
            UpdatedUsers = updatedUsers;
            DeletedUsers = deletedUsers;
        }

        /// <summary>
        /// Ruft die neu erstellten Nutzer ab.
        /// </summary>
        public IList<User> CreatedUsers { get; private set; }

        /// <summary>
        /// Ruft die geänderten Nutzer ab.
        /// </summary>
        public IList<User> UpdatedUsers {
            get; private set;
        }

        /// <summary>
        /// Ruft die Nutzer ab, die bei der Synchronisation gelöscht, archiviert bzw. stillgelegt wurden.
        /// </summary>
        public IList<User> DeletedUsers {
            get; private set;
        }

        

    }
}