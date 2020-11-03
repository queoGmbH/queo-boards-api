using System;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Spring.Data.NHibernate.Generic;

namespace Queo.Boards.Core.Persistence.Impl {
    /// <summary>
    ///     Dao mit Methoden zum Persistieren von Nutzern.
    /// </summary>
    public class UserDao : GenericDao<User, int>, IUserDao {
        /// <summary>
        ///     Gibt die Anzahl der aktiven Nutzer zurück
        /// </summary>
        /// <returns></returns>
        public int CountAllActive() {
            HibernateDelegate<int> finder = delegate(ISession session) {
                ICriteria criteria = session.CreateCriteria(typeof(User)).SetProjection(Projections.RowCount());
                criteria.Add(Restrictions.Eq(nameof(User.IsEnabled), true));

                return (int)(criteria.UniqueResult());
            };
            return HibernateTemplate.Execute(finder);
        }

        /// <summary>
        ///     Findet einen Nutzer anhand einer PasswortResetRerquestId.
        /// </summary>
        /// <param name="passwordResetRequestId"></param>
        /// <returns></returns>
        public User FindByPasswordResetRequestId(Guid? passwordResetRequestId) {
            if (passwordResetRequestId != null && passwordResetRequestId != Guid.Empty) {
                HibernateDelegate<User> finder = delegate(ISession session) {
                    IQueryOver<User, User> queryOver = session.QueryOver<User>().Where(user => user.PasswordResetRequestId == passwordResetRequestId);
                    return queryOver.SingleOrDefault();
                };

                return HibernateTemplate.Execute(finder);
            }

            throw new ArgumentException();
        }

        /// <summary>
        ///     Sucht seitenweise nach Nutzern, die eine bestimmte Rolle haben.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="role">Die Rolle, die ein Nutzer haben muss, um gefunden zu werden.</param>
        /// <returns></returns>
        public IPage<User> FindByRole(IPageable pageRequest, string role) {
            Require.NotNull(pageRequest, "pageRequest");
            Require.NotNullOrWhiteSpace(role, "role");

            Action<ICriteria> criteriaBuilder = delegate(ICriteria criteria) {
                ICriteria rolesCriteria = criteria.CreateCriteria("Roles", JoinType.LeftOuterJoin);
                rolesCriteria.Add(Restrictions.In("elements", new[] { role }));
            };
            return Find(pageRequest, criteriaBuilder);
        }

        /// <summary>
        ///     Sucht nach einem Nutzer anhand seines Nutzernamens.
        /// </summary>
        /// <param name="username">Der eindeutige Name des Nutzers.</param>
        /// <returns></returns>
        public User FindByUsername(string username) {
            Require.NotNullOrWhiteSpace(username, "username");

            HibernateDelegate<User> finder = delegate(ISession session) {
                IQueryOver<User, User> queryOver = session.QueryOver<User>().Where(user => user.UserName == username);
                return queryOver.SingleOrDefault();
            };

            return HibernateTemplate.Execute(finder);
        }
    }
}