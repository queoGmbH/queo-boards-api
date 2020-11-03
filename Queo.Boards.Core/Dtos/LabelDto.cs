using FluentValidation.Attributes;
using Queo.Boards.Core.Validators.Labels;

namespace Queo.Boards.Core.Dtos {
    /// <summary>
    ///     Dto für Labels
    /// </summary>
    [Validator(typeof(LabelDtoValidator))]
    public class LabelDto {
        public LabelDto() {
        }

        public LabelDto(string name, string color) {
            Name = name;
            Color = color;
        }

        /// <summary>
        ///     Liefert oder setzt die Farbe
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        ///     Liefert oder setzt den Titel
        /// </summary>
        public string Name { get; set; }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((LabelDto)obj);
        }

        public override int GetHashCode() {
            unchecked { return (Color != null ? Color.GetHashCode() : 0) * 397 ^ (Name != null ? Name.GetHashCode() : 0); }
        }

        protected bool Equals(LabelDto other) {
            return string.Equals(Color, other.Color) && string.Equals(Name, other.Name);
        }
    }
}