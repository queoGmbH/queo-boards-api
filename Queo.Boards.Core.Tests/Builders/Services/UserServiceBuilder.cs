using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;

namespace Queo.Boards.Core.Tests.Builders.Services {
    /// <summary>
    ///     Builder for <see cref="UserService" /> implementation of <see cref="IUserService" />
    /// </summary>
    public class UserServiceBuilder : Builder<UserService> {
        private IActiveDirectoryService _activeDirectoryService;
        private ISecurityService _securityService;
        private readonly IEmailNotificationService _emailNotificationService;
        private IUserDao _userDao;

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="userDao"></param>
        /// <param name="activeDirectoryService"></param>
        /// <param name="securityService"></param>
        /// <param name="emailNotificationService"></param>
        public UserServiceBuilder(IUserDao userDao, IActiveDirectoryService activeDirectoryService, ISecurityService securityService, IEmailNotificationService emailNotificationService) {
            _userDao = userDao;
            _activeDirectoryService = activeDirectoryService;
            _securityService = securityService;
            _emailNotificationService = emailNotificationService;
        }

        /// <summary>
        ///     Creates a new <see cref="UserService" /> instance.
        /// </summary>
        /// <returns></returns>
        public override UserService Build() {
            return new UserService(_userDao, _activeDirectoryService, _securityService, _emailNotificationService);
        }

        /// <summary>
        ///     Sets a <see cref="IUserDao" />
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public UserServiceBuilder With(IUserDao userDao) {
            _userDao = userDao;
            return this;
        }

        /// <summary>
        ///     Sets a <see cref="IActiveDirectoryService" />
        /// </summary>
        /// <param name="activeDirectoryService"></param>
        /// <returns></returns>
        public UserServiceBuilder With(IActiveDirectoryService activeDirectoryService) {
            _activeDirectoryService = activeDirectoryService;
            return this;
        }

        /// <summary>
        ///     Sets a <see cref="ISecurityService" />
        /// </summary>
        /// <param name="securityService"></param>
        /// <returns></returns>
        public UserServiceBuilder With(ISecurityService securityService) {
            _securityService = securityService;
            return this;
        }
    }
}