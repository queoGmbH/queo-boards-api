namespace Queo.Boards.Hubs {
    /// <summary>
    /// Bildet ein Event auf einem Channel ab.
    /// </summary>
    /// <remarks>
    /// 
    /// Die Struktur des ChannelEvent wird vom Angular-Client verwendet, um den dort implementierten Store zu manipulieren.
    /// Das Command entspricht dabei dem Namen des <see cref="Command">Commands</see> im Angular und der <see cref="Payload"/> enthält den/die Command-Parameter.
    /// </remarks>
    public class ChannelEvent {
        /// <summary>
        /// Erzeugt ein Event, welches über Signal R an die Clients gesendet wird.
        /// </summary>
        /// <param name="command">Namen des Commands</param>
        /// <param name="payload">Payload, also der bzw. die Parameter für das Command</param>
        public ChannelEvent(string command, object payload) {
            Command = command;
            Payload = payload;
        }
        
        /// <summary>
        /// Ruft den Namen des Commands ab.
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Ruft den Payload, also den bzw. die Parameter für das Command ab.
        /// </summary>
        public object Payload { get; private set; }
    }
}