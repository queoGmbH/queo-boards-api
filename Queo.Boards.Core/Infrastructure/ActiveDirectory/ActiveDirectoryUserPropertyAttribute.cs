using System.ComponentModel;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {

    /// <summary>
    /// Attribute mit dessen Hilfe das Schlüsselwort zur Abfrage dieser Eigenschaft aus dem ActiveDirectory definiert werden kann.
    /// </summary>
    public class ActiveDirectoryUserPropertyAttribute : DescriptionAttribute {
        private readonly string _shortcut;

        /// <summary>
        /// Erzeugt eine neue Instanz für das <see cref="ActiveDirectoryUserPropertyAttribute"/> 
        /// mit dem angegebenen Schlüsselwort.
        /// </summary>
        /// <param name="shortcut"></param>
        public ActiveDirectoryUserPropertyAttribute(string shortcut) {
            _shortcut = shortcut;
        }

        /// <summary>
        /// Liefert das Schlüsselwort.
        /// </summary>
        public string Shortcut {
            get {
                return _shortcut;
            }
        }
    }
}