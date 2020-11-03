using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence.Mappings;

namespace Queo.Boards.Core.Persistence.Mappings.Notifications {

    /// <summary>
    /// Mapping für die Basisklasse der Benachrichtigungen.
    /// </summary>
    public class NotificationMap : EntityMap<Notification> {
        protected NotificationMap() {
            Map(notification => notification.CreationDateTime).Not.Nullable();
            Map(notification => notification.EmailSendAt).Nullable();
            Map(notification => notification.EmailSend).Not.Nullable();
            Map(notification => notification.IsMarkedAsRead).Not.Nullable();

            References(notification => notification.NotificationFor).Not.Nullable().ForeignKey("FK_NOTIFICATION_FOR_USER");

            DiscriminateSubClassesOnColumn("NotificationCategory").SqlType("nvarchar(100)");
        }
    }
}