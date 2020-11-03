using FluentNHibernate.Mapping;
using Queo.Boards.Core.Domain.Notifications;

namespace Queo.Boards.Core.Persistence.Mappings.Notifications {
    /// <summary>
    /// Mapping für eine Benachrichtigung für eine Karte.
    /// </summary>
    public class CardNotificationMap : SubclassMap<CardNotification> {
        public CardNotificationMap() {

            DiscriminatorValue(NotificationCategory.Card);

            Join("tblCardNotification",
                j => {
                    j.KeyColumn("Id");
                    j.Map(notification => notification.NotificationReason).Not.Nullable().CustomSqlType("nvarchar(100)");
                    j.References(notification => notification.Card).Not.Nullable().ForeignKey("FK_NOTIFICATION_FOR_CARD").Cascade.Delete();
                });
        }
    }
}