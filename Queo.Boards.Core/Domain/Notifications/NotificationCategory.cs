namespace Queo.Boards.Core.Domain.Notifications {
    /// <summary>
    /// Beschreibt, welche Entität die Benachrichtigung betrifft.
    /// </summary>
    public enum NotificationCategory {
        /// <summary>
        /// Die Benachrichtigung betrifft ein Board.
        /// </summary>
        Board,

        /// <summary>
        /// Die Benachrichtigung betrifft eine Karte.
        /// </summary>
        Card,

        /// <summary>
        /// Die Benachrichtigung betrifft einen Kommentar.
        /// </summary>
        Comment
    }
}