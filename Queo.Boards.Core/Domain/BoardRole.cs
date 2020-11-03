namespace Queo.Boards.Core.Domain {
    /// <summary>
    /// Die Rolle eines Nutzer an einem Board.
    /// </summary>
    public static class BoardRole {
        ///// <summary>
        /////     Ruft den Namen der Rolle eines Nutzers am Board ab, der Besitzer des Boards ist.
        ///// </summary>
        //public const string BOARD_ROLE_MEMBER = "BOARD_MEMBER";

        /// <summary>
        ///     Ruft den Namen der Rolle eines Nutzers am Board ab, der Besitzer des Boards ist.
        /// </summary>
        public const string BOARD_ROLE_OWNER = "BOARD_OWNER";

        /// <summary>
        ///     Ruft den Namen der Rolle eines Nutzers am Board ab, der Nutzer (also Besitzer, explizites Mitglied oder Mitglied
        ///     eines zugewiesenen Teams) des Boards ist.
        /// </summary>
        public const string BOARD_ROLE_USER = "BOARD_MEMBER";
    }
}