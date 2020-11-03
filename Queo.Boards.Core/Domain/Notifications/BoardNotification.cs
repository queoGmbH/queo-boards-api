namespace Queo.Boards.Core.Domain.Notifications {
    /// <summary>
    ///     Bildet eine Benachrichtigung zu einem Board ab.
    /// </summary>
    public class BoardNotification : Notification {
        /// <summary>
        ///     Legt beim Überschreiben fest, welche Entität die Benachrichtigung betrifft.
        ///     Wird als Discriminator für NHibernate verwendet.
        /// </summary>
        public override NotificationCategory NotificationCategory {
            get { return NotificationCategory.Board; }
        }

        /// <summary>
        ///     Ruft den Anzeigenamen für die Benachrichtigung ab.
        /// </summary>
        public override string DisplayText {
            get {
                return string.Format("{0} for {1}", NotificationCategory, NotificationFor.UserName);
            }
        }
    }
}