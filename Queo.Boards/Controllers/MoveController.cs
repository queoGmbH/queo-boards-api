using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Commands.Cards;
using Queo.Boards.Commands.Lists;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller für Verschiebe-Bewegungen
    /// </summary>
    [RoutePrefix("api")]
    public class MoveController : AuthorizationRequiredApiController {
        private readonly IBoardService _boardService;
        private readonly ICardService _cardService;
        private readonly IListService _listService;

        /// <summary>
        /// </summary>
        /// <param name="listService"></param>
        /// <param name="boardService"></param>
        /// <param name="cardService"></param>
        public MoveController(IListService listService, IBoardService boardService, ICardService cardService) {
            _listService = listService;
            _boardService = boardService;
            _cardService = cardService;
        }

        /// <summary>
        ///     Verschiebt eine Karte in eine andere Liste inkl. einer Zielkarte
        /// </summary>
        /// <param name="list">Die Liste, auf welcher die Karte eingefügt werden soll.</param>
        /// <param name="moveCardCommand">Das Command mit den Parametern zum Verschieben der Karte.</param>
        /// <returns>Das Quell- und das Ziel-Board der Verschiebung.</returns>
        [HttpPost]
        [Route("lists/{list:Guid}/movedcards")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SourceAndTargetModel<BoardModel, CardModel>), Description = "Das Board, auf welches die Karte verschoben wurde.")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SwaggerSignalR(typeof(BoardChannelHub), "Quell-Board", "Es werden alle Nutzer benachrichtigt, die auf den Kanal des Quell-Boards lauschen.")]
        [SignalrNotification(typeof(BoardChannelHub), "moveCardCommand.Source.List.Board.BusinessId")]
        public IHttpActionResult MoveCard([ModelBinder] List list, MoveCardCommand moveCardCommand) {
            Require.NotNull(list, "list");
            Require.NotNull(moveCardCommand, "moveCardCommand");

            Card cardToMove = moveCardCommand.Source;
            Board sourcBoard = cardToMove.List.Board;
            int position = 0;
            if (moveCardCommand.InsertAfter != null) {
                position = list.Cards.Except(new[] { cardToMove }).ToList().IndexOf(moveCardCommand.InsertAfter) + 1;
            }
            _cardService.MoveCard(cardToMove, list, position);
            return Ok(new SourceAndTargetModel<BoardModel, CardModel>(BoardModelBuilder.Build(sourcBoard), BoardModelBuilder.Build(list.Board), CardModelBuilder.Build(cardToMove)));
        }

        /// <summary>
        ///     Verschiebt eine Liste an eine neue Position auf einem Board. Verwenden, wenn es eine Zielliste gibt, hinter der die
        ///     zu verschiebende Liste eingefügt werden soll.
        /// </summary>
        /// <param name="board">Id des Zielboards als Guid</param>
        /// <param name="moveListCommand">Parameter zum Verschieben des Boards.</param>
        /// <returns>Das Quell- und das Ziel-Board der Verschiebung.</returns>
        [HttpPost]
        [Route("boards/{board:Guid}/movedlists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SourceAndTargetModel<BoardModel, ListModel>), Description = "Das Board, auf welches die Liste verschoben wurde.")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SwaggerSignalR(typeof(BoardChannelHub), "Quell-Board", "Es werden alle Nutzer benachrichtigt, die auf den Kanal des Quell-Boards lauschen.")]
        [SignalrNotification(typeof(BoardChannelHub), "moveListCommand.Source.Board.BusinessId")]
        public IHttpActionResult MoveList([ModelBinder] Board board, [FromBody] MoveListCommand moveListCommand) {
            Require.NotNull(board, "board");
            Require.NotNull(moveListCommand, "moveListCommand");

            List listToMove = moveListCommand.Source;
            Board sourceBoard = listToMove.Board;
            int position = 0;
            if (moveListCommand.InsertAfter != null) {
                position = board.Lists.Except(new[] { listToMove }).ToList().IndexOf(moveListCommand.InsertAfter) + 1;
            }

            _listService.MoveList(listToMove, board, position);
            return Ok(new SourceAndTargetModel<BoardModel, ListModel>(BoardModelBuilder.Build(sourceBoard), BoardModelBuilder.Build(board), ListModelBuilder.Build(listToMove)));
        }
    }
}