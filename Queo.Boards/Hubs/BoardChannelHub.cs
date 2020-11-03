using Common.Logging;
using Microsoft.AspNet.SignalR.Hubs;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Hubs {
    /// <summary>
    ///     Hub, das verwendet wird um Board-spezifische Events per SignalR zu publishen/pushen.
    ///     Die Beschränkung auf genau ein Board erfolgt beim Anmelden über den Query-Parameter "board", in welchem die
    ///     <see cref="Board.BusinessId">BusinessId</see> des Boards enthalten sein muss.
    ///     Wird keine BusinessId übergeben, kann keine Anmeldung erfolgen.
    /// </summary>
    public class BoardChannelHub : GenericHub {

        protected static bool TryGetChannel(HubCallerContext context, out string channel) {
            channel = context.QueryString["board"];
            if (channel != null) {
                channel = channel.ToLowerInvariant();
                return true;
            } else {
                return false;
            }
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
        ///     Meldet einen Client vom Hub ab.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        ///     true  - bei erfolgreicher Abmeldung
        ///     false - wenn Abmeldung nicht erfolgreich war.
        /// </returns>
        protected override bool Unsubscribe(HubCallerContext context) {
            string channel;
            if (TryGetChannel(context, out channel)) {
                Groups.Remove(context.ConnectionId, channel);
            } else {
                return false;
            }

            return base.Unsubscribe(context);
        }
    }
}