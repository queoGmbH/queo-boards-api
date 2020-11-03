using System;
using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Card" />
    /// </summary>
    public class CardService : ICardService {
        private readonly ICardDao _cardDao;
        private readonly IChecklistService _checklistService;

        private readonly ICommentService _commentService;

        /// <summary>
        /// </summary>
        /// <param name="cardDao"></param>
        /// <param name="checklistService"></param>
        /// <param name="commentService"></param>
        public CardService(ICardDao cardDao, IChecklistService checklistService, ICommentService commentService) {
            Require.NotNull(cardDao, nameof(cardDao));
            Require.NotNull(checklistService, nameof(checklistService));
            Require.NotNull(commentService, nameof(commentService));

            _cardDao = cardDao;
            _checklistService = checklistService;
            _commentService = commentService;
        }

        /// <summary>
        ///     Überprüft, ob eine Karte geändert werden darf.
        /// </summary>
        /// <param name="card"></param>
        public static void ValidateCanEditCard(Card card) {
            if (card.IsArchived) {
                throw new ArgumentOutOfRangeException("card", "Eine archivierte Karte kann nicht bearbeitet werden.");
            }

            /*Jetzt noch überprüfen, ob Änderungen auf der Karte vorgenommen werden können.*/
            ListService.ValidateCanEditList(card.List);
        }

        /// <summary>
        ///     Überprüft, ob Karten auf einer Liste geändert werden können.
        /// </summary>
        /// <param name="list"></param>
        public static void ValidateCanEditCardsOnList(List list) {
            Require.NotNull(list, "list");

            if (list.Board.IsTemplate) {
                throw new ArgumentOutOfRangeException("list", "Auf Vorlagen dürfen keine Karten erstellt oder geändert werden.");
            }
        }

        /// <summary>
        ///     Fügt einer Karte ein Label hinzu.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="label"></param>
        [Transaction]
        public Card AddLabel(Card card, Label label) {
            Require.NotNull(card, nameof(card));
            Require.NotNull(label, nameof(label));
            ValidateCanEditCard(card);

            if (card.Labels == null || !card.Labels.Contains(label)) {
                card.AddLabel(label);
            }
            return card;
        }

        /// <summary>
        ///     Weißt der Karte einen Nutzer zu.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="user">Der hinzuzufügende Nutzer</param>
        /// <returns></returns>
        [Transaction]
        public IList<User> AssignUser(Card card, User user) {
            Require.NotNull(user, "user");
            Require.NotNull(card, "card");
            ValidateCanEditCard(card);

            IList<User> boardUsers = card.List.Board.GetBoardUsers();
            if (!boardUsers.Contains(user)) {
                throw new InvalidOperationException("Der Nutzer ist kein Mitglied des Boards.");
            }

            if (!card.AssignedUsers.Contains(user)) {
                card.AssignUser(user);
            }

            return card.AssignedUsers;
        }

        /// <summary>
        ///     Kopiert eine Karte und speichert die Kopie auf der Liste und an der entsprechenden Position bzw. am Ende der Liste.
        ///     Es werden alle Eigenschaften außer Anhängen übernommen.
        /// </summary>
        /// <param name="sourceCard"></param>
        /// <param name="targetList"></param>
        /// <param name="copyName"></param>
        /// <param name="copier">Der Nutzer der die Karte kopiert und damit Ersteller der Kopie ist.</param>
        /// <param name="position"></param>
        /// <returns></returns>
        [Transaction]
        public Card Copy(Card sourceCard, List targetList, string copyName, User copier, int position = 0) {
            Require.NotNull(sourceCard, "sourceCard");
            Require.NotNull(targetList, "targetList");
            Require.NotNull(copier, "copier");

            if (sourceCard.IsArchived) {
                throw new InvalidOperationException("Eine archivierte Karte darf nicht kopiert werden.");
            }

            ListService.ValidateCanEditList(targetList);
            ValidateCanEditCardsOnList(targetList);

            Card cardCopy = new Card(targetList, copyName, sourceCard.Description, sourceCard.Due, new List<Label>(), new EntityCreatedDto(copier, DateTime.Now));
            if (sourceCard.IsArchived) {
                cardCopy.Archive(DateTime.UtcNow);
            }

            /*An der entsprechenden Position auf der Liste einfügen.*/
            position = NormalizePosition(targetList, position);
            targetList.Cards.Insert(position, cardCopy);

            _cardDao.Save(cardCopy);

            /*Checklisten kopieren*/
            foreach (Checklist sourceCardChecklist in sourceCard.Checklists) {
                _checklistService.Copy(sourceCardChecklist, cardCopy);
            }

            /*Kommentare kopieren*/
            foreach (Comment sourceCardComment in sourceCard.Comments.Where(c => !c.IsDeleted)) {
                _commentService.Copy(sourceCardComment, cardCopy);
            }

            /*Kopieren nur, wenn das Board gleich bleibt.*/
            if (sourceCard.List.Board.Equals(cardCopy.List.Board)) {
                /*Zugewiesene Nutzer kopieren*/
                foreach (User assignedUser in sourceCard.AssignedUsers) {
                    cardCopy.AssignUser(assignedUser);
                }

                /*Zugewiesene Labels kopieren*/
                foreach (Label label in sourceCard.Labels) {
                    cardCopy.AddLabel(label);
                }
            }

            return cardCopy;
        }

        /// <summary>
        ///     Erstellt eine neue Karte
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dto"></param>
        /// <param name="assignedUsers">Initial der Karte zugeordnete Nutzer.</param>
        /// <param name="creator"></param>
        /// <returns></returns>
        [Transaction]
        public Card Create(List list, CardDto dto, IList<User> assignedUsers, User creator) {
            Require.NotNull(list);
            Require.NotNull(dto);
            Require.NotNull(assignedUsers, "assignedUsers");
            Require.NotNull(creator, "createdBy");

            ListService.ValidateCanEditList(list);
            ValidateCanEditCardsOnList(list);

            int cardPosition = list.Cards.Count;

            IList<Label> labels = new List<Label>();
            if (dto.AssignedLabels != null) {
                foreach (Label labelModel in dto.AssignedLabels) {
                    labels.Add(labelModel);
                }
            }

            Card card = new Card(list, dto.Title, dto.Description, dto.Due, labels, new EntityCreatedDto(creator, DateTime.Now));
            list.Cards.Insert(cardPosition, card);

            _cardDao.Save(card);

            foreach (User userToAssign in assignedUsers) {
                AssignUser(card, userToAssign);
            }
            return card;
        }

        /// <summary>
        ///     Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen für die Abfrage.</param>
        /// <param name="user">Der Nutzer, für den die Karten abgerufen werden sollen.</param>
        /// <param name="searchTerm">Optionale Suchzeichenfolge, zur Einschränkung der Ergebnis-Liste</param>
        /// <returns></returns>
        public IPage<Card> FindCardsForUser(IPageable pageRequest, User user, string searchTerm = null) {
            return _cardDao.FindCardsForUser(pageRequest, user, searchTerm);
        }

        /// <summary>
        ///     Sucht nach allen, nicht archivierten Karten
        /// </summary>
        /// <param name="board">Das Board, auf dem nach Karten gesucht werden soll, dem die Nutzer zugewiesen sind</param>
        /// <param name="users">Liste mit Nutzern, nach deren Karten gesucht werden soll.</param>
        /// <returns></returns>
        public IList<Card> FindCardsOnBoardForUsers(Board board, params User[] users) {
            return _cardDao.FindCardsOnBoardForUsers(board, users);
        }

        /// <summary>
        ///     Ermittelt die Position, an der eine Karte eingefügt werden muss, um hinter einer bestimmten Karte auf einer Liste
        ///     eingefügt zu werden.
        /// </summary>
        /// <param name="targetList">Die Liste auf welcher die Karte eingefügt werden soll.</param>
        /// <param name="insertAfter">Die Karte hinter welche die neue Liste eingefügt werden soll.</param>
        /// <returns></returns>
        public int GetPositionByTarget(List targetList, Card insertAfter) {
            Require.NotNull(targetList, "targetList");

            if (insertAfter == null) {
                return 0;
            }

            return NormalizePosition(targetList, targetList.Cards.IndexOf(insertAfter) + 1);
        }

        /// <summary>
        ///     Verschiebt eine Karte zwischen Listen
        /// </summary>
        /// <param name="cardToMove">Die zu verschiebende Karte</param>
        /// <param name="targetList">Die Zielliste</param>
        /// <param name="position">
        ///     Der Index an welcher die Karte auf der Zielliste eingefügt werden soll.
        /// </param>
        /// <returns></returns>
        [Transaction]
        public List MoveCard(Card cardToMove, List targetList, int position = 0) {
            Require.NotNull(cardToMove, nameof(cardToMove));
            Require.NotNull(targetList, nameof(targetList));

            ValidateCanEditCard(cardToMove);
            ValidateCanEditCardsOnList(targetList);
            ListService.ValidateCanEditList(targetList);
            
            Board sourceBoard = cardToMove.List.Board;
            Board targetBoard = targetList.Board;

            List originList = cardToMove.List;
            originList.Cards.Remove(cardToMove);
            cardToMove.UpdateParent(targetList);

            position = NormalizePosition(targetList, position);
            targetList.Cards.Insert(position, cardToMove);

            if (!sourceBoard.Equals(targetBoard)) {
                cardToMove.Labels.Clear();
                cardToMove.ClearAssignedUsers();
            }

            return targetList;
        }

        /// <summary>
        ///     Entfernt das Label von einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="label"></param>
        [Transaction]
        public Card RemoveLabel(Card card, Label label) {
            Require.NotNull(card, "card");
            ValidateCanEditCard(card);

            if (card.Labels != null && card.Labels.Contains(label)) {
                card.Labels.Remove(label);
            }
            return card;
        }

        /// <summary>
        ///     Entfernt einen Nutzer aus der Liste der einer Karte zugewiesenen Nutzer.
        /// </summary>
        /// <param name="card">Die Karte, von welcher der Nutzer entfernt werden sollen.</param>
        /// <param name="users">Die Nutzer, deren Zuordnung entfernt werden soll.</param>
        /// <returns>Die Liste der nach dem entfernen noch der Karte zugewiesenen Nutzer.</returns>
        [Transaction]
        public IList<User> UnassignUsers(Card card, params User[] users) {
            Require.NotNull(card, "card");
            Require.NotNull(users, "users");
            ValidateCanEditCard(card);

            foreach (User user in users) {
                card.UnassignUser(user);
            }

            return card.AssignedUsers.ToList();
        }

        /// <summary>
        ///     Aktualisiert ob eine Karte archiviert ist.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isArchived"></param>
        /// <returns></returns>
        [Transaction]
        public Card UpdateArchived(Card card, bool isArchived) {
            Require.NotNull(card, "card");
            
            if (isArchived) {
                card.Archive(DateTime.UtcNow);
            } else {
                card.Restore();
            }

            return card;
        }

        /// <summary>
        ///     Aktualisiert die Beschreibung einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [Transaction]
        public Card UpdateDescription(Card card, string description) {
            Require.NotNull(card, "card");
            ValidateCanEditCard(card);

            card.UpdateDescription(description);
            return card;
        }

        /// <summary>
        ///     Aktualisiert das Fälligkeitsdatum
        /// </summary>
        /// <param name="card"></param>
        /// <param name="due"></param>
        /// <returns></returns>
        [Transaction]
        public Card UpdateDue(Card card, DateTime? due) {
            Require.NotNull(card, "card");
            ValidateCanEditCard(card);

            if (due.HasValue && due.Value.Kind != DateTimeKind.Utc) {
                due = due.Value.ToUniversalTime();
            }

            if (!Equals(card.Due, due)) {
                /*Due aktualisieren*/
                card.UpdateDue(due);

                /*Flag und Uhrzeit für erfolgte Benachrichtigung bei Ablauf der Fälligkeit zurücksetzen*/
                card.ResetDueExpirationNotificationCreated();
            }

            return card;
        }

        /// <summary>
        ///     Aktualisiert die Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="title"></param>
        [Transaction]
        public Card UpdateTitle(Card card, string title) {
            Require.NotNull(card, "card");
            ValidateCanEditCard(card);

            card.UpdateTitle(title);
            return card;
        }

        private int NormalizePosition(List targetList, int position) {
            /*Die Position darf nicht kleiner als 0 sein.*/
            position = Math.Max(0, position);
            /*Die Position darf nicht größer als die Anzahl der Karten auf der Liste sein.*/
            position = Math.Min(position, targetList.Cards.Count);

            return position;
        }
    }
}