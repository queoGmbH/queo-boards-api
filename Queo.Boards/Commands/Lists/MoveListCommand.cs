using Queo.Boards.Core.Domain;

namespace Queo.Boards.Commands.Lists {
    /// <summary>
    /// Command mit den Parametern zum Verschieben einer Liste.
    /// </summary>
    public class MoveListCommand {
        
        /// <summary>
        /// Ruft die Liste ab, welche verschoben werden soll oder legt diese fest.
        /// </summary>
        public List Source {
            get; set;
        }

        /// <summary>
        /// Ruft die Liste ab, hinter welcher die zu verschiebende Liste auf dem Board eingefügt werden soll oder legt diese fest.
        /// </summary>
        public List InsertAfter {
            get; set;
        }

    }
}