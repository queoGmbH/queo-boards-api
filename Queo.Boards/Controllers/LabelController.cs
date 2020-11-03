using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using FluentValidation.Results;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Validators.Labels;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller für <see cref="Label" />
    /// </summary>
    [RoutePrefix("api")]
    public class LabelController : AuthorizationRequiredApiController {
        private readonly LabelDtoValidator _labelDtoValidator;
        private readonly ILabelService _labelService;

        /// <summary>
        /// </summary>
        /// <param name="labelService"></param>
        /// <param name="labelDtoValidator"></param>
        public LabelController(ILabelService labelService, LabelDtoValidator labelDtoValidator) {
            _labelService = labelService;
            _labelDtoValidator = labelDtoValidator;
        }

        /// <summary>
        ///     Erstellt ein neues Label zu einem Board
        /// </summary>
        /// <param name="board">Id des Boards als GUID.</param>
        /// <param name="labelDto">DTO mit den Erstellungsdaten.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("boards/{board:Guid}/labels")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LabelModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult CreateLabel([ModelBinder] Board board, LabelDto labelDto) {
            Require.NotNull(board, "board");

            Label created = _labelService.Create(board, labelDto);
            return Ok(LabelModelBuilder.Build(created));
        }

        /// <summary>
        ///     Löscht ein Label und all dessen Zuordnungen zu Karten.
        /// </summary>
        /// <param name="label">Id des Labels als Guid</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("labels/{label:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LabelModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult Delete([ModelBinder] Label label) {
            Label deletedLabel = _labelService.Delete(label);
            return Ok(LabelModelBuilder.Build(deletedLabel));
        }

        /// <summary>
        ///     Aktualisiert ein Label
        /// </summary>
        /// <param name="label"></param>
        /// <param name="labelDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("labels/{label:Guid}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LabelModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult Update([ModelBinder] Label label, LabelDto labelDto) {
            ValidationResult validationResult = _labelDtoValidator.Validate(labelDto);
            if (!validationResult.IsValid) {
                return UnprocessableEntity(validationResult.Errors);
            }
            Label updated = _labelService.Update(label, labelDto);
            return Ok(LabelModelBuilder.Build(updated));
        }
    }
}