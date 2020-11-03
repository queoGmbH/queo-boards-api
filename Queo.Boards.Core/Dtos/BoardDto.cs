using FluentValidation.Attributes;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Validators.Boards;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Dto für die Daten zur Erstellung eines Boards
    /// </summary>
    [Validator(typeof(BoardCreateAndUpdateValidator))]
    public class BoardDto {
        public BoardDto() {
        }

        public BoardDto(string title, Accessibility accessibility, string colorScheme) {
            Accessibility = accessibility;
            ColorScheme = colorScheme;
            Title = title;
        }

        /// <summary>
        ///     Liefert oder setzt die Sichtbarkeit eines Boards
        /// </summary>
        public Accessibility Accessibility { get; set; }

        /// <summary>
        ///     Liefert oder setzt das Farbschema des Boards
        /// </summary>
        public string ColorScheme { get; set; }

        /// <summary>
        ///     Liefert oder setzt den Titel des Boards
        /// </summary>
        public string Title { get; set; }

        protected bool Equals(BoardDto other) {
            return Accessibility == other.Accessibility && string.Equals(ColorScheme, other.ColorScheme) && string.Equals(Title, other.Title);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BoardDto)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (int)Accessibility;
                hashCode = (hashCode * 397) ^ (ColorScheme != null ? ColorScheme.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}