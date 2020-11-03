using Queo.Boards.Core.Domain;

namespace Queo.Boards.Commands.Lists {

    /// <summary>
    /// Command mit den Parametern zum Kopieren einer Liste.
    /// </summary>
    public class CopyListCommand {
        
        /// <summary>
        /// Ruft den Namen der zu erstellenden Kopie ab oder legt diesen fest.
        /// </summary>
        public string CopyName { get; set; }
        
        /// <summary>
        /// Ruft die Liste ab, welche kopiert werden soll oder legt diese fest.
        /// </summary>
        public List Source { get; set; }

        /// <summary>
        /// Ruft die Liste ab, hinter welcher die neue Liste auf dem das Board eingefügt werden soll oder legt diese fest.
        /// </summary>
        public List InsertAfter { get; set; }

    }
}