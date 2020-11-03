using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Domain;

namespace Queo.Boards.Core.Domain {
    public class User : Entity {
        private readonly IList<string> _roles = new List<string>();
        private readonly UserCategory _userCategory;
        private bool _canWrite = true;
        private string _company;
        private string _department;
        private string _email;
        private string _firstname;
        private bool _isEnabled;
        private string _lastname;
        private string _passwordHash;

        private Guid? _passwordResetRequestId;
        private DateTime? _passwordResetRequestValidTo;

        private string _phone;
        private string _userName;

        /// <summary>
        ///     Ctor für NHibernate
        /// </summary>
        public User() {
        }

        /// <summary>
        ///     Liefert eine neue Instanz von <see cref="User" />
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        public User(string userName, UserAdministrationDto administrationDto, UserProfileDto profileDto) {
            Require.NotNullOrWhiteSpace(userName, "userName");
            Require.NotNull(profileDto, "profileDto");
            Require.NotNull(administrationDto, "administrationDto");
            _userCategory = UserCategory.ActiveDirectory;
            _userName = userName;
            Update(administrationDto);
            Update(profileDto);
        }

        /// <summary>
        ///     Liefert eine neue Instanz von <see cref="User" />
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="passwordHash"></param>
        /// <param name="administrationDto"></param>
        /// <param name="profileDto"></param>
        public User(string userName, string passwordHash, UserAdministrationDto administrationDto, UserProfileDto profileDto) {
            Require.NotNullOrWhiteSpace(userName, "userName");
            Require.NotNull(profileDto, "profileDto");
            Require.NotNull(administrationDto, "administrationDto");
            _userCategory = UserCategory.Local;
            _userName = userName;
            _passwordHash = passwordHash;
            Update(administrationDto);
            Update(profileDto);
        }

        /// <summary>
        ///     Liefert ob der Nutzer schreiben darf.
        /// </summary>
        public virtual bool CanWrite {
            get { return _canWrite; }
        }

        /// <summary>
        ///     Ruft das Unternehmen des Nutzers ab.
        /// </summary>
        public virtual string Company {
            get { return _company; }
        }

        /// <summary>
        ///     Ruft die Abteilung des Nutzers ab.
        /// </summary>
        public virtual string Department {
            get { return _department; }
        }

        /// <summary>
        ///     Ruft die E-Mail-Adresse des Nutzers ab.
        /// </summary>
        public virtual string Email {
            get { return _email; }
        }

        /// <summary>
        ///     Ruft den Vornamen des Nutzers ab.
        /// </summary>
        public virtual string Firstname {
            get { return _firstname; }
        }

        /// <summary>
        ///     Ruft ab, ob der Nutzer sich anmelden darf oder nicht.
        /// </summary>
        public virtual bool IsEnabled {
            get { return _isEnabled; }
        }

        /// <summary>
        ///     Ruft den Nachnamen des Nutzers ab.
        /// </summary>
        public virtual string Lastname {
            get { return _lastname; }
        }

        /// <summary>
        ///     Liefert den Passwort Hash des Nutzer, wenn es sich um einen lokalen Nutzer handelt.
        /// </summary>
        public virtual string PasswordHash {
            get { return _passwordHash; }
        }

        /// <summary>
        ///     Liefert die eine Guid für einen Passwort-Reset
        /// </summary>
        public virtual Guid? PasswordResetRequestId {
            get { return _passwordResetRequestId; }
        }

        /// <summary>
        ///     Liefert das Datum an dem ein Passwort-Reset-Request abläuft.
        /// </summary>
        public virtual DateTime? PasswordResetRequestValidTo {
            get { return _passwordResetRequestValidTo; }
        }

        /// <summary>
        ///     Ruft die Telefonnummer des Nutzers ab.
        /// </summary>
        public virtual string Phone {
            get { return _phone; }
        }

