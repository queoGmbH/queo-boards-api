namespace Queo.Boards.Core.Domain {
    /// <summary>
    ///     Dto mit allgemeinen Informationen zu einem Team.
    /// </summary>
    public class TeamDto {
        public TeamDto() {
        }

        public TeamDto(string name, string description) {
            Description = description;
            Name = name;
        }

        /// <summary>
        ///     Ruft die Beschreibung des Teams ab oder legt diese fest.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Ruft den Namen des Teams ab oder legt diesen fest.
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
            return Equals((TeamDto)obj);
        }

        public override int GetHashCode() {
            unchecked { return (Name != null ? Name.GetHashCode() : 0) * 397 ^ (Description != null ? Description.GetHashCode() : 0); }
        }

        protected bool Equals(TeamDto other) {
            return string.Equals(Name, other.Name) && string.Equals(Description, other.Description);
        }
    }
}