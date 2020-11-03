using System;
using Common.Logging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Hubs;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Infrastructure.SignalR;

namespace Queo.Boards.Hubs {
    /// <summary>
    ///     Hub zum Benachrichtigen von Nutzern, dass es Änderungen an den <see cref="Notification">Benachrichtigungen</see>
    ///     gab.
    ///     Außer beim Erstellen einer neuen Benachrichtigung, funktioniert der Hub so, wie z.B. der
    ///     <see cref="BoardChannelHub" /> und kann über die Header-Parameter gesteuert werden.
    ///     Der Channel entspricht jeweils der <see cref="User.BusinessId">Id des Nutzer</see>. Der jeweilige Nutzer ist der
    ///     einzige, der sich an "seinem" Channel anmelden kann.
    /// 
    ///     Um die Benachrichtigung beim Erstellen kümmert sich der <see cref="SignalrUserNotificationAdvice"/>.
    /// </summary>
    public class UserNotificationHub : GenericHub {
        /// <summary>
        ///     Versucht den Channel anhand des Requests zu ermitteln und überprüft auch, ob es sich um einen validen Channel
        ///     handelt.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        protected static bool TryGetChannel(HubCallerContext context, out string channel) {
            channel = context.QueryString["user"];
            if (channel != null) {
                /*Zunächst den Channel ermitteln*/
                channel = channel.ToLowerInvariant();

                /*Jetzt überprüfen, ob der Nutzer sich auch an seinem Channel anmeldet.
                 *Dazu muss der Channel der Id des angemeldeten Nutzers entsprechen */
                if (IsValidChannel(channel, context)) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Meldet einen Nutzer am Hub an.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        ///     true  - bei erfolgreicher Anmeldung
        ///     false - wenn Anmeldung nicht erfolgreich war.
        /// </returns>
        protected override bool Subscribe(HubCallerContext context) {
            string channel;
            if (TryGetChannel(context, out channel)) {
                Groups.Add(context.ConnectionId, channel);
                LogManager.GetLogger("SignalR").DebugFormat("Der Nutzer [{0}] hat sich auf dem Client [{1}] mit dem Hub [{2}] auf den Channel [{3}] verbunden.", context.User.Identity.Name, context.ConnectionId, GetType().Name, channel);
            } else {
                LogManager.GetLogger("SignalR").DebugFormat("Der Nutzer [{0}] auf dem Client [{1}] konnte sich nicht mit dem Hub [{2}] verbinden, da kein Channel angegeben wurde.", context.User.Identity.Name, context.ConnectionId, GetType().Name, channel);
                return false;
            }

            return base.Subscribe(context);
        }

        /// <summary>
        ///     Überprüft, ob der übergebene Channel valide ist.
        ///     Dazu muss er der Id des angemeldeten Nutzers entsprechen. Klein und Großschreibung ist dabei egal.
        /// </summary>
        /// <param name="channel">Der Channel, an dem sich der Nutzer versucht anzumelden.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool IsValidChannel(string channel, HubCallerContext context) {
            try {
                string idOfAuthenticatedUser = context.User.Identity.GetUserId();
                if (channel == idOfAuthenticatedUser.ToLowerInvariant()) {
                    LogManager.GetLogger("SignalR").InfoFormat("Der Nutzer {0} hat sich erfolgreich am Channel [{1}] des {2} angemeldet.", context.User.Identity.Name, channel, typeof(UserNotificationHub));
                    return true;
                } else {
                    LogManager.GetLogger("SignalR").InfoFormat("Der Nutzer {0} mit der Id [{1}] hat versucht sich unerlaubterweise am Channel [{2}] des {3} anzumelden.", context.User.Identity.Name, idOfAuthenticatedUser, channel, typeof(UserNotificationHub));
                    return false;
                }
            } catch (Exception ex) {
                LogManager.GetLogger("SignalR").InfoFormat("Bei der Anmeldung am {0} konnte die Id des angemeldeten Nutzers nicht ermittelt werden.", ex, typeof(UserNotificationHub));
                return false;
            }
        }
    }
}