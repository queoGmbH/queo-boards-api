namespace Queo.Boards.Core.Domain {

    /// <summary>
    /// Auflistung der möglichen Sichtbarkeiten/Zugänglichkeiten eines Boards.
    /// </summary>
    public enum Accessibility {
        /// <summary>
        /// Das Board ist privat und kann nur von Mitgliedern gesehen werden.
        /// </summary>
        Restricted = 0,

        /// <summary>
        /// Das Board ist öffentlich und kann von jedem gesehen werden.
        /// </summary>
        Public = 1
    }
}