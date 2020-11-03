using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    /// <summary>
    ///     Tests für den <see cref="NotificationService" />.
    /// </summary>
    [TestClass]
    public class NotificationServiceTest : CreateBaseTest {
        /// <summary>
        ///     Testet das Erstellen einer Benachrichtigung zu einer Karte für mehrere Nutzer
        /// </summary>
        [TestMethod]
        public void TestCreateCardNotificationForMultipleUsers() {
            //Given: Eine Karte für die eine Benachrichtigung erstellt werden soll 
            User user1 = Create.User();
            User user2 = Create.User();

            Card card = Create.Card();

            Mock<INotificationDao> notificationDaoMock = new Mock<INotificationDao>();
            notificationDaoMock.Setup(d => d.Save(It.Is<IList<Notification>>(list => list.Count == 2)));
            NotificationService notificationService = new NotificationService(notificationDaoMock.Object, null, null);

            //When: Die Benachrichtigungen erstellt werden.
            IList<CardNotification> createdNotifications = notificationService.CreateCardNotification(new[] { user1, user2 }, card, CardNotificationReason.DueExpiration);

            //Then: Müssen mehrere Benachrichtigungen erstellt werden.
            createdNotifications.Should().HaveCount(2, "Es müssen 2 Benachrichtigungen angelegt werden.");
            createdNotifications.Should().OnlyContain(notification => notification.Card.Equals(card), "An allen Benachrichtigungen muss die Karte hinterlegt sein.");
            createdNotifications.Should().OnlyContain(notification => notification.NotificationReason == CardNotificationReason.DueExpiration, "An allen Benachrichtigungen muss der Benachrichtigungs-Anlass korrekt hinterlegt sein.");
            createdNotifications.All(n => n.EmailSend == false).Should().BeTrue("Die Benachrichtigungen dürfen nicht mit dem Flag 'E-Mail-versendet' markiert sein.");
            createdNotifications.All(n => n.EmailSendAt.HasValue == false).Should().BeTrue("Die Benachrichtigungen dürfen nicht mit einem Zeitstempel für das versenden der E-Mail versehen sein.");

            notificationDaoMock.Verify(d => d.Save(It.Is<IList<Notification>>(list => list.Count == 2)), Times.Once);
        }

        /// <summary>
        ///     Testet das Erstellen von Benachrichtigungen der Nutzer an einem Board, wenn das Fälligkeitsdatum einer Karte des
        ///     Boards abgelaufen ist.
        /// </summary>
        [TestMethod]
        public void TestCreateCardNotificationsForDueExpiration() {
            //Given: Ein Board mit mehreren Mitgliedern
            User assignedUser1 = Create.User();
            User assignedUser2 = Create.User();
            User creator = Create.User();

            Board board = Create.Board().Build();

            //Given: Eine Karte am Board, deren Fälligkeitsdatum abgelaufen ist
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Due(DateTime.Now.AddMinutes(5)).WithAssignedUsers(assignedUser1, assignedUser2).CreatedBy(creator);

            Mock<INotificationDao> notificationDaoMock = new Mock<INotificationDao>();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(
                        d => d.FindCardsWithExpiredDueAndWithoutNotifications(It.IsInRange(DateTime.UtcNow.AddMinutes(NotificationService.CARDS_DUE_LIMIT_OFFSET_IN_MINUTES).AddSeconds(-1), DateTime.UtcNow.AddMinutes(NotificationService.CARDS_DUE_LIMIT_OFFSET_IN_MINUTES).AddSeconds(1), Range.Inclusive)))
                    .Returns(new List<Card> { card });
            NotificationService notificationService = new NotificationService(notificationDaoMock.Object, cardDaoMock.Object, null);

            //When: Die Benachrichtigungen für abgelaufene Karten erstellen werden sollen.
            IList<CardNotification> cardNotificationsForDueExpiration = notificationService.CreateCardNotificationsForDueExpiration();

            //Then: Müssen alle Nutzer an dem Board über das Ablaufen der Fälligkeit für die Karte informiert werden
            cardNotificationsForDueExpiration.Should().OnlyContain(n => n.Card.Equals(card));
            cardNotificationsForDueExpiration.Should().OnlyContain(n => n.EmailSend == false);
            cardNotificationsForDueExpiration.Should().OnlyContain(n => n.EmailSendAt == null);
            cardNotificationsForDueExpiration.Select(notification => notification.NotificationFor).Should().BeEquivalentTo(assignedUser1, assignedUser2, creator);

            card.DueExpirationNotificationCreated.Should().BeTrue();
            card.DueExpirationNotificationCreatedAt.Should().BeCloseTo(DateTime.Now, 1000);
        }

        /// <summary>
        ///     Testet das Erstellen einer Benachrichtigung zu einem Kommentar für mehrere Nutzer
        /// </summary>
        [TestMethod]
        public void TestCreateCommentNotificationForMultipleUsers() {
            //Given: Ein Kommentar über dessen Erstellung mehrere Nutzer Benachrichtigt werden sollen.
            User user1 = Create.User();
            User user2 = Create.User();

            Comment comment = Create.Comment().Build();

            Mock<INotificationDao> notificationDaoMock = new Mock<INotificationDao>();
            notificationDaoMock.Setup(d => d.Save(It.Is<IList<Notification>>(list => list.Count == 2)));
            NotificationService notificationService = new NotificationService(notificationDaoMock.Object, null, null);

            //When: Die Benachrichtigungen erstellt werden.
            IList<CommentNotification> createdNotifications = notificationService.CreateCommentNotification(new[] { user1, user2 }, comment, CommentNotificationReason.CommentCreated);

            //Then: Müssen mehrere Benachrichtigungen erstellt werden.
            createdNotifications.Should().HaveCount(2, "Es müssen 2 Benachrichtigungen angelegt werden.");
            createdNotifications.Should().OnlyContain(notification => notification.Comment.Equals(comment), "An allen Benachrichtigungen muss der Kommentar hinterlegt sein.");
            createdNotifications.Should().OnlyContain(notification => notification.NotificationReason == CommentNotificationReason.CommentCreated, "An allen Benachrichtigungen muss der Benachrichtigungs-Anlass korrekt hinterlegt sein.");
            createdNotifications.All(n => n.EmailSend == false).Should().BeTrue("Die Benachrichtigungen dürfen nicht mit dem Flag 'E-Mail-versendet' markiert sein.");
            createdNotifications.All(n => n.EmailSendAt.HasValue == false).Should().BeTrue("Die Benachrichtigungen dürfen nicht mit einem Zeitstempel für das versenden der E-Mail versehen sein.");

            notificationDaoMock.Verify(d => d.Save(It.Is<IList<Notification>>(list => list.Count == 2)), Times.Once);
        }


        /// <summary>
        /// Testet das Versenden von E-Mails für ungelesene Benachrichtigungen.
        /// </summary>
        [TestMethod]
        public void TestSendMailsForUnreadNotifications() {
            //Given: Ein Dao der mehrere ungelesene Benachrichtigungen liefert
            CommentNotification commentNotification = Create.CommentNotification();
            CardNotification cardNotification = Create.CardNotification();

            Mock<INotificationDao> notificationDaoMock = new Mock<INotificationDao>();
            notificationDaoMock.Setup(d => d.FindNotificationWhereToSendEmail()).Returns(new List<Notification> { commentNotification, cardNotification});

            Mock<IEmailNotificationService> emailNotificationServiceMock = new Mock<IEmailNotificationService>();
            emailNotificationServiceMock.Setup(s => s.NotifyUserOnCommentCreated(commentNotification.NotificationFor, commentNotification.Comment));
            emailNotificationServiceMock.Setup(s => s.NotifyUserOnCardDueExpiration(cardNotification.NotificationFor, cardNotification.Card));

            NotificationService notificationService = new NotificationService(notificationDaoMock.Object, null, emailNotificationServiceMock.Object);

            //When: E-Mails für ungelesene Benachrichtigungen versendet werden sollen
            IList<Notification> mailSendFor = notificationService.SendEmailForUnreadNotifications();

            //Then: Müssen für alle ungelesenen Benachrichtigungen E-Mails versendet werden
            mailSendFor.Should().BeEquivalentTo(commentNotification, cardNotification);

            commentNotification.EmailSend.Should().BeTrue();
            cardNotification.EmailSend.Should().BeTrue();

            commentNotification.EmailSendAt.Should().BeCloseTo(DateTime.UtcNow, 1000);
            cardNotification.EmailSendAt.Should().BeCloseTo(DateTime.UtcNow, 1000);

            emailNotificationServiceMock.Verify(s => s.NotifyUserOnCommentCreated(commentNotification.NotificationFor, commentNotification.Comment), Times.Once);
            emailNotificationServiceMock.Verify(s => s.NotifyUserOnCardDueExpiration(cardNotification.NotificationFor, cardNotification.Card), Times.Once);
        }
    }
}