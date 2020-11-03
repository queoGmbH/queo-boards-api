using System.Net.Mail;
using Queo.Boards.Core.Infrastructure.Templating;

namespace Queo.Boards.Core.Infrastructure.Email {

    /// <summary>
    /// Beschreibt einen Service, der Methoden zum Erstellen und Versenden von E-Mail-Nachrichten bereitstellt.
    /// </summary>
    public interface IEmailService {
        /// <summary>
        /// Erzeugt eine neue Email.
        /// </summary>
        /// <param name="to">Empfänger</param>
        /// <param name="model">Daten</param>
        /// <param name="mailTemplateName">Name des Templates für die Email</param>
        /// <param name="sendAsHtml">Soll die E-Mail als HTML-Mail versendet werden (true) oder als Plain-Text (false)</param>
        /// <returns>Die <see cref="MailMessage">Email</see></returns>
        MailMessage CreateMailMessage(string to, ModelMap model, string mailTemplateName, bool sendAsHtml);

        /// <summary>
        /// Versendet die Mail.
        /// </summary>
        /// <param name="mailMessage"></param>
        void SendMessage(MailMessage mailMessage);
    }
}