using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Data.NHibernate.Generic;

namespace Queo.Boards.Core.Persistence.Impl {

    /// <summary>
    /// Dao, der Methoden anbietet, um <see cref="Notification">Benachrichtigungen</see>zu persistieren.
    /// </summary>
    public class NotificationDao : GenericDao<Notification, int>, INotificationDao {
        /// <summary>
        /// Sucht nach Benachrichtigungen, für die eine E-Mail versendet werden muss.
        /// </summary>
        /// <returns>Benachrichtigungen, die bisher nicht gelesen wurden und zu denen noch keine E-Mail versendet wurde.</returns>
        /// <remarks>
        /// TODO: Die Überprüfung, ob der Nutzer in seinem Profil die E-Mail-Benachrichtigung aktiviert hat, wird später implementiert.
        /// </remarks>
        public IList<Notification> FindNotificationWhereToSendEmail() {

            FindHibernateDelegate<Notification> finder = delegate(ISession session) {
                return 
                    session.QueryOver<Notification>()
                        /*Keine gelesenen Benachrichtigungen*/
                        .WhereNot(notification => notification.IsMarkedAsRead)
                        /*Keine Benachrichtigungen, wo schon eine Mail versendet wurde*/
                        .AndNot(notification => notification.EmailSend)
                        .JoinQueryOver(notification => notification.NotificationFor)
                            .WhereRestrictionOn(user => user.Email).IsInsensitiveLike("_%@_%._%", MatchMode.Anywhere)
                        .List();
            };
            return HibernateTemplate.ExecuteFind(finder);
        }

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
        public IPage<Notification> FindForUser(IPageable pageRequest, User user, bool? isMarkedAsRead = null) {
            Require.NotNull(pageRequest, "pageRequest");
            Require.NotNull(user, "user");

            HibernateDelegate<IPage<Notification>> finder = delegate(ISession session) {
                IQueryOver<Notification, Notification> queryOver = session.QueryOver<Notification>();
                queryOver.Where(notification => notification.NotificationFor == user);
                if (isMarkedAsRead.HasValue) {
                    queryOver.And(notification => notification.IsMarkedAsRead == isMarkedAsRead.Value);
                }

                return FindPage(queryOver, pageRequest);
            };
            return HibernateTemplate.Execute(finder);
        }
    }
}