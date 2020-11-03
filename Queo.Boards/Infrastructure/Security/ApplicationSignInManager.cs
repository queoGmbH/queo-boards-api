using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Queo.Boards.Infrastructure.Security {
    public class ApplicationSignInManager : SignInManager<SecurityUser, string> {
        public ApplicationSignInManager(UserManager<SecurityUser, string> userManager, IAuthenticationManager authenticationManager) : base(userManager, authenticationManager) {
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context) {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }



        /// <summary>
        ///     Creates a user identity
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(SecurityUser user) {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
    }
}