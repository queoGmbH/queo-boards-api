using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentValidation.Results;
using NSwag.Annotations;
using Queo.Boards.Commands.Teams;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Validators.Teams;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller zum Verarbeiten von team-spezifischen Anfragen.
    /// </summary>
    [RoutePrefix("api/teams")]
    public class TeamController : AuthorizationRequiredApiController {
        private readonly IBoardService _boardService;
        private readonly TeamNameValidator _teamNameValidator;
        private readonly ITeamService _teamService;

        /// <summary>
        /// Erzeugt einen neuen TeamController.
        /// </summary>
        /// <param name="teamService"></param>
        /// <param name="boardService"></param>
        /// <param name="teamNameValidator"></param>
        public TeamController(ITeamService teamService, IBoardService boardService, TeamNameValidator teamNameValidator) {
            _teamService = teamService;
            _boardService = boardService;
            _teamNameValidator = teamNameValidator;
        }

        /// <summary>
        ///     Fügt einem Team neue Mitglieder hinzu.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<UserModel>), Description = "Die Mitglieder des Teams")]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        [Route("{team:Guid}/members")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        public IHttpActionResult AddMembers([ModelBinder] Team team, AddMemberCommand addMemberCommand) {
            Require.NotNull(team, "team");

            IList<User> teamMembers = _teamService.AddMembers(team, addMemberCommand.Users.ToArray());
            return Ok(UserModelBuilder.BuildUsers(teamMembers));
        }

        /// <summary>
        ///     Erstellt ein neues Team.
        /// </summary>
        /// <param name="createTeamCommand"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, typeof(TeamModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 75 Zeichen")]
        [HttpPost]
        [Route("")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        public IHttpActionResult Create(CreateTeamCommand createTeamCommand, [SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(createTeamCommand, "createTeamCommand");
            Require.NotNull(currentUser, "currentUser");

            ValidationResult validationResult = _teamNameValidator.Validate(createTeamCommand.Name);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }

            TeamDto teamDto = new TeamDto(createTeamCommand.Name, createTeamCommand.Description);
            Team team = _teamService.Create(teamDto, new List<User>(), new EntityCreatedDto(currentUser, DateTime.UtcNow));

            return Ok(TeamModelBuilder.Build(team, new List<Board>()));
        }

        /// <summary>
        ///     Löscht ein Team.
        /// </summary>
        /// <returns>Das gelöschte Team. !!! Achtung: Das Team existiert in der DB nicht mehr !!!</returns>
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TeamSummaryModel), Description = "Das gelöschte Team. !!! Achtung: Das Team existiert in der DB nicht mehr !!!")]
        [Route("{team:Guid}")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        public IHttpActionResult Delete([ModelBinder] Team team) {
            Require.NotNull(team, "team");

            _teamService.Delete(team);
            return Ok(TeamModelBuilder.BuildSummary(team));
        }

        /// <summary>
        ///     Ruft ein Team inklusive seiner Mitglieder ab.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(TeamModel), Description = "Das Team inklusive seiner Mitglieder")]
        [Route("{team:Guid}")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        public IHttpActionResult Get([ModelBinder] Team team) {
            Require.NotNull(team, "team");

            IPage<Board> boardsWithTeam = _boardService.FindBoardsWithTeam(PageRequest.All, team);

            return Ok(TeamModelBuilder.Build(team, boardsWithTeam.ToList()));
        }

        /// <summary>
        ///     Ruft eine Liste aller Teams ab.
        ///     Die gelieferten Teams enthalten keine Liste der Mitglieder.
        ///     Diese muss separat abgerufen werden.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<TeamSummaryModel>), Description = "Liste mit allen vorhandenen Teams")]
        [Route("")]
        public IHttpActionResult GetAll() {
            return Ok(_teamService.GetAll(PageRequest.All).Select(TeamModelBuilder.BuildSummary));
        }

        /// <summary>
        ///     Ruft die Mitglieder eines Teams ab.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<UserModel>), Description = "Die Mitglieder des Teams")]
        [Route("{team:Guid}/members")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        public IHttpActionResult GetMembers([ModelBinder] Team team) {
            Require.NotNull(team, "team");
            return Ok(UserModelBuilder.BuildUsers(team.Members));
        }

        /// <summary>
        ///     Entfernt ein Mitglied eines Teams.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<UserModel>), Description = "Die verbleibenden Mitglieder des Teams")]
        [Route("{team:Guid}/members/{member:Guid}")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        public IHttpActionResult RemoveMember([ModelBinder] Team team, [ModelBinder] User member) {
            Require.NotNull(team, "team");

            IList<User> remainingMembers = _teamService.RemoveMembers(team, member);
            return Ok(UserModelBuilder.BuildUsers(remainingMembers));
        }

        /// <summary>
        ///     Ändert ein Team.
        /// </summary>
        /// <param name="team">Das zu ändernde Team</param>
        /// <param name="updateTeamCommand">Parameter zum Ändern des Teams</param>
        /// <param name="currentUser">Der Nutzer, der das Team ändert</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, typeof(TeamModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 75 Zeichen")]
        [HttpPut]
        [Route("{team:Guid}")]
        [Queo.Boards.Infrastructure.Http.Authorize(Roles = UserRole.ADMINISTRATOR)]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        public IHttpActionResult Update([ModelBinder] Team team, [FromBody] UpdateTeamCommand updateTeamCommand, [SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(updateTeamCommand, "updateTeamCommand");
            Require.NotNull(currentUser, "currentUser");

            ValidationResult validationResult = _teamNameValidator.Validate(updateTeamCommand.Name);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }

            TeamDto teamDto = new TeamDto(updateTeamCommand.Name, updateTeamCommand.Description);
            _teamService.Update(team, teamDto);

            IPage<Board> boardsWithTeam = _boardService.FindBoardsWithTeam(PageRequest.All, team);

            return Ok(TeamModelBuilder.Build(team, boardsWithTeam.ToList()));
        }
    }
}