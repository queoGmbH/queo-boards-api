using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class NotificationDaoTests : PersistenceBaseTest {
        /// <summary>
        ///     Legt den zu testenden Dao fest.
        /// </summary>
        public NotificationDao NotificationDao { get; set; }

        public CardDao CardDao { get; set; }

        /// <summary>
        ///     Testet das beim Suchen nach Benachrichtigungen für die eine E-Mail versendet werden muss, keine Benachrichtigungen
        ///     für Nutzer mit ungültiger E-Mail-Adresse gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindNotificationWhereToSendEmailShouldFindNotificationsForUsersWithoutValidEmail() {
            //Given: Nicht gelesene Benachrichtigungen für Nutzer mit E-Mail, bei welcher kein @ enthalten ist.
            User userWithEmailWithoutAt = Create.User().WithEmail("info(at)queo.de");
            Notification unreadNotification1 = Create.CardNotification().MarkedAsRead(false).For(userWithEmailWithoutAt);

            //Given: Nicht gelesene Benachrichtigungen für Nutzer mit E-Mail, bei welcher kein Punkt hinter dem @ ist.
            User userWithEmailWithoutDotAfterAt = Create.User().WithEmail("info.fuer.alle@queode");
            Notification unreadNotification2 = Create.CardNotification().MarkedAsRead(false).For(userWithEmailWithoutDotAfterAt);

            //Given: Nicht gelesene Benachrichtigungen für Nutzer mit E-Mail, bei welcher der Punkt direkt hinter dem @ ist.
            User userWithEmailWithDotDirectlyAfterAt = Create.User().WithEmail("info@.de");
            Notification unreadNotification3 = Create.CardNotification().MarkedAsRead(false).For(userWithEmailWithDotDirectlyAfterAt);

            //Given: Nicht gelesene Benachrichtigungen für Nutzer mit E-Mail, bei welcher der Punkt direkt hinter dem @ ist.
            User userWithEmailEndingWithDot = Create.User().WithEmail("info@queo.");
            Notification unreadNotification4 = Create.CardNotification().MarkedAsRead(false).For(userWithEmailEndingWithDot);

            //When: nach Benachrichtigungen für die eine E-Mail versendet werden muss gesucht wird
            IList<Notification> notificationsWhereMailWasSend = NotificationDao.FindNotificationWhereToSendEmail();

            //Then: Dürfen keine Benachrichtigung gefunden werden
            notificationsWhereMailWasSend.Should().NotContain(unreadNotification1);
            notificationsWhereMailWasSend.Should().NotContain(unreadNotification2);
            notificationsWhereMailWasSend.Should().NotContain(unreadNotification3);
            notificationsWhereMailWasSend.Should().NotContain(unreadNotification4);
        }

        /// <summary>
        ///     Testet das beim Suchen nach Benachrichtigungen für die eine E-Mail versendet werden muss, ungelesene
        ///     Benachrichtigungen gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindNotificationWhereToSendEmailShouldFindUnreadNotifications() {
            //Given: Eine nicht gelesene Benachrichtigung.
            Notification unreadNotification = Create.CardNotification().MarkedAsRead(false);

            //When: nach Benachrichtigungen für die eine E-Mail versendet werden muss gesucht wird
            IList<Notification> notificationsWhereMailWasSend = NotificationDao.FindNotificationWhereToSendEmail();

            //Then: Muss die Benachrichtigung gefunden werden
            notificationsWhereMailWasSend.Should().Contain(unreadNotification);
        }

        /// <summary>
        ///     Testet das beim Suchen nach Benachrichtigungen für die eine E-Mail versendet werden muss, ungelesene
        ///     Benachrichtigungen gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindNotificationWhereToSendEmailShouldNotFindNotificationsWhereMailWasSent() {
            //Given: Eine nicht gelesene Benachrichtigung, für die bereits eine E-Mail versendet wurde.
            Notification unreadNotificationWithSentEmail = Create.CardNotification().MarkedAsRead(true).WithEmailSendAt(DateTime.UtcNow.AddDays(-1));

            //When: nach Benachrichtigungen für die eine E-Mail versendet werden muss gesucht wird
            IList<Notification> notificationsWhereMailWasSend = NotificationDao.FindNotificationWhereToSendEmail();

            //Then: Dürfen keine Benachrichtigungen gefunden werden, wo schon eine E-Mail versendet wurde.
            notificationsWhereMailWasSend.Should().NotContain(unreadNotificationWithSentEmail);
        }

        /// <summary>
        ///     Testet das beim Suchen nach Benachrichtigungen für die eine E-Mail versendet werden muss, ungelesene
        ///     Benachrichtigungen gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindNotificationWhereToSendEmailShouldNotFindReadNotifications() {
            //Given: Eine gelesene Benachrichtigung.
            Notification readNotification = Create.CardNotification().MarkedAsRead(true);

            //When: nach Benachrichtigungen für die eine E-Mail versendet werden muss gesucht wird
            IList<Notification> notificationsWhereMailWasSend = NotificationDao.FindNotificationWhereToSendEmail();

            //Then: Dürfen keine Benachrichtigungen gefunden werden, die schon gelesen wurden.
            notificationsWhereMailWasSend.Should().NotContain(readNotification);
        }

        /// <summary>
        ///     Testet das Speichern und Laden einer Benachrichtigung zu einer Karte.
        /// </summary>
        [TestMethod]
        public void TestSaveCardNotification() {
            //Given: Eine Karte, zu der eine Benachrichtigung erstellt werden soll.
            Card card = Create.Card();
            User user = Create.User();
            DateTime creationDateTime = new DateTime(2017, 07, 07, 07, 07, 07);
            DateTime emailSendAt = new DateTime(2017, 08, 08, 08, 08, 08);

            CardNotification cardNotification = new CardNotification(card, CardNotificationReason.DueExpiration, user, creationDateTime, true, emailSendAt, true);

            //When: Die Benachrichtigung gespeichert und anschließend wieder geladen wird
            NotificationDao.Save(cardNotification);
            NotificationDao.FlushAndClear();
            CardNotification reloaded = NotificationDao.Get(cardNotification.Id) as CardNotification;

            //Then: Müssen alle Informationen korrekt erhalten bleiben.
            reloaded.Should().NotBeNull();
            reloaded.Should().Be(cardNotification);

            reloaded.NotificationFor.Should().Be(user);
            reloaded.NotificationCategory.Should().Be(NotificationCategory.Card);
            reloaded.CreationDateTime.Should().Be(creationDateTime);
            reloaded.EmailSendAt.Should().Be(emailSendAt);
            reloaded.IsMarkedAsRead.Should().BeTrue();
            reloaded.EmailSend.Should().BeTrue();
            reloaded.NotificationReason.Should().Be(CardNotificationReason.DueExpiration);
        }

        /// <summary>
        ///     Testet das Speichern und Laden einer Benachrichtigung zu einem Kommentar.
        /// </summary>
        [TestMethod]
        public void TestSaveCommentNotification() {
            //Given: Ein Kommentar, über dessen Erstellung benachrichtigt werden soll.
            Card card = Create.Card();
            Comment comment = Create.Comment().OnCard(card).Build();
            User user = Create.User();
            DateTime creationDateTime = new DateTime(2017, 07, 07, 07, 07, 07);
            DateTime emailSendAt = new DateTime(2017, 08, 08, 08, 08, 08);

            CommentNotification commentNotification = new CommentNotification(comment, CommentNotificationReason.CommentCreated, user, creationDateTime, true, emailSendAt, true);

            //When: Die Benachrichtigung gespeichert und anschließend wieder geladen wird
            NotificationDao.Save(commentNotification);
            NotificationDao.FlushAndClear();
            CommentNotification reloaded = NotificationDao.Get(commentNotification.Id) as CommentNotification;

            //Then: Müssen alle Informationen korrekt erhalten bleiben.
            reloaded.Should().NotBeNull();
            reloaded.Should().Be(commentNotification);

            reloaded.NotificationFor.Should().Be(user);
            reloaded.NotificationCategory.Should().Be(NotificationCategory.Comment);
            reloaded.CreationDateTime.Should().Be(creationDateTime);
            reloaded.EmailSendAt.Should().Be(emailSendAt);
            reloaded.IsMarkedAsRead.Should().BeTrue();
            reloaded.EmailSend.Should().BeTrue();
            reloaded.NotificationReason.Should().Be(CommentNotificationReason.CommentCreated);
        }


        /// <summary>
        /// Testet, dass beim Suchen nach Benachrichtigungen für einen Nutzer nur die Benachrichtigungen für den entsprechenden Nutzer gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindForUserShouldOnlyReturnNotificationsForTheQueriedUser() {

            //Given: Zwei Nutzer
            User user1 = Create.User();
            User user2 = Create.User();

            //Given: Benachrichtigungen für beide Nutzer
            Notification notification1ForUser1 = Create.CardNotification().For(user1);
            Notification notification2ForUser1 = Create.CardNotification().For(user1);

            Notification notification1ForUser2 = Create.CardNotification().For(user2);
            Notification notification2ForUser2 = Create.CardNotification().For(user2);

            //When: Die Benachrichtigungen für einen der beiden Nutzer abgerufen werden
            IPage<Notification> foundNotifications = NotificationDao.FindForUser(PageRequest.All, user1);

            //Then: Dürfen auch nur die Benachrichtigungen für den angefragten Nutzer geliefert werden.
            foundNotifications.Should().BeEquivalentTo(notification1ForUser1, notification2ForUser1);
        }

        /// <summary>
        /// Testet das Abrufen, ausschließlich ungelesener Benachrichtigungen für einen Nutzer
        /// </summary>
        [TestMethod]
        public void TestFindForUserAndOnlyUnreadNotifications() {

            //Given: Zwei Nutzer
            User user1 = Create.User();
            User user2 = Create.User();

            //Given: Benachrichtigungen für beide Nutzer
            Notification readNotificationForUser1 = Create.CardNotification().For(user1).MarkedAsRead(true);
            Notification unreadNotificationForUser1 = Create.CardNotification().For(user1).MarkedAsRead(false);

            Notification readNotificationForUser2 = Create.CardNotification().For(user2).MarkedAsRead(true);
            Notification unreadNotificationForUser2 = Create.CardNotification().For(user2).MarkedAsRead(false);

            //When: Ungelesene Benachrichtigungen für einen der beiden Nutzer abgerufen  werden sollen
            IPage<Notification> foundNotifications = NotificationDao.FindForUser(PageRequest.All, user1, false);

            //Then: Dürfen auch nur ungelesene Nachrichten dieses Nutzers geliefert werden
            foundNotifications.Should().BeEquivalentTo(unreadNotificationForUser1);

        }

        /// <summary>
        /// Testet das Abrufen, ausschließlich ungelesener Benachrichtigungen für einen Nutzer
        /// </summary>
        [TestMethod]
        public void TestFindForUserAndOnlyReadNotifications() {

            //Given: Zwei Nutzer
            User user1 = Create.User();
            User user2 = Create.User();

            //Given: Benachrichtigungen für beide Nutzer
            Notification readNotificationForUser1 = Create.CardNotification().For(user1).MarkedAsRead(true);
            Notification unreadNotificationForUser1 = Create.CardNotification().For(user1).MarkedAsRead(false);

            Notification readNotificationForUser2 = Create.CardNotification().For(user2).MarkedAsRead(true);
            Notification unreadNotificationForUser2 = Create.CardNotification().For(user2).MarkedAsRead(false);

            //When: Nur gelesene Benachrichtigungen für einen der beiden Nutzer abgerufen  werden sollen
            IPage<Notification> foundNotifications = NotificationDao.FindForUser(PageRequest.All, user1, true);

            //Then: Dürfen auch nur gelesene Nachrichten dieses Nutzers geliefert werden
            foundNotifications.Should().BeEquivalentTo(readNotificationForUser1);

        }

        /// <summary>
        /// Testet das Speichern und Laden einer Benachrichtigung im Zusammenhang mit einer Karte
        /// </summary>
        [TestMethod]
        public void TestSaveAndLoad() {

            //Given: Eine Kartenbenachrichtigung
            CardNotification cardNotification = Create.CardNotification();
            NotificationDao.FlushAndClear();
            CardDao.FlushAndClear();

            //When: Die Karte und die Benachrichtigung geladen werden
            CardNotification reloadedNotification = (CardNotification)NotificationDao.GetByBusinessId(cardNotification.BusinessId);
            Card reloadedCard = CardDao.GetByBusinessId(cardNotification.Card.BusinessId);

            //Then: Müssen sie jeweils referenziert sein
            reloadedNotification.Card.Should().Be(reloadedCard);
            reloadedCard.CardNotifications.Should().Contain(reloadedNotification);

        }
    }
}