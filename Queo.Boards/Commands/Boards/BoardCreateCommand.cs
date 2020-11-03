using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Commands.Boards {
    /// <summary>
    ///     Beschreibt ein Command mit den Eingabedaten für die Erstellung eines neuen Boards.
    /// </summary>
    public class BoardCreateCommand {
        /// <summary>
        ///     Ruft die Zugänglichkeit des Boards ab oder legt diese fest.
        /// </summary>
        public Accessibility Accessibility { get; set; }

        /// <summary>
        ///     Ruft den Namen des Farbschemas ab oder legt diesen fest.
        /// </summary>
        public string ColorScheme { get; set; }

        /// <summary>
        ///     Ruft das Template ab, aus dem das Board erstellt werden soll oder legt dieses fest.
        ///     Soll das Board von Grund auf neu erstellt werden, dann null.
        /// </summary>
        public Board Template { get; set; }

        /// <summary>
        ///     Ruft den Titel des Boards ab oder legt diesen fest.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Erzeugt ein <see cref="BoardDto" /> anhand der Daten im Command.
        /// </summary>
        /// <returns></returns>
        public BoardDto GetBoardDto() {
            return new BoardDto(Title, Accessibility, ColorScheme);
        }
    }
}