using System;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     Grundlegendes Model
    /// </summary>
    public abstract class EntityModel {
        private readonly Guid _businessId;

        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        protected EntityModel(Guid businessId) {
            _businessId = businessId;
        }

        /// <summary>
        ///     Liefert die BusinessId
        /// </summary>
        public Guid BusinessId {
            get { return _businessId; }
        }
    }
}