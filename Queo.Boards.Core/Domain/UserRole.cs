using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Rolle eines Nutzers in der Anwendung.
    /// </summary>
    public static class UserRole {
        /// <summary>
        ///     Ruft den Namen der Rolle für Administratoren ab.
        /// </summary>
        public const string ADMINISTRATOR = "Administrator";

        /// <summary>
        ///     Ruft den Namen der Rolle für normale Nutzer ab.
        /// </summary>
        public const string USER = "User";

        /// <summary>
        ///     Liefert den Namen der Rolle für den System-Admin
        /// </summary>
        public const string SYSTEM_ADMINISTRATOR = "System-Administrator";

        /// <summary>
        ///     Returns all entries.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IList<string> ToList() {
            return typeof(UserRole)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
        }
    }
}