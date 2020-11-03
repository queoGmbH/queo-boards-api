using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Infrastructure.Filter {
    /// <summary>
    ///     Filter, der gewährleistet, dass Boards nur über die richtige Ressource aufgerufen werden.
    ///     "Normale" Boards, also nicht archiviert und keine Vorlage, nur über den Parameternamen "board"
    ///     Board-Vorlagen, also mit <see cref="Board.IsTemplate" /> == true, nur über den Parameternamen "boardTemplate"
    ///     archivierte Boards, also mit <see cref="Board.IsArchived" /> == true, über den Parameternamen "archivedBoard"
    /// </summary>
    public class BoardTypeFilterAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext actionContext) {
            if (actionContext.ActionArguments.ContainsKey("board")) {
                if (!IsValidBoard(actionContext.ActionArguments["board"] as Board)) {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.NotFound);
                    return;
                }
            }
            if (actionContext.ActionArguments.ContainsKey("boardTemplate")) {
                if (!IsValidBoardTemplate(actionContext.ActionArguments["boardTemplate"] as Board)) {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.NotFound);
                    return;
                }
            }
            if (actionContext.ActionArguments.ContainsKey("archivedBoard")) {
                if (!IsValidArchivedBoard(actionContext.ActionArguments["archivedBoard"] as Board)) {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.NotFound);
                    return;
                }
            }

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Überprüft, ob es sich um ein archiviertes Board handelt.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsValidArchivedBoard(Board board) {
            if (board == null) {
                return false;
            }

            /*Das Board muss archiviert und darf nicht als Vorlage markiert sein*/
            return board.IsArchived && !board.IsTemplate;
        }

        /// <summary>
        /// Überprüft, ob es sich um ein nicht archiviertes Board handelt.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsValidBoard(Board board) {
            if (board == null) {
                return false;
            }

            /*Das Board darf nicht archiviert und nicht als Vorlage markiert sein*/
            return !board.IsArchived && !board.IsTemplate;
        }

        /// <summary>
        /// Überprüft, ob es sich um eine Board-Vorlage handelt.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static bool IsValidBoardTemplate(Board board) {
            if (board == null) {
                return false;
            }

            /*Das Board darf nicht archiviert und muss als Vorlage markiert sein*/
            return board.IsTemplate && !board.IsArchived;
        }
    }
}