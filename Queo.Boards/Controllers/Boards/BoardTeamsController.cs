using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers.Boards {
    /// <summary>
    /// Controller zur Verwaltung der Teams eines Boards.
    /// </summary>
    [RoutePrefix("api/boards/{board:Guid}/teams")]
    public class BoardTeamsController : AuthorizationRequiredApiController {

        private readonly IBoardService _boardService;

        public BoardTeamsController(IBoardService boardService) {
            _boardService = boardService;
        }

        /// <summary>
        /// Weißt ein Team dem Board zu.
        /// </summary>
        /// <param name="board">Das Board, welchem das Team zugeordnet werden soll.</param>
        /// <param name="team">Das Team, das dem Board zugeordnet werden soll.</param>
        /// <param name="currentUser">Der Nutzer, der das Team dem Board hinzugefügt.</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [Route("")]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult AddTeam([ModelBinder] Board board, EntityFromBody<Team> team, [SwaggerIgnore, ModelBinder]User currentUser) {
            Require.NotNull(board, "board");
            Require.NotNull(team.Entity, "team");

            _boardService.AddTeam(board, team.Entity, currentUser);

            return Ok(BoardModelBuilder.Build(board));
        }


        /// <summary>
        ///     Entfernt einen Team von einem Board.
        ///     Ist das Team dem Board nicht zugeordnet, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, von welchem der Nutzer entfernt werden soll.</param>
        /// <param name="team">Das Team, dessen Zuordnung zum Board gelöscht werden soll.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{team:guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult RemoveBoardMember([ModelBinder] Board board, [ModelBinder] Team team) {
            Require.NotNull(board, "board");
            Require.NotNull(team, "team");

            _boardService.RemoveTeam(board, team);

            return Ok(BoardModelBuilder.Build(board));
        }
    }
}