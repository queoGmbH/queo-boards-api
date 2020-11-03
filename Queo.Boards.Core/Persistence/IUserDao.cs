using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Persistence {
    public interface IUserDao : IGenericDao<User, int> {
        /// <summary>
        ///     Gibt die Anzahl der aktiven Nutzer zurück
        /// </summary>
        /// <returns></returns>
        int CountAllActive();

        /// <summary>
        ///     Findet einen Nutzer anhand einer PasswortResetRerquestId.
        /// </summary>
        /// <param name="passwordResetRequestId"></param>
        /// <returns></returns>
        User FindByPasswordResetRequestId(Guid? passwordResetRequestId);

        /// <summary>
        ///     Sucht seitenweise nach Nutzern, die eine bestimmte Rolle haben.
        /// </summary>
        /// <param name="pageRequest">Seiteninformationen</param>
        /// <param name="role">Die Rolle, die ein Nutzer haben muss, um gefunden zu werden.</param>
        /// <returns></returns>
        IPage<User> FindByRole(IPageable pageRequest, string role);

        /// <summary>
        ///     Sucht nach einem Nutzer anhand seines Nutzernamens.
        /// </summary>
        /// <param name="username">Der eindeutige Name des Nutzers.</param>
        /// <returns></returns>
        User FindByUsername(string username);
    }
}