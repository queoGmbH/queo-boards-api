using System.Web.Http.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;

namespace Queo.Boards.Infrastructure.Http {

    /// <summary>
    /// Hilfsmethoden, zur Untersuchung des HttpActionContexts.
    /// </summary>
    public static class HttpActionContextHelper {


        /// <summary>
        ///     Versucht aus dem Request den Board-Kontext (für welches Board soll etwas gemacht werden?) zu ermitteln.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="board">Das Board in dessen Scope der Request läuft</param>
        /// <param name="list">Die Liste in deren Scope der Request läuft</param>
        /// <param name="card">Die Karte in deren Scope der Request läuft</param>
        /// <param name="comment">Der Kommentar in dessen Scope der Request läuft</param>
        /// <param name="checklist">Die Checkliste in deren Scope der Request läuft</param>
        /// <param name="task">Der Task in dessen Scope der Request läuft</param>
        /// <returns></returns>
        public static bool TryGetBoardScopeFromAction(HttpActionContext actionContext, out Board board, out List list, out Card card, out Comment comment, out Checklist checklist, out Task task) {
            board = null;
            list = null;
            card = null;
            comment = null;
            checklist = null;
            task = null;

            if (actionContext.ActionArguments.ContainsKey("task")) {
                /*In der Url wurde ein Task gefunden. Daraus kann ein Board-Scope ermittelt werden.*/
                task = (Task)actionContext.ActionArguments["task"];
                checklist = task.Checklist;
                card = checklist.Card;
                list = card.List;
                board = list.Board;
                return true;
            }
            else if(actionContext.ActionArguments.ContainsKey("checklist")) {
                /*In der Url wurde eine Checkliste gefunden. Daraus kann ein Board-Scope ermittelt werden.*/
                checklist = (Checklist)actionContext.ActionArguments["checklist"];
                card = checklist.Card;
                list = card.List;
                board = list.Board;
                return true;
            } else if (actionContext.ActionArguments.ContainsKey("comment")) {
                /*In der Url wurde ein Kommentar gefunden. Daraus kann ein Board-Scope ermittelt werden.*/
                comment = (Comment)actionContext.ActionArguments["comment"];
                card = comment.Card;
                list = card.List;
                board = list.Board;
                return true;
            } else if (actionContext.ActionArguments.ContainsKey("card")) {
                /*In der Url wurde eine Karte gefunden. Daraus kann ein Board-Scope ermittelt werden.*/
                card = (Card)actionContext.ActionArguments["card"];
                list = card.List;
                board = list.Board;
                return true;
            } else if (actionContext.ActionArguments.ContainsKey("list")) {
                /*In der Url wurde eine List gefunden. Daraus kann ein Board-Scope ermittelt werden.*/
                list = (List)actionContext.ActionArguments["list"];
                board = list.Board;
                return true;
            } else if (actionContext.ActionArguments.ContainsKey("board")) {
                /*In der Url wurde ein Board gefunden.*/
                board = (Board)actionContext.ActionArguments["board"];
                return true;
            } else if (actionContext.ActionArguments.ContainsKey("archivedBoard")) {
                /*In der Url wurde ein archiviertes Board gefunden.*/
                board = (Board)actionContext.ActionArguments["archivedBoard"];
                return true;
            } else if (actionContext.ActionArguments.ContainsKey("boardTemplate")) {
                /*In der Url wurde eine Board-Vorlage gefunden.*/
                board = (Board)actionContext.ActionArguments["boardTemplate"];
                return true;
            } else {
                /*Aus der URL kann kein Board ermittelt werden.*/
                return false;
            }
        }

        /// <summary>
        ///     Versucht aus dem Request den Nutzer-Kontext (für welchen Nutzer soll etwas gemacht werden?) zu ermitteln.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool TryGetUserScopeFromAction(HttpActionContext actionContext, out User user) {
            user = null;
            
            if (actionContext.ActionArguments.ContainsKey("user")) {
                /*In der URL wurde ein Nutzer angegeben.*/
                user = (User)actionContext.ActionArguments["user"];
                return true;
            } else {
                /*Aus der URL kann kein Nutzer-Scope ermittelt werden.*/
                return false;
            }
        }

        /// <summary>
        ///     Versucht aus dem Request den Benachrichtigung-Kontext (für welche Benachrichtigung soll etwas gemacht werden?) zu ermitteln.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public static bool TryGetNotificationScopeFromAction(HttpActionContext actionContext, out Notification notification) {
            
            if (actionContext.ActionArguments.ContainsKey("notification")) {
                /*In der URL wurde eine Benachrichtigung angegeben.*/
                notification = (Notification)actionContext.ActionArguments["notification"];
                return true;
            } else {
                /*Aus der URL kann kein Notification-Scope ermittelt werden.*/
                notification = null;
                return false;
            }
        }
    }
}