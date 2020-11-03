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


    [RoutePrefix("api/archived-boards")]
    public class ArchivedBoardController : AuthorizationRequiredApiController {

        private readonly IBoardService _boardService;

        public ArchivedBoardController(IBoardService boardService) {
            _boardService = boardService;
        }

        /// <summary>
        /// Liefert alle archivierten Boards sortiert nach Archivierungsdatum. Das zuletzt archivierte Board wird dabei zuerst geliefert.
        /// </summary>
        /// <returns>Die archivierten Boards</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel[]))]
        public IHttpActionResult GetArchivedBoards([SwaggerIgnore, ModelBinder]User currentUser) {
            IList<Board> boards = _boardService.FindArchivedBoardsForUser(PageRequest.All, currentUser).ToList();
            IList<BoardSummaryModel> summary = boards.Select(BoardSummaryModelBuilder.Build).ToList();

            return Ok(summary);
        }

        /// <summary>
        ///     Archiviert ein bisher nicht archiviertes Board.
        /// </summary>
        /// <param name="board">Das zu archivierende Board</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{board:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        public IHttpActionResult ArchiveBoard([ModelBinder] Board board) {
            Require.NotNull(board, "board");

            Board updatedBoard = _boardService.UpdateIsArchived(board, true);
            return Ok(BoardSummaryModelBuilder.Build(updatedBoard));
        }

        /// <summary>
        /// Löscht eine archiviertes Board endgültig. Es kann anschließend NICHT wiederhergestellt werden.
        /// </summary>
        /// <returns>Das gelöschte archivierte Board</returns>
        [HttpDelete]
        [Route("{archivedBoard:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardBreadCrumbModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.Admins)]
        public IHttpActionResult DeleteArchivedBoard([ModelBinder]Board archivedBoard, [SwaggerIgnore, ModelBinder]User currentUser) {
            Require.NotNull(archivedBoard, "archivedBoard");

            _boardService.Delete(archivedBoard);

            return Ok(BreadCrumbsModelBuilder.Build(archivedBoard));
        }
    }
}