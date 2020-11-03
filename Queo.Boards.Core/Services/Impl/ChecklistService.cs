using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Checklist" />
    /// </summary>
    public class ChecklistService : IChecklistService {
        private readonly IChecklistDao _checklistDao;
        private readonly ITaskService _taskService;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public ChecklistService(IChecklistDao checklistDao, ITaskService taskService) {
            _checklistDao = checklistDao;
            _taskService = taskService;
        }

        /// <summary>
        ///     Kopiert eine Checkliste inklusive aller Tasks.
        /// </summary>
        /// <param name="source">Die zu kopierende Checkliste.</param>
        /// <param name="targetCard">Die Karte auf welche die Checkliste kopiert werden soll.</param>
        /// <returns>Die erstellte Kopie</returns>
        [Transaction]
        public Checklist Copy(Checklist source, Card targetCard) {
            Require.NotNull(source, "source");
            Require.NotNull(targetCard, "targetCard");
            
            Checklist copy = Create(targetCard, source.Title);
            foreach (Task sourceTask in source.Tasks) {
                _taskService.Copy(copy, sourceTask);
            }

            return copy;
        }

        /// <summary>
        ///     Erzeugt eine neue Checkliste an einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="title"></param>
        /// <param name="checklistToCopy"></param>
        /// <returns></returns>
        [Transaction]
        public Checklist Create(Card card, string title, Checklist checklistToCopy = null) {
            Require.NotNull(card, nameof(card));
            CardService.ValidateCanEditCard(card);

            Checklist checklist = new Checklist(card, title);
            card.Checklists.Add(checklist);
            _checklistDao.Save(checklist);
            if (checklistToCopy != null) {
                CopyTasks(checklistToCopy, checklist);
            }
            return checklist;
        }

        /// <summary>
        ///     Löscht eine Checkliste inkl. der Aufgaben.
        /// </summary>
        /// <param name="checklist"></param>
        [Transaction]
        public void Delete(Checklist checklist) {
            Require.NotNull(checklist, "checklist");
            CardService.ValidateCanEditCard(checklist.Card);

            _checklistDao.Delete(checklist);
        }

        /// <summary>
        ///     Liefert alle Checklisten einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public IList<Checklist> FindAllOnCard(Card card) {
            return _checklistDao.FindAllOnCard(card);
        }

        /// <summary>
        ///     Liefert die Checklisten eines Boards
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public IList<Checklist> FindChecklistsOnBoard(Board board) {
            return _checklistDao.FindAllOnBoard(board);
        }

        /// <summary>
        ///     Sucht nach einer Checklist anhand ihrer <see cref="Entity.BusinessId" />.
        /// </summary>
        /// <param name="businessId"></param>
        /// <returns></returns>
        public Checklist GetByBusinessId(Guid businessId) {
            return _checklistDao.GetByBusinessId(businessId);
        }

        /// <summary>
        ///     Aktualisiert den Titel einer Karte
        /// </summary>
        /// <param name="checklist"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [Transaction]
        public Checklist UpdateTitle(Checklist checklist, string title) {
            Require.NotNull(checklist, "checklist");
            ValidateCanEditChecklist(checklist);

            checklist.Update(title);
            return checklist;
        }

        /// <summary>
        /// Überprüft, ob eine Checkliste geändert werden kann.
        /// </summary>
        /// <param name="checklist"></param>
        public static void ValidateCanEditChecklist(Checklist checklist) {
            Require.NotNull(checklist, "checklist");

            CardService.ValidateCanEditCard(checklist.Card);
        }

        private void CopyTasks(Checklist source, Checklist target) {
            foreach (Task originTask in source.Tasks) {
                _taskService.Create(target, originTask.Title);
            }
        }
    }
}