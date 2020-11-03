using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Common.Logging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Hubs;
using Queo.Boards.Infrastructure.Http;
using Spring.Context.Support;

namespace Queo.Boards.Infrastructure.SignalR {
    /// <summary>
    ///     Filter, der automatisch dafür sorgt, dass eine Benachrichtigung an Clients erfolgt.
    ///     Das Hub und die Parameter werden aus dem Header des Request ausgelesen.
    ///     Als Inhalt der Benachrichtigung, wird derselbe verwendet wie bei der Original-Antwort.
    ///     Die Benachrichtigung erfolgt nur, bei einem erfolgreichen Request (HttpStatusCode = 200).
    /// </summary>
    public class SignalrNotificationAttribute : ActionFilterAttribute {
        /// <summary>
        ///     Ruft den Namen des HTTP-Header-Parameters ab, in dem der Name des Commands für Angular hinterlegt ist.
        /// </summary>
        public const string SIGNAL_R_COMMAND_NAME = "X-SignalR-Command";

        /// <summary>
        ///     Ruft den Namen des HTTP-Header-Parameters ab, in dem der oder die Ids der zu ignorierenden Signal-R-Clients
        ///     hinterlegt sind.
        /// </summary>
        public const string SIGNAL_R_IGNORE_CONNECTIONS = "X-SignalR-Ignore";

        private static readonly ILog _logger = LogManager.GetLogger("SignalR");
        private readonly Type _hubType;
        private readonly SignalrNotificationScope _scope;
        private readonly string _signalrNotificationScopePath;

        private string[] _channels = new string[0];

        /// <summary>
        ///     Erstellt die Signal-R-Definition für einen fest definierten Scope.
        /// </summary>
        /// <param name="hubType"></param>
        /// <param name="scope"></param>
        public SignalrNotificationAttribute(Type hubType, SignalrNotificationScope scope) {
            Require.NotNull(hubType, "hubType");

            _hubType = hubType;
            _scope = scope;
        }

        /// <summary>
        ///     Erstellt die Signal-R-Definition für einen zur Laufzeit aus den Request-Parametern ermittelten Scope.
        /// </summary>
        /// <param name="hubType">Der Typ des Hubs, über den die Benachrichtigung erfolgt.</param>
        /// <param name="signalrNotificationScopePath">
        ///     Der Pfad für einen Request-Parameter, aus dem der Scope für die
        ///     Benachrichtigung ermittelt werden soll.
        /// </param>
        public SignalrNotificationAttribute(Type hubType, string signalrNotificationScopePath) {
            Require.NotNull(hubType, "hubType");
            Require.NotNullOrWhiteSpace(signalrNotificationScopePath, "signalrNotificationScopePath");

            _hubType = hubType;
            _scope = SignalrNotificationScope.RequestParameter;
            _signalrNotificationScopePath = signalrNotificationScopePath;
        }

        /// <summary>
        ///     Versucht den Namen des Command, für Angular aus dem Request-Header zu ermitteln. Das Command muss mit dem Schlüssel
        ///     <see cref="SIGNAL_R_COMMAND_NAME" /> im Header stehen.
        ///     Es wird nur der erste Eintrag verwendet.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        public static string GetAngularCommandFromHttpHeader(HttpActionExecutedContext actionExecutedContext) {
            IEnumerable<string> commands;
            if (actionExecutedContext.Request.Headers.TryGetValues(SIGNAL_R_COMMAND_NAME, out commands)) {
                /*Der Name des Commands steht im Request-Header*/
                string commandName = commands.First();
                _logger.DebugFormat("Ermittelter Command-Name: {0}", commandName);
                return commandName;
            } else {
                return null;
            }
        }

        /// <summary>
        ///     Versucht das SignalR-Hub bzw. den HubContext anhand des Hub-Namens zu ermitteln.
        /// </summary>
        /// <param name="hubName"></param>
        /// <returns></returns>
        public static IHubContext GetContextFromHubName(string hubName) {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext(hubName);
            return hubContext;
        }

