using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Interface for services for <see cref="UserRole" />
    /// </summary>
    public interface IRoleService {
        /// <summary>
        ///     Returns all entries from <see cref="UserRole" />
        /// </summary>
        /// <returns></returns>
        IList<string> GetAll();
    }
}