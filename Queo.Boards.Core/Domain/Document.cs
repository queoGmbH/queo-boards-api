using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Ein Dokument. D.h. in diesem Fall Metadaten zu einer Datei, die unter
    ///     DocumentPath/BoardId/CardId/DocumentId.[jpg,png,pdf,..] (Id = GUID) abliegt.
    /// </summary>
    public class Document : Entity {
        private readonly Card _card;
        private readonly string _name;

        /// <summary>
        ///     NHibernate
        /// </summary>
        public Document() {
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="Entity" />-Klasse.
        /// </summary>
        public Document(Card card, string name) {
            _card = card;
            _name = name;
        }

        /// <summary>
        ///     Liefert die Karte
        /// </summary>
        public virtual Card Card {
            get { return _card; }
        }

        /// <summary>
        ///     Liefert den Namen des Dokuments
        /// </summary>
        public virtual string Name {
            get { return _name; }
        }
    }
}