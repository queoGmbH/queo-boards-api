using System;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Model f�r ein Team.
    ///     Aus Performance-Gr�nden aber ohne Team-Mitglieder.
    /// </summary>
    public class TeamSummaryModel : EntityModel {
        public TeamSummaryModel(Guid businessId, string name, string description)
            : base(businessId) {
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Ruft den Namen des Teams ab.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Ruft die Beschreibung des Teams ab.
        /// </summary>
        public string Description {
            get;
        }
    }
}