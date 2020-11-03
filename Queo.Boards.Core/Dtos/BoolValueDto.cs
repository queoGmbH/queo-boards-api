namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Dto für einen Bool Wert
    /// </summary>
    public class BoolValueDto {
        /// <summary>Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.</summary>
        public BoolValueDto(bool value) {
            Value = value;
        }

        /// <summary>
        ///     Liefert oder setzt den Bool Wert
        /// </summary>
        public bool Value { get; private set; }
    }
}