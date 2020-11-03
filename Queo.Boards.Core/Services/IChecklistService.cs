using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle für Services für <see cref="Checklist" />
    /// </summary>
    public interface IChecklistService {
        /// <summary>
        ///     Kopiert eine Checkliste inklusive aller Tasks.
        /// </summary>
        /// <param name="source">Die zu kopierende Checkliste.</param>
        /// <param name="targetCard">Die Karte auf welche die Checkliste kopiert werden soll.</param>
        /// <returns>Die erstellte Kopie</returns>
        Checklist Copy(Checklist source, Card targetCard);

        /// <summary>
        ///     Erzeugt eine neue Checkliste an einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="title"></param>
        /// <param name="checklistToCopy"></param>
        /// <returns></returns>
        Checklist Create(Card card, string title, Checklist checklistToCopy = null);

        /// <summary>
        ///     Löscht eine Checkliste inkl. der Aufgaben.
        /// </summary>
        /// <param name="checklist"></param>
        void Delete(Checklist checklist);

        /// <summary>
        ///     Liefert alle Checklisten einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        IList<Checklist> FindAllOnCard(Card card);

        /// <summary>
        ///     Liefert die Checklisten eines Boards
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        IList<Checklist> FindChecklistsOnBoard(Board board);

        /// <summary>
        ///     Sucht nach einer Checklist anhand ihrer <see cref="Entity.BusinessId" />.
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        Checklist GetByBusinessId(Guid businessId);

        /// <summary>
        ///     Aktualisiert den Titel einer Karte
        /// </summary>
        /// <param name="checklist"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        Checklist UpdateTitle(Checklist checklist, string title);
    }
}