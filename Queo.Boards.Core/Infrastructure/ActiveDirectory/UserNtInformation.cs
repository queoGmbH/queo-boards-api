using System;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {
    /// <summary>
    ///   Sammelt Informationen für einen Nutzer die in der Domäne bzw. ActiveDirectory hinterlegt sind.
    /// </summary>
    public class UserNtInformation : IEquatable<UserNtInformation> {
        private readonly string _company;
        private readonly string _department;
        private readonly string _email;
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _phone;

        /// <summary>
        ///   Erzeugt neue Informationen für einen Nutzer.
        /// </summary>
        /// <param name="firstName"> Vorname des Nutzers </param>
        /// <param name="lastName"> Nachname des Nutzers </param>
        /// <param name="company"> Der Name der Firma, für welche der Nutzer arbeitet. </param>
        /// <param name="department"> Abteilung des Nutzers </param>
        /// <param name="email"> Die E-Mail-Adresse des Nutzers. </param>
        /// <param name="phone"> Die Telefonnummer des Nutzers. </param>
        public UserNtInformation(string firstName, string lastName, string company, string department, string email, string phone) {
            _department = department;
            _email = email;
            _phone = phone;
            _lastName = lastName;
            _company = company;
            _firstName = firstName;
        }

        /// <summary>
        ///   Ruft die Firma ab, für welche der Nutzer arbeitet.
        /// </summary>
        public string Company {
            get { return _company; }
        }

        /// <summary>
        ///   Ruft die Abteilung des Nutzers ab, die in der Domäne hinterlegt ist.
        /// </summary>
        public string Department {
            get { return _department; }
        }
        /// <summary>
        ///   Ruft die E-Mail-Adresse des Nutzers ab.
        /// </summary>
        public string Email {
            get { return _email; }
        }

        /// <summary>
        ///   Ruft den Vornamen des Nutzers ab, der in der Domäne hinterlegt ist.
        /// </summary>
        public string FirstName {
            get { return _firstName; }
        }

        /// <summary>
        ///   Ruft den Nachnamen ab, der in der Domäne hinterlegt ist.
        /// </summary>
        public string LastName {
            get { return _lastName; }
        }

        /// <summary>
        /// Ruft die Telefonnummer des Nutzer ab. 
        /// 
        /// !!! In welcher vorm die Telefonnummer hinterlegt ist, hängt von der Art und Weise der Pflege im AD ab.
        /// </summary>
        public string Phone {
            get { return _phone; }
        }

        /// <summary>
        ///   Gibt an, ob das aktuelle Objekt einem anderen Objekt des gleichen Typs entspricht.
        /// </summary>
        /// <returns> true, wenn das aktuelle Objekt gleich dem <paramref name="other" /> -Parameter ist, andernfalls false. </returns>
        /// <param name="other"> Ein Objekt, das mit diesem Objekt verglichen werden soll. </param>
        public bool Equals(UserNtInformation other) {
            if (other == null) {
                return false;
            }

            // Alle Eigenschaften müssen übereinstimmen.
            return Equals(FirstName, other.FirstName) && Equals(LastName, other.LastName) && Equals(Email, other.Email) && Equals(Department, other.Department) && Equals(Company, other.Company);
        }
    }
}