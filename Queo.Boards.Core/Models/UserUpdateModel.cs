using System.Collections.Generic;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model zum Aktualisieren eines Nutzers
    /// </summary>
    public class UserUpdateModel {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="company"></param>
        /// <param name="department"></param>
        /// <param name="mail"></param>
        /// <param name="phone"></param>
        /// <param name="roles"></param>
        /// <param name="isEnabled"></param>
        /// <param name="canWrite"></param>
        public UserUpdateModel(
            string name, string firstname, string lastname, string company, string department, string mail, string phone,
            IList<string> roles, bool isEnabled, bool canWrite) {
            Name = name;
            Firstname = firstname;
            Lastname = lastname;
            Company = company;
            Department = department;
            Mail = mail;
            Phone = phone;
            Roles = roles;
            IsEnabled = isEnabled;
            CanWrite = canWrite;
        }

        /// <summary>
        ///     Liefert ob der Nutzer schreiben darf
        /// </summary>
        public bool CanWrite { get; }

        /// <summary>
        ///     Liefert die Firma
        /// </summary>
        public string Company { get; }

        /// <summary>
        ///     Liefert die Abteilung
        /// </summary>
        public string Department { get; }

        /// <summary>
        ///     Liefert den Vornamen
        /// </summary>
        public string Firstname { get; }

        /// <summary>
        ///     Liefert ob der Nutzer aktiviert ist.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        ///     Liefert den Nachnamen
        /// </summary>
        public string Lastname { get; }

        /// <summary>
        ///     Liefert die E-Mail
        /// </summary>
        public string Mail { get; }

        /// <summary>
        ///     Liefert den Nutzernamen
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Liefert die Telefonnummer
        /// </summary>
        public string Phone { get; }

        /// <summary>
        ///     Liefert die Rollen
        /// </summary>
        public IList<string> Roles { get; }
    }
}