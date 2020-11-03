using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Persistence.Impl;

namespace Queo.Boards.Core.Tests.Builders {
    public class CommentNotificationBuilder : NotificationBuilder<CommentNotificationBuilder, CommentNotification> {
        private readonly CommentBuilder _commentBuilder;
        private readonly UserBuilder _userBuilder;
        private Comment _comment;
        private CommentNotificationReason _notificationReason;

        public CommentNotificationBuilder(INotificationDao notificationDao, CommentBuilder commentBuilder, UserBuilder userBuilder)
            : base(notificationDao) {
            Require.NotNull(commentBuilder, "commentBuilder");
            Require.NotNull(userBuilder, "userBuilder");

            _commentBuilder = commentBuilder;
            _userBuilder = userBuilder;
        }

        protected override CommentNotificationBuilder ActualBuilder {
            get { return this; }
        }

        public override CommentNotification Build() {

            if (_comment == null) {
                _comment = _commentBuilder.Build();
            }
            if (NotificationFor == null) {
                NotificationFor = _userBuilder.Build();
            }

            CommentNotification commentNotification = new CommentNotification(_comment, _notificationReason, NotificationFor, CreationDateTime, EmailSend, EmailSendAt, IsMarkedAsRead);
            _comment.CommentNotifications.Add(commentNotification);
            if (NotificationDao != null) {
                NotificationDao.Save(commentNotification);
                NotificationDao.Flush();
            }
            return commentNotification;
        }

        public CommentNotificationBuilder ForComment(Comment comment) {
            _comment = comment;

            return this;
        }

        public CommentNotificationBuilder WithReason(CommentNotificationReason reason) {
            _notificationReason = reason;
            return this;
        }
    }
}