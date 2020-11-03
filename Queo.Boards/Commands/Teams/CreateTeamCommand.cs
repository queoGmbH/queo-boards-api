namespace Queo.Boards.Commands.Teams {
    /// <summary>
    ///     Command zum Erstellen eines neuen Teams.
    /// </summary>
    public class CreateTeamCommand {
        /// <summary>
        ///     Ruft die Beschreibung des Teams ab.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Ruft den Namen des zu erstellenden Teams ab oder legt diesen fest.
        /// </summary>
        public string Name { get; set; }
    }
}