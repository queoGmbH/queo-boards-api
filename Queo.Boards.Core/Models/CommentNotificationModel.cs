using System;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model, dass eine Benachrichtigung über eine Aktion an einem Kommentar abbildet.
    /// </summary>
    public class CommentNotificationModel : NotificationModel {
        public CommentNotificationModel(UserModel notificationFor, CommentModel comment, CommentNotificationReason notificationReason, DateTime createdAt, bool emailSend, DateTime? emailSendAt, bool isMarkedAsRead)
            : base(notificationFor, createdAt, emailSend, emailSendAt, isMarkedAsRead) {
            Require.NotNull(comment, "comment");

            Comment = comment;
            NotificationReason = notificationReason;
        }

        /// <summary>
        ///     Ruft den Kommentar ab, für welchen die Benachrichtigung erstellt wurde.
        /// </summary>
        public CommentModel Comment { get; private set; }


        public CommentNotificationReason NotificationReason { get; private set; }
    }
}