using System;

namespace Queo.Boards.Core.Models.SignalrMessages {
    public class CrudMessageBase {
        private readonly Guid _id;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public CrudMessageBase(Guid id) {
            _id = id;
        }

        public Guid ID {
            get { return _id; }
        }
    }
}