using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Services;
using Spring.Context.Support;
using Task = System.Threading.Tasks.Task;

namespace Queo.Boards.Infrastructure.Security {
    public class ApplicationUserManager : UserManager<SecurityUser> {
        public ApplicationUserManager(IUserStore<SecurityUser> store) : base(store) {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> arg1, IOwinContext arg2) {
            UserStoreAdapter userStoreAdapter = ContextRegistry.GetContext().GetObject<UserStoreAdapter>();
            return new ApplicationUserManager(userStoreAdapter);
        }

        /// <summary>
        ///     Return a user with the specified username and password or null if there is no match.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override async Task<SecurityUser> FindAsync(string userName, string password) {
            UserStoreAdapter userStoreAdapter = ContextRegistry.GetContext().GetObject<UserStoreAdapter>();
            IUserService userService = ContextRegistry.GetContext().GetObject<IUserService>();

            /* ToDo: prüfen, ob der hier auch durch rennt, wenn später mal 'nen Request mit einem Token gemacht wird.
             * Das wäre dann ungünstig, weil dann mit jedem Request der Nutzer aus der DB geholt wird. */

            /* Den Nutzer aus der DB laden, um ggf. gleich abbrechen zu können, wenn er gar nicht existiert. */
            User user = userService.FindByUsername(userName);
            if (user == null) {
                return await Task.FromResult<SecurityUser>(null);
            }

            /* Gibt es den Nutzer in der DB, die Credentials prüfen */
            bool isCredentialsValide;
            switch (user.UserCategory) {
                case UserCategory.ActiveDirectory:
                    isCredentialsValide = userService.ValidateCredentials(user.UserName, password);
                    break;
                case UserCategory.Local:
                    PasswordVerificationResult passwordVerificationResult = PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
                    isCredentialsValide = passwordVerificationResult == PasswordVerificationResult.Success;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!isCredentialsValide) {
                return await Task.FromResult<SecurityUser>(null);
            }

            /* Sind auch die Credentials Ok, den SecurityUser erzeugen lassen */
            SecurityUser byNameAsync = UserStoreAdapter.CreateQueoBoardsSecurityUser(user);
            return byNameAsync;
        }

        public override Task<ClaimsIdentity> CreateIdentityAsync(SecurityUser user, string authenticationType) {
            
            IList<Claim> claims = user.GetClaims();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationType);

            return Task.FromResult(claimsIdentity);
        }
    }
}