using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Schnittstelle, die einen Dao beschreibt, der Methoden anbietet, um
    ///     <see cref="Notification">Benachrichtigungen</see>zu persistieren.
    /// </summary>
    public interface INotificationDao : IGenericDao<Notification, int> {
        /// <summary>
        ///     Ruft seitenweise <see cref="Notification">Benachrichtigungen</see> für Nutzer ab.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="user">Der Nutzer, dessen Benachrichtigungen abgerufen werden sollen.</param>
        /// <param name="isMarkedAsRead">
        ///     Sollen nur ungelesene Benachrichtigungen abgerufen werden.
        ///     true  = Es werden nur <see cref="Notification.IsMarkedAsRead">gelesene Nachrichten</see> abgerufen.
        ///     false = Es werden nur <see cref="Notification.IsMarkedAsRead">ungelesene Nachrichten</see> abgerufen.
        ///     null  = Es werden alle Nachrichten, unabhängig vom <see cref="Notification.IsMarkedAsRead">Gelesen-Status</see>
        ///     abgerufen.
        /// </param>
        /// <returns></returns>
        IPage<Notification> FindForUser(IPageable pageRequest, User user, bool? isMarkedAsRead = null);

        /// <summary>
        ///     Sucht nach Benachrichtigungen, für die eine E-Mail versendet werden muss.
        /// </summary>
        /// <returns>Benachrichtigungen, die bisher nicht gelesen wurden und zu denen noch keine E-Mail versendet wurde.</returns>
        IList<Notification> FindNotificationWhereToSendEmail();
    }
}