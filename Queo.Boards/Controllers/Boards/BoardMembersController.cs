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
    /// Controller zur Verwaltung von Mitgliedern eines Boards.
    /// </summary>
    [RoutePrefix("api/boards/{board:Guid}/members")]
    public class BoardMembersController : AuthorizationRequiredApiController {
        private readonly IBoardService _boardService;

        public BoardMembersController(IBoardService boardService) {
            _boardService = boardService;
        }

        /// <summary>
        ///     Entfernt einen Nutzer von einem Board.
        ///     Ist der Nutzer dem Board nicht zugeordnet, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, von welchem der Nutzer entfernt werden soll.</param>
        /// <param name="member">Der Nutzer, dessen Mitgliedschaft am Board geändert werden soll.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{member:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult RemoveBoardMember([ModelBinder] Board board, [ModelBinder] User member) {
            Require.NotNull(board, "board");
            Require.NotNull(member, "member");

            _boardService.RemoveMember(board, member);

            return Ok(BoardModelBuilder.Build(board));
        }

        /// <summary>
        ///     Fügt einen Nutzer dem Board hinzu.
        ///     Ist der Nutzer bereits dem Board zugeordnet, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, zu welchem der Nutzer hinzugefügt werden soll.</param>
        /// <param name="user">Der Nutzer bzw. dessen Id, der dem Board hinzugefügt werden soll.</param>
        /// <param name="currentUser">Der Nutzer, der den Nutzer hinzufügt.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        public IHttpActionResult AddBoardMember([ModelBinder] Board board, EntityFromBody<User> user, [SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(board, "board");
            Require.NotNull(user.Entity, "user");

            _boardService.AddMember(board, user.Entity, currentUser);

            return Ok(BoardModelBuilder.Build(board));
        }
    }
}