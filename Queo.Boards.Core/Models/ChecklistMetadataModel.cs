using System;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model mit Checklist Metadaten im Kontext der Kartenübergreifenden Auswahl beim Checklist Erstellen.
    /// </summary>
    public class ChecklistMetadataModel : EntityModel {
        private readonly string _title;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ChecklistMetadataModel(Guid businessId, string title, CardBreadCrumbModel card)
            : base(businessId) {
            Card = card;
            _title = title;
        }

        /// <summary>
        ///     Ruft die Id der Karte ab.
        /// </summary>
        public CardBreadCrumbModel Card { get; private set; }

        /// <summary>
        ///     Liefert den Titel
        /// </summary>
        public string Title {
            get { return _title; }
        }
    }
}