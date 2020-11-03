using System;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Beschreibt einen Service, der Methoden anbietet, um Nutzer zu verwalten.
    /// </summary>
    public interface IUserService {
        /// <summary>
        ///     Creates a new local user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwdHash"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        /// <param name="canWrite"></param>
        /// <returns></returns>
        User Create(string username, string pwdHash, UserAdministrationDto administrationDto, UserProfileDto profileDto, bool canWrite);

        /// <summary>
        ///     Erstellt einen neuen lokalen Nutzer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwdHash"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        User Create(string username, string pwdHash, UserAdministrationDto administrationDto, UserProfileDto profileDto);

        /// <summary>
        ///     Erstellt einen neuen AD Nutzer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        User Create(string username, UserAdministrationDto administrationDto, UserProfileDto profileDto);

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

        IPage<User> GetAll(IPageable pageRequest);

        /// <summary>
        ///     Liefert einen Nutzer, anhand seiner <see cref="User.BusinessId" />
        /// </summary>
        /// <param name="id">Die BusinessId des Nutzers.</param>
        /// <returns></returns>
        User GetById(Guid id);

        /// <summary>
        ///     Synchronisiert die Nutzer mit dem Active Directory.
        /// </summary>
        /// <returns></returns>
        ActiveDirectorySynchronisationSummary SynchronizeWithActiveDirectory();

        /// <summary>
        ///     Updates the given user.
        /// </summary>
        /// <param name="userToUpdate"></param>
        /// <param name="updateData"></param>
        /// <returns></returns>
        User UpdateLocalUser(User userToUpdate, UserUpdateModel updateData);

        /// <summary>
        /// Updates the password for the User.
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="oldPassword">the old password</param>
        /// <param name="newPassword">the new password</param>
        /// <returns></returns>
        User Update(User user, string oldPassword, string newPassword);

        bool ValidateCredentials(string username, string password);
        
        /// <summary>
        /// Überschreibt das bestehende Passwort eines Nuutzers durch ein neues.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        User Update(User user, string newPassword);

        void PasswordResetRequest(string userName);

        /// <summary>
        ///     Führt einen Passwort-Reset aus, sofern der Nutzer zu finden ist.
        /// </summary>
        /// <param name="PasswordResetRequestId"></param>
        /// <param name="newPassword"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        void ResetPassword(Guid? PasswordResetRequestId, string newPassword);
    }
}