        /// <summary>
        ///     Versucht die zu ignorierenden SignalR Clients aus dem Request-Header zu ermitteln. Die ermittelten Clients werden
        ///     nicht per SignalR benachrichtigt. Es wird dazu der Header-Eintrag mit dem Schlüssel
        ///     <see cref="SIGNAL_R_IGNORE_CONNECTIONS" /> ausgewertet.
        /// </summary>
        public static string[] GetIgnoredClientsFromHttpHeader(HttpActionExecutedContext actionExecutedContext) {
            IEnumerable<string> ignoreClientsEnum;
            string[] ignoreClients = null;
            if (actionExecutedContext.Request.Headers.TryGetValues(SIGNAL_R_IGNORE_CONNECTIONS, out ignoreClientsEnum)) {
                ignoreClients = ignoreClientsEnum.ToArray();
            }

            return ignoreClients;
        }

        /// <summary>
        ///     Methode, zum Senden der SignalR-Benachrichtigung.
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="command"></param>
        /// <param name="channels"></param>
        /// <param name="ignoreClients"></param>
        /// <param name="payload"></param>
        public static void Publish(IHubContext hubContext, string command, string[] channels, string[] ignoreClients, object payload) {
            Require.NotNull(hubContext, "hubContext");

            if (ignoreClients == null) {
                ignoreClients = new string[0];
            } else {
                _logger.DebugFormat("SignalR-Event wird veröffentlicht an Hub {0} außer an Clients [{1}]", hubContext, string.Join(", ", ignoreClients));
            }

            ChannelEvent channelEvent = new ChannelEvent(command, payload);
            string jsonPayload = JsonConvert.SerializeObject(channelEvent,
                new JsonSerializerSettings() {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            if (channels != null && channels.Any()) {
                _logger.DebugFormat("SignalR-Event wird veröffentlicht an Hub {0} in den Channels {1}: Command => [{2}]", hubContext, string.Join(", ", channels), command);
                /*Nachricht nur an Clients des Channel/der Gruppe*/
                hubContext.Clients.Groups(channels.Select(c => c.ToLowerInvariant()).ToList(), ignoreClients).Execute(jsonPayload);
            } else {
                _logger.InfoFormat("Kein SignalR-Event, weil keine Channels ermittelt werden konnten");
            }
        }

        /// <summary>Tritt nach dem Aufrufen der Aktionsmethode auf.</summary>
        /// <param name="actionExecutedContext">Der Kontext nach der Ausführung der Aktion.</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) {
            try {
                NotifyClientsViaSignalR(actionExecutedContext);
            } catch (Exception ex) {
                LogManager.GetLogger("SignalR").ErrorFormat("Fehler bei SignalR-Benachrichtigung:", ex);
            }
        }

        public override void OnActionExecuting(HttpActionContext actionContext) {
            _channels = GetChannelsFromScopeBeforeExecution(actionContext, _scope);
        }

        /// <summary>
        ///     Liefert die BusinessIds aller Nutzer der Anwendung.
        /// </summary>
        /// <returns></returns>
        private string[] GetAllApplicationUsersChannels() {
            IUserService userService = ContextRegistry.GetContext().GetObject<IUserService>();
            return userService.GetAll(PageRequest.All).Select(user => user.BusinessId.ToString()).ToArray();
        }

        /// <summary>
        ///     Liefert die BusinessIds aller Nutzer der Anwendung mit der Rolle <see cref="UserRole.ADMINISTRATOR" />
        /// </summary>
        /// <returns></returns>
        private string[] GetApplicationAdminChannels() {
            IUserService userService = ContextRegistry.GetContext().GetObject<IUserService>();
            return userService.FindByRole(PageRequest.All, UserRole.ADMINISTRATOR).Select(user => user.BusinessId.ToString()).ToArray();
        }

        /// <summary>
        ///     Liefert die BusinessIds aller Nutzer der Anwendung mit der Rolle <see cref="UserRole.USER" />
        /// </summary>
        /// <returns></returns>
        private string[] GetApplicationUsersChannels() {
            IUserService userService = ContextRegistry.GetContext().GetObject<IUserService>();
            return userService.FindByRole(PageRequest.All, UserRole.USER).Select(user => user.BusinessId.ToString()).ToArray();
        }

        /// <summary>
        ///     Liefert die BusinessIds des Boards welches mit dem Request bearbeitet werden soll.
        /// </summary>
        /// <returns></returns>
        private string[] GetBoardChannelFromRequestScope(HttpActionContext actionContext) {
            string[] channels = { };
            Board board = null;
            List list = null;
            Card card = null;
            Comment comment = null;
            Checklist checklist = null;
            Task task = null;
            if (HttpActionContextHelper.TryGetBoardScopeFromAction(actionContext, out board, out list, out card, out comment, out checklist, out task)) {
                channels = new[] { board.BusinessId.ToString() };
            }

            return channels;
        }

        /// <summary>
        ///     Liefert die BusinessIds aller Nutzer des Boards.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        private string[] GetBoardUsersChannelsFromBoard(Board board) {
            if (board.Accessibility == Accessibility.Public) {
                return GetAllApplicationUsersChannels();
            } else {
                return board.GetBoardUsers().Select(user => user.BusinessId.ToString()).ToArray();
            }
        }

