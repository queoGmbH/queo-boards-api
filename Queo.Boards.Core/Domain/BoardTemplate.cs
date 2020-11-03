using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Board Template bzw. Board Basisklasse
    /// </summary>
    public class BoardTemplate : Entity {
        private string _colorScheme;
        private string _title;

        /// <summary>
        ///     Ctor für NHibernate
        /// </summary>
        protected BoardTemplate() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="BoardTemplate" />-Klasse.
        /// </summary>
        public BoardTemplate(string colorScheme, string title) {
            _colorScheme = colorScheme;
            _title = title;
        }

        /// <summary>
        ///     Liefert die Farbe
        /// </summary>
        public virtual string ColorScheme {
            get { return _colorScheme; }
        }

        /// <summary>
        ///     Liefert die Liste der Labels
        /// </summary>
        //public IList<Label> Labels { get; private set; }
        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public virtual string Title {
            get { return _title; }
        }

        /// <summary>
        ///     Aktualisiert Titel und Farbschema
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="newColorScheme"></param>
        protected void Update(string newTitle, string newColorScheme) {
            _title = newTitle;
            _colorScheme = newColorScheme;
        }
    }
}