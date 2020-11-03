using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {

    [TestClass]
    public class NotificationServiceIntegrationTest : ServiceBaseTest {

        public INotificationService NotificationService { get; set; }

        /// <summary>
        /// Testet die Benachrichtigung, wenn das Fälligkeitsdatum einer Karte abläuft.
        /// </summary>
        [TestMethod]
        public void TestNotifyOnCardsDueExpiration() {

            //Given: 3 Nutzer
            User user1 = Create.User();
            User user2 = Create.User();
            User user3 = Create.User();

            //Given: Eine abgelaufene Karte an der 2 der 3 Nutzer angemeldet sind, für die noch keine Benachrichtigung erstellt wurde
            Card expiredCard1WithoutCreatedNotifications =
                    Create.Card()
                        .CreatedBy(user1)
                        .WithAssignedUsers(user1, user2)
                        .Due(new DateTime(2017, 07, 07, 07, 07, 07));


            //Given: Eine in 10 Minuten ablaufende Karte an der 1 der 3 Nutzer angemeldet sind, für die noch keine Benachrichtigung erstellt wurde
            Card expiredCard2WithoutCreatedNotifications =
                    Create.Card()
                        .WithAssignedUsers(user3)
                        .CreatedBy(user1)
                        .Due(DateTime.UtcNow.AddMinutes(10));

            //Given: Eine NICHT abgelaufene Karte an der alle 3 Nutzer angemeldet sind, für die noch keine Benachrichtigung erstellt wurde
            Card notExpiredCardWithoutCreatedNotifications =
                    Create.Card()
                        .WithAssignedUsers(user1, user2, user3)
                        .Due(DateTime.UtcNow.AddDays(1));

            //Given: Eine abgelaufene Karte an der alle 3 Nutzer angemeldet sind, für die aber schon eine Benachrichtigung erstellt wurde
            Card expiredCardWithCreatedNotifications =
                    Create.Card()
                        .WithAssignedUsers(user1, user2, user3)
                        .Due(DateTime.UtcNow.AddMinutes(-10))
                        .WithDueNotificationDoneAt(DateTime.UtcNow.AddMinutes(-10));


            //When: Die Benachrichtigung für das Ablaufen einer Karte erstellt werden soll
            IList<CardNotification> createdNotifications = NotificationService.CreateCardNotificationsForDueExpiration();
            
            //Then: Müssen für die Nutzer Benachrichtigungen erstellt werden.
            createdNotifications.Should().HaveCount(4);

            createdNotifications.Should().Contain(n => n.Card.Equals(expiredCard1WithoutCreatedNotifications) && n.NotificationFor.Equals(user1), "Benachrichtigung für angemeldeten Nutzer und Ersteller der 1. ausgelaufenen Karte");
            createdNotifications.Should().Contain(n => n.Card.Equals(expiredCard1WithoutCreatedNotifications) && n.NotificationFor.Equals(user2), "Benachrichtigung für angemeldeten Nutzer der 1. ausgelaufenen Karte");

            createdNotifications.Should().Contain(n => n.Card.Equals(expiredCard2WithoutCreatedNotifications) && n.NotificationFor.Equals(user3), "Benachrichtigung für angemeldeten Nutzer der 2. ausgelaufenen Karte");
            createdNotifications.Should().Contain(n => n.Card.Equals(expiredCard2WithoutCreatedNotifications) && n.NotificationFor.Equals(user1), "Benachrichtigung für Ersteller der 2. ausgelaufenen Karte");

        }

    }
}