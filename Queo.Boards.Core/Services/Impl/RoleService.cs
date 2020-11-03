using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service for <see cref="UserRole" />
    /// </summary>
    public class RoleService : IRoleService {
        /// <summary>
        ///     Returns all entries from <see cref="UserRole" />
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAll() {
            return UserRole.ToList();
        }
    }
}