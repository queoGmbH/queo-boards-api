using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     ein Label
    /// </summary>
    public class Label : Entity {
        private readonly Board _board;
        private string _color;
        private string _name;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public Label() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public Label(Board board, string color, string name) {
            _board = board;
            _color = color;
            _name = name;
        }

        /// <summary>
        ///     Liefert das Board
        /// </summary>
        public virtual Board Board {
            get { return _board; }
        }

        /// <summary>
        ///     Liefert die Farbe
        /// </summary>
        public virtual string Color {
            get { return _color; }
        }

        /// <summary>
        ///     Liefert den Namen
        /// </summary>
        public virtual string Name {
            get { return _name; }
        }

        /// <summary>
        ///     Aktualisiert die Label Daten
        /// </summary>
        /// <param name="name"></param>
        /// <param name="color"></param>
        public virtual void Update(string name, string color) {
            _name = name;
            _color = color;
        }

        /// <summary>
        /// Erzeugt das DTO für dieses Label.
        /// </summary>
        /// <returns></returns>
        public virtual LabelDto GetDto() {
            return new LabelDto(_name, _color);
        }
    }
}