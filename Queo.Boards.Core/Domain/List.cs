using System;
using System.Collections.Generic;
using System.Diagnostics;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Eine Liste
    /// </summary>
    [DebuggerDisplay("'{Title}' auf '{Board.Title}'")]
    public class List : ListTemplate {
        private readonly IList<Card> _cards;
        private bool _isArchived;
        private DateTime? _archivedAt;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public List() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public List(Board board, string title) : base(board, title) {
            _cards = new List<Card>();
        }

        /// <summary>
        ///     Liefert die Karten der Liste
        /// </summary>
        public virtual IList<Card> Cards {
            get { return _cards; }
        }

        /// <summary>
        ///     Liefert ob die Liste archiviert ist.
        /// </summary>
        public virtual bool IsArchived {
            get { return _isArchived; }
        }

        /// <summary>
        /// Ruft ab, wann die Liste archiviert wurde. 
        /// Wurde die Liste bisher nicht archiviert, wird NULL geliefert.
        /// </summary>
        public virtual DateTime? ArchivedAt {
            get { return _archivedAt; }
        }

        /// <summary>
        ///     Archiviert die Liste.
        /// </summary>
        /// <param name="archivedAt"></param>
        public virtual void Archive(DateTime archivedAt) {
            _isArchived = true;
            _archivedAt = archivedAt;
        }

        /// <summary>
        /// Stellt die Liste wieder her und hebt damit die Archivierung auf.
        /// </summary>
        public virtual void Restore() {
            _isArchived = false;
            _archivedAt = null;
        }

        /// <summary>
        /// Ruft die Position der Liste auf dem Board ab.
        /// </summary>
        /// <returns></returns>
        public virtual int GetPositionOnBoard() {
            return Board.Lists.IndexOf(this);
        }
    }
}