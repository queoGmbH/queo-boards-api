using System.ComponentModel;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Infrastructure.SignalR {
    /// <summary>
    ///     Auflistung der Möglichen Kontexte, in denen Nutzer per Signal R benachrichtigt werden.
    /// </summary>
    public enum SignalrNotificationScope {
        /// <summary>
        ///     Es werden alle Nutzer in der Rolle <see cref="UserRole.ADMINISTRATOR" /> per SignalR benachrichtigt.
        /// </summary>
        [Description("Es werden alle Nutzer in der Rolle Administrator per SignalR benachrichtigt.")]
        Admins,

        /// <summary>
        ///     Es werden alle Nutzer in der Rolle <see cref="UserRole.USER" /> per SignalR benachrichtigt.
        /// </summary>
        [Description("Es werden alle Nutzer in der Rolle Nutzer per SignalR benachrichtigt.")]
        Users,

        /// <summary>
        ///     Es werden alle Nutzer benachrichtigt
        /// </summary>
        [Description("Es werden alle Nutzer benachrichtigt.")]
        AllUsers,

        /// <summary>
        ///     Es werden alle Nutzer eines Boards benachrichtigt.
        /// </summary>
        [Description("Es werden alle Nutzer eines Boards benachrichtigt.")]
        BoardUsers,

        /// <summary>
        ///     Es werden alle Nutzer benachrichtigt, die auf ein Board lauschen (es geöffnet haben).
        /// </summary>
        [Description("Es werden alle Nutzer benachrichtigt, die auf ein Board lauschen (es geöffnet haben).")]
        Board,

        /// <summary>
        /// Informiert nur die Clients des Nutzers, der den Request ausführt.
        /// </summary>
        [Description("Es wird nur der Nutzer benachrichtigt, der den Request ausführt.")]
        CurrentUser,

        /// <summary>
        /// Die zu ermittelnden Clients, werden manuell aus dem Request ermittelt.
        /// </summary>
        [Description("Die zu ermittelnden Clients, werden manuell anhand eines Parameters aus dem Request ermittelt.")]
        RequestParameter,
    }
}