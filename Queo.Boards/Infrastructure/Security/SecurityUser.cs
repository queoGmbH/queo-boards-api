using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Queo.Boards.Infrastructure.Security {
    public class SecurityUser : IUser<string> {
        private readonly IList<string> _roles;
        private readonly bool _isEnabled;
        private string _userName;

        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;

        public SecurityUser() {
        }

        public SecurityUser(string id, string userName, string firstName, string lastName, string email, IList<string> roles, bool isEnabled) {
            _roles = roles;
            _isEnabled = isEnabled;
            Id = id;
            _userName = userName;
            _firstName = firstName;
            _lastName = lastName;
            _email = email;
        }

        /// <summary>
        /// Unique key for the user
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Ruft ab, ob der Nutzer aktiviert ist oder nicht.
        /// </summary>
        public bool IsEnabled {
            get { return _isEnabled; }
        }

        /// <summary>
        ///     Ruft eine schreibgeschützte Kopie der Liste mit Rollen des Nutzers ab.
        /// </summary>
        public IList<string> Roles {
            get { return new ReadOnlyCollection<string>(_roles); }
        }

        public string UserName {
            get { return _userName; }
            set { _userName = value; }
        }

        public IList<Claim> GetClaims() {
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, Id));
            claims.Add(new Claim(ClaimTypes.Name, _userName));
            if (!string.IsNullOrWhiteSpace(_email)) {
                claims.Add(new Claim(ClaimTypes.Email, _email));
            }
            if (!string.IsNullOrWhiteSpace(_firstName)) {
                claims.Add(new Claim(ClaimTypes.GivenName, _firstName));
            }
            if (!string.IsNullOrWhiteSpace(_lastName)) {
                claims.Add(new Claim(ClaimTypes.Surname, _lastName));
            }

            foreach (string role in Roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<SecurityUser> manager) {
            // Beachten Sie, dass der "authenticationType" mit dem in "CookieAuthenticationOptions.AuthenticationType" definierten Typ übereinstimmen muss.
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Benutzerdefinierte Benutzeransprüche hier hinzufügen
            return userIdentity;
        }
    }
}