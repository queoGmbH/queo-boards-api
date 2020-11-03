using System.Net.Mail;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Infrastructure.Email;
using Queo.Boards.Core.Infrastructure.Frontend;
using Queo.Boards.Core.Infrastructure.Templating;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    /// <summary>
    ///     Enthält Tests für den <see cref="EmailNotificationService" />
    /// </summary>
    [TestClass]
    public class EmailNotificationServiceTest : CreateBaseTest {

        /// <summary>
        /// Testet das Versenden der Benachrichtigung beim Einladen eines Nutzer zur Teilnahme an einem Board.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationOnBoardInvitation() {
            //Given: Ein Nutzer der zu einem Board eingeladen wurde
            User user = Create.User();
            User addingUser = Create.User();
            Board board = Create.Board().Build();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            MailMessage mailMessage = new MailMessage();
            emailServiceMock.Setup(service => service.CreateMailMessage(user.Email, It.Is<ModelMap>(map => map["user"].Equals(user) && map["board"].Equals(board)), "BoardInvitation", false)).Returns(mailMessage);
            emailServiceMock.Setup(service => service.SendMessage(It.IsAny<MailMessage>()));
            EmailNotificationService emailNotificationService = new EmailNotificationService(emailServiceMock.Object, new FrontendConfiguration());

            //When: Die Benachrichtigung über die Einladung versendet werden soll.
            MailMessage createdMailMessage = emailNotificationService.NotifyUserAddedToBoard(user, board, addingUser);

            //Then: Muss eine Nachricht aus dem korrekt Template erzeugt werden.
            createdMailMessage.Should().Be(mailMessage);
            emailServiceMock.Verify(service => service.CreateMailMessage(user.Email, It.IsAny<ModelMap>(), "BoardInvitation", false), Times.Once);
            emailServiceMock.Verify(service => service.SendMessage(It.IsAny<MailMessage>()), Times.Once);
        }


        /// <summary>
        /// Testet das Versenden der E-Mail an einen Nutzer, mit der Information, dass ein neuer Kommentar erstellt wurde.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationOnCommentCreated() {
            //Given: Ein Nutzer, der über einen neuen Kommentar informiert werden soll
            User user = Create.User();
            Comment comment = Create.Comment().Build();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            MailMessage mailMessage = new MailMessage();
            emailServiceMock.Setup(service => service.CreateMailMessage(user.Email, It.Is<ModelMap>(map => map["user"].Equals(user) && map["comment"].Equals(comment)), "CommentCreated", false)).Returns(mailMessage);
            emailServiceMock.Setup(service => service.SendMessage(It.IsAny<MailMessage>()));
            EmailNotificationService emailNotificationService = new EmailNotificationService(emailServiceMock.Object, new FrontendConfiguration());

            //When: Die Benachrichtigung über den neuen Kommentar versendet werden soll.
            MailMessage createdMailMessage = emailNotificationService.NotifyUserOnCommentCreated(user, comment);

            //Then: Muss eine Nachricht aus dem korrekten Template erzeugt und versendet werden.
            createdMailMessage.Should().Be(mailMessage);
            emailServiceMock.Verify(service => service.CreateMailMessage(user.Email, It.IsAny<ModelMap>(), "CommentCreated", false), Times.Once);
            emailServiceMock.Verify(service => service.SendMessage(It.IsAny<MailMessage>()), Times.Once);
        }


        /// <summary>
        /// Testet das Versenden der E-Mail an einen Nutzer, mit der Information, dass eine Karte abgelaufen ist.
        /// </summary>
        [TestMethod]
        public void TestSendNotificationOnCardDueExpiration() {
            //Given: Ein Nutzer, der über eine abgelaufene Karte informiert werden soll
            User user = Create.User();
            Card card = Create.Card().Build();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            MailMessage mailMessage = new MailMessage();
            emailServiceMock.Setup(service => service.CreateMailMessage(user.Email, It.Is<ModelMap>(map => map["user"].Equals(user) && map["card"].Equals(card)), "CardDueExpiration", false)).Returns(mailMessage);
            emailServiceMock.Setup(service => service.SendMessage(It.IsAny<MailMessage>()));
            EmailNotificationService emailNotificationService = new EmailNotificationService(emailServiceMock.Object, new FrontendConfiguration());

            //When: Die Benachrichtigung über die abgelaufene Karte versendet werden soll.
            MailMessage createdMailMessage = emailNotificationService.NotifyUserOnCardDueExpiration(user, card);

            //Then: Muss eine Nachricht aus dem korrekten Template erzeugt und versendet werden.
            createdMailMessage.Should().Be(mailMessage);
            emailServiceMock.Verify(service => service.CreateMailMessage(user.Email, It.IsAny<ModelMap>(), "CardDueExpiration", false), Times.Once);
            emailServiceMock.Verify(service => service.SendMessage(It.IsAny<MailMessage>()), Times.Once);
        }
    }
}