using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Logging;
using Microsoft.AspNet.Identity;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Infrastructure.Http;

namespace Queo.Boards.Infrastructure.Filter {
    /// <summary>
    ///     Filter, der steuert, ob der Request im Zusammenhang mit einem Board, vom angemeldeten Nutzer ausgeführt werden
    ///     kann.
    ///     Dazu wird die URL nach der Angabe eines Boards durchsucht und für das Board kontrolliert, ob der Nutzer darauf
    ///     zugreifen kann.
    /// </summary>
    public class BoardScopeAuthorizationFilterAttribute : ActionFilterAttribute {
        private const string LOGGER_NAME = "Security";

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, einen Request einer bestimmten Methode auf das Board auszuführen.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <param name="requestHttpMethod"></param>
        /// <returns></returns>
        public static bool AuthorizeBoard(Board board, Type controller, IPrincipal requestContextPrincipal, HttpMethod requestHttpMethod) {
            ILog logger = LogManager.GetLogger(LOGGER_NAME);
            if (!AuthorizeViewBoard(board, requestContextPrincipal)) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, das Board [{1} => {2}] zu sehen.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return false;
            }

            if (requestHttpMethod != HttpMethod.Get) {
                /*Wenn es sich nicht um ein Get handelt, dann auf Recht zum Ändern prüfen.*/
                if (!AuthorizeUpdateBoard(board, controller, requestContextPrincipal)) {
                    logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, das Board [{1} => {2}] zu bearbeiten.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, einen Request einer bestimmten Methode auf die Karte auszuführen.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <param name="requestHttpMethod"></param>
        /// <returns></returns>
        public static bool AuthorizeCard(Card card, IPrincipal requestContextPrincipal, HttpMethod requestHttpMethod) {
            ILog logger = LogManager.GetLogger(LOGGER_NAME);
            if (!AuthorizeViewCard(card, requestContextPrincipal)) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Karte [{1}] zu sehen.", requestContextPrincipal.Identity.Name, card);
                return false;
            }

            if (requestHttpMethod != HttpMethod.Get) {
                /*Wenn es sich nicht um ein Get handelt, dann auf Recht zum Ändern prüfen.*/
                if (!AuthorizeUpdateCard(card, requestContextPrincipal)) {
                    logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Karte [{1}] zu bearbeiten.", requestContextPrincipal.Identity.Name, card);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, einen Request einer bestimmten Methode auf die Liste auszuführen.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <param name="requestHttpMethod"></param>
        /// <returns></returns>
        public static bool AuthorizeList(List list, IPrincipal requestContextPrincipal, HttpMethod requestHttpMethod) {
            ILog logger = LogManager.GetLogger(LOGGER_NAME);
            if (!AuthorizeViewList(list, requestContextPrincipal)) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Liste [{1}] zu sehen.", requestContextPrincipal.Identity.Name, list);
                return false;
            }

            if (requestHttpMethod != HttpMethod.Get) {
                /*Wenn es sich nicht um ein Get handelt, dann auf Recht zum Ändern prüfen.*/
                if (!AuthorizeUpdateList(list, requestContextPrincipal)) {
                    logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Liste [{1}] zu bearbeiten.", requestContextPrincipal.Identity.Name, list);
                    return false;
                }
            }

            return true;
        }

        /// <summary>Tritt vor dem Aufrufen der Aktionsmethode auf.</summary>
        /// <param name="actionContext">Der Aktionskontext.</param>
        public override void OnActionExecuting(HttpActionContext actionContext) {
            Board board = null;
            List list = null;
            Card card = null;
            Comment comment = null;
            Checklist checklist = null;
            Task task = null;
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (HttpActionContextHelper.TryGetBoardScopeFromAction(actionContext, out board, out list, out card, out comment, out checklist, out task)) {
                /*Es wurde ein Board als Parameter des Request gefunden, also muss ausgewertet werden, ob der aktuelle Nutzer das Recht hat, auf das Board zuzugreifen.*/
                logger.DebugFormat("Der Request [{0} => {1}] findet im Scope eines Boards oder eines Board-Bestandteils statt. Es wird überprüft, ob der aktuelle Nutzer darauf zugreifen kann.", actionContext.Request.Method, actionContext.Request.RequestUri);

                /*Es kann jedoch sein, dass das Board nicht aus dem Request gebunden werden konnte, deswegen muss das auch geprüft werden.*/
                if (board != null) {
                    if (actionContext.RequestContext.Principal == null) {
                        logger.DebugFormat("Es konnte kein Nutzer für die Überprüfung des Board-Scopes ermittelt werden.");
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        return;
                    }

                    if (card != null) {
                        logger.DebugFormat("Der Request findet im Scope der Karte [{0}] statt", card);
                        if (!AuthorizeCard(card, actionContext.RequestContext.Principal, actionContext.Request.Method)) {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                        }
                        return;
                    }

                    if (list != null) {
                        logger.DebugFormat("Der Request findet im Scope der Liste [{0}] statt", list);
                        if (!AuthorizeList(list, actionContext.RequestContext.Principal, actionContext.Request.Method)) {
                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                        }
                        return;
                    }

                    if (!AuthorizeBoard(board, actionContext.ControllerContext.Controller.GetType(), actionContext.RequestContext.Principal, actionContext.Request.Method)) {
                        logger.DebugFormat("Der Request findet im Scope des Boards [{0}] statt", board);
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
                        return;
                    }
                } else {
                    logger.DebugFormat("Der Request findet zwar im Scope eines Boards statt, jedoch konnte das Board nicht ermittelt werden: {0}", actionContext.Request.RequestUri.OriginalString);
                }
            } else {
                logger.DebugFormat("Der aktuelle Request findet nicht im Scope eines Boards statt: {0}", actionContext.Request.RequestUri.OriginalString);
            }

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer autorisiert ist, das Board oder Bestandteile davon zu bearbeiten.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="controllerType"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <returns></returns>
        private static bool AuthorizeUpdateBoard(Board board, Type controllerType, IPrincipal requestContextPrincipal) {
            Require.NotNull(board, "board");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);


            if (requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                /*Admins dürfen Boards immer bearbeiten*/
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] bearbeiten, da er die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (board.Owners.Select(owner => owner.BusinessId.ToString()).Contains(requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] bearbeiten, da er Besitzer des Boards ist.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (board.IsArchived && requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                /*Archivierte Boards können nur von deren Eigentümern (wird vorher geprüft) oder Anwendungs-Administratoren bearbeitet werden*/
                logger.InfoFormat("Der Nutzer [{0}] darf das archivierte Board [{1} => {2}] bearbeiten, da er die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (board.IsTemplate && requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                /*Board-Vorlagen können nur von Anwendungs-Administratoren bearbeitet werden*/
                logger.InfoFormat("Der Nutzer [{0}] darf die Board-Vorlage [{1} => {2}] bearbeiten, da er die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (controllerType == typeof(LabelController) && IsBoardUser(board, requestContextPrincipal.Identity.GetUserId())) {
                // Jedes Mitglied des Boards, darf Labels bearbeiten.
                // TODO: Hier beginnt es langsam unschön zu werden, da dieser Filter viel zu viel abdeckt. Die Stelle sollte überarbeitet werden und die feingranulare Rechteprüfung im Controller passieren.
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] bearbeiten, da jedes Mitglied des Boards Labels bearbeiten kann.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }
            if ((controllerType == typeof(ListController) || controllerType == typeof(CopyController) || controllerType == typeof(MoveController)) && IsBoardUser(board, requestContextPrincipal.Identity.GetUserId())) {
                // Jedes Mitglied des Boards, darf Listen bearbeiten bzw. Karten und Listen auf das Board kopieren oder verschieben.
                // TODO: Siehe vorheriges If.
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] bearbeiten, da jedes Mitglied des Boards Listen bearbeiten kann bzw. Karten und Listen auf das Board kopieren oder verschieben.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            return false;
        }


        private static bool IsBoardUser(Board board, string userId) {
            if (board == null) {
                return false;
            }

            List<string> boardUsersIds = board.GetBoardUsers().Select(user => user.BusinessId.ToString()).ToList();
            bool isBoardUser = boardUsersIds.Contains(userId);
            if (!isBoardUser) {
                ILog logger = LogManager.GetLogger(LOGGER_NAME);
                if (logger.IsDebugEnabled) {
                    logger.DebugFormat("Der Nutzer mit der Id {0} ist nicht in der Liste der Board-Nutzer enthalten: {1}", userId, string.Join(", ", boardUsersIds));
                }
            }
            return isBoardUser;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, die Karte zu bearbeiten.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <returns></returns>
        private static bool AuthorizeUpdateCard(Card card, IPrincipal requestContextPrincipal) {
            Require.NotNull(card, "card");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Karte [{1}] bearbeiten, da er die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, card);
                return true;
            }

            if (card.IsArchived && !requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR) && !IsBoardUser(card.List.Board, requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Karte [{1}] zu bearbeiten, da sie archiviert ist und er nicht die Rolle Administrator hat oder Board-Mitglied ist.", requestContextPrincipal.Identity.Name, card);
                return false;
            }

            if (card.List.Board.Accessibility == Accessibility.Public) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Karte [{1}] bearbeiten, da das Board öffentlich ist.", requestContextPrincipal.Identity.Name, card);
                return true;
            }

            if (card.List.Board.Owners.Select(owner => owner.BusinessId.ToString()).Contains(requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Karte [{1}] bearbeiten, da er Besitzer des Boards ist.", requestContextPrincipal.Identity.Name, card);
                return true;
            }

            if (IsBoardUser(card.List.Board, requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Karte [{1}] bearbeiten, da er Mitglied des Boards ist.", requestContextPrincipal.Identity.Name, card);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, die Liste zu bearbeiten.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <returns></returns>
        private static bool AuthorizeUpdateList(List list, IPrincipal requestContextPrincipal) {
            Require.NotNull(list, "list");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Liste [{1}] bearbeiten, da er die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, list);
                return true;
            }

            if (list.IsArchived && !requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR) && !IsBoardUser(list.Board, requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Liste [{1}] zu bearbeiten, da sie archiviert ist und er nicht die Rolle Administrator hat bzw. ein Mitglied des Boards ist.", requestContextPrincipal.Identity.Name, list);
                return false;
            }

            if (list.Board.Accessibility == Accessibility.Public) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Liste [{1}] bearbeiten, da das Board öffentlich ist.", requestContextPrincipal.Identity.Name, list);
                return true;
            }

            if (list.Board.Owners.Select(owner => owner.BusinessId.ToString()).Contains(requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] darf die Liste [{1}] bearbeiten, da er Besitzer des Boards ist.", requestContextPrincipal.Identity.Name, list);
                return true;
            }

            if (IsBoardUser(list.Board, requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1}] bearbeiten, da er Mitglied des Boards ist.", requestContextPrincipal.Identity.Name, list);
                return true;
            }

            return true;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, das Board oder Bestandteile davon zu sehen.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <returns></returns>
        private static bool AuthorizeViewBoard(Board board, IPrincipal requestContextPrincipal) {
            Require.NotNull(board, "board");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (board.Owners.Select(owner => owner.BusinessId.ToString()).Contains(requestContextPrincipal.Identity.GetUserId())) {
                /*Besitzer eines Boards dürfen dieses immer sehen*/
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] sehen, da er Besitzer des Boards ist.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                /*Admins dürfen Boards immer sehen*/
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] sehen, da er die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (board.IsArchived && !requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR)) {
                /*Archivierte Board können nur von Besitzern des Boards (vorherige Bedingung) oder Administratoren gesehen werden*/
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, das Board [{1} => {2}] zu sehen, da es archiviert ist und er nicht die Rolle Administrator hat.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return false;
            }
            
            if (board.IsTemplate) {
                /*Board-Vorlagen können von allen gesehen werden*/
                logger.InfoFormat("Der Nutzer [{0}] darf die Board-Vorlage [{1} => {2}] sehen, da Vorlagen von allen gesehen werden können.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (board.Accessibility == Accessibility.Public) {
                /*Öffentliche Boards können von allen gesehen werden*/
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] sehen, da es öffentlich ist.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            if (board.GetBoardUsers().Select(user => user.BusinessId.ToString()).Contains(requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] darf das Board [{1} => {2}] sehen, da er Nutzer des Boards ist.", requestContextPrincipal.Identity.Name, board.Id, board.Title);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, die Karte oder Bestandteile davon zu sehen.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <returns></returns>
        private static bool AuthorizeViewCard(Card card, IPrincipal requestContextPrincipal) {
            Require.NotNull(card, "card");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (!AuthorizeViewList(card.List, requestContextPrincipal)) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Karte [{1}] zu sehen, da er bereits die Liste nicht sehen kann.", requestContextPrincipal.Identity.Name, card);
                return false;
            }

            if (card.IsArchived && !requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR) && !IsBoardUser(card.List.Board, requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Karte [{1}] zu sehen, da sie archiviert ist und er nicht die Rolle Administrator hat oder Board-Eigentümer ist.", requestContextPrincipal.Identity.Name, card);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Überprüft, ob der Nutzer berechtigt ist, die Liste oder Bestandteile davon zu sehen.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="requestContextPrincipal"></param>
        /// <returns></returns>
        private static bool AuthorizeViewList(List list, IPrincipal requestContextPrincipal) {
            Require.NotNull(list, "list");
            Require.NotNull(requestContextPrincipal, "requestContextPrincipal");
            ILog logger = LogManager.GetLogger(LOGGER_NAME);

            if (!AuthorizeViewBoard(list.Board, requestContextPrincipal)) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Liste [{1}] zu sehen, da er bereits das Board nicht sehen kann.", requestContextPrincipal.Identity.Name, list);
                return false;
            }

            if (list.IsArchived && !requestContextPrincipal.IsInRole(UserRole.ADMINISTRATOR) && !IsBoardUser(list.Board, requestContextPrincipal.Identity.GetUserId())) {
                logger.InfoFormat("Der Nutzer [{0}] hat keine Berechtigung, die Liste [{1}] zu sehen, da sie archiviert ist und er nicht die Rolle Administrator hat bzw. ein Eigentümer des Boards ist.", requestContextPrincipal.Identity.Name, list);
                return false;
            }

            return true;
        }
    }
}