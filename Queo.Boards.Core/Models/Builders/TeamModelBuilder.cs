using System.Collections.Generic;
using System.Linq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;

namespace Queo.Boards.Core.Models.Builders {
    /// <summary>
    ///     Builder zur Erstellung von TeamModels.
    /// </summary>
    public static class TeamModelBuilder {
        /// <summary>
        ///     Erstellt ein (vollständiges) Model für das Team, inklusive Mitgliedern.
        /// </summary>
        /// <param name="team"></param>
        /// <param name="boardsWithTeam"></param>
        /// <returns></returns>
        public static TeamModel Build(Team team, IList<Board> boardsWithTeam) {
            Require.NotNull(team, "team");

            return new TeamModel(team.BusinessId, team.Name, team.Description, team.Members.Select(UserModelBuilder.BuildUser).ToList(), boardsWithTeam.Select(BoardSummaryModelBuilder.Build).ToList());
        }

        /// <summary>
        ///     Erstellt ein (reduziertes) Model für das Team.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public static TeamSummaryModel BuildSummary(Team team) {
            Require.NotNull(team, "team");

            return new TeamSummaryModel(team.BusinessId, team.Name, team.Description);
        }
    }
}