using System.ComponentModel;

namespace Queo.Boards.Core.Infrastructure.ActiveDirectory {

    /// <summary>
    /// Enumeration der Eigenschaften die aus dem ActiveDirectory für einen Nutzer ausgelesen werden können.
    /// 
    /// Sollten weitere Attribute hinzugefügt werden müssen, können diese hier nachgelesen werden: http://msdn.microsoft.com/en-us/library/windows/desktop/ms675090%28v=vs.85%29.aspx
    /// </summary>
    public enum ActiveDirectoryUserProperty {
        
        /// <summary>
        /// Abfrage des Vollständigen Namens eines Benutzers.
        /// </summary>
        [Description("Voller Name des Benutzers")]
        [ActiveDirectoryUserProperty("cn")]
        LoginName,
        
        /// <summary>
        /// Abfrage des Vornamens eines Benutzers.
        /// </summary>
        [Description("Vorname des Benutzers")]
        [ActiveDirectoryUserProperty("givenName")]
        FirstName,

        /// <summary>
        /// Abfrage des Nachnamens eines Benutzers.
        /// </summary>
        [Description("Nachname des Benutzers")]
        [ActiveDirectoryUserProperty("sn")]
        LastName,

        /// <summary>
        /// Abfrage der Mail-Adresse eines AD-Benutzers.
        /// </summary>
        [Description("Mail-Adresse des Benutzers")]
        [ActiveDirectoryUserProperty("mail")]
        Mail,

        /// <summary>
        /// Abfrage des Firmennamens eines AD-Benutzers.
        /// </summary>
        [Description("Unternehmen des Benutzers")]
        [ActiveDirectoryUserProperty("company")]
        Company,

        /// <summary>
        /// Abfrage des Namens der Abteilung eines AD-Benutzers.
        /// </summary>
        [Description("Abteilung des Benutzers")]
        [ActiveDirectoryUserProperty("department")]
        Department,

        /// <summary>
        /// Abfrage der Telefonnummer eines AD-Benutzers.
        /// </summary>
        [Description("Telefonnummer")]
        [ActiveDirectoryUserProperty("telephoneNumber")]
        Phone
    }

    public static class ExtensionMethods {
        
        /// <summary>
        /// Liefert eine Abkürzung die für das Abfragen der Eigenschaft aus dem ActiveDirectory verwendet werden kann.
        /// Die Abkürzung wird über das <see cref="ActiveDirectoryUserPropertyAttribute"/> ermittelt/definiert.
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetShortcut(this ActiveDirectoryUserProperty en) {
            ActiveDirectoryUserPropertyAttribute[] attributes =
                (ActiveDirectoryUserPropertyAttribute[])
                en.GetType().GetField(en.ToString()).GetCustomAttributes(typeof(ActiveDirectoryUserPropertyAttribute), false);
            if (attributes.Length > 0) {
                return attributes[0].Shortcut;
            }

            return "";
        }

    }
}