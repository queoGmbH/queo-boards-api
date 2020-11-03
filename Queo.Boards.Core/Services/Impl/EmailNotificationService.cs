using System.Net.Mail;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.Checks;
using Queo.Boards.Core.Infrastructure.Email;
using Queo.Boards.Core.Infrastructure.Frontend;
using Queo.Boards.Core.Infrastructure.Templating;

namespace Queo.Boards.Core.Services.Impl {
    /// <summary>
    ///     Service der Methoden für die E-Mail-Benachrichtigung von Nutzern bei bestimmten Ereignissen bereitstellt.
    /// </summary>
    public class EmailNotificationService : IEmailNotificationService {
        private readonly IEmailService _emailService;
        private readonly FrontendConfiguration _frontendConfiguration;

        /// <summary>
        ///     Konstruktor für Testfälle.
        /// </summary>
        /// <param name="emailService"></param>
        /// <param name="frontendConfiguration">Die Konfiguration des Frontends</param>
        public EmailNotificationService(IEmailService emailService, FrontendConfiguration frontendConfiguration) {
            Require.NotNull(emailService, "emailService");

            _emailService = emailService;
            _frontendConfiguration = frontendConfiguration;
        }

        /// <summary>
        ///     Informiert einen AD-Nutzer, das er beim Login sein AD-Passwort zu nutzen hat.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public MailMessage SendADUserPasswordResetMessage(User user) {
            ModelMap modelMap = new ModelMap {
                { "user", user }
            };

            if (!string.IsNullOrWhiteSpace(user.Email)) {
                MailMessage mailMessage = _emailService.CreateMailMessage(user.Email, modelMap, "PassworResetADUser", false);
                _emailService.SendMessage(mailMessage);
                return mailMessage;
            }

            return null;
        }

        /// <summary>
        ///     Informiert einen lokalen Nutzer, wie er sein Passwort zurücksetzen kann.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordResetRequestDto"></param>
        /// <returns></returns>
        public MailMessage SendLocalUserPasswordResetMessage(User user, PasswordResetRequestDto passwordResetRequestDto) {
            ModelMap modelMap = new ModelMap {
                { "user", user },
                { "resetPasswordDto", passwordResetRequestDto},
                { "frontendConfiguration", _frontendConfiguration }
            };

            if (!string.IsNullOrWhiteSpace(user.Email)) {
                MailMessage mailMessage = _emailService.CreateMailMessage(user.Email, modelMap, "PassworResetLocalUser", false);
                _emailService.SendMessage(mailMessage);
                return mailMessage;
            }

            return null;
        }

        /// <summary>
        ///     Versendet eine E-Mail, mit dem Hinweis an einen Nutzer, dass dieser zur Teilnahme an einem Board eingeladen wurde.
        /// </summary>
        /// <param name="addedUser"></param>
        /// <param name="toBoard"></param>
        /// <param name="addingUser"></param>
        /// <returns></returns>
        public MailMessage NotifyUserAddedToBoard(User addedUser, Board toBoard, User addingUser) {
            Require.NotNull(addedUser, "addedUser");
            Require.NotNull(toBoard, "toBoard");

            ModelMap modelMap = new ModelMap {
                { "user", addedUser },
                { "board", toBoard },
                { "frontendConfiguration", _frontendConfiguration }
            };
            if (!string.IsNullOrWhiteSpace(addingUser.Email)) {
                MailMessage mailMessage = _emailService.CreateMailMessage(addedUser.Email, modelMap, "BoardInvitation", false);
                _emailService.SendMessage(mailMessage);
                return mailMessage;
            }

            return null;
        }

        /// <summary>
        ///     Informiert einen Nutzer per E-Mail, dass eine Karte abgelaufen ist.
        /// </summary>
        /// <param name="user">Der zu informierende Nutzer</param>
        /// <param name="expiredCard">Die Karte, deren Fälligkeit abgelaufen ist.</param>
        /// <returns></returns>
        public MailMessage NotifyUserOnCardDueExpiration(User user, Card expiredCard) {
            ModelMap modelMap = new ModelMap {
                { "user", user },
                { "card", expiredCard },
                { "list", expiredCard.List },
                { "board", expiredCard.List.Board },
                { "frontendConfiguration", _frontendConfiguration }
            };

            if (!string.IsNullOrWhiteSpace(user.Email)) {
                MailMessage mailMessage = _emailService.CreateMailMessage(user.Email, modelMap, "CardDueExpiration", false);
                _emailService.SendMessage(mailMessage);
                return mailMessage;
            }

            return null;
        }

        /// <summary>
        ///     Informiert einen Nutzer per E-Mail, dass ein Kommentar abgegeben wurde.
        /// </summary>
        /// <param name="user">Der zu informierende Nutzer</param>
        /// <param name="createdComment">Der abgegebene Kommentar</param>
        /// <returns></returns>
        public MailMessage NotifyUserOnCommentCreated(User user, Comment createdComment) {
            ModelMap modelMap = new ModelMap {
                { "user", user },
                { "comment", createdComment },
                { "card", createdComment.Card },
                { "list", createdComment.Card.List },
                { "board", createdComment.Card.List.Board },
                { "frontendConfiguration", _frontendConfiguration }
            };

            if (!string.IsNullOrWhiteSpace(user.Email)) {
                MailMessage mailMessage = _emailService.CreateMailMessage(user.Email, modelMap, "CommentCreated", false);
                _emailService.SendMessage(mailMessage);
                return mailMessage;
            }

            return null;
        }
    }
}