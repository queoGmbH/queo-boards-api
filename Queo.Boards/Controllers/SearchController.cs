using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using NSwag.Annotations;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Services;
using Queo.Boards.Infrastructure.Controller;

namespace Queo.Boards.Controllers {
    /// <summary>
    ///     Controller zur Verarbeitung von Suchanfragen.
    /// </summary>
    [RoutePrefix("api/search")]
    public class SearchController : AuthorizationRequiredApiController {
        private readonly IBoardService _boardService;
        private readonly ICardService _cardService;
        private readonly ICommentService _commentService;

        public SearchController(IBoardService boardService, ICardService cardService, ICommentService commentService) {
            _boardService = boardService;
            _cardService = cardService;
            _commentService = commentService;
        }

        /// <summary>
        ///     Sucht nach Boards, Karten und Kommentaren
        /// </summary>
        /// <param name="q">Die Suchzeichenfolge</param>
        /// <returns></returns>
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(SearchResultModel))]
        [HttpGet]
        public IHttpActionResult Search([SwaggerIgnore] [ModelBinder] User currentUser, [FromUri] string q = null) {

            string decodedQueryString = q;
            if (!string.IsNullOrWhiteSpace(decodedQueryString)) {
                /*Damit die Suche auch bei Umlauten funktioniert, muss die Suchzeichenfolge decodiert werden.*/
                decodedQueryString = HttpUtility.UrlDecode(decodedQueryString);
            }

            IPage<Board> foundBoards = _boardService.FindBoardsForUser(PageRequest.All, currentUser, decodedQueryString);
            IPage<Card> foundCards = _cardService.FindCardsForUser(PageRequest.All, currentUser, decodedQueryString);
            IPage<Comment> foundComments = _commentService.FindCommentsForUser(PageRequest.All, currentUser, decodedQueryString);

            return Ok(new SearchResultModel(foundBoards.Select(BoardModelBuilder.Build).ToList(), foundCards.Select(CardModelBuilder.Build).ToList(), foundComments.Select(CommentModelBuilder.Build).ToList()));
        }
    }
}