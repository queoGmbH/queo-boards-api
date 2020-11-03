using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Command mit den Parametern zur Erstellung einer neuen Karte.
    /// </summary>
    public class CardCreateCommand {
        public CardCreateCommand() {
        }

        public CardCreateCommand(IList<User> assignedUsers, IList<Label> assignedLabels, string title, string description, DateTime? due) {
            AssignedLabels = assignedLabels;
            Description = description;
            Title = title;
            Due = due;
            AssignedUsers = assignedUsers;
        }

        /// <summary>
        ///     Liefert oder setzt die zugewiesenen Labels
        /// </summary>
        public IList<Label> AssignedLabels { get; set; }

        /// <summary>
        ///     Ruft die Liste der optional direkt der Karte zugeordneten Nutzern ab oder legt diese fest.
        /// </summary>
        public IList<User> AssignedUsers { get; set; }

        /// <summary>
        ///     Liefert oder setzt die Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Liefert oder setzt das Fälligkeitsdatum oder NULL
        /// </summary>
        public DateTime? Due { get; set; }

        /// <summary>
        ///     Liefert oder setzt den Titel
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Ruft das Dto mit allgemeinen Informationen über die Karte ab oder legt dieses fest.
        /// </summary>
        public CardDto GetCardDto() {
            return new CardDto() {
                AssignedLabels = AssignedLabels,
                Description = Description,
                Due = Due,
                Title = Title
            };
        }
    }
}