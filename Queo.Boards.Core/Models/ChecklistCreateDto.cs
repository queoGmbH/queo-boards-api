using System;

namespace Queo.Boards.Core.Models {
    /// <summary>
    ///     DTO mit den Daten zum Erstellen einer Checkliste
    /// </summary>
    public class ChecklistCreateDto {
        /// <summary>
        ///     Liefert oder setzt die BusinessId der optionalen Checkliste, deren Daten in die anzulegenden Checkliste kopiert werden sollen.
        /// </summary>
        public Guid? ChecklistToCopyBusinessId { get; set; }

        /// <summary>
        ///     Liefert oder setzt den Titel
        /// </summary>
        public string Title { get; set; }
    }
}