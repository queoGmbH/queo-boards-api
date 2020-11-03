using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Common.Logging;
using FluentValidation.Results;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Validators.Cards;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller für Karten
    /// </summary>
    [RoutePrefix("api")]
    public class CardController : AuthorizationRequiredApiController {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(CardController));

        public CardController() {
        }

        /// <summary>
        /// </summary>
        /// <param name="cardService"></param>
        /// <param name="checklistService"></param>
        /// <param name="commentService"></param>
        /// <param name="documentService"></param>
        /// <param name="cardTitleValidator"></param>
        /// <param name="dueValidator"></param>
        public CardController(ICardService cardService,
            IChecklistService checklistService,
            ICommentService commentService,
            IDocumentService documentService,
            IUserService userService,
            CardTitleValidator cardTitleValidator,
            DueValidator dueValidator) {
            CardService = cardService;
            ChecklistService = checklistService;
            CommentService = commentService;
            DocumentService = documentService;
            CardTitleValidator = cardTitleValidator;
            Validator = dueValidator;
            UserService = userService;
        }

        /// <summary>
        ///     Legt den Service zur Verwaltung von Karten fest.
        /// </summary>
        public ICardService CardService { private get; set; }

        /// <summary>
        ///     Legt den Validator zur Überprüfung des Titels einer Karte fest.
        /// </summary>
        public CardTitleValidator CardTitleValidator { private get; set; }

        /// <summary>
        ///     Legt den Service zur Verwaltung von Checklisten fest.
        /// </summary>
        public IChecklistService ChecklistService { private get; set; }

        /// <summary>
        ///     Legt den Service zur Verwaltung von Kommentaren fest.
        /// </summary>
        public ICommentService CommentService { private get; set; }

        /// <summary>
        ///     Legt den Service zur Verwaltung von Dokumenten fest.
        /// </summary>
        public IDocumentService DocumentService { private get; set; }

        /// <summary>
        ///     Setzt den Pfad zum temporären Dokumentenverzeichnis
        /// </summary>
        public string DocumentTempPath { set; private get; }

        /// <summary>
        ///     Legt den Service zur Verwaltung von Nutzern
        /// </summary>
        public IUserService UserService { get; set; }

        /// <summary>
        ///     Legt den Validator zur Überprüfung der Fälligkeit fest.
        /// </summary>
        public DueValidator Validator { private get; set; }

        /// <summary>
        ///     Fügt einer Karte ein Label hinzu
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="label">Id des Labels als Guid</param>
        /// <returns></returns>
        [HttpPut]
        [Route("cards/{card:Guid}/labels/{label:Guid}/assignment")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult AddLabel([ModelBinder] Card card, [ModelBinder] Label label) {
            Require.NotNull(card, nameof(card));
            Require.NotNull(label, nameof(label));
            Card updatedCard = CardService.AddLabel(card, label);
            return Ok(CardModelBuilder.Build(updatedCard));
        }

        /// <summary>
        ///     Fügt einer Karte einen Nutzer hinzu
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="userId">Id des Nutzers der zugewiesen werden soll als Guid</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cards/{card:Guid}/assignedUsers")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel), Description = "Die Karte.")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(string), Description = "Der zugewiesene Nutzer ist kein Board-Nutzer.")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult AssignUser([ModelBinder] Card card, GuidValueDto userId) {
            Require.NotNull(card, nameof(card));
            Require.NotNull(userId, nameof(userId));

            User user = UserService.GetById(userId.Value);
            
            if (!card.List.Board.GetBoardUsers().Contains(user)) {
                return Conflict("Der Nutzer ist kein Nutzer des Boards, auf dem sich die Karte befindet.");
            }

            CardService.AssignUser(card, user);
            return Ok(CardModelBuilder.Build(card));
        }

        /// <summary>
        ///     Erstellt eine neue Checkliste zu einer Karte
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="checklistCreateDto">Titel der Checkliste</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cards/{card:Guid}/checklists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ChecklistModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CreateChecklist([ModelBinder] Card card, ChecklistCreateDto checklistCreateDto) {
            Checklist checklistToCopy = null;
            if (checklistCreateDto.ChecklistToCopyBusinessId.HasValue) {
                checklistToCopy = ChecklistService.GetByBusinessId(checklistCreateDto.ChecklistToCopyBusinessId.Value);
            }
            Checklist checklist = ChecklistService.Create(card, checklistCreateDto.Title, checklistToCopy);
            return Ok(ChecklistModelBuilder.Build(checklist));
        }

        /// <summary>
        ///     Erstellt einen neuen Kommentar an einer Karte
        /// </summary>
        /// <param name="card">BusinessId der Karte als Guid</param>
        /// <param name="currentUser">Der Nutzer, der den Request ausführt.</param>
        /// <param name="textDto">Dto mit dem Kommentartext</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cards/{card:Guid}/comments")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CommentModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CreateComment([ModelBinder] Card card, [ModelBinder] [SwaggerIgnore] User currentUser, StringValueDto textDto) {
            Comment comment = CommentService.Create(card, textDto.Value, currentUser);
            return Ok(CommentModelBuilder.Build(comment));
        }

        /// <summary>
        ///     Liefert alle Kommentare einer Karte
        /// </summary>
        /// <param name="card">Business Id der Karte als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("cards/{card:Guid}/comments")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult FindAllCommentsOnCard([ModelBinder] Card card) {
            IList<Comment> comments = CommentService.FindAllOnCard(card);
            return Ok(comments.Select(CommentModelBuilder.Build).ToList());
        }

        /// <summary>
        ///     Sucht nach Karten für den angemeldeten Nutzer.
        /// </summary>
        /// <param name="currentUser">Der Nutzer, für den die Karten abgerufen werden sollen.</param>
        /// <param name="queryString">Suchzeichenfolge zur Einschränkung der Karten.</param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<CardModel>))]
        [Route("cards")]
        public IHttpActionResult FindCards([FromUri] [ModelBinder] [SwaggerIgnore] User currentUser, [FromUri] string queryString = null) {
            Require.NotNull(currentUser, "currentUser");

            IPage<Card> foundCards = CardService.FindCardsForUser(PageRequest.All, currentUser, queryString);
            return Ok(foundCards.Select(CardModelBuilder.Build));
        }

        /// <summary>
        ///     Liefert alle nicht archivierten Karten, der nicht archivierten Listen eines Boards.
        /// </summary>
        /// <param name="board">Id des Boards als Guid</param>
        /// <param name="users">
        ///     Die Nutzer, deren Karten gefunden werden sollen oder null, wenn alle Karten des Boards geliefert
        ///     werden sollen.
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("boards/{board:Guid}/cards")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<CardModel>))]
        public IHttpActionResult FindCardsOnBoard([ModelBinder] Board board, [FromUri] IList<Guid> users = null) {
            Require.NotNull(board, "board");

            IList<User> userList = new List<User>();
            if (users != null) {
                try {
                    userList = users.Select(UserService.GetById).ToList();
                } catch (Exception e) {
                    return BadRequest("Es konnten nicht alle Nutzer anhand der Ids geladen werden");
                }
            }

            IList<Card> foundCards = CardService.FindCardsOnBoardForUsers(board, userList.ToArray());
            IList<CardModel> cardModels = foundCards.Select(CardModelBuilder.Build).ToList();
            return Ok(cardModels);
        }

        /// <summary>
        ///     Liefert eine Karte anhand der Business Id
        /// </summary>
        /// <param name="card">Business Id der Karte als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("cards/{card:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        public IHttpActionResult Get([ModelBinder] Card card) {
            return Ok(CardModelBuilder.Build(card));
        }

        /// <summary>
        ///     Liefert alle Checklisten einer Karte
        /// </summary>
        /// <param name="card">Business Id der Karte als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("cards/{card:Guid}/checklists")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<ChecklistModel>))]
        public IHttpActionResult GetAllChecklists([ModelBinder] Card card) {
            return Ok(ChecklistService.FindAllOnCard(card).Select(ChecklistModelBuilder.Build).ToList());
        }

        /// <summary>
        ///     Liefert alle archivierten Karten, eines Boards.
        /// </summary>
        /// <param name="board">Id des Boards als Guid</param>
        /// <returns></returns>
        [HttpGet]
        [Route("boards/{board:Guid}/archived-cards")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<CardModel>))]
        public IHttpActionResult GetArchivedCardsByBoard([ModelBinder] Board board) {
            Require.NotNull(board, "board");

            IList<Card> archivedCards = board.Lists.Where(list => !list.IsArchived).SelectMany(list => list.Cards).Where(card => card.IsArchived).ToList();
            IList<CardModel> cardModels = archivedCards.Select(CardModelBuilder.Build).ToList();
            return Ok(cardModels);
        }

        /// <summary>
        ///     Liefert die Anhänge einer Karte
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="currentUser">Der Nutzer der die Anhänge abruft.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("cards/{card:Guid}/attachments")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IList<AttachmentModel>))]
        public IHttpActionResult GetAttachments([ModelBinder] Card card, [ModelBinder] [SwaggerIgnore] User currentUser) {
            IList<Document> allOnCard = DocumentService.FindAllOnCard(card);
            return Ok(allOnCard.Select(d => DocumentModelBuilder.Build(d, currentUser)));
        }

        /// <summary>
        ///     Entfernt das Fälligkeitsdatum von einer Karte
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("cards/{card:Guid}/due")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        public IHttpActionResult RemoveDue([ModelBinder] Card card) {
            Card updatedCard = CardService.UpdateDue(card, null);
            return Ok(CardModelBuilder.Build(updatedCard));
        }

        /// <summary>
        ///     Hebt die Zuordnung eines Labels an eine Karte auf.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("cards/{card:Guid}/labels/{label:Guid}/assignment")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult RemoveLabel([ModelBinder] Card card, [ModelBinder] Label label) {
            Card updatedCard = CardService.RemoveLabel(card, label);
            return Ok(CardModelBuilder.Build(updatedCard));
        }

        /// <summary>
        ///     Setzt ein Fälligkeitsdatum auf einer Karte
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="dateTimeUpdateDto">Neues Fälligkeitsdatum</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cards/{card:Guid}/due")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerResponse((HttpStatusCode)442, typeof(string), Description = "Das Datum muss in als UTC angegeben sein und in der Zukunft liegen.")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult SetDue([ModelBinder] Card card, DateTimeUpdateDto dateTimeUpdateDto) {
            ValidationResult validationResult = Validator.Validate(dateTimeUpdateDto);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }
            Card updateCard = CardService.UpdateDue(card, dateTimeUpdateDto.Value);
            return Ok(CardModelBuilder.Build(updateCard));
        }

        /// <summary>
        ///     Entfernt einen Nutzer aus der Liste der Karte zugewiesenen Nutzer.
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="user">Id des Nutzers der zugewiesen werden soll als Guid</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("cards/{card:Guid}/assignedUsers/{user:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel), Description = "Die Karte.")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(string), Description = "Der Nutzer ist kein Board-Mitglied.")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UnassignUser([ModelBinder] Card card, [ModelBinder] User user) {
            Require.NotNull(card, nameof(card));
            Require.NotNull(user, "user");

            if (!card.AssignedUsers.Contains(user)) {
                /*Der Nutzer ist kein Board-Mitglied.*/
                return NotFound();
            }

            CardService.UnassignUsers(card, user);
            return Ok(CardModelBuilder.Build(card));
        }

        /// <summary>
        ///     Aktualisiert die Beschreibung einer bestehenden Karte
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="card">BusinessId der Karte als Guid</param>
        /// <param name="createDto">Neue Beschreibung</param>
        /// <returns></returns>
        [HttpPut]
        [Route("cards/{card:Guid}/description")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string), Description = "Nutzer hat keine Schreibrechte")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateCardDescription([SwaggerIgnore] [ModelBinder] User currentUser, [ModelBinder] Card card,
            StringValueDto createDto) {
            if (!currentUser.CanWrite) {
                return Unauthorized();
            }
            return Ok(CardModelBuilder.Build(CardService.UpdateDescription(card, createDto.Value)));
        }

        /// <summary>
        ///     Aktualisiert den Titel einer bestehenden Karte
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="card">Busines Id der Karte als Guid</param>
        /// <param name="updateDto">Neuer Titel</param>
        /// <returns></returns>
        [HttpPut]
        [Route("cards/{card:Guid}/title")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(string), Description = "Nutzer hat keine Schreibrechte")]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Titel länger als 75 Zeichen")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateCardTitle([SwaggerIgnore] [ModelBinder] User currentUser, [ModelBinder] Card card, StringValueDto updateDto) {
            if (!currentUser.CanWrite) {
                return Unauthorized();
            }
            ValidationResult validationResult = CardTitleValidator.Validate(updateDto);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }

            CardModel cardModel = CardModelBuilder.Build(CardService.UpdateTitle(card, updateDto.Value));
            return Ok(cardModel);
        }

        /// <summary>
        ///     Aktualisiert, ob eine Karte archiviert ist.
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <param name="isArchived">Dto mit einem Wert ob die Karte archiviert ist.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("cards/{card:Guid}/isArchived")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CardModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult UpdateIsArchived([ModelBinder] Card card, BoolValueDto isArchived) {
            Card updatedCard = CardService.UpdateArchived(card, isArchived.Value);
            return Ok(CardModelBuilder.Build(updatedCard));
        }

        /// <summary>
        ///     Fügt einer Karte ein Anhang hinzu.
        ///     Der Anhang muss als multipart/form-data über den Post eines HTML Forms übergeben werden.
        ///     &lt;html&gt;
        ///     &lt;body&gt;
        ///     &lt;form method="POST" enctype="multipart/form-data"
        ///     action="https://localhost:44376/api/cards/c8e742c7-de98-4e68-96a8-3a6e6bd24971/attachments"
        ///     &gt;
        ///     &lt;input type="file" accept="text/*"&gt;
        ///     &lt;button type="submit"&gt;Eingaben absenden&lt;/button&gt;
        ///     &lt;/form&gt;
        ///     &lt;/body&gt;
        ///     &lt;/html&gt;
        /// </summary>
        /// <param name="card">Id der Karte als Guid</param>
        /// <returns></returns>
        [HttpPost]
        [Route("cards/{card:Guid}/attachments")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AttachmentModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(string), Description = "Dateiendung ist nicht erlaubt")]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public async Task<IHttpActionResult> UploadAttachment([ModelBinder] Card card, [ModelBinder] [SwaggerIgnore] User currentUser) {
            _logger.Info("Start upload: " + DocumentTempPath);
            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            if (!Directory.Exists(DocumentTempPath)) {
                _logger.Error("Dokumenten Upload-Verzeichnis nicht vorhanden: " + DocumentTempPath);
                return InternalServerError();
            }

            try {
                MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(DocumentTempPath);

                await Request.Content.ReadAsMultipartAsync(provider).ConfigureAwait(false);
                string tempFileName = provider.FileData.First().LocalFileName;
                string originalFileName = provider.FileData.First().Headers.ContentDisposition.FileName.Replace("\"", "");

                if (!DocumentService.CanUploadFileWithExtension(originalFileName)) {
                    _logger.Error($"Dateierweiterung von {originalFileName} ist laut Konfiguration nicht erlaubt.");
                    return UnprocessableEntity("Dateiendung ist nicht erlaubt");
                }

                Document document = DocumentService.CreateDocumentAtCard(card, tempFileName, originalFileName);

                return Ok(DocumentModelBuilder.Build(document, currentUser));
            } catch (Exception ex) {
                _logger.Error("Error on upload: ", ex);
            }
            return InternalServerError();
        }
    }
}