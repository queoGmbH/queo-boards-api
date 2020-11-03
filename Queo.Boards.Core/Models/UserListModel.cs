using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für die Anzeige einer Nutzerliste
    /// </summary>
    public class UserListModel : ExtendedUserModel {
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
        /// <param name="userCategory"></param>
        public UserListModel(
            Guid businessId, string name, string firstname, string lastname, string company, string department, string mail, string phone,
            IList<string> roles, bool isEnabled, bool canWrite, UserCategory userCategory) : base(
            businessId,
            name,
            firstname,
            lastname,
            company,
            department,
            mail,
            phone,
            roles,
            isEnabled,
            canWrite) {
            UserCategory = userCategory;
        }

        /// <summary>
        ///     Liefert die Kategorie des Nutzers
        /// </summary>
        public UserCategory UserCategory { get; }
    }
}