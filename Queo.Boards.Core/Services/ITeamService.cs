using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle, die einen Service beschreibt, der Methoden für die Verwaltung von Teams bereitstellt.
    /// </summary>
    public interface ITeamService {
        /// <summary>
        ///     Fügt dem Team neue Nutzer als Mitglieder hinzu.
        ///     Nutzer, die bereits Mitglieder des Teams sind, werden nicht erneut hinzugefügt.
        /// </summary>
        /// <param name="team">Das Team, welchem die Nutzer als Mitglieder hinzugefügt werden sollen.</param>
        /// <param name="members">Die Nutzer, die dem Team als neue Mitglieder hinzugefügt werden.</param>
        /// <returns>Alle Mitglieder des Teams.</returns>
        IList<User> AddMembers(Team team, params User[] members);

        /// <summary>
        ///     Erstellt ein neues Team.
        /// </summary>
        /// <param name="teamDto">Name und Beschreibung des Teams</param>
        /// <param name="members">Die initialen Mitglieder des Teams</param>
        /// <param name="createDto">Erstellungsinformationen</param>
        /// <returns></returns>
        Team Create(TeamDto teamDto, IList<User> members, EntityCreatedDto createDto);

        /// <summary>
        ///     Ruft seitenweise Teams ab.
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        IPage<Team> GetAll(IPageable pageRequest);

        /// <summary>
        ///     Entfernt Mitglieder eines Teams.
        /// </summary>
        /// <param name="team">Das Team, dessen Mitglieder entfernt werden sollen.</param>
        /// <param name="members">Die aus dem Team zu entfernenden Mitglieder.</param>
        /// <returns>Die verbleibenden Mitglieder</returns>
        IList<User> RemoveMembers(Team team, params User[] members);

        /// <summary>
        ///     Ändert ein Team.
        /// </summary>
        /// <param name="team">Das zu ändernde Team.</param>
        /// <param name="teamDto">Der neue Name und die neue Beschreibung des Teams.</param>
        void Update(Team team, TeamDto teamDto);

        /// <summary>
        /// Löscht ein Team.
        /// </summary>
        /// <param name="team">Das zu löschende Team.</param>
        void Delete(Team team);
    }
}