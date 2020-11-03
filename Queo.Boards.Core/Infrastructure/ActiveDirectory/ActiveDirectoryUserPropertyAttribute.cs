using System.ComponentModel;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {

    /// <summary>
    /// Attribute mit dessen Hilfe das Schl�sselwort zur Abfrage dieser Eigenschaft aus dem ActiveDirectory definiert werden kann.
    /// </summary>
    public class ActiveDirectoryUserPropertyAttribute : DescriptionAttribute {
        private readonly string _shortcut;

        /// <summary>
        /// Erzeugt eine neue Instanz f�r das <see cref="ActiveDirectoryUserPropertyAttribute"/> 
        /// mit dem angegebenen Schl�sselwort.
        /// </summary>
        /// <param name="shortcut"></param>
        public ActiveDirectoryUserPropertyAttribute(string shortcut) {
            _shortcut = shortcut;
        }

        /// <summary>
        /// Liefert das Schl�sselwort.
        /// </summary>
        public string Shortcut {
            get {
                return _shortcut;
            }
        }
    }
}