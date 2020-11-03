using System.Collections.Generic;
using Queo.Boards.Core.Models.BreadCrumbModels;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model mit Karten und Checklist Metadaten im Kontext der Kartenübergreifenden Auswahl beim Checklist Erstellen.
    /// </summary>
    public class CardWithChecklistsMetadataModel {
        public CardWithChecklistsMetadataModel(string title, ListBreadCrumbModel list, IList<ChecklistMetadataModel> checklists) {
            Title = title;
            List = list;
            Checklists = checklists;
        }

        /// <summary>
        ///     Liefert die Checklisten der Karte
        /// </summary>
        public IList<ChecklistMetadataModel> Checklists { get; private set; }

        /// <summary>
        ///     Liefert den Kartentitel
        /// </summary>
        public string Title { get; private set; }

        public ListBreadCrumbModel List { get; private set; }
    }
}