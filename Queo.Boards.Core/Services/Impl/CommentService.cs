using System;
using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service für <see cref="Comment" />
    /// </summary>
    public class CommentService : ICommentService {
        private readonly ICommentDao _commentDao;
        private readonly INotificationService _notificationService;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CommentService(ICommentDao commentDao, INotificationService notificationService) {
            _commentDao = commentDao;
            _notificationService = notificationService;
        }

        /// <summary>
        ///     Erstellt eine Kopie eines Kommentars.
        /// </summary>
        /// <param name="sourceComment">Der zu kopierende Kommentar.</param>
        /// <param name="targetCard">Die Karte, an welche der Kommentar kopiert werden soll.</param>
        /// <returns></returns>
        [Transaction]
        public Comment Copy(Comment sourceComment, Card targetCard) {
            Require.NotNull(sourceComment, "sourceComment");
            Require.NotNull(targetCard, "targetCard");
            CardService.ValidateCanEditCard(targetCard);
            if (sourceComment.IsDeleted) {
                throw new ArgumentOutOfRangeException("sourceComment", "Ein gelöschter Kommentar kann nicht kopiert werden.");
            }
            

            Comment copy = new Comment(targetCard, sourceComment.Text, sourceComment.Creator, DateTime.UtcNow);
            _commentDao.Save(copy);

            if (sourceComment.IsDeleted) {
                copy.UpdateIsDeleted(true);
            }

            targetCard.Comments.Add(copy);

            return copy;
        }

        /// <summary>
        ///     Erstellt eine neue Karte
        /// </summary>
        /// <param name="card"></param>
        /// <param name="text"></param>
        /// <param name="creator"></param>
        /// <returns></returns>
        [Transaction]
        public Comment Create(Card card, string text, User creator) {
            Require.NotNull(card, "card");
            Require.NotNull(creator, "creator");
            CardService.ValidateCanEditCard(card);

            Comment comment = new Comment(card, text, creator, DateTime.Now.ToUniversalTime());
            _commentDao.Save(comment);

            if (card.AssignedUsers.Any()) {
                IList<User> usersToNotify = card.AssignedUsers.Except(new List<User> {creator}).ToList();
                _notificationService.CreateCommentNotification(usersToNotify, comment, CommentNotificationReason.CommentCreated);
            }

            return comment;
        }

        /// <summary>
        ///     Liefert alle Kommentare einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public IList<Comment> FindAllOnCard(Card card) {
            return _commentDao.FindAllOnCard(card).OrderByDescending(x => x.CreationDate).ToList();
        }

        /// <summary>
        ///     Aktualisiert einen Kommentar
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="newText"></param>
        /// <returns></returns>
        [Transaction]
        public Comment UpdateText(Comment comment, string newText) {
            Require.NotNull(comment, "comment");
            ValidateCanEditComment(comment);

            comment.UpdateText(newText);
            return comment;
        }

        private void ValidateCanEditComment(Comment comment) {
            Require.NotNull(comment, "comment");

            if (comment.IsDeleted) {
                throw new ArgumentOutOfRangeException("comment", "Ein gelöschter Kommentar kann nicht bearbeitet werden.");
            }
            CardService.ValidateCanEditCard(comment.Card);
        }

        /// <summary>
        /// Aktualisiert den Löschzustand eines Kommentars
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        [Transaction]
        public Comment UpdateIsDeleted(Comment comment, bool isDeleted) {
            Require.NotNull(comment, "comment");
            CardService.ValidateCanEditCard(comment.Card);

            comment.UpdateIsDeleted(isDeleted);
            return comment;
        }

        /// <summary>
        /// Sucht seitenweise nach Kommentaren, auf Karten, welche für den Nutzer zugänglich sind.
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <param name="user"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IPage<Comment> FindCommentsForUser(IPageable pageRequest, User user, string query = null) {
            return _commentDao.FindCommentsForUser(pageRequest, user, query);
        }
    }
}