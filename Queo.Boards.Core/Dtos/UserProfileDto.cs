namespace Queo.Boards.Core.Dtos {
    /// <summary>
    /// Dto mit Profilinformationen zu einem Nutzer.
    /// </summary>
    public class UserProfileDto {

        /// <summary>
        /// Konstruktor für ModelBinding und Testfälle.
        /// </summary>
        public UserProfileDto() {
        }

        /// <summary>
        /// Konstruktor zur vollständigen Initialisierung des Dtos.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <param name="company"></param>
        /// <param name="department"></param>
        /// <param name="phone"></param>
        public UserProfileDto(string email, string firstname, string lastname, string company, string department, string phone) {
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Company = company;
            Department = department;
            Phone = phone;
        }

        /// <summary>
        /// Ruft die E-Mail-Adresse des Nutzers ab oder legt diese fest.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Ruft den Vornamen des Nutzers ab oder legt diesen fest.
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Ruft den Nachnamen des Nutzers ab oder legt diesen fest.
        /// </summary>
        public string Lastname {
            get; set;
        }
        /// <summary>
        /// Ruft das Unternehmen des Nutzers ab oder legt diesen fest.
        /// </summary>
        public string Company {
            get; set;
        }
        /// <summary>
        /// Ruft die Abteilung des Nutzers ab oder legt diesen fest.
        /// </summary>
        public string Department {
            get; set;
        }

        /// <summary>
        ///     Ruft die Telefonnummer des Nutzers ab oder legt diese fest.
        /// </summary>
        public string Phone { get; set; }

        protected bool Equals(UserProfileDto other) {
            return string.Equals(Email, other.Email) && string.Equals(Firstname, other.Firstname) && string.Equals(Lastname, other.Lastname) && string.Equals(Company, other.Company) && string.Equals(Department, other.Department) && string.Equals(Phone, other.Phone);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserProfileDto)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Firstname != null ? Firstname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Lastname != null ? Lastname.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Company != null ? Company.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Department != null ? Department.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}