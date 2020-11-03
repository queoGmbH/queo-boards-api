namespace Queo.Boards.Commands.Teams {
    /// <summary>
    ///     Command zum Ändern eines bestehenden Teams.
    /// </summary>
    public class UpdateTeamCommand {
        /// <summary>
        ///     Ruft die neue Beschreibung des Teams ab.
        /// </summary>
        public string Description {
            get; set;
        }

        /// <summary>
        ///     Ruft den neuen Namen des Teams ab oder legt diesen fest.
        /// </summary>
        public string Name {
            get; set;
        }
    }
}