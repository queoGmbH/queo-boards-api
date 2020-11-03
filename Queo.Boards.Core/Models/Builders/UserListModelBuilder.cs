using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Model Builder for <see cref="UserListModel" />
    /// </summary>
    public static class UserListModelBuilder {
        /// <summary>
        ///     Builds a <see cref="UserListModel" /> from a <see cref="User" />
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserListModel Build(User user) {
            return new UserListModel(
                user.BusinessId,
                user.UserName,
                user.Firstname,
                user.Lastname,
                user.Company,
                user.Department,
                user.Email,
                user.Phone,
                user.Roles,
                user.IsEnabled,
                user.CanWrite,
                user.UserCategory);
        }

        /// <summary>
        ///     Builds a list of <see cref="UserListModel" /> from a list of <see cref="User" />
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public static IList<UserListModel> Build(IList<User> users) {
            IList<UserListModel> listModels = new List<UserListModel>();
            foreach (User user in users) {
                listModels.Add(Build(user));
            }

            return listModels;
        }
    }
}