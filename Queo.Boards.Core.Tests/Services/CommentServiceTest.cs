using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Notifications;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {

    [TestClass]
    public class CommentServiceTest : ServiceBaseTest{


        /// <summary>
        /// Testet das Kopieren eines Kommentars.
        /// </summary>
        [TestMethod]
        public void TestCopyComment() {
            //Given: Ein Kommentar auf einer Karte und eine andere Karte
            Comment sourceComment = Create.Comment().Build();
            Card targetCard = Create.Card().Build();

            Mock<ICommentDao> commentDaoMock = new Mock<ICommentDao>();
            CommentService commentService = new CommentService(commentDaoMock.Object, null);

            //When: der Kommentar kopiert werden soll
            Comment copy = commentService.Copy(sourceComment, targetCard);

            //Then: Müssen alle Eigenschaften übernommen werden, auch der Ersteller und das Erstellungsdatum.
            copy.Should().NotBe(sourceComment);
            copy.Card.Should().Be(targetCard);
            targetCard.Comments.Should().Contain(copy);
            sourceComment.Card.Comments.Should().NotContain(copy);
            copy.Creator.Should().Be(sourceComment.Creator);
            copy.CreationDate.Should().BeCloseTo(DateTime.UtcNow, 1000);
            copy.Text.Should().Be(sourceComment.Text);
        }

        /// <summary>
        /// Testet das beim Kopieren eines Kommentars, das Erstellungsdatum auf das aktuelle Datum gesetzt wird.
        /// 
        /// CWETMNDS-570
        /// </summary>
        [TestMethod]
        public void TestCopyCommentShouldResetCreationDate() {

            //Given: Ein Kommentar
            Comment comment = Create.Comment().CreationDate(new DateTime(2015,01,02,03,04,05)).Build();

            Mock<ICommentDao> commentDaoMock = new Mock<ICommentDao>();
            CommentService commentService = new CommentService(commentDaoMock.Object, null);

            //When: Der Kommentar kopiert wird
            Comment copy = commentService.Copy(comment, Create.Card());

            //Then: Muss das Erstellungsdatum angepasst werden
            copy.CreationDate.Should().BeCloseTo(DateTime.UtcNow, 1000);

        }

        /// <summary>
        /// Testet, dass beim Erstellen eines Kommentars, die Nutzer, die auf die Karte lauschen, benachrichtigt werden.
        /// </summary>
        [TestMethod]
        public void TestCreateCommentShouldNotifyCardMembers() {

            //Given: Eine Karte, an der 2 Nutzer angemeldet sind.
            string commentText = "Ein Kommentar, über den 2 Nutzer benachrichtigt werden müssen.";

            User user1 = Create.User();
            User user2 = Create.User();

            Card cardWithUsers = Create.Card().WithAssignedUsers(user1, user2);
            Mock<ICommentDao> commentDaoMock = new Mock<ICommentDao>();
            Mock<INotificationService> notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(s => s.CreateCommentNotification(new List<User> { user1, user2 }, It.Is<Comment>(c => c.Text == commentText), CommentNotificationReason.CommentCreated));

            CommentService commentService = new CommentService(commentDaoMock.Object, notificationServiceMock.Object);

            //When: Ein Kommentar zur Karte erstellt wird.
            commentService.Create(cardWithUsers, commentText, Create.User());

            //Then: Müssen für die beiden Nutzer 2 Benachrichtigungen erstellt werden.
            notificationServiceMock.Verify(s => s.CreateCommentNotification(new List<User> { user1, user2 }, It.Is<Comment>(c => c.Text == commentText), CommentNotificationReason.CommentCreated), Times.Once);

        }


        /// <summary>
        /// Testet, dass der Ersteller eines Kommentars, nicht über seinen eigenen neuen Kommentar benachrichtigt wird, wenn er an der Karate angemeldet ist.
        /// </summary>
        [TestMethod]
        public void TestCreateCommentShouldNotNotifyCommentCreator() {

            //Given: Ein Nutzer der an einer Karte angemeldet ist.
            User userAtCardCreatingComment = Create.User();
            Card card = Create.Card().WithAssignedUsers(userAtCardCreatingComment);

            Mock<ICommentDao> commentDaoMock = new Mock<ICommentDao>();
            Mock<INotificationService> notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(s => s.CreateCommentNotification(It.Is<IList<User>>(l => l.Contains(userAtCardCreatingComment)), It.IsAny<Comment>(), CommentNotificationReason.CommentCreated));


            //When: Der Nutzer einen Kommentar an der Karte abgibt 
            new CommentService(commentDaoMock.Object, notificationServiceMock.Object).Create(card, "Mein eigener Kommentar", userAtCardCreatingComment);

            //Then: Darf er nicht darüber benachrichtigt werden
            notificationServiceMock.Verify(s => s.CreateCommentNotification(It.Is<IList<User>>(l => l.Contains(userAtCardCreatingComment)), It.IsAny<Comment>(), CommentNotificationReason.CommentCreated), Times.Never);

        }

    }
}