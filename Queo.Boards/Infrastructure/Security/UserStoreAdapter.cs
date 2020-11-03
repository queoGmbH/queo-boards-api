using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Services;
using Task = System.Threading.Tasks.Task;

namespace Queo.Boards.Infrastructure.Security {
    public class UserStoreAdapter : IUserStore<SecurityUser> {
        public UserStoreAdapter(IUserService userService) {
            Require.NotNull(userService, "userService");

            UserService = userService;
        }

        public IUserService UserService { get; set; }

        public static SecurityUser CreateQueoBoardsSecurityUser(User user) {
            return new SecurityUser(user.BusinessId.ToString(), user.UserName, user.Firstname, user.Lastname, user.Email, user.Roles, user.IsEnabled);
        }

        public Task CreateAsync(SecurityUser user) {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(SecurityUser user) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            // TODO: Muss hier was gemacht werden?
        }

        public Task<SecurityUser> FindByIdAsync(string userId) {
            User foundByUsername = UserService.GetById(Guid.Parse(userId));
            if (foundByUsername != null) {
                return Task.FromResult(new SecurityUser());
            } else {
                return Task.FromResult<SecurityUser>(null);
            }
        }

        public Task<SecurityUser> FindByNameAsync(string userName) {
            User foundByUsername = UserService.FindByUsername(userName);
            if (foundByUsername != null) {
                return Task.FromResult(CreateQueoBoardsSecurityUser(foundByUsername));
            } else {
                return Task.FromResult<SecurityUser>(null);
            }
        }

        public Task UpdateAsync(SecurityUser user) {
            throw new NotImplementedException();
        }
    }
}