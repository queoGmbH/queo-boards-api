using System.Threading.Tasks;
using Common.Logging;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Queo.Boards.Hubs {

    /// <summary>
    /// Basisklasse für Hubs, die generisch und anhand des Request-Headers eine SignalR-Benachrichtigung veröffentlichen.
    /// 
    /// Die Veröffentlichung erfolgt über die Methode <see cref="Info"/>.
    /// </summary>
    public abstract class GenericHub : Hub {

        public void Execute(ChannelEvent channelEvent) {
            Clients.All.Execute(channelEvent);
        }


        public void Execute(string channel, string[] ignore, object payload) {
            Clients.Group(channel, ignore).Execute(payload);
        }

        public void Execute(string[] ignore, object payload) {
            Clients.AllExcept(ignore).Execute(payload);
        }

        public void NotifyChannel(string channel, string[] ignore, object payload) {
            Clients.Group(channel, ignore).Execute(payload);
        }

        public void Notify(string[] ignore, object payload) {
            Clients.AllExcept(ignore).Execute(payload);
        }


        public void Info(string message) {
            Clients.All.Info(message);
            Clients.All.AddMessage("Nachricht", "Vom Server");
        }




        /// <summary>
        /// Meldet einen Nutzer am Hub an.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// true  - bei erfolgreicher Anmeldung
        /// false - wenn Anmeldung nicht erfolgreich war.
        /// </returns>
        protected virtual bool Subscribe(HubCallerContext context) {
            LogManager.GetLogger("SignalR").DebugFormat("Der Nutzer [{0}] hat sich auf dem Client [{1}] mit dem [{2}] verbunden.", context.User.Identity.Name, context.ConnectionId, this.GetType().Name);
            return true;
        }

        /// <summary>
        /// Meldet einen Client vom Hub ab.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>
        /// true  - bei erfolgreicher Abmeldung
        /// false - wenn Abmeldung nicht erfolgreich war.
        /// </returns>
        protected virtual bool Unsubscribe(HubCallerContext context) {
            LogManager.GetLogger("SignalR").DebugFormat("Der Nutzer [{0}] hat sich auf dem Client [{1}] vom [{2}] abgemeldet.", context.User.Identity.Name, context.ConnectionId, this.GetType().Name);
            return true;
        }

        /// <summary>
        /// Called when the connection connects to this hub instance.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
        public override Task OnConnected() {
            /*Am Kanal anmelden.*/
            if (Subscribe(Context)) {
                return base.OnConnected();
            }

            return null;
        }

        /// <summary>
        /// Called when a connection disconnects from this hub gracefully or due to a timeout.
        /// </summary>
        /// <param name="stopCalled">
        /// true, if stop was called on the client closing the connection gracefully;
        /// false, if the connection has been lost for longer than the
        /// <see cref="P:Microsoft.AspNet.SignalR.Configuration.IConfigurationManager.DisconnectTimeout" />.
        /// Timeouts can be caused by clients reconnecting to another SignalR server in scaleout.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
        public override Task OnDisconnected(bool stopCalled) {
            /*Am Kanal abmelden.*/
            Unsubscribe(Context);

            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Called when the connection reconnects to this hub instance.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /></returns>
        public override Task OnReconnected() {
            /*Am Kanal anmelden.*/
            if (Subscribe(Context)) {
                return base.OnReconnected();
            }

            return null;
        }
    }
}