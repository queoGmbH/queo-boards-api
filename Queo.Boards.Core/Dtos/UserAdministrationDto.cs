using System.Collections.Generic;
using Queo.Boards.Core.Infrastructure.Utils;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    /// Dto das administrative Informationen über einen Nutzer trägt.
    /// </summary>
    public class UserAdministrationDto {

        /// <summary>
        /// Konstruktor für ModelBinding und Tests.
        /// </summary>
        public UserAdministrationDto() {
            Roles = new List<string>();
        }

        /// <summary>
        /// Konstruktor zu vollständigen Initialisierung des Dtos.
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="isEnabled"></param>
        public UserAdministrationDto(IList<string> roles, bool isEnabled) {
            Roles = roles;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Ruft ALLE Rollen des Nutzers ab oder legt diese fest.
        /// </summary>
        public IList<string> Roles { get; set; }

        /// <summary>
        /// Ruft ab oder legt fest, ob der Nutzer sich anmelden darf oder nicht.
        /// </summary>
        public bool IsEnabled { get; set; }

        protected bool Equals(UserAdministrationDto other) {
            return 
                ListHelper.AreEquivalent(Roles, other.Roles) && IsEnabled == other.IsEnabled;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserAdministrationDto)obj);
        }

        public override int GetHashCode() {
            unchecked { return ((Roles != null ? Roles.GetHashCode() : 0) * 397) ^ IsEnabled.GetHashCode(); }
        }
    }
}