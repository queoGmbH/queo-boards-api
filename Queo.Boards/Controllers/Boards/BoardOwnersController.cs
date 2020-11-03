using System.Collections.Generic;
using System.Linq;
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
    ///     Controller zur Verwaltung von Besitzern eines Boards.
    /// </summary>
    [RoutePrefix("api/boards/{board:Guid}/owners")]
    public class BoardOwnersController : AuthorizationRequiredApiController {
        private readonly IBoardService _boardService;

        public BoardOwnersController(IBoardService boardService) {
            _boardService = boardService;
        }

        /// <summary>
        ///     Fügt dem Board einen Besitzer hinzu.
        ///     Ist der Nutzer bereits Besitzer des Boards, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, dessen Besitzer der Nutzer werden soll.</param>
        /// <param name="owner">Der Nutzer bzw. dessen Id, der Besitzer des Boards werden soll.</param>
        /// <param name="currentUser">Der Nutzer, der den Besitzer hinzufügt.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult AddBoardOwner([ModelBinder] Board board, EntityFromBody<User> owner, [SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(board, "board");
            Require.NotNull(owner.Entity, "owner");

            IList<User> boardMembers = _boardService.AddOwner(board, owner.Entity);

            return Ok(BoardModelBuilder.Build(board));
        }

        /// <summary>
        ///     Entfernt einen Besitzer von einem Board.
        ///     Ist der Nutzer kein Besitzer des Board, werden keine Änderungen vorgenommen.
        /// </summary>
        /// <param name="board">Das Board, von welchem der Besitzer entfernt werden soll.</param>
        /// <param name="owner">Der Nutzer, der kein Besitzer des Boards mehr sein soll.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{owner:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(string), Description = "Der letzte Besitzer eines Boards kann nicht gelöscht werden.")]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult RemoveBoardOwner([ModelBinder] Board board, [ModelBinder] User owner) {
            Require.NotNull(board, "board");
            Require.NotNull(owner, "owner");

            if (board.Owners.All(o => o.Equals(owner))) {
                return Conflict("Der letzte Besitzer eines Boards darf nicht gelöscht werden, da es mindestens einen Besitzer geben muss.");
            }

            _boardService.RemoveOwner(board, owner);

            return Ok(BoardModelBuilder.Build(board));
        }
    }
}