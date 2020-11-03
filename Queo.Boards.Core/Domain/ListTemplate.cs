using System;
using Newtonsoft.Json;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     LIst Template bzw. List Basisklasse
    /// </summary>
    public class ListTemplate : Entity {
        private Board _board;
        private string _title;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public ListTemplate() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public ListTemplate(Board board, string title) {
            _board = board;
            _title = title;
        }

        /// <summary>
        ///     Liefert das Board, dem die Liste zugeordnet ist.
        /// </summary>
        [JsonIgnore]
        public virtual Board Board {
            get { return _board; }
        }

        /// <summary>
        ///     Liefert die BusinessId des zugewiesenen Boards.
        /// </summary>
        public virtual Guid BoardId {
            get { return Board.BusinessId; }
        }
        
        /// <summary>
        ///     Liefert den Titel der Liste
        /// </summary>
        public virtual string Title {
            get { return _title; }
        }

        /// <summary>
        ///     Aktualisiert den Listentitel
        /// </summary>
        /// <param name="newTitle"></param>
        public virtual void Update(string newTitle) {
            _title = newTitle;
        }

        /// <summary>
        ///     Aktualisiert das Board, dem die Liste zugeordnet ist.
        /// </summary>
        /// <param name="targetBoard"></param>
        public virtual void UpdateParent(Board targetBoard) {
            _board = targetBoard;
        }
        
    }
}