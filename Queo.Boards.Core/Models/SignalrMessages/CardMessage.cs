using System;

namespace Queo.Boards.Core.Models.SignalrMessages {
    public class CardMessage : BoardMessage {
        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CardMessage(Guid id)
            : base(id) {
        }
    }
}