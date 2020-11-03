using System;
using System.Collections.Generic;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Utils;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     DTO für die Daten zur Kartenerstellung
    /// </summary>
    public class CardDto {
        public CardDto() {
            AssignedLabels = new List<Label>();
        }

        public CardDto(string title, string description, DateTime? due, IList<Label> assignedLabels) {
            AssignedLabels = assignedLabels;
            Description = description;
            Due = due;
            Title = title;
        }

        /// <summary>
        ///     Liefert oder setzt die zugewiesenen Labels
        /// </summary>
        public IList<Label> AssignedLabels {
            get; set;
        }

        /// <summary>
        ///     Liefert oder setzt die Beschreibung
        /// </summary>
        public string Description {
            get; set;
        }

        /// <summary>
        ///     Liefert oder setzt das Fälligkeitsdatum oder NULL
        /// </summary>
        public DateTime? Due {
            get; set;
        }

        /// <summary>
        ///     Liefert oder setzt den Titel
        /// </summary>
        public string Title {
            get; set;
        }

        protected bool Equals(CardDto other) {
            return 
                ListHelper.AreEquivalent(AssignedLabels, other.AssignedLabels) && 
                string.Equals(Description, other.Description) && 
                Due.Equals(other.Due) && 
                string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CardDto)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (AssignedLabels != null ? AssignedLabels.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Due.GetHashCode();
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}