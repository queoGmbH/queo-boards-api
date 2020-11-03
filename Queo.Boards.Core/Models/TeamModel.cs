using System;
using System.Collections.Generic;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model für ein Team.
    /// </summary>
    public class TeamModel : TeamSummaryModel {
        /// <summary>
        ///     Erstellt ein neues TeamModel.
        /// </summary>
        /// <param name="businessId">Id des Teams</param>
        /// <param name="name">Der Name des Teams</param>
        /// <param name="description">Beschreibung des Teams</param>
        /// <param name="members">Die Mitglieder des Teams</param>
        /// <param name="boards">Die Boards, denen das Team zugeordnet ist.</param>
        public TeamModel(Guid businessId, string name, string description, IList<UserModel> members, IList<BoardSummaryModel> boards)
            : base(businessId, name, description) {
            Members = members;
            Boards = boards;
        }

        /// <summary>
        ///     Ruft die Boards ab, denen das Team zugeordnet ist.
        /// </summary>
        public IList<BoardSummaryModel> Boards { get; }

        /// <summary>
        ///     Ruft die Mitglieder des Teams ab.
        /// </summary>
        public IList<UserModel> Members { get; }
    }
}