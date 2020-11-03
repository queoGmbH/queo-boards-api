using Queo.Boards.Core.Domain;

namespace Queo.Boards.Commands.Cards {
    /// <summary>
    /// Command mit den Parametern zum Verschieben einer Karte.
    /// </summary>
    public class MoveCardCommand {

        /// <summary>
        /// Ruft die Karte ab, welche verschoben werden soll oder legt diese fest.
        /// </summary>
        public Card Source {
            get; set;
        }

        /// <summary>
        /// Ruft die Karte ab, hinter welcher die zu verschiebende Karte in der Liste eingefügt werden soll oder legt diese fest.
        /// </summary>
        public Card InsertAfter {
            get; set;
        }

    }
}