        /// <summary>
        ///     Liefert die BusinessIds aller Nutzer des Boards, welches mit dem Request bearbeitet werden soll.
        /// </summary>
        private string[] GetBoardUsersChannelsFromRequestScope(HttpActionContext actionContext) {
            string[] channels = { };
            Board board;
            List list;
            Card card;
            Comment comment;
            Checklist checklist;
            Task task;
            if (HttpActionContextHelper.TryGetBoardScopeFromAction(actionContext, out board, out list, out card, out comment, out checklist, out task)) {
                channels = GetBoardUsersChannelsFromBoard(board);
            } else {
                channels = GetCurrentUsersChannelFromRequestScope(actionContext);
            }

            return channels;
        }

        private string[] GetChannelsFromPath(HttpActionContext actionContext, string signalrNotificationScopePath) {
            Require.NotNullOrWhiteSpace(signalrNotificationScopePath, "signalrNotificationScopePath");

            try {
                string propertyPath;
                string requestParameterName = ParseScopePath(signalrNotificationScopePath, out propertyPath);

                if (!actionContext.ActionArguments.ContainsKey(requestParameterName)) {
                    /*Der im Pfad angegebene Request-Parameter existiert nicht => keine Kanäle*/
                    return new string[0];
                }

                object requestParameterValue = actionContext.ActionArguments[requestParameterName];
                if (requestParameterValue == null) {
                    /*Der im Pfad angegebene Request-Parameter ist null => keine Kanäle*/
                    return new string[0];
                }
                if (propertyPath != null) {
                    object propertyValue = GetScopePathValue(requestParameterValue, propertyPath);
                    if (propertyValue != null) {
                        return new[] { propertyValue.ToString() };
                    } else {
                        /*Es konnte kein */
                        return new string[0];
                    }
                } else {
                    return new[] { requestParameterValue.ToString() };
                }
            } catch (Exception e) {
                return new string[0];
            }
        }

        private static object GetScopePathValue(object requestParameterValue, string propertyPath) {
            try {
                _logger.DebugFormat("Anhand des Pfades [{0}] wird versucht, den Wert aus den Request-Parametern ermittelt werden.", propertyPath);
                ParameterExpression parameterExp = Expression.Parameter(requestParameterValue.GetType(), "type");
                MemberExpression propertyExp = null;
                foreach (string part in propertyPath.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries)) {
                    if (propertyExp == null) {
                        propertyExp = Expression.Property(parameterExp, part);
                    } else {
                        propertyExp = Expression.Property(propertyExp, part);
                    }
                }

                LambdaExpression lambdaExpression = Expression.Lambda(propertyExp, parameterExp);
                Delegate @delegate = lambdaExpression.Compile();
                object propertyValue = @delegate.DynamicInvoke(requestParameterValue);
                _logger.InfoFormat("Anhand des Pfades [{0}] wurde folgender Wert aus den Request-Parametern ermittelt: {1}", propertyPath, propertyValue);
                return propertyValue;
            } catch (Exception e) {
                _logger.ErrorFormat("Anhand des Pfades [{0}] konnte kein Wert aus den Request-Parametern ermittelt werden.", e, propertyPath);
                return null;
            }
            
        }

        private static string ParseScopePath(string signalrNotificationScopePath, out string propertyPath) {
            string[] split = signalrNotificationScopePath.Split(new[] { "." }, 2, StringSplitOptions.RemoveEmptyEntries);
            string requestParameterName = split[0];
            propertyPath = null;
            if (split.Length > 1) {
                propertyPath = split[1];
            }
            return requestParameterName;
        }

        private string[] GetChannelsFromScopeAfterExecution(HttpActionExecutedContext actionExecutedContext, SignalrNotificationScope scope) {
            switch (scope) {
                case SignalrNotificationScope.Admins:
                    return GetApplicationAdminChannels();
                case SignalrNotificationScope.Users:
                    return GetApplicationUsersChannels();
                case SignalrNotificationScope.AllUsers:
                    return GetAllApplicationUsersChannels();
                case SignalrNotificationScope.BoardUsers:
                    return GetBoardUsersChannelsFromRequestScope(actionExecutedContext.ActionContext);
                case SignalrNotificationScope.Board:
                    return GetBoardChannelFromRequestScope(actionExecutedContext.ActionContext);
                case SignalrNotificationScope.CurrentUser:
                    return GetCurrentUsersChannelFromRequestScope(actionExecutedContext.ActionContext);
                default:
                    return new string[0];
            }
        }

        private string[] GetChannelsFromScopeBeforeExecution(HttpActionContext actionContext, SignalrNotificationScope scope) {
            switch (scope) {
                case SignalrNotificationScope.BoardUsers:
                    return GetBoardUsersChannelsFromRequestScope(actionContext);
                case SignalrNotificationScope.RequestParameter:
                    return GetChannelsFromPath(actionContext, _signalrNotificationScopePath);
                default:
                    return new string[] { };
            }
        }

        private string[] GetCurrentUsersChannelFromRequestScope(HttpActionContext actionContext) {
            return new[] {
                actionContext.RequestContext.Principal.Identity.GetUserId()
            };
        }

        /// <summary>
        ///     Versucht den Payload aus der Response zu ermittelt.
        ///     Der Payload entspricht eigentlich immer der Response.
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <returns></returns>
        private object GetPayload(HttpActionExecutedContext actionExecutedContext) {
            if (!(actionExecutedContext.Response.Content is ObjectContent)) {
                /*Es konnte kein sinnvoller Response-Wert ermittelt werden*/
                _logger.DebugFormat("Es konnte kein Payload ermittelt werden. Response-Content: {0}", actionExecutedContext.Response.Content);
                return null;
            } else {
                ObjectContent objectContent = (ObjectContent)actionExecutedContext.Response.Content;
                _logger.DebugFormat("Es wurde folgender Payload ermittelt: {0}", objectContent.Value);
                return objectContent.Value;
            }
        }

        private void NotifyClientsViaSignalR(HttpActionExecutedContext actionExecutedContext) {
            if (actionExecutedContext.Response == null) {
                LogManager.GetLogger("SignalR").DebugFormat("Kein SignalR-Event da Response gleich null: {0}=>{1}", actionExecutedContext.Request.Method, actionExecutedContext.Request.RequestUri);
                return;
            }

            if (actionExecutedContext.Response.StatusCode != HttpStatusCode.OK) {
                LogManager.GetLogger("SignalR").DebugFormat("Kein SignalR-Event da Status-Code nicht 200, sondern [{0}]: {1}=>{2}", actionExecutedContext.Response.StatusCode, actionExecutedContext.Request.Method, actionExecutedContext.Request.RequestUri);

                /*Wenn bei der Anforderung Fehler aufgetreten sind, dann keine Signal R Nachricht.*/
                return;
            }

            if (actionExecutedContext.Request.Method == HttpMethod.Get) {
                LogManager.GetLogger("SignalR").DebugFormat("Kein SignalR-Event da Request-Methode = HttpGet");

                /*Aus Sicherheitsgründen, kann automatisches SignalR nur bei Request ausgeführt werden, die nicht mit HttpGet angefordert werden.*/
                return;
            }

            string command = GetAngularCommandFromHttpHeader(actionExecutedContext);
            string[] ignoreClients = GetIgnoredClientsFromHttpHeader(actionExecutedContext);
            object payload = GetPayload(actionExecutedContext);

            /*Automatisch eine Signal R Benachrichtigung schicken*/
            IHubContext hubContext = GetContextFromHubName(_hubType.Name);
            string[] channels = GetChannelsFromScopeAfterExecution(actionExecutedContext, _scope).Union(_channels).ToArray();

            if (hubContext != null && payload != null && !string.IsNullOrWhiteSpace(command)) {
                Publish(hubContext, command, channels, ignoreClients, payload);
            } else {
                LogManager.GetLogger("SignalR").InfoFormat("Kein SignalR-Event da HubContext, Payload oder Command null!: {0}=>{1}", actionExecutedContext.Request.Method, actionExecutedContext.Request.RequestUri);
            }
        }
    }
}