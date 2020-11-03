using System.Net.Mail;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;

namespace Queo.Boards.Core.Services {
    /// <summary>
    ///     Schnittstelle, die einen Service beschreibt, der Methoden zur E-Mail-Benachrichtigung von Nutzern bei bestimmten
    ///     Ereignissen bereitstellt.
    /// </summary>
    public interface IEmailNotificationService {
        /// <summary>
        ///     Versendet eine E-Mail, mit dem Hinweis an einen Nutzer, dass dieser zur Teilnahme an einem Board eingeladen wurde.
        /// </summary>
        /// <param name="addedUser">Der Nutzer der dem Board hinzugefügt wurde.</param>
        /// <param name="toBoard">Das Board, dem der Nutzer hinzugefügt wurde.</param>
        /// <param name="addingUser">Der Nutzer, der den Nutzer zum Board hinzugefügt hat.</param>
        /// <returns></returns>
        MailMessage NotifyUserAddedToBoard(User addedUser, Board toBoard, User addingUser);

        /// <summary>
        /// Informiert einen Nutzer per E-Mail, dass eine Karte abgelaufen ist.
        /// </summary>
        /// <param name="user">Der zu informierende Nutzer</param>
        /// <param name="expiredCard">Die Karte, deren Fälligkeit abgelaufen ist.</param>
        /// <returns></returns>
        MailMessage NotifyUserOnCardDueExpiration(User user, Card expiredCard);

        /// <summary>
        /// Informiert einen Nutzer per E-Mail, dass ein Kommentar abgegeben wurde.
        /// </summary>
        /// <param name="user">Der zu informierende Nutzer</param>
        /// <param name="createdComment">Der abgegebene Kommentar</param>
        /// <returns></returns>
        MailMessage NotifyUserOnCommentCreated(User user, Comment createdComment);

        /// <summary>
        ///     Informiert einen AD-Nutzer, das er beim Login sein AD-Passwort zu nutzen hat.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        MailMessage SendADUserPasswordResetMessage(User user);

        /// <summary>
        ///     Informiert einen lokalen Nutzer, wie er sein Passwort zurücksetzen kann.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordResetRequestDto"></param>
        /// <returns></returns>
        MailMessage SendLocalUserPasswordResetMessage(User user, PasswordResetRequestDto passwordResetRequestDto);
    }
}