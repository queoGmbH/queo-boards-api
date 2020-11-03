using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;

namespace Queo.Boards.Core.Tests.Builders {
    public class UserBuilder : Builder<User> {
        private readonly IUserDao _userDao;
        private bool _canWrite = true;
        private string _company = "queo";
        private string _department = "queo-boards Zentrale";
        private string _email;
        private string _firstname = "Vorname";
        private bool _isEnabled = true;
        private string _lastname = "Nachname";
        private readonly IList<string> _roles = new List<string> {UserRole.USER};
        private string _userName;
        private string _phone;
        private bool _isLocal;
        private string _passwordHash;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public UserBuilder(IUserDao userDao) {
            _userDao = userDao;

            SetDefaults();
        }
        
        public UserBuilder AsLocalWithHash(string passwordHash) {
            _isLocal = true;
            _passwordHash = passwordHash;
            return this;
        }

        private void SetDefaults() {
            _email = GetRandomString(10) + "@queo-boards.de";
            _phone = "0049" + GetRandomNumber(12);
            _userName = "user_" + GetRandomString(5);
        }

        public override User Build() {
            User user = null;
            if (!_isLocal) {
                user = new User(_userName,
                    new UserAdministrationDto(_roles, _isEnabled),
                    new UserProfileDto(_email, _firstname, _lastname, _company, _department, _phone));
            } else {
                user = new User(_userName,_passwordHash,
                    new UserAdministrationDto(_roles, _isEnabled),
                    new UserProfileDto(_email, _firstname, _lastname, _company, _department, _phone));
            }
            user.UpdateCanWrite(_canWrite);
            if (_userDao != null) {
                _userDao.Save(user);
                _userDao.Flush();
            }
            try {
                return user;
            } finally {
                SetDefaults();
            }
        }

        public UserBuilder CanWrite(bool canWrite) {
            _canWrite = canWrite;
            return this;
        }

        /// <summary>
        ///     Legt die Abteilung des zu erstellenden Nutzers fest.
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>
        public UserBuilder InDepartment(string department) {
            _department = department;
            return this;
        }

        public UserBuilder WhoIsDisabled() {
            _isEnabled = false;
            return this;
        }

        /// <summary>
        ///     Legt die E-Mail-Adresse des zu erstellenden Nutzers fest.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public UserBuilder WithEmail(string email) {
            _email = email;
            return this;
        }

        /// <summary>
        ///     Legt den Vornamen des zu erstellenden Nutzers fest.
        /// </summary>
        /// <param name="firstname"></param>
        /// <returns></returns>
        public UserBuilder WithFirstname(string firstname) {
            _firstname = firstname;
            return this;
        }

        /// <summary>
        ///     Legt den Nachnamen des zu erstellenden Nutzers fest.
        /// </summary>
        /// <param name="lastname"></param>
        /// <returns></returns>
        public UserBuilder WithLastname(string lastname) {
            _lastname = lastname;
            return this;
        }

        /// <summary>
        ///     Legt die Rollen des Nutzers fest.
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        /// <remarks>Alle bisher zugeordneten Rollen werden entfernt.</remarks>
        public UserBuilder WithRoles(params string[] roles) {
            
                _roles.Clear();
                foreach (string role in roles) {
                    _roles.Add(role);
                }
            
            return this;
        }

        /// <summary>
        ///     Legt den Nutzernamen des zu erstellenden Nutzers fest.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public UserBuilder WithUserName(string userName) {
            _userName = userName;
            return this;
        }

        /// <summary>
        ///     Legt das Unternehmen des zu erstellenden Nutzers fest.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public UserBuilder WorkingFor(string company) {
            _company = company;
            return this;
        }
    }
    
}