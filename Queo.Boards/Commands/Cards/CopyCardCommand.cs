using Queo.Boards.Core.Domain;

namespace Queo.Boards.Commands.Cards {
    /// <summary>
    /// Command mit den Parametern zum Kopieren einer Karte.
    /// </summary>
    public class CopyCardCommand {

        /// <summary>
        /// Ruft den Namen der zu erstellenden Kopie ab oder legt diesen fest.
        /// </summary>
        public string CopyName {
            get; set;
        }

        /// <summary>
        /// Ruft die Karte ab, welche kopiert werden soll oder legt diese fest.
        /// </summary>
        public Card Source {
            get; set;
        }

        /// <summary>
        /// Ruft die Karte ab, hinter welcher die neue Karte in der Liste eingefügt werden soll oder legt diese fest.
        /// </summary>
        public Card InsertAfter {
            get; set;
        }

    }
}