        /// <summary>
        ///     Ruft eine schreibgeschützte Kopie der Liste ALLER Rollen des Nutzers ab.
        /// </summary>
        public virtual IList<string> Roles {
            get { return new ReadOnlyCollection<string>(_roles); }
        }

        /// <summary>
        ///     Liefert die Kategorie des Nutzers (AD oder LOCAL)
        /// </summary>
        public virtual UserCategory UserCategory {
            get { return _userCategory; }
        }

        /// <summary>
        ///     Liefert den Nutzernamen
        /// </summary>
        public virtual string UserName {
            get { return _userName; }
        }

        /// <summary>
        ///     Erzeugt das <see cref="UserAdministrationDto" /> für den Nutzer.
        /// </summary>
        /// <returns></returns>
        public virtual UserAdministrationDto GetAdministrationDto() {
            return new UserAdministrationDto(_roles, _isEnabled);
        }

        /// <summary>
        ///     Ruft das <see cref="PasswordResetRequestDto" /> für diesen Nutzer ab
        /// </summary>
        /// <returns></returns>
        public virtual PasswordResetRequestDto GetPasswortResetRequestDto() {
            return new PasswordResetRequestDto(_passwordResetRequestValidTo, _passwordResetRequestId);
        }

        /// <summary>
        ///     Ruft das <see cref="UserProfileDto" /> für diesen Nutzer ab.
        /// </summary>
        /// <returns></returns>
        public virtual UserProfileDto GetProfileDto() {
            return new UserProfileDto(_email, _firstname, _lastname, _company, _department, _phone);
        }

        /// <summary>
        ///     Aktualisiert die Eigenschaften des Nutzers.
        /// </summary>
        /// <param name="userAdministrationDto"></param>
        /// <param name="userProfileDto"></param>
        public virtual void Update(UserAdministrationDto userAdministrationDto, UserProfileDto userProfileDto) {
            Require.NotNull(userAdministrationDto, "userAdministrationDto");
            Require.NotNull(userProfileDto, "userProfileDto");

            Update(userAdministrationDto);
            Update(userProfileDto);
        }

        /// <summary>
        ///     Aktualisiert die werte für einen PasswortResetRequest am Nutzer.
        /// </summary>
        /// <param name="passwordResetRequestDto"></param>
        public virtual void Update(PasswordResetRequestDto passwordResetRequestDto) {
            _passwordResetRequestId = passwordResetRequestDto.PasswordResetRequestId;
            _passwordResetRequestValidTo = passwordResetRequestDto.PasswordResetRequestValidTo;
        }

        /// <summary>
        ///     Aktualisiert ob ein Nutzer schreiben darf
        /// </summary>
        /// <param name="canWrite"></param>
        public virtual void UpdateCanWrite(bool canWrite) {
            _canWrite = canWrite;
        }

        /// <summary>
        ///     Updates user name and password
        /// </summary>
        /// <param name="newUserName"></param>
        /// <param name="newPasswordHash"></param>
        public virtual void UpdateLoginAndPasswordForLocalUser(string newUserName, string newPasswordHash) {
            if (UserCategory == UserCategory.Local) {
                _userName = newUserName;
                _passwordHash = newPasswordHash;
            }
        }

        /// <summary>
        ///     Updates user name
        /// </summary>
        /// <param name="name"></param>
        public virtual void UpdateLoginForLocalUser(string name) {
            if (UserCategory == UserCategory.Local) {
                _userName = name;
            }
        }

        private void Update(UserProfileDto userAdministrationDto) {
            _company = userAdministrationDto.Company;
            _department = userAdministrationDto.Department;
            _email = userAdministrationDto.Email;
            _firstname = userAdministrationDto.Firstname;
            _lastname = userAdministrationDto.Lastname;
            _phone = userAdministrationDto.Phone;
        }

        private void Update(UserAdministrationDto userAdministrationDto) {
            _roles.Clear();
            foreach (string role in userAdministrationDto.Roles) {
                _roles.Add(role);
            }

            _isEnabled = userAdministrationDto.IsEnabled;
        }
    }
}