using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Commands.Boards;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers.Boards {
    /// <summary>
    ///     Controller für alles rund ums <see cref="Board" />
    /// </summary>
    [RoutePrefix("api/boards")]
    public class BoardController : AuthorizationRequiredApiController {
        private readonly IBoardService _boardService;
        private readonly IChecklistService _checklistService;
        private readonly IListService _listService;

        /// <summary>
        ///     Erstellt eine neue Instanz eines <see cref="BoardController" />.
        /// </summary>
        /// <param name="boardService"></param>
        /// <param name="checklistService"></param>
        public BoardController(IBoardService boardService, IChecklistService checklistService, IListService listService) {
            _boardService = boardService;
            _checklistService = checklistService;
            _listService = listService;
        }

        /// <summary>
        ///     Erstellt ein neues Board
        /// </summary>
        /// <param name="boardCreateCommand">Command zur Erstellung des Boards</param>
        /// <param name="currentUser">Der Nutzer, der das Board erstellt und Besitzer des Boards wird.</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 60 Zeichen")]
        [Route("")]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult Create(BoardCreateCommand boardCreateCommand, [SwaggerIgnore] [ModelBinder] User currentUser) {
            Require.NotNull(boardCreateCommand, nameof(boardCreateCommand));
            Require.NotNull(currentUser, nameof(currentUser));

            Board board;
            if (boardCreateCommand.Template == null) {
                /*Von Grund auf ein neues Board erstellen*/
                board = _boardService.Create(boardCreateCommand.GetBoardDto(), currentUser);
            } else {
                /*Board aus einer Vorlage erstellen*/
                board = _boardService.Copy(boardCreateCommand.Template, boardCreateCommand.GetBoardDto(), currentUser);
            }

            return Ok(BoardSummaryModelBuilder.Build(board));
        }

        /// <summary>
        ///     Liefert Zusammenfassungen aller Boards, für welcher der aktuelle Nutzer die Berechtigung hat, das Board zu sehen,
        ///     da es entweder öffentlich sichtbar ist oder der Nutzer Eigentümer oder Mitglied des Boards ist.
        ///     Die Boards werden sortiert nach Sichtbarkeit (restricted, public) und Titel (A-Z) geliefert.
        /// </summary>
        /// <param name="currentUser">Der Nutzer, für den die Boards abgerufen werden sollen.</param>
        /// <param name="queryString">Optionale Suchzeichenfolge zur Einschränkung der gelieferten Boards.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel[]))]
        public IHttpActionResult FindBoardsForUser([SwaggerIgnore] [ModelBinder] User currentUser, [FromUri] string queryString = null) {
            Require.NotNull(currentUser, "currentUser");

            IList<Board> boards = _boardService.FindBoardsForUser(PageRequest.All, currentUser, queryString).ToList();
            IList<BoardSummaryModel> summary = boards.Select(BoardSummaryModelBuilder.Build).ToList();
            return Ok(summary);
        }

        /// <summary>
        ///     Liefert alle Checklisten eines Boards
        /// </summary>
        /// <param name="board">Id des Boards als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{board:Guid}/checklists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<CardWithChecklistsMetadataModel>))]
        public IHttpActionResult FindChecklistsOnBoard([ModelBinder] Board board) {
            IList<Checklist> checklists = _checklistService.FindChecklistsOnBoard(board);
            return Ok(CardWithChecklistsMetadataModelBuilder.Build(checklists));
        }

        /// <summary>
        ///     Liefert alle Checklisten eines Boards
        /// </summary>
        /// <param name="board">Id des Boards als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{board:Guid}/comments")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<CommentModel>))]
        public IHttpActionResult GetBoardComments([ModelBinder] Board board) {
            IList<Comment> comments = _boardService.FindAggregatedBoardComments(board);
            return Ok(comments.Select(CommentModelBuilder.Build));
        }

        /// <summary>
        ///     Liefert ein komplettes Board in Form eines <see cref="BoardModel" />.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{board:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        public IHttpActionResult GetCompleteBoard([ModelBinder] Board board) {
            IList<List> boardLists = _listService.FindAllListsAndCardsByBoardId(board.BusinessId);
            return Ok(BoardModelBuilder.Build(board, boardLists));
        }

        /// <summary>
        ///     Stellt ein Board aus dem Archiv wieder her.
        /// </summary>
        /// <param name="archivedBoard">Das wiederherzustellende archivierte Board</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{archivedBoard:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel))]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.BoardUsers)]
        public IHttpActionResult RestoreBoard([ModelBinder] Board archivedBoard) {
            Require.NotNull(archivedBoard, "archivedBoard");

            Board restoredBoard = _boardService.UpdateIsArchived(archivedBoard, false);
            return Ok(BoardSummaryModelBuilder.Build(restoredBoard));
        }

        /// <summary>
        ///     Aktualisiert ein bestehendes Board
        /// </summary>
        /// <param name="board">Business ID <see cref="Guid" /> des Boards</param>
        /// <param name="boardDto">Update Dto mit den neuen Board-Daten</param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardSummaryModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 60 Zeichen")]
        [Route("{board:Guid}")]
        [SwaggerSignalR(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        [SignalrNotification(typeof(UserNotificationHub), SignalrNotificationScope.AllUsers)]
        public IHttpActionResult Update([ModelBinder] Board board, BoardDto boardDto) {
            Require.NotNull(board);
            Require.NotNull(boardDto);

            return Ok(BoardSummaryModelBuilder.Build(_boardService.Update(board, boardDto)));
        }

        
    }
}