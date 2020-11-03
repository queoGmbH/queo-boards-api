using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentValidation.Results;
using NSwag.Annotations;
using Queo.Boards.Commands.Cards;
using Queo.Boards.Commands.Lists;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Validators.Cards;
using Queo.Boards.Core.Validators.Lists;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {
    [RoutePrefix("api")]
    public class CopyController : AuthorizationRequiredApiController {
        private readonly CardNameValidator _cardNameValidator;
        private readonly ICardService _cardService;
        private readonly ListNameValidator _listNameValidator;

        private readonly IListService _listService;

        // Lazy<IHubContext> _boardChannelHub = new Lazy<IHubContext>( () => GlobalHost.ConnectionManager.GetHubContext<BoardChannelHub>());

        public CopyController(IListService listService, ICardService cardService, ListNameValidator listNameValidator, CardNameValidator cardNameValidator) {
            _listService = listService;
            _cardService = cardService;
            _listNameValidator = listNameValidator;
            _cardNameValidator = cardNameValidator;
        }

        /// <summary>
        ///     Kopiert eine Karte
        /// </summary>
        /// <param name="list">Business Id der Liste als Guid</param>
        /// <param name="currentUser">Der Nutzer, der die Karte kopiert</param>
        /// <param name="copyCardCommand">Command mit den Parametern zum Kopieren einer Liste.</param>
        /// <returns>Das Board der Liste, in welche die Karte eingefügt wurde.</returns>
        [HttpPost]
        [Route("lists/{list:Guid}/copiedcards")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 75 Zeichen")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CopyCard([ModelBinder] List list, [ModelBinder, SwaggerIgnore]User currentUser, CopyCardCommand copyCardCommand) {
            Require.NotNull(list, nameof(list));
            Require.NotNull(copyCardCommand, nameof(copyCardCommand));

            ValidationResult validationResult = _cardNameValidator.Validate(copyCardCommand.CopyName);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }

            int position = _cardService.GetPositionByTarget(list, copyCardCommand.InsertAfter);
            Card createdCard = _cardService.Copy(copyCardCommand.Source, list, copyCardCommand.CopyName, currentUser, position);

            BoardModel boardModel = BoardModelBuilder.Build(createdCard.List.Board);
            return Ok(boardModel);
        }

        /// <summary>
        ///     Kopiert eine Liste
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="board">Id (Guid) des Boards, zu dem die Liste angelegt werden soll</param>
        /// <param name="copyListCommand">Command mit Parametern zum Kopieren einer Liste.</param>
        /// <returns>Das Board in welches die Liste kopiert wurde.</returns>
        [HttpPost]
        [Route("boards/{board:Guid}/copiedlists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(BoardModel))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string), Description = "Nutzer hat keine Schreibrechte")]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 120 Zeichen")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CopyList([SwaggerIgnore] [ModelBinder] User currentUser, [ModelBinder] Board board, [FromBody] CopyListCommand copyListCommand) {
            if (!currentUser.CanWrite) {
                return Unauthorized();
            }
            ValidationResult validationResult = _listNameValidator.Validate(copyListCommand.CopyName);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }





            int position = _listService.GetPositionByTarget(board, copyListCommand.InsertAfter);
            List list = _listService.Copy(copyListCommand.Source, board, copyListCommand.CopyName, currentUser, position);

            return Ok(BoardModelBuilder.Build(list.Board));
        }
    }
}