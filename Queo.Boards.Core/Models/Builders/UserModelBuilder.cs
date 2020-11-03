using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder für <see cref="UserModel" />
    /// </summary>
    public static class UserModelBuilder {
        /// <summary>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserModel BuildUser(User user) {
            return new UserModel(user.BusinessId, user.UserName, user.Firstname, user.Lastname);
        }

        /// <summary>
        /// </summary>
        /// <param name="users">Liste der Nutzer deren Models erstellt werden sollen.</param>
        /// <returns></returns>
        public static IList<UserModel> BuildUsers(IList<User> users) {
            if (users == null) {
                return new List<UserModel>();
            }
            return users.Where(x => x != null).Select(BuildUser).ToList();
        }
    }
}