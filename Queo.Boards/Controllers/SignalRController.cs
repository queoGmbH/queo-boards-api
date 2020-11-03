using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Controllers {

    /// <summary>
    /// Hilfscontroller, der das Testweise senden von Signal-R-Benachrichtigungen ermöglicht.
    /// </summary>
    public class SignalRController : ApiController {
        /// <summary>
        ///     Der Name der Signal R Methode die aufgerufen wird.
        /// 
        ///     Sind die Parameter der Methode (außer <see cref="payload"/>) im Header definiert, erfolgt die Signal-R-Benachrichtigung automatisch.
        ///     Als Payload wird dabei der Rückgabewert der aufgerufenen WebApi-Methode verwendet.
        /// 
        /// 
        ///     Bsp.: Sind im Header für den Post-Request auf "/api/boards" folgenden Parameter enthalten:
        /// 
        ///     signal_r_hub          =	"BoardChannelHub"
        /// 
        ///     signal_r_command = "Board Created Command"
        /// 
        ///     signal_r_channels      = "b6190145-8139-474b-be09-65f1aadb512a"
        /// 
        ///     würde per Signal R auf dem Hub "BoardChannelHub" im Kanal "b6190145-8139-474b-be09-65f1aadb512a" die Methode Execute aufgerufen werden.
        /// 
        /// 
        ///     Als Parameter der Execute-Methode würde ein JSon-Objekt mit den Eigenschaften Command (="Board Created Command") und Payload (= Rückgabewert des Post auf /api/boards) verwendet werden.
        /// 
        /// 
        ///     
        /// </summary>
        /// <remarks>
        /// Die Leerzeilen im Summary, dienen der Formatierung im Swagger.</remarks>
        /// <param name="signal_r_command">
        ///     Der Name des Commands, das auf Client-Seite verwendet wird, um den Store zu
        ///     manipulieren.
        /// 
        ///     Im Html-Header: "X-SignalR-Command"
        /// </param>
        /// <param name="signal_r_hub">
        ///     Der Name des Hubs, welches verwendet werden soll, um die Benachrichtigung zu
        ///     veröffentlichen.
        ///     Im Html-Header: "X-SignalR-Hub"
        /// </param>
        /// <param name="signal_r_channels">
        ///     Der optionale Channel des Hubs, um die Benachrichtigung einzuschränken. Wird kein Name
        ///     übergeben, werden alle Clients, die auf das HUB lauschen benachrichtigt.
        /// 
        ///     Im Html-Header: "X-SignalR-Channel"
        /// </param>
        /// <param name="signal_r_ignore">
        ///     Liste, der nicht zu benachrichtigenden Clients.
        /// 
        ///     Im Html-Header: "X-SignalR-Ignore"
        /// </param>
        /// <param name="payload"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [Route("execute")]
        public IHttpActionResult Execute([FromUri]string signal_r_hub, [FromUri]string signal_r_command, [FromUri]string[] signal_r_channels, [FromUri]string[] signal_r_ignore, [FromBody] object payload) {
            try {
                IHubContext contextFromHubName = SignalrNotificationAttribute.GetContextFromHubName(signal_r_hub);
                SignalrNotificationAttribute.Publish(contextFromHubName, signal_r_command, signal_r_channels, signal_r_ignore, payload);
                return Ok("SignalR-Notification send");
            } catch (Exception e) {
                return InternalServerError(e);
            }
        }


        /// <summary>
        /// Hub, zum Veröffentlichen von Benachrichtigungen über Änderungen an einem Board.
        /// 
        /// Beim Verbinden zum Hub, muss als Query-Parameter die Id des Boards übergeben werden.
        /// </summary>
        /// <param name="board">Die Id des Boards (wird als Channel verwendet)</param>
        /// <returns></returns>
        [Route("boardChannelHub")]
        [HttpGet]
        public IHttpActionResult BoardChannelHub(string board) {
            return Ok("BoardChannelHub");
        }

        /// <summary>
        /// Hub, zum Veröffentlichen von Benachrichtigungen über Änderungen an einer Karte.
        /// 
        /// Beim Verbinden zum Hub, muss als Query-Parameter die Id der Karte übergeben werden.
        /// </summary>
        /// <param name="board">Die Id der Karte (wird als Channel verwendet)</param>
        /// <returns></returns>
        [Route("cardChannelHub")]
        [HttpGet]
        public IHttpActionResult CardChannelHub(string board) {
            return Ok("CardChannelHub");
        }
    }
}