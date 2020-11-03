using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Commands.Teams {

    /// <summary>
    /// Command zum Hinzufügen von Nutzern zu einem Team.
    /// </summary>
    public class AddMemberCommand {

        /// <summary>
        /// Konstruktor für Tests und ModelBinding
        /// </summary>
        public AddMemberCommand() {
        }

        /// <summary>
        /// Konstruktor zur vollständigen Initialisierung des Commands.
        /// </summary>
        /// <param name="users"></param>
        public AddMemberCommand(IList<User> users) {
            Require.NotNull(users, "users");
            
            Users = users;
        }

        /// <summary>
        /// Ruft die Liste mit Nutzern ab, die dem Team hinzugefügt werden sollen oder legt diese fest.
        /// </summary>
        public IList<User> Users { get; set; }

    }
}