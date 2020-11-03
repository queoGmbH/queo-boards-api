using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain.Notifications;

namespace Queo.Boards.Core.Persistence.Mappings.Notifications {
    /// <summary>
    /// Mapping für eine Benachrichtigung für eine Karte.
    /// </summary>
    public class CommentNotificationMap : SubclassMap<CommentNotification> {
        public CommentNotificationMap() {

            DiscriminatorValue(NotificationCategory.Comment);

            Join("tblCommentNotification",
                j => {
                    j.KeyColumn("Id");
                    j.Map(notification => notification.NotificationReason).Not.Nullable().CustomSqlType("nvarchar(100)");
                    j.References(notification => notification.Comment).Not.Nullable().ForeignKey("FK_NOTIFICATION_FOR_COMMENT");
                });
        }
    }
}