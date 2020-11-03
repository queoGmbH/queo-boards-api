using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentValidation.Attributes;
using FluentValidation.Results;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
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
    /// <summary>
    ///     Controller für Listen
    /// </summary>
    [RoutePrefix("api")]
    public class ListController : AuthorizationRequiredApiController {
        /// <summary>
        /// </summary>
        /// <param name="listService"></param>
        /// <param name="cardService"></param>
        /// <param name="userService"></param>
        /// <param name="listCreateAndUpdateValidator"></param>
        /// <param name="cardDtoValidator"></param>
        public ListController(IListService listService, ICardService cardService, IUserService userService,
            ListCreateAndUpdateValidator listCreateAndUpdateValidator, CardDtoValidator cardDtoValidator) {
            ListService = listService;
            CardService = cardService;
            UserService = userService;
            CreateAndUpdateValidator = listCreateAndUpdateValidator;
            DtoValidator = cardDtoValidator;
        }

        private ICardService CardService { get; }

        private ListCreateAndUpdateValidator CreateAndUpdateValidator { get; }

        private CardDtoValidator DtoValidator { get; }

        private IListService ListService { get; }

        private IUserService UserService { get; set; }

        /// <summary>
        ///     Erzeugt eine neue Liste am Board
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="board">Id (Guid) des Boards, zu dem die Liste angelegt werden soll</param>
        /// <param name="listUpdateDto">Der Name der Liste</param>
        /// <returns></returns>
        [HttpPost]
        [Route("boards/{board:Guid}/lists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ListModel))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string), Description = "Nutzer hat keine Schreibrechte")]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 120 Zeichen")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CreateList([SwaggerIgnore] [ModelBinder] User currentUser, [ModelBinder] Board board, [FromBody] [Validator(typeof(ListCreateAndUpdateValidator))] StringValueDto listUpdateDto) {
            if (!currentUser.CanWrite) {
                return Unauthorized();
            }

            List list = ListService.Create(board, listUpdateDto.Value);
            return Ok(ListModelBuilder.Build(list));
        }

        /// <summary>
        ///     Erstellt eine neue Karte in einer Liste
        /// </summary>
        /// <param name="list">Business Id der Liste als Guid</param>
        /// <param name="currentUser">Der Nutzer, der die Liste kopiert.</param>
        /// <param name="cardCreateCommand">Command mit Parametern zu Erstellung einer Karte.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("lists/{list:Guid}/cards")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 75 Zeichen")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(string), Description = "Einer der zugewiesenen Nutzer ist kein Board-Mitglied.")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CreateCard([ModelBinder] List list, [ModelBinder] [SwaggerIgnore] User currentUser, CardCreateCommand cardCreateCommand) {
            Require.NotNull(list, nameof(list));
            Require.NotNull(cardCreateCommand, nameof(cardCreateCommand));

            /*Standardfall: Erstellen einer neuen Liste*/
            ValidationResult validationResult = DtoValidator.Validate(cardCreateCommand.GetCardDto());
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }

            IList<User> initiallyAssignedUsers = new List<User>();
            if (cardCreateCommand.AssignedUsers != null && cardCreateCommand.AssignedUsers.Any()) {
                initiallyAssignedUsers = cardCreateCommand.AssignedUsers;
            }

            if (!initiallyAssignedUsers.All(list.Board.Members.Contains)) {
                return Conflict("Nicht alle initial zugewiesenen Nutzer sind auch Mitglieder des Boards.");
            }

            Card createdCard = CardService.Create(list, cardCreateCommand.GetCardDto(), initiallyAssignedUsers, currentUser);

            return Ok(CardModelBuilder.Build(createdCard));
        }

        /// <summary>
        ///     Liefert alle archivierten Listen eines Boards
        /// </summary>
        /// <param name="board">Id des Boards als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("boards/{board:Guid}/archived-lists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<ListModel>))]
        public IHttpActionResult GetArchivedListsByBoard([ModelBinder] Board board) {
            List<List> archivedLists = board.Lists.Where(l => l != null && l.IsArchived).ToList();
            return Ok(archivedLists.Select(ListModelBuilder.Build));
        }

        /// <summary>
        ///     Aktualisiert ob eine Liste archiviert ist
        /// </summary>
        /// <param name="list">Id der Liste als Guid</param>
        /// <param name="boolValueDto">Dto mit einem Wert ob die LIste archiviert ist.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("lists/{list:Guid}/isArchived")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ListWithCardsModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateIsArchived([ModelBinder] List list, BoolValueDto boolValueDto) {
            List updatedList = ListService.UpdateArchived(list, boolValueDto.Value);
            return Ok(ListWithCardsModelBuilder.Build(updatedList));
        }

        /// <summary>
        ///     Aktualisiert den Titel einer Liste
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="list">Id der Liste als Guid</param>
        /// <param name="listUpdateDto">Neuer Titel</param>
        /// <returns></returns>
        [HttpPut]
        [Route("lists/{list:Guid}/title")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string), Description = "Nutzer hat keine Schreibrechte")]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 120 Zeichen")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ListModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateListTitle([SwaggerIgnore] [ModelBinder] User currentUser, [ModelBinder] List list,
            [FromBody] StringValueDto listUpdateDto) {
            if (!currentUser.CanWrite) {
                return Unauthorized();
            }
            ValidationResult validationResult = CreateAndUpdateValidator.Validate(listUpdateDto);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }
            List changedList = ListService.Update(list, listUpdateDto.Value);

            ListModel listModel = ListModelBuilder.Build(changedList);
            return Ok(listModel);
        }
    }
}