using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Validators.Teams;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service, der Methoden für die Verwaltung von Teams bereitstellt.
    /// </summary>
    public class TeamService : ITeamService {
        private readonly ITeamDao _teamDao;
        private readonly IBoardService _boardService;
        private readonly ICardService _cardService;
        private readonly TeamNameValidator _teamNameValidator;

        public TeamService(ITeamDao teamDao, IBoardService boardService, ICardService cardService, TeamNameValidator teamNameValidator) {
            _teamDao = teamDao;
            _boardService = boardService;
            _cardService = cardService;
            _teamNameValidator = teamNameValidator;
        }

        /// <summary>
        ///     Erstellt ein neues Team.
        /// </summary>
        /// <param name="teamDto">Name und Beschreibung des Teams</param>
        /// <param name="members">Die initialen Mitglieder des Teams</param>
        /// <param name="createDto">Erstellungsinformationen</param>
        /// <returns></returns>
        [Transaction]
        public Team Create(TeamDto teamDto, IList<User> members, EntityCreatedDto createDto) {
            Require.NotNull(createDto, "createDto");
            Require.NotNull(members, "members");

            ValidationResult validationResult = _teamNameValidator.Validate(teamDto.Name);
            if (!validationResult.IsValid) {
                throw new ArgumentOutOfRangeException(nameof(teamDto), "Der Name des Teams ist ungültig");
            }

            Team team = new Team(teamDto, members, createDto);
            _teamDao.Save(team);

            return team;
        }

        /// <summary>
        ///     Ruft seitenweise Teams ab.
        /// </summary>
        /// <param name="pageRequest"></param>
        /// <returns></returns>
        public IPage<Team> GetAll(IPageable pageRequest) {
            return _teamDao.GetAll(pageRequest);
        }

        /// <summary>
        ///     Ändert ein Team.
        /// </summary>
        /// <param name="team">Das zu ändernde Team.</param>
        /// <param name="teamDto">Der neue Name und die neue Beschreibung des Teams.</param>
        [Transaction]
        public void Update(Team team, TeamDto teamDto) {
            Require.NotNull(team, "team");
            Require.NotNull(teamDto, "teamDto");

            ValidationResult validationResult = _teamNameValidator.Validate(teamDto.Name);
            if (!validationResult.IsValid) {
                throw new ArgumentOutOfRangeException(nameof(teamDto), "Der Name des Teams ist ungültig");
            }

            team.Update(teamDto);
        }

        /// <summary>
        /// Fügt dem Team neue Nutzer als Mitglieder hinzu.
        /// 
        /// Nutzer, die bereits Mitglieder des Teams sind, werden nicht erneut hinzugefügt.
        /// </summary>
        /// <param name="team">Das Team, welchem die Nutzer als Mitglieder hinzugefügt werden sollen.</param>
        /// <param name="members">Die Nutzer, die dem Team als neue Mitglieder hinzugefügt werden.</param>
        /// <returns>Alle Mitglieder des Teams</returns>
        [Transaction]
        public IList<User> AddMembers(Team team, params User[] members) {
            Require.NotNull(team, "team");
            Require.NotNull(members, "members");

            team.AddMembers(members);

            return team.Members.ToList();
        }

        /// <summary>
        /// Entfernt Mitglieder eines Teams.
        /// </summary>
        /// <param name="team">Das Team, dessen Mitglieder entfernt werden sollen.</param>
        /// <param name="members">Die aus dem Team zu entfernenden Mitglieder.</param>
        /// <returns>Die verbleibenden Mitglieder</returns>
        [Transaction]
        public IList<User> RemoveMembers(Team team, params User[] members) {
            Require.NotNull(team, "team");
            Require.NotNull(members, "members");

            team.RemoveMembers(members);

            /*Alle Boards laden, denen das Team zugeordnet ist*/
            IList<Board> teamBoards = _boardService.FindBoardsWithTeam(PageRequest.All, team).ToList();
            /*Alle Nutzer der Boards laden*/
            Dictionary<Board, IList<User>> teamBoardsWithTheirUsers = teamBoards.ToDictionary(t => t, t => t.GetBoardUsers());

            /*Alle aus dem Team entfernten Nutzer, auch von den Karten der Boards löschen, deren Nutzer sie jetzt nicht mehr sind.*/
            foreach (User removedMember in members) {

                /*Alle Boards suchen, bei denen der Nutzer kein Mitglied mehr ist*/
                foreach (Board board in teamBoardsWithTheirUsers.Where(tb => !tb.Value.Contains(removedMember)).Select(tb => tb.Key)) {

                    /*Alle Karten auf dem Board suchen, an denen der Nutzer angemeldet ist*/
                    foreach (Card card in board.Lists.SelectMany(list => list.Cards).Where(c => c.AssignedUsers.Contains(removedMember))) {
                        _cardService.UnassignUsers(card, removedMember);
                    }
                }
            }
            

            return team.Members.ToList();
        }

        /// <summary>
        /// Löscht ein Team.
        /// </summary>
        /// <param name="team">Das zu löschende Team.</param>
        [Transaction]
        public void Delete(Team team) {

            List<Board> boards = _boardService.FindBoardsWithTeam(PageRequest.All, team).ToList();
            foreach (Board board in boards) {
                _boardService.RemoveTeam(board, team);
            }

            _teamDao.Delete(team);
        }
    }
}