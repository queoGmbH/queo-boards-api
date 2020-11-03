using System;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.Builders {
    public static class NotificationModelBuilder {
        public static NotificationModel Build(Notification notification) {
            Require.NotNull(notification, "notification");

            switch (notification.NotificationCategory) {
                case NotificationCategory.Card:
                    return Build((CardNotification)notification);

                case NotificationCategory.Comment:
                    return Build((CommentNotification)notification);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static CommentNotificationModel Build(CommentNotification commentNotification) {
            Require.NotNull(commentNotification, "commentNotification");

            return new CommentNotificationModel(UserModelBuilder.BuildUser(commentNotification.NotificationFor),
                CommentModelBuilder.Build(commentNotification.Comment),
                commentNotification.NotificationReason,
                commentNotification.CreationDateTime,
                commentNotification.EmailSend,
                commentNotification.EmailSendAt,
                commentNotification.IsMarkedAsRead);
        }

        public static CardNotificationModel Build(CardNotification cardNotification) {
            Require.NotNull(cardNotification, "cardNotification");

            return new CardNotificationModel(UserModelBuilder.BuildUser(cardNotification.NotificationFor),
                CardModelBuilder.Build(cardNotification.Card),
                cardNotification.NotificationReason,
                cardNotification.CreationDateTime,
                cardNotification.EmailSend,
                cardNotification.EmailSendAt,
                cardNotification.IsMarkedAsRead);
        }
    }
}