using System;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Dto für DateTime
    /// </summary>
    public class DateTimeUpdateDto {
        /// <summary>
        ///     Liefert oder setzt den DateTime Wert. Lässt NULL zu.
        /// </summary>
        public DateTime? Value { get; set; }
    }
}