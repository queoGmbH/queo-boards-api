using System;
using System.Linq;
using System.Net.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Email;
using Queo.Boards.Core.Infrastructure.Frontend;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    /// <summary>
    ///     Enthält Tests für den <see cref="EmailNotificationService" />
    /// </summary>
    [TestClass]
    public class EmailNotificationServiceIntegrationTest : ServiceBaseTest {
        public IEmailNotificationService EmailNotificationService { private get; set; }

        public FrontendConfiguration FrontendConfiguration { get; set; }

        public IEmailService EmailService { get; set; }

        /// <summary>
        ///     Testet das Versenden der E-Mail, wenn die Fälligkeit einer Karte erreicht ist.
        /// </summary>
        [TestMethod]
        public void TestSendCardDueExpirationEmail() {
            //Given: Eine Karte
            User user = Create.User();
            Card expiredCard = Create.Card().Due(DateTime.UtcNow.AddYears(-1));

            //When: Die Fälligkeits-E-mail versendet werden soll
            MailMessage notifyUserOnCardDueExpiration = EmailNotificationService.NotifyUserOnCardDueExpiration(user, expiredCard);

            //Then: Muss eine korrekte Mail-Message erstellt werden
            notifyUserOnCardDueExpiration.Body.Should().Contain(FrontendConfiguration.BaseUrl);
            notifyUserOnCardDueExpiration.To.First().Should().Be(user.Email);
            notifyUserOnCardDueExpiration.Body.Should().Contain(expiredCard.BusinessId.ToString());
            notifyUserOnCardDueExpiration.Body.Should().NotContain("Liquid syntax error");
        }

        /// <summary>
        ///     Testet das versendet einer Mail bei neuem Kommentar.
        /// </summary>
        [TestMethod]
        public void TestSendCommentCreatedEmail() {
            //Given: Ein Kommentar und ein zu informierender Nutzer
            User user = Create.User();
            Comment comment = Create.Comment().Build();

            //When: Der Nutzer eine E-Mail erhalten soll
            MailMessage createdMailMessage = EmailNotificationService.NotifyUserOnCommentCreated(user, comment);

            //Then: Muss eine Nachricht ohne Syntax-Errors und mit dem Link zur Karte des Kommentars versendet werden.
            createdMailMessage.To[0].Address.Should().BeEquivalentTo(user.Email);
            createdMailMessage.Body.Should().Contain(FrontendConfiguration.BaseUrl);
            createdMailMessage.Body.Should().Contain(comment.BusinessId.ToString());
            createdMailMessage.Body.Should().Contain(comment.Card.BusinessId.ToString());
            createdMailMessage.Body.Should().Contain(comment.Card.List.Board.BusinessId.ToString());
            createdMailMessage.Body.Should().NotContain("Liquid syntax error");
        }

        /// <summary>
        ///     Testet das Versenden der Benachrichtigung beim Einladen eines Nutzer zur Teilnahme an einem Board.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationOnBoardInvitation() {
            //Given: Ein Nutzer der zu einem Board eingeladen wurde
            User user = Create.User();
            User addingUser = Create.User();
            Board board = Create.Board().Build();

            //When: Die Benachrichtigung über die Einladung versendet werden soll.
            MailMessage createdMailMessage = EmailNotificationService.NotifyUserAddedToBoard(user, board, addingUser);

            //Then: Muss eine Nachricht aus dem korrekt Template erzeugt werden.
            createdMailMessage.To[0].Address.Should().BeEquivalentTo(user.Email);
            createdMailMessage.Subject.Should().Be("Einladung zum Board " + board.Title);
            createdMailMessage.Body.Should().Contain(board.BusinessId.ToString());
            createdMailMessage.Body.Should().Contain(FrontendConfiguration.BaseUrl);
            createdMailMessage.Body.Should().NotContain("Liquid syntax error");
        }
    }
}