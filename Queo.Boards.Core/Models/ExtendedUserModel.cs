using System;
using System.Collections.Generic;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Erweiteres Nutzer Modell um EIgenschaften für z.B. Listenansichten aller Nutzer
    /// </summary>
    public class ExtendedUserModel : UserModel {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="businessId"></param>
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
        public ExtendedUserModel(
            Guid businessId, string name, string firstname, string lastname, string company, string department, string mail, string phone,
            IList<string> roles, bool isEnabled, bool canWrite) : base(businessId, name, firstname, lastname) {
            Company = company;
            Department = department;
            Mail = mail;
            Phone = phone;
            Roles = roles;
            IsEnabled = isEnabled;
            CanWrite = canWrite;
        }

        /// <summary>
        ///     Liefert ob der Nutzer schreben darf
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
        ///     Liefer ob der Nutzer aktiv ist
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        ///     Liefert die E-Mail Adresse
        /// </summary>
        public string Mail { get; }

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