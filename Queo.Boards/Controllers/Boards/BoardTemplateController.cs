using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.BreadCrumbModels;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers.Boards {

    [RoutePrefix("api/board-templates")]
    public class BoardTemplateController : AuthorizationRequiredApiController{

        private readonly IBoardService _boardService;

        public BoardTemplateController(IBoardService boardService) {
            _boardService = boardService;
        }

        /// <summary>
        /// Liefert alle Board-Vorlagen, die für den Nutzer zugänglich sind.
        /// </summary>
        /// <returns>Die Board-Vorlagen</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel[]))]
        public IHttpActionResult GetBoardTemplates([SwaggerIgnore, ModelBinder]User currentUser) {
            IList<Board> boards = _boardService.FindBoardTemplatesForUser(PageRequest.All, currentUser).ToList();
            IList<BoardSummaryModel> summary = boards.Select(BoardSummaryModelBuilder.Build).ToList();

            return Ok(summary);
        }

        /// <summary>
        /// Löscht eine Board-Vorlage endgültig. Sie kann anschließend NICHT wiederhergestellt werden.
        /// </summary>
        /// <returns>Die gelöschte Board-Vorlagen</returns>
        [HttpDelete]
        [Route("{boardTemplate:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardBreadCrumbModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult DeleteBoardTemplate([ModelBinder]Board boardTemplate, [SwaggerIgnore, ModelBinder]User currentUser) {
            Require.NotNull(boardTemplate, "boardTemplate");

            _boardService.Delete(boardTemplate);

            return Ok(BreadCrumbsModelBuilder.Build(boardTemplate));
        }

        /// <summary>
        ///     Erstellt eine Vorlage aus einem Board.
        /// </summary>
        /// <param name="board">Das Board, aus welchem die Vorlage erstellt werden soll.</param>
        /// <returns>Die erstellte Vorlage.</returns>
        [HttpPost]
        [Route("{board:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult CreateTemplateFromBoard([ModelBinder] Board board, [SwaggerIgnore, ModelBinder]User currentUser) {
            Require.NotNull(board, "board");
            Require.NotNull(currentUser, "currentUser");

            Board template = _boardService.CreateTemplateFromBoard(board, currentUser);
            return Ok(BoardSummaryModelBuilder.Build(template));
        }
    }
}