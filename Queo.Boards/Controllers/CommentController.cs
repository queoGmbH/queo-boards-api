using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Controller;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller für <see cref="Comment" />
    /// </summary>
    [RoutePrefix("api")]
    public class CommentController : AuthorizationRequiredApiController {
        private readonly ICommentService _commentService;
        private const string ERROR_UNDELETE_COMMENT =
                "Ein Kommnetar kann nur als gelöscht markiert werden. Ein Aufheben der Markierung ist nicht möglich.";

        /// <summary>
        /// </summary>
        public CommentController(ICommentService commentService) {
            _commentService = commentService;
        }

        /// <summary>
        ///     Aktualisiert den Inhalt eines bestehenden Kommentars
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="textUpdateDto"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("comments/{comment:Guid}/text")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CommentModel))]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult Update([ModelBinder] Comment comment, StringValueDto textUpdateDto) {
            Comment updatedComment = _commentService.UpdateText(comment, textUpdateDto.Value);
            return Ok(CommentModelBuilder.Build(updatedComment));
        }

        /// <summary>
        /// Markiert ein Kommentar als gelöscht. Ein "Löschen" rückgängig zu machen ist nicht möglich!
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="boolValueDto"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("comments/{comment:Guid}/isDeleted")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(CommentModel))]
        [SwaggerResponse((HttpStatusCode)422, typeof(void),Description = ERROR_UNDELETE_COMMENT)]
        [SwaggerSignalR(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        [SignalrNotification(typeof(BoardChannelHub), SignalrNotificationScope.Board)]
        public IHttpActionResult MarkAsDeleted([ModelBinder] Comment comment, BoolValueDto boolValueDto) {
            if (!boolValueDto.Value) {
                return UnprocessableEntity(ERROR_UNDELETE_COMMENT);
            }
            Comment updatedComment = _commentService.UpdateIsDeleted(comment, boolValueDto.Value);
            return Ok(CommentModelBuilder.Build(updatedComment));
        }
    }
}