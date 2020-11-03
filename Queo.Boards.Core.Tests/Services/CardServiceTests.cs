using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Queo.Boards.Core.Tests.Asserts;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;
using Spring.Transaction.Interceptor;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class CardServiceTests : CreateBaseTest {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestAddLabelToCard() {
            // Given: 
            Card card = Create.Card().Build();
            Label label = Create.Label().Build();
            ICardService cardService = CreateService.CardService().Build();

            // When: 
            cardService.AddLabel(card, label);

            // Then: 
            Assert.AreEqual(label, card.Labels.Single());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestAddLabelTwiceShouldNotAddItSecondTIme() {
            // Given: 
            Card card = Create.Card().Build();
            Label label = Create.Label().Build();
            ICardService cardService = CreateService.CardService().Build();

            // When: 
            cardService.AddLabel(card, label);
            cardService.AddLabel(card, label);

            // Then: 
            Assert.AreEqual(label, card.Labels.Single());
        }

        /// <summary>
        ///     Testet das Hinzufügen eines weiteren Nutzers zu einer Karte
        /// </summary>
        [TestMethod]
        public void TestAssignAnotherUserToCard() {
            //Given: Eine Karte und mit einem zugewiesenen Nutzer und ein weiterer Nutzer.
            User user = Create.User();
            User anotherUser = Create.User();
            Board board = Create.Board().WithMembers(user).WithMembers(anotherUser).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).WithAssignedUsers(user).Build();

            CardService cardService = CreateService.CardService().Build();

            //When: Der Nutzer der Karte erneut zugewiesen wird
            IList<User> assignUsers = cardService.AssignUser(card, anotherUser);

            //Then: Muss der Nutzer weiterhin genau einmal in der Liste der zugewiesenen Nutzer enthalten sein.
            assignUsers.Should().BeEquivalentTo(user, anotherUser);
            card.AssignedUsers.Should().BeEquivalentTo(user, anotherUser);
        }

        /// <summary>
        ///     Testet das Hinzufügen eines Nutzers, der Mitglied des Boards ist, zu einer Karte
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsBaordMember() {
            //Given: Eine Karte und ein Nutzer der Mitglied des Boards ist und dieser noch nicht zugewiesen ist
            User user = Create.User();
            Board board = Create.Board().WithMembers(user).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Build();

            CardService cardService = CreateService.CardService().Build();

            //When: Der Nutzer der Karte zugewiesen wird
            IList<User> assignUsers = cardService.AssignUser(card, user);

            //Then: Muss der Nutzer in der Liste der zugewiesenen Nutzer enthalten sein.
            assignUsers.Should().BeEquivalentTo(user);
            card.AssignedUsers.Should().BeEquivalentTo(user);
        }

        /// <summary>
        ///     Testet das Hinzufügen eines Nutzers, der Besitzer des Boards ist, zu einer Karte
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsBoardOwner() {
            //Given: Eine Karte und ein Nutzer der dieser noch nicht zugewiesen ist und Besitzer des Boards ist
            User user = Create.User();
            Board board = Create.Board().WithOwners(user).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Build();

            CardService cardService = CreateService.CardService().Build();

            //When: Der Nutzer der Karte zugewiesen wird
            IList<User> assignUsers = cardService.AssignUser(card, user);

            //Then: Muss der Nutzer in der Liste der zugewiesenen Nutzer enthalten sein.
            assignUsers.Should().BeEquivalentTo(user);
            card.AssignedUsers.Should().BeEquivalentTo(user);
        }

        /// <summary>
        ///     Testet das Hinzufügen eines Nutzers zu einer Karte
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsMemberOfAssignedTeam() {
            //Given: Eine Karte und ein Nutzer der dieser noch nicht zugewiesen ist und Mitglied eines Teams ist, welches dem Board zugewiesen ist.
            User user = Create.User();
            Team team = Create.Team().WithMembers(user);
            Board board = Create.Board().WithTeams(team).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Build();

            CardService cardService = CreateService.CardService().Build();

            //When: Der Nutzer der Karte zugewiesen wird
            IList<User> assignUsers = cardService.AssignUser(card, user);

            //Then: Muss der Nutzer in der Liste der zugewiesenen Nutzer enthalten sein.
            assignUsers.Should().BeEquivalentTo(user);
            card.AssignedUsers.Should().BeEquivalentTo(user);
        }

        /// <summary>
        ///     Testet das erneute Hinzufügen eines Nutzers zu einer Karte
        /// </summary>
        [TestMethod]
        public void TestAssignUserSecondTime() {
            //Given: Eine Karte und ein Nutzer der dieser schon zugewiesen ist
            User user = Create.User();
            Board board = Create.Board().WithMembers(user).Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).WithAssignedUsers(user).Build();

            CardService cardService = CreateService.CardService().Build();

            //When: Der Nutzer der Karte erneut zugewiesen wird
            IList<User> assignUsers = cardService.AssignUser(card, user);

            //Then: Muss der Nutzer weiterhin genau einmal in der Liste der zugewiesenen Nutzer enthalten sein.
            assignUsers.Should().BeEquivalentTo(user);
            card.AssignedUsers.Should().BeEquivalentTo(user);
        }

        /// <summary>
        ///     Testet, dass wenn ein Nutzer der nicht Mitglied eines Boards ist, keiner Karte des Boards zugewiessen werden kann.
        /// </summary>
        [TestMethod]
        public void TestAssignUserWhoIsNoBoardMemberToCardShouldThrowException() {
            //Given: Ein Nutzer der nicht Mitglied des Boards einer Karte ist
            User user = Create.User();
            Board board = Create.Board().Build();
            List list = Create.List().OnBoard(board).Build();
            Card card = Create.Card().OnList(list).Build();

            CardService cardService = CreateService.CardService().Build();

            //When: Der Nutzer der Karte hinzugefügt werden soll
            Action action = () => cardService.AssignUser(card, user);

            //Then: Muss eine Exception fliegen.
            action.ShouldThrow<InvalidOperationException>();
        }

        /// <summary>
        ///     Testet das Kopieren einer archivierten Karte
        /// </summary>
        [TestMethod]
        public void TestCopyArchivedCardThrowInvalidOperationException() {
            //Given: Eine archivierte Karte auf einer Liste die kopiert werden soll
            Board board = Create.Board().Build();
            List listOfBoard = Create.List().OnBoard(board).Build();
            Card source = Create.Card().OnList(listOfBoard).ArchivedAt(DateTime.UtcNow);
            Card targetAfter = Create.Card().OnList(listOfBoard).Build();
            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(d => d.Save(It.Is<Card>(c => !c.Equals(source) && !c.Equals(targetAfter))));
            CardService cardService = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte kopiert werden soll.
            Action action = () => cardService.Copy(source, listOfBoard, source.Title, Create.User());

            //Then: Darf das nicht funktionieren.
            action.ShouldThrow<InvalidOperationException>();
            cardDaoMock.Verify(d => d.Save(It.Is<Card>(c => !c.Equals(source) && !c.Equals(targetAfter))), Times.Never);
        }

        /// <summary>
        ///     Testet das Kopieren einer Karte
        /// </summary>
        [TestMethod]
        public void TestCopyCard() {
            //Given: Eine Karte auf einer Liste die kopiert werden soll
            Board board = Create.Board().Build();
            List listOfBoard = Create.List().OnBoard(board).Build();
            Card source = Create.Card().OnList(listOfBoard);
            Card targetAfter = Create.Card().OnList(listOfBoard).Build();
            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(d => d.Save(It.Is<Card>(c => !c.Equals(source) && !c.Equals(targetAfter))));
            CardService cardService = CreateService.CardService().With(cardDaoMock.Object);
            string COPY_TITLE = "Kopie";
            User copier = Create.User();

            //When: Die Karte kopiert werden soll.
            Card copy = cardService.Copy(source, listOfBoard, COPY_TITLE, copier, listOfBoard.Cards.IndexOf(targetAfter));

            //Then: Muss eine Kopie mit allen Eigenschaften außer den angehängten Dokumenten erfolgen.
            cardDaoMock.Verify(d => d.Save(It.Is<Card>(c => !c.Equals(source) && !c.Equals(targetAfter))), Times.Once);

            copy.Title.Should().Be(COPY_TITLE);
            copy.CreatedBy.Should().Be(copier);
            copy.List.Should().Be(listOfBoard);
            listOfBoard.Cards.Should().Contain(copy);
            copy.Description.Should().Be(source.Description);
            copy.Due.Should().Be(source.Due);
            copy.AssignedUsers.Should().BeEquivalentTo(source.AssignedUsers);
            copy.Labels.Should().BeEquivalentTo(source.Labels);
            
        }

        /// <summary>
        ///     Testet, dass beim Kopieren einer Karte auf ein anderes Board die Labels und Nutzer nicht mit kopiert werden.
        /// </summary>
        [TestMethod]
        public void TestCopyCardToListOfOtherBoardShouldNotCopyAssignedUsersAndLabels() {
            //Given: Eine Karte auf einer Liste auf einem Board und eine Liste auf einem anderen Board
            //Given: Die Karte hat einen zugewiesenen Nutzer und ein Label.
            Label label = Create.Label().Build();
            User user = Create.User().Build();

            Board board = Create.Board().Build();
            Board otherBoard = Create.Board().Build();
            List listOfBoard = Create.List().OnBoard(board).Build();
            List listOnOtherBoard = Create.List().OnBoard(otherBoard).Build();
            Card card = Create.Card().OnList(listOfBoard).WithLabels(label).WithAssignedUsers(user);
            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(d => d.Save(It.Is<Card>(c => !c.Equals(card))));

            CardService cardService = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte auf das andere Board kopiert werden soll
            Card copy = cardService.Copy(card, listOnOtherBoard, card.Title, Create.User());

            //Then: Dürfen die Nutzer und Labels nicht mit kopiert werden.
            cardDaoMock.Verify(d => d.Save(It.Is<Card>(c => !c.Equals(card))), Times.Once);
            copy.AssignedUsers.Should().BeEmpty();
            copy.Labels.Should().BeEmpty();
        }

        /// <summary>
        ///     Testet das Kopieren einer Karte mit zugewiesenen Nutzern.
        /// </summary>
        [TestMethod]
        public void TestCopyCardWithAssignedUsers() {
            //Given: Ein Karte mit zwei zugewiesenen Nutzern
            User user1 = Create.User();
            User user2 = Create.User();
            Card card = Create.Card().WithAssignedUsers(user1, user2).Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();

            CardService service = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte kopiert werden soll
            Card copy = service.Copy(card, card.List, card.Title, Create.User());

            //Then: Müssen auch die zugewiesenen Nutzer kopiert werden.
            copy.AssignedUsers.Should().BeEquivalentTo(user1, user2);
        }

        /// <summary>
        ///     Testet das Kopieren einer Karte mit einer Checkliste.
        /// </summary>
        [TestMethod]
        public void TestCopyCardWithChecklist() {
            //Given: Ein Karte mit zwei Checklisten
            Card card = Create.Card().Build();

            Checklist checklist = Create.Checklist().OnCard(card).Build();
            Checklist checklist2 = Create.Checklist().OnCard(card).Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            Mock<IChecklistService> checklistServiceMock = new Mock<IChecklistService>();
            checklistServiceMock.Setup(s => s.Copy(checklist, It.IsAny<Card>())).Returns(new Checklist());
            checklistServiceMock.Setup(s => s.Copy(checklist2, It.IsAny<Card>())).Returns(new Checklist());

            CardService service = CreateService.CardService().With(cardDaoMock.Object).With(checklistServiceMock.Object);

            //When: Die Karte kopiert werden soll
            Card copy = service.Copy(card, card.List, card.Title, Create.User());

            //Then: Müssen auch die Checklisten kopiert werden.
            checklistServiceMock.Verify(s => s.Copy(checklist, It.IsAny<Card>()), Times.Once);
            checklistServiceMock.Verify(s => s.Copy(checklist2, It.IsAny<Card>()), Times.Once);
        }

        /// <summary>
        ///     Testet das Kopieren einer Karte mit Kommentaren.
        /// </summary>
        [TestMethod]
        public void TestCopyCardWithComments() {
            //Given: Ein Karte mit zwei Kommentaren
            Card card = Create.Card().Build();

            Comment comment1 = Create.Comment().OnCard(card).Build();
            Comment comment2 = Create.Comment().OnCard(card).Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            Mock<ICommentService> commentServiceMock = new Mock<ICommentService>();
            commentServiceMock.Setup(s => s.Copy(comment1, It.IsAny<Card>())).Returns(new Comment());
            commentServiceMock.Setup(s => s.Copy(comment2, It.IsAny<Card>())).Returns(new Comment());

            CardService service = CreateService.CardService().With(cardDaoMock.Object).With(commentServiceMock.Object);

            //When: Die Karte kopiert werden soll
            Card copy = service.Copy(card, card.List, card.Title, Create.User());

            //Then: Müssen auch die Kommentare kopiert werden.
            commentServiceMock.Verify(s => s.Copy(comment1, It.IsAny<Card>()), Times.Once);
            commentServiceMock.Verify(s => s.Copy(comment2, It.IsAny<Card>()), Times.Once);
        }

        /// <summary>
        ///     Testet das Kopieren einer Karte mit Labels.
        /// </summary>
        [TestMethod]
        public void TestCopyCardWithLabels() {
            //Given: Ein Karte mit zwei zugeordneten Labels
            Label label1 = Create.Label().Build();
            Label label2 = Create.Label().Build();
            Card card = Create.Card().WithLabels(label1, label2).Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            CardService service = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte kopiert werden soll
            Card copy = service.Copy(card, card.List, card.Title, Create.User());

            //Then: Müssen auch die Label-Zuordnungen kopiert werden.
            copy.Labels.Should().BeEquivalentTo(label1, label2);
        }

        /// <summary>
        ///     Testet das Kopieren einer Karte mit Labels auf ein anderes Board.
        /// </summary>
        [TestMethod]
        public void TestCopyCardWithLabelsToOtherBoard() {
            //Given: Ein Karte mit zwei zugeordneten Labels und eine Liste auf einem anderen Board als die zu kopierende Karte.
            Label label1 = Create.Label().Build();
            Label label2 = Create.Label().Build();
            Card card = Create.Card().WithLabels(label1, label2).Build();
            List listOnOtherBoard = Create.List().Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            CardService service = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte kopiert werden soll
            Card copy = service.Copy(card, listOnOtherBoard, card.Title, Create.User());

            //Then: Dürfen die Label-Zuordnungen NICHT kopiert werden.
            copy.Labels.Should().BeEmpty();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardAtEmptyBoard() {
            // Given: 
            CardDto dto = new CardDto() { Title = "Kartentitel", Description = "Beschreibung" };

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(x => x.Save(It.IsAny<Card>()));

            ICardService cardService = CreateService.CardService().With(cardDaoMock.Object).Build();

            // When: 
            Card created = cardService.Create(Create.List().Build(), dto, new List<User>(), Create.User());

            // Then: 
            cardDaoMock.Verify(x => x.Save(created), Times.Once);
            Assert.AreEqual("Kartentitel", created.Title);
            Assert.AreEqual("Beschreibung", created.Description);
            Assert.AreEqual(0, created.GetPositionInList());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardAtThirdPosition() {
            // Given: 
            CardDto dto = new CardDto() { Title = "Kartentitel", Description = "Beschreibung" };

            List list = Create.List().Build();
            Card card1 = Create.Card().OnList(list).Build();
            Card card2 = Create.Card().OnList(list).Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(x => x.Save(It.IsAny<Card>()));

            ICardService cardService = CreateService.CardService().With(cardDaoMock.Object).Build();

            // When: 
            Card created = cardService.Create(list, dto, new List<User>(), Create.User());

            // Then: 
            cardDaoMock.Verify(x => x.Save(created), Times.Once);
            Assert.AreEqual("Kartentitel", created.Title);
            Assert.AreEqual("Beschreibung", created.Description);
            Assert.AreEqual(2, created.GetPositionInList());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardWithInitialUserNotOnBoard() {
            // Given: 
            User user1 = Create.User();
            User user2 = Create.User();
            Board board = Create.Board().WithMembers(user1).Build();
            List list = Create.List().OnBoard(board).Build();

            CardDto dto = new CardDto();
            Mock<ICardDao> cardDao = new Mock<ICardDao>();
            cardDao.Setup(d => d.Save(It.IsAny<Card>())).Returns<Card>(c => c);

            ICardService cardService = CreateService.CardService().With(cardDao.Object).Build();

            // When: 
            Action action = () => cardService.Create(list, dto, new List<User> { user1, user2 }, Create.User());

            // Then: 
            action.ShouldThrow<InvalidOperationException>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardWithLabels() {
            // Given: 
            Label label = Create.Label().Build();

            List list = Create.List().Build();

            CardDto dto = new CardDto() { AssignedLabels = new List<Label>() { label } };

            ICardService cardService = CreateService.CardService().Build();

            // When: 
            Card created = cardService.Create(list, dto, new List<User>(), Create.User());

            // Then: 
            Assert.AreEqual(1, created.Labels.Count);
            Assert.AreEqual(label, created.Labels.Single());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateCardWithUsers() {
            // Given: 
            User user1 = Create.User();
            User user2 = Create.User();
            Board board = Create.Board().WithMembers(user1, user2).Build();
            List list = Create.List().OnBoard(board).Build();

            CardDto dto = new CardDto();

            ICardService cardService = CreateService.CardService().Build();

            // When: 
            Card created = cardService.Create(list, dto, new List<User> { user1, user2 }, Create.User());

            // Then: 
            created.AssignedUsers.Should().BeEquivalentTo(user1, user2);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestMoveCardBetweenOtherCardsUpButNotToTop() {
            // Given: 
            List list = Create.List().Build();
            Card topCard = Create.Card().OnList(list).Position(0).Build();
            Card card1 = Create.Card().OnList(list).Position(1).Build();
            Card card2 = Create.Card().OnList(list).Position(2).Build();
            Card bottomCard = Create.Card().OnList(list).Position(3).Build();

            ICardService listService = CreateService.CardService().Build();

            // When: 
            List listWithMovedCards = listService.MoveCard(card2, list, list.Cards.IndexOf(topCard) + 1);

            // Then: 
            card1.GetPositionInList().Should().Be(2);
            card2.GetPositionInList().Should().Be(1);
        }

        /// <summary>
        /// Testet das Verschieben einer Karte auf ein anderes Board
        /// </summary>
        [TestMethod]
        public void TestMoveCardToOtherBoard() {

            //Given: Eine Karte in einer Liste auf einem Board und eine andere Liste auf einem anderen Board
            Board sourceBoard = Create.Board().Build();
            List sourceList = Create.List().OnBoard(sourceBoard).Build();
            Card card = Create.Card().OnList(sourceList);

            List targetList = Create.List().Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(d => d.Save(It.IsAny<Card>()));

            CardService cardService = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte von der einen Liste / dem einen Board auf die andere Liste / das andere Board verschoben werden soll
            cardService.MoveCard(card, targetList, 0);

            //Then: Muss die Karte korrekt verschoben werden
            cardDaoMock.Verify(d => d.Save(It.IsAny<Card>()), Times.Never);
            card.List.Should().Be(targetList);
        }

        /// <summary>
        /// Testet das beim Verschieben einer Karte auf ein anderes Board die Labels entfernt werden
        /// </summary>
        [TestMethod]
        public void TestMoveCardToOtherBoardShouldRemoveLabels() {

            //Given: Eine Karte mit Labels in einer Liste auf einem Board und eine andere Liste auf einem anderen Board
            Board sourceBoard = Create.Board().Build();
            List sourceList = Create.List().OnBoard(sourceBoard).Build();
            Label label1OnSourceBoard = Create.Label().ForBoard(sourceBoard).Build();
            Label label2OnSourceBoard = Create.Label().ForBoard(sourceBoard).Build();
            Card card = Create.Card().OnList(sourceList).WithLabels(label1OnSourceBoard, label2OnSourceBoard);

            List targetList = Create.List().Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(d => d.Save(It.IsAny<Card>()));

            CardService cardService = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte von der einen Liste / dem einen Board auf die andere Liste / das andere Board verschoben werden soll
            cardService.MoveCard(card, targetList, 0);

            //Then: Müssen die Label-Zuordnungen aufgehoben werden
            card.Labels.Should().BeEmpty();
        }

        /// <summary>
        /// Testet das beim Verschieben einer Karte auf ein anderes Board die Nutzer entfernt werden
        /// </summary>
        [TestMethod]
        public void TestMoveCardToOtherBoardShouldRemoveUsers() {

            //Given: Eine Karte mit Nutzern in einer Liste auf einem Board und eine andere Liste auf einem anderen Board
            User boardUser1 = Create.User();
            User boardUser2 = Create.User();
            Board sourceBoard = Create.Board().WithMembers(boardUser1, boardUser2).Build();
            List sourceList = Create.List().OnBoard(sourceBoard).Build();
            Card card = Create.Card().OnList(sourceList).WithAssignedUsers(boardUser1, boardUser2);

            List targetList = Create.List().Build();

            Mock<ICardDao> cardDaoMock = new Mock<ICardDao>();
            cardDaoMock.Setup(d => d.Save(It.IsAny<Card>()));

            CardService cardService = CreateService.CardService().With(cardDaoMock.Object);

            //When: Die Karte von der einen Liste / dem einen Board auf die andere Liste / das andere Board verschoben werden soll
            cardService.MoveCard(card, targetList, 0);

            //Then: Müssen die Nutzer-Zuordnungen aufgehoben werden
            card.AssignedUsers.Should().BeEmpty();
        }


        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestRemoveLabel() {
            // Given: 
            Card card = Create.Card().Build();
            Label label = Create.Label().Build();
            card.AddLabel(label);
            ICardService cardService = CreateService.CardService().Build();

            // When: 
            cardService.RemoveLabel(card, label);

            // Then: 
            Assert.AreEqual(0, card.Labels.Count);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestRemoveLabelNotAssignedShouldNotThrowAnything() {
            // Given: 
            Label label = Create.Label().Build();
            Card card = Create.Card().WithLabels(label).Build();
            ICardService cardService = CreateService.CardService().Build();

            // When: 
            cardService.RemoveLabel(card, label);

            // Then: 
            Assert.AreEqual(0, card.Labels.Count);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateArchivedIsTransactional() {
            Expression<Action> action = () => new CardService(null, null, null).UpdateArchived(default(Card), default(bool));
            action.ShouldHaveAttribute<TransactionAttribute>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateDescription() {
            // Given: 
            Card card = Create.Card().Build();
            ICardService cardService = CreateService.CardService().Build();

            // When: 
            Card updated = cardService.UpdateDescription(card, "new description");

            // Then: 
            Assert.AreEqual("new description", updated.Description);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateDue() {
            // Given: 
            Card card = Create.Card().Build();
            ICardService cardService = CreateService.CardService().Build();
            DateTime newDue = new DateTime(2017, 04, 05);

            // When: 
            Card updated = cardService.UpdateDue(card, newDue);

            // Then: 
            updated.Due.Should().Be(newDue.ToUniversalTime());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateIsArchivated() {
            // Given: 
            bool isArchivated = true;

            Mock<Card> cardMock = new Mock<Card>();
            cardMock.Setup(x => x.Archive(It.IsAny<DateTime>()));

            ICardService cardService = CreateService.CardService().Build();

            // When: 
            Card updatedCard = cardService.UpdateArchived(cardMock.Object, isArchivated);

            // Then: 
            cardMock.Verify(x => x.Archive(It.IsAny<DateTime>()), Times.Once);
            Assert.AreEqual(cardMock.Object.BusinessId, updatedCard.BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateTitle() {
            // Given: 
            Card card = Create.Card().Build();
            ICardService cardService = CreateService.CardService().Build();

            // When: 
            Card updated = cardService.UpdateTitle(card, "new title");

            // Then: 
            Assert.AreEqual("new title", updated.Title);
        }

        /// <summary>
        /// Testet das Entfernen einen zugewiesenen Nutzers von einer Karte
        /// </summary>
        [TestMethod]
        public void TestUnassignUser() {
            //Given: Eine Karte mit zwei zugewiesenen Nutzern
            User user1 = Create.User();
            User user2 = Create.User();
            Card card = Create.Card().WithAssignedUsers(user1, user2);

            CardService cardService = CreateService.CardService().Build();

            //When: Einer der beiden Nutzer von der Karte entfernt werden soll 
            IList<User> remainingAssignedUsers = cardService.UnassignUsers(card, user1);

            //Then: Darf nur noch der andere Nutzer der Karte zugewiesen sein.
            remainingAssignedUsers.Should().BeEquivalentTo(user2);
            card.AssignedUsers.Should().BeEquivalentTo(user2);
        }


        /// <summary>
        /// Testet das beim Entfernen einen Nutzers von einer Karte, der sowieso nicht zugewiesen ist, einfach nichts passiert.
        /// </summary>
        [TestMethod]
        public void TestUnassignUserThatIsNotAssignedShouldNotDoAnything() {
            //Given: Eine Karte mit zwei zugewiesenen Nutzern und ein anderer Nutzer, welcher der Karte nicht zugewiesen ist
            User user1 = Create.User();
            User user2 = Create.User();
            User unassignedUser = Create.User();
            Card card = Create.Card().WithAssignedUsers(user1, user2);

            CardService cardService = CreateService.CardService().Build();

            //When: Der nicht-zugewiesene Nutzer von der Karte entfernt werden soll 
            IList<User> remainingAssignedUsers = cardService.UnassignUsers(card, unassignedUser);

            //Then: Darf nichts passieren und alles muss so bleiben wie es ist.
            remainingAssignedUsers.Should().BeEquivalentTo(user1, user2);
            card.AssignedUsers.Should().BeEquivalentTo(user1, user2);
        }


        /// <summary>
        /// Testet, dass beim Ändern des Fälligkeitsdatums einer Karte, die Information, dass bereits eine Benachrichtigung versendet wurde, zurückgesetzt wird
        /// </summary>
        [TestMethod]
        public void TestUpdateCardsDueShouldResetNotificationCreated() {

            //Given: Eine Karte mit einem Fälligkeitsdatum, über dessen Ablauf bereits eine Benachrichtigung erfolgte.
            Card card = Create.Card().Due(DateTime.Now.AddDays(-1)).WithDueNotificationDoneAt(DateTime.Now.AddDays(-1));

            CardService cardService = CreateService.CardService().Build();

            //When: Das Fälligkeitsdatum der Karte geändert wird
            cardService.UpdateDue(card, DateTime.Now.AddDays(1));

            //Then: Muss die Kennzeichnung, ob und wann die Benachrichtigung erfolgte, zurückgesetzt werden.
            card.DueExpirationNotificationCreated.Should().BeFalse();
            card.DueExpirationNotificationCreatedAt.Should().BeNull();
        }

        /// <summary>
        /// Testet, dass beim Ändern des Fälligkeitsdatums einer Karte, die Information, dass bereits eine Benachrichtigung versendet wurde, NICHT zurückgesetzt wird, wenn sich das Fälligkeitsdatum nicht ändert.
        /// </summary>
        [TestMethod]
        public void TestUpdateCardsDueShouldNotResetNotificationCreatedIfDueIsUnchanged() {

            //Given: Eine Karte mit einem Fälligkeitsdatum, über dessen Ablauf bereits eine Benachrichtigung erfolgte.
            DateTime dueDate = DateTime.Now.AddDays(-1);
            DateTime dueExpirationNotificationAt = DateTime.Now.AddDays(-1).AddMinutes(-10).ToUniversalTime();
            Card card = Create.Card().Due(dueDate).WithDueNotificationDoneAt(dueExpirationNotificationAt);

            CardService cardService = CreateService.CardService().Build();

            //When: Die Update-Methode aufgerufen, das Fälligkeitsdatum der Karte aber NICHT geändert wird
            cardService.UpdateDue(card, dueDate);

            //Then: Muss die Kennzeichnung, ob und wann die Benachrichtigung erfolgte, unverändert bleiben.
            card.DueExpirationNotificationCreated.Should().BeTrue();
            card.DueExpirationNotificationCreatedAt.Should().Be(dueExpirationNotificationAt);
        }


        /// <summary>
        /// Testet, dass man keine Karten Hinzufügen kann, wenn die Liste auf der die Karte angelegt werden soll, archiviert ist.
        /// </summary>
        [TestMethod]
        public void TestAddCardShouldNotBeAllowedWhenListIsArchived() {
            //Given: Ein archivierte Liste
            List archivedList = Create.List().ArchivedAt(DateTime.UtcNow);

            //When: Eine Karte auf der Liste erstellt werden soll
            Action action = () => CreateService.CardService().Build().Create(archivedList, new CardDto("neue Karte auf archivierter Liste", "", null, new List<Label>()), new List<User>(), Create.User());

            //Then: Darf das nicht funktionieren
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Testet, dass man keine Karten hinzufügen kann, wenn die Liste auf der die Karte angelegt werden soll, auf einem archivierten Board ist.
        /// </summary>
        [TestMethod]
        public void TestAddCardShouldNotBeAllowedWhenListIsOnArchivedBoard() {
            //Given: Ein Liste auf einem archivierten Board
            Board archivedBoard = Create.Board().ArchivedAt(DateTime.UtcNow);
            List listOnArchivedBoard = Create.List().OnBoard(archivedBoard);

            //When: Eine Karte auf der Liste erstellt werden soll
            Action action = () => CreateService.CardService().Build().Create(listOnArchivedBoard, new CardDto("neue Karte auf archiviertem Board", "", null, new List<Label>()), new List<User>(), Create.User());

            //Then: Darf das nicht funktionieren
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Testet, dass man auf Vorlagen keine Karten hinzufügen kann.
        /// </summary>
        [TestMethod]
        public void TestAddCardShouldNotBeAllowedOnTemplate() {
            //Given: Ein Liste in einer Vorlage
            Board template = Create.Board().AsTemplate();
            List listOnTemplate = Create.List().OnBoard(template);

            //When: Eine Karte auf der Liste erstellt werden soll
            Action action = () => CreateService.CardService().Build().Create(listOnTemplate, new CardDto("neue Karte auf Vorlage", "", null, new List<Label>()), new List<User>(), Create.User());

            //Then: Darf das nicht funktionieren
            action.ShouldThrow<ArgumentOutOfRangeException>();
        }
    }
}