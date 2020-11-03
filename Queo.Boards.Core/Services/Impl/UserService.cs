using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Exceptions;
using Queo.Boards.Core.Infrastructure.ActiveDirectory;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Persistence;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service, der Methoden anbietet um Nutzer zu Verwalten.
    /// </summary>
    public class UserService : IUserService {
        private readonly IActiveDirectoryService _activeDirectoryService;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ISecurityService _securityService;
        private readonly IUserDao _userDao;

        /// <summary>
        ///     Konstruktor für Testfälle.
        /// </summary>
        /// <param name="userDao"></param>
        /// <param name="activeDirectoryService"></param>
        /// <param name="securityService"></param>
        /// <param name="emailNotificationService"></param>
        public UserService(IUserDao userDao, IActiveDirectoryService activeDirectoryService, ISecurityService securityService,
            IEmailNotificationService emailNotificationService) {
            Require.NotNull(activeDirectoryService, nameof(activeDirectoryService));
            Require.NotNull(userDao, nameof(userDao));
            Require.NotNull(securityService, nameof(securityService));

            _userDao = userDao;
            _activeDirectoryService = activeDirectoryService;
            _securityService = securityService;
            _emailNotificationService = emailNotificationService;
        }

        /// <summary>
        ///     Configuration value, set by spring
        /// </summary>
        public int MaxUser { get; set; }

        /// <summary>
        ///     Liefert oder setzt wie lange ein Passwort-Reset-Request gültig ist.
        ///     Wird von Spring gesetzt un kommt aus der Konfig.
        /// </summary>
        public double PasswordResetHours { get; set; }

        public static UserProfileDto CreateProfileForActiveDirectoryInformation(UserNtInformation adInformation) {
            if (adInformation != null) {
                return new UserProfileDto(
                    adInformation.Email,
                    adInformation.FirstName,
                    adInformation.LastName,
                    adInformation.Company,
                    adInformation.Department,
                    adInformation.Phone);
            }

            return new UserProfileDto();
        }

        /// <summary>
        ///     Erstellt einen neuen AD Nutzer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        [Transaction]
        public User Create(string username, UserAdministrationDto administrationDto, UserProfileDto profileDto) {
            Require.NotNullOrWhiteSpace(username, "username");
            Require.NotNull(administrationDto, "administrationDto");
            Require.NotNull(profileDto, "profileDto");

            User user = new User(username, administrationDto, profileDto);
            _userDao.Save(user);

            return user;
        }

        /// <summary>
        ///     Creates a new local user.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwdHash"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        /// <param name="canWrite"></param>
        /// <returns></returns>
        [Transaction]
        public User Create(string username, string pwdHash, UserAdministrationDto administrationDto, UserProfileDto profileDto, bool canWrite) {
            User user = Create(username, pwdHash, administrationDto, profileDto);
            user.UpdateCanWrite(canWrite);
            return user;
        }

        /// <summary>
        ///     Erstellt einen neuen lokalen Nutzer
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwdHash"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        /// <returns></returns>
        [Transaction]
        public User Create(string username, string pwdHash, UserAdministrationDto administrationDto, UserProfileDto profileDto) {
            Require.NotNullOrWhiteSpace(username, "username");
            Require.NotNull(administrationDto, "administrationDto");
            Require.NotNull(profileDto, "profileDto");

            CheckUserLimit();

            User user = new User(username, pwdHash, administrationDto, profileDto);
            _userDao.Save(user);

            return user;
        }

        public IPage<User> FindByRole(IPageable pageRequest, string role) {
            return _userDao.FindByRole(pageRequest, role);
        }

        /// <summary>
        ///     Sucht nach einem Nutzer anhand seines Nutzernamens.
        /// </summary>
        /// <param name="username">Der eindeutige Name des Nutzers.</param>
        /// <returns></returns>
        public User FindByUsername(string username) {
            return _userDao.FindByUsername(username);
        }

        public IPage<User> GetAll(IPageable pageRequest) {
            return _userDao.GetAll(pageRequest);
        }

        /// <summary>
        ///     Liefert einen Nutzer, anhand seiner <see cref="User.BusinessId" />
        /// </summary>
        /// <param name="id">Die BusinessId des Nutzers.</param>
        /// <returns></returns>
        public User GetById(Guid id) {
            return _userDao.GetByBusinessId(id);
        }

        /// <summary>
        ///     Setzt einen Passwort-Reset-Request anhand das UserName
        /// </summary>
        /// <param name="userName"></param>
        [Transaction]
        public void PasswordResetRequest(string userName) {
            User requesedUser = _userDao.FindByUsername(userName);
            if (requesedUser != null) {
                switch (requesedUser.UserCategory) {
                    case UserCategory.ActiveDirectory:
                        _emailNotificationService.SendADUserPasswordResetMessage(requesedUser);
                        break;
                    case UserCategory.Local:
                        Guid resetGuid = Guid.NewGuid();
                        DateTime resetExpiredDate = DateTime.Now.AddHours(PasswordResetHours);
                        PasswordResetRequestDto passwordResetRequestDto = new PasswordResetRequestDto(resetExpiredDate, resetGuid);

                        requesedUser.Update(passwordResetRequestDto);
                        _emailNotificationService.SendLocalUserPasswordResetMessage(requesedUser, passwordResetRequestDto);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     Führt einen Passwort-Reset aus, sofern der Nutzer zu finden ist.
        /// </summary>
        /// <param name="passwordResetRequestId"></param>
        /// <param name="newPassword"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        [Transaction]
        public void ResetPassword(Guid? passwordResetRequestId, string newPassword) {
            if (passwordResetRequestId != null && passwordResetRequestId != Guid.Empty) {
                Require.NotNullOrWhiteSpace(newPassword, nameof(newPassword));

                User toResetUser = _userDao.FindByPasswordResetRequestId(passwordResetRequestId);

                if (toResetUser != null) {
                    try {
                        if (toResetUser.PasswordResetRequestValidTo >= DateTime.Now) {
                            toResetUser.UpdateLoginAndPasswordForLocalUser(toResetUser.UserName, _securityService.HashPassword(newPassword));
                        } else {
                            throw new ArgumentOutOfRangeException("Resetzeitraum ist abgelaufen.");
                        }
                    } finally {
                        toResetUser.Update(new PasswordResetRequestDto(null, null));
                    }
                } else {
                    throw new ArgumentNullException("Für die Id konnte kein Nutzer gefunden werden.");
                }
            } else {
                throw new ArgumentException("Die Passwort-Reset-Request-Id darf nicht leer sein.");
            }
        }

        /// <summary>
        ///     Synchronisiert die Nutzer mit dem Active Directory.
        /// </summary>
        /// <returns></returns>
        [Transaction]
        public ActiveDirectorySynchronisationSummary SynchronizeWithActiveDirectory() {
            LogManager.GetLogger<UserService>().Info("Synchronisierung der Nutzer mit Active Directory gestartet.");
            try {
                /*Die Nutzername aller Administratoren für queo-boards aus dem AD laden.*/
                IList<string> queoBoardsAdministrators = _activeDirectoryService.FindQueoBoardsAdministrators();
                /*Die Nutzername aller Nutzer für queo-boards aus dem AD laden.*/
                IList<string> queoBoardsUsers = _activeDirectoryService.FindQueoBoardsUsers();

                /*Die Nutzernamen ermitteln, die SOWOHL Admin ALS AUCH normaler Nutzer sein sollen*/
                IList<string> queoBoardsAdministratorsAndUsers = queoBoardsAdministrators.Intersect(queoBoardsUsers).ToList();
                /*Die Nutzernamen ermitteln, die NUR normaler Nutzer sein sollen*/
                queoBoardsUsers = queoBoardsUsers.Except(queoBoardsAdministratorsAndUsers).ToList();
                /*Die Nutzernamen ermitteln, die NUR Administrator sein sollen*/
                queoBoardsAdministrators = queoBoardsAdministrators.Except(queoBoardsAdministratorsAndUsers).ToList();

                /*Die vorhandenen Nutzer aus der Datenbank laden*/
                IList<User> users = _userDao.GetAll().Where(x => x.UserCategory == UserCategory.ActiveDirectory).ToList();
                IList<User> createdUsers = new List<User>();
                IList<User> updatedUsers = new List<User>();
                IList<User> deletedUsers = new List<User>();

                /*Alle Nutzer, deren Name nicht mehr in der Liste der Nutzer aus dem AD vorkommt müssen gelöscht werden*/
                IList<User> usersToDelete =
                    users.Where(
                        user =>
                            !(queoBoardsAdministrators.Contains(user.UserName) || queoBoardsUsers.Contains(user.UserName)
                                                                               || queoBoardsAdministratorsAndUsers.Contains(user.UserName))).ToList();
                foreach (User user in usersToDelete) {
                    Update(user, new UserAdministrationDto(user.Roles, false), user.GetProfileDto());
                    deletedUsers.Add(user);
                }

                /*Nutzer synchronisieren, die sowohl Administrator als auch Nutzer sind*/
                SynchronizeQueoBoardsUsersForRoles(
                    queoBoardsAdministratorsAndUsers,
                    users,
                    new List<string> { UserRole.ADMINISTRATOR, UserRole.USER },
                    createdUsers,
                    updatedUsers);

                /*Nutzer synchronisieren, die nur Administrator sind*/
                SynchronizeQueoBoardsUsersForRoles(
                    queoBoardsAdministrators,
                    users,
                    new List<string> { UserRole.ADMINISTRATOR },
                    createdUsers,
                    updatedUsers);

                /*Nutzer synchronisieren, die nur normaler Nutzer sind*/
                SynchronizeQueoBoardsUsersForRoles(queoBoardsUsers, users, new List<string> { UserRole.USER }, createdUsers, updatedUsers);

                ActiveDirectorySynchronisationSummary activeDirectorySynchronisationSummary =
                    new ActiveDirectorySynchronisationSummary(createdUsers, updatedUsers, deletedUsers);
                LogManager.GetLogger<UserService>()
                    .InfoFormat(
                        "Synchronisierung der Nutzer mit Active Directory fertiggestellt. " + Environment.NewLine + "{0} erstellte Nutzer: {1} " +
                        Environment.NewLine + "{2} aktualisierte Nutzer: {3} " + Environment.NewLine + "{4} gelöschte Nutzer: {5}",
                        activeDirectorySynchronisationSummary.CreatedUsers.Count,
                        string.Join(", ", activeDirectorySynchronisationSummary.CreatedUsers.Select(u => u.UserName)),
                        activeDirectorySynchronisationSummary.UpdatedUsers.Count,
                        string.Join(", ", activeDirectorySynchronisationSummary.UpdatedUsers.Select(u => u.UserName)),
                        activeDirectorySynchronisationSummary.DeletedUsers.Count,
                        string.Join(", ", activeDirectorySynchronisationSummary.DeletedUsers.Select(u => u.UserName)));
                return activeDirectorySynchronisationSummary;
            } catch (Exception exception) {
                LogManager.GetLogger<UserService>().Error("Fehler beim Synchronisieren der Nutzer.", exception);
                throw;
            }
        }

        [Transaction]
        public void Update(User user, UserAdministrationDto administrationDto, UserProfileDto profileDto) {
            Require.NotNull(user, "user");
            Require.NotNull(administrationDto, "administrationDto");
            Require.NotNull(profileDto, "profileDto");

            user.Update(administrationDto, profileDto);
        }

        /// <summary>
        ///     Updates the password for the User.
        /// </summary>
        /// <param name="user">the user</param>
        /// <param name="oldPassword">the old password</param>
        /// <param name="newPassword">the new password</param>
        /// <returns></returns>
        [Transaction]
        public User Update(User user, string oldPassword, string newPassword) {
            return Update(user, newPassword);
        }

        /// <summary>
        ///     Überschreibt das bestehende Passwort eines Nuutzers durch ein neues.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [Transaction]
        public User Update(User user, string newPassword) {
            string hashPassword = _securityService.HashPassword(newPassword);
            user.UpdateLoginAndPasswordForLocalUser(user.UserName, hashPassword);
            return user;
        }

        /// <summary>
        ///     Updates the given user.
        /// </summary>
        /// <param name="userToUpdate"></param>
        /// <param name="updateData"></param>
        /// <returns>Changed user or unchanged user if its a user from the active directory</returns>
        [Transaction]
        public User UpdateLocalUser(User userToUpdate, UserUpdateModel updateData) {
            if (userToUpdate.UserCategory == UserCategory.ActiveDirectory) {
                return userToUpdate;
            }

            UserAdministrationDto userAdministrationDto = userToUpdate.GetAdministrationDto();
            userAdministrationDto.IsEnabled = updateData.IsEnabled;
            userAdministrationDto.Roles = updateData.Roles;

            UserProfileDto userProfileDto = userToUpdate.GetProfileDto();
            userProfileDto.Company = updateData.Company;
            userProfileDto.Department = updateData.Department;
            userProfileDto.Email = updateData.Mail;
            userProfileDto.Firstname = updateData.Firstname;
            userProfileDto.Lastname = updateData.Lastname;
            userProfileDto.Phone = updateData.Phone;

            userToUpdate.Update(userAdministrationDto, userProfileDto);
            userToUpdate.UpdateCanWrite(updateData.CanWrite);

            userToUpdate.UpdateLoginForLocalUser(updateData.Name);

            return userToUpdate;
        }

        /// <summary>
        ///     Prüft,
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password) {
            return _activeDirectoryService.Authenticate(username, password);
        }

        private static User FindExistingUserByName(IList<User> users, string queoBoardsAdministratorsAndUser) {
            if (!users.Any()) {
                return null;
            }

            return users.FirstOrDefault(user => user.UserName == queoBoardsAdministratorsAndUser);
        }

        /// <summary>
        ///     Check limit of users defined in properties file
        /// </summary>
        private void CheckUserLimit() {
            if (MaxUser > 0 && _userDao.CountAllActive() >= MaxUser) {
                LogManager.GetLogger<UserService>().Warn($"Das Limit ({MaxUser}) der möglichen Benutzer wurde überschritten.");
                throw new UserLimitReachedException();
            }
        }

        private bool IsDirty(User existingUser, UserAdministrationDto userAdministrationDto, UserProfileDto profileDto) {
            if (!existingUser.GetAdministrationDto().Equals(userAdministrationDto)) {
                return true;
            }

            if (!existingUser.GetProfileDto().Equals(profileDto)) {
                return true;
            }

            return false;
        }

        private void SynchronizeQueoBoardsUsersForRoles(
            IList<string> activeDirectoryUsernames, IList<User> users, IList<string> rolesToGiveUsers,
            IList<User> createdUsers, IList<User> updatedUsers) {
            foreach (string activeDirectoryUsername in activeDirectoryUsernames) {
                User existingUser = FindExistingUserByName(users, activeDirectoryUsername);
                UserNtInformation findUserInformation = _activeDirectoryService.FindUserInformation(activeDirectoryUsername);
                if (existingUser == null) {
                    User newUser = Create(
                        activeDirectoryUsername,
                        new UserAdministrationDto(rolesToGiveUsers, true),
                        CreateProfileForActiveDirectoryInformation(findUserInformation));
                    createdUsers.Add(newUser);
                }
                else {
                    UserProfileDto profileForActiveDirectoryInformation = CreateProfileForActiveDirectoryInformation(findUserInformation);
                    UserAdministrationDto userAdministrationDto = new UserAdministrationDto(rolesToGiveUsers, true);
                    if (IsDirty(existingUser, userAdministrationDto, profileForActiveDirectoryInformation)) {
                        Update(existingUser, userAdministrationDto, profileForActiveDirectoryInformation);
                        updatedUsers.Add(existingUser);
                    }
                }
            }
        }
    }
}