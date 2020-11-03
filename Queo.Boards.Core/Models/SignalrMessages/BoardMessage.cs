using System;
using Newtonsoft.Json;

namespace Queo.Boards.Core.Models.SignalrMessages {
    public class BoardMessage : CrudMessageBase{
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("colorScheme")]
        public string Color { get; set; }

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public BoardMessage(Guid id)
            : base(id) {
        }
    }
}