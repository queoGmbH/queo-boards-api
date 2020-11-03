using System;
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
    public class ListServiceTests : CreateBaseTest {
        /// <summary>
        ///     Testet das Kopieren einer archivierten Liste
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCopyArchivedListThrowsInvalidOperationException() {
            //Given: Eine archivierte Liste die kopiert werden soll
            Board board = Create.Board().Build();
            List sourceList = Create.List().OnBoard(board).ArchivedAt(DateTime.UtcNow).Build();

            const string COPY_NAME = "Kopie einer Liste";

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(d => d.Save(It.IsAny<List>()));
            ListService listService = new ListService(listDaoMock.Object, null, null);

            //When: Eine Kopie der Liste erstellt werden soll
            Action action = () => listService.Copy(sourceList, board, COPY_NAME, Create.User());

            //Then: Muss eine neue Liste mit denselben Werten erzeugt werden.
            listDaoMock.Verify(d => d.Save(It.IsAny<List>()), Times.Never());
            action.ShouldThrow<InvalidOperationException>();
        }

        /// <summary>
        ///     Testet das Kopieren einer Liste
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCopyList() {
            //Given: Eine Liste die kopiert werden soll
            Board board = Create.Board().Build();
            List sourceList = Create.List().OnBoard(board).Build();
            List targetList = Create.List().OnBoard(board).Build();
            List listAfterTargetList = Create.List().OnBoard(board).Build();
            const string COPY_NAME = "Kopie einer Liste";

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(d => d.Save(It.Is<List>(l => !l.Equals(sourceList) && !l.Equals(targetList))));
            ListService listService = new ListService(listDaoMock.Object, null, null);

            //When: Eine Kopie der Liste erstellt werden soll
            List copy = listService.Copy(sourceList, board, COPY_NAME, Create.User(), board.Lists.IndexOf(targetList) + 1);

            //Then: Muss eine neue Liste mit denselben Werten erzeugt werden.
            listDaoMock.Verify(d => d.Save(It.Is<List>(l => !l.Equals(sourceList) && !l.Equals(targetList))), Times.Once);
            copy.Title.Should().Be(COPY_NAME);
            copy.Board.Should().Be(board);
            copy.GetPositionOnBoard().Should().Be(2);
        }

        /// <summary>
        ///     Testet das Kopieren einer Liste auf ein anderes Board.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCopyListToOtherBoard() {
            //Given: Eine Liste die kopiert werden soll und eine weitere Liste, die sich auf einem anderen Board befindet hinter der die Kopie eingefügt werden soll.
            Board sourceBoard = Create.Board().Build();
            Board targetBoard = Create.Board().Build();
            List sourceList = Create.List().OnBoard(sourceBoard).Build();

            List listBeforeTarget = Create.List().OnBoard(targetBoard).Position(0).Build();
            List targetList = Create.List().OnBoard(targetBoard).Position(1).Build();
            List listAfterTarget = Create.List().OnBoard(targetBoard).Position(2).Build();
            const string COPY_NAME = "Kopie einer Liste";

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(d => d.Save(It.Is<List>(l => !l.Equals(sourceList) && !l.Equals(targetList))));
            ListService listService = new ListService(listDaoMock.Object, null, null);

            //When: Eine Kopie der Liste erstellt werden soll
            List copy = listService.Copy(sourceList, targetBoard, COPY_NAME, Create.User(), targetBoard.Lists.IndexOf(targetList) + 1);

            //Then: Muss eine neue Liste mit denselben Werten erzeugt werden.
            listDaoMock.Verify(d => d.Save(It.Is<List>(l => !l.Equals(sourceList) && !l.Equals(targetList))), Times.Once);
            copy.Title.Should().Be(COPY_NAME);
            copy.Board.Should().Be(targetBoard);
            copy.GetPositionOnBoard().Should().Be(2);
        }

        /// <summary>
        ///     Testet das Kopieren einer Liste mit Karten
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCopyListWithCards() {
            //Given: Eine Liste mit Karten die kopiert werden soll
            Board board = Create.Board().Build();
            List sourceList = Create.List().OnBoard(board).Build();
            Card card1OnSourceList = Create.Card().OnList(sourceList).WithTitle("Karte 1");
            Card card2OnSourceList = Create.Card().OnList(sourceList).WithTitle("Karte 2");
            Card archivedCardOnSourceList = Create.Card().OnList(sourceList).WithTitle("Archivierte Karte").ArchivedAt(DateTime.UtcNow);
            User copier = Create.User();
            const string COPY_NAME = "Kopie einer Liste";

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(d => d.Save(It.Is<List>(l => !l.Equals(sourceList))));
            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.Copy(card1OnSourceList, It.IsAny<List>(), card1OnSourceList.Title, copier, 0)).Returns<Card, List, string, User, int>((sourceCard, targetList, title, user, position) => {
                Card cardCopy = new Card();
                targetList.Cards.Add(cardCopy);
                return cardCopy;
            });
            cardServiceMock.Setup(s => s.Copy(card2OnSourceList, It.IsAny<List>(), card2OnSourceList.Title, copier, 1)).Returns<Card, List, string, User, int>((sourceCard, targetList, title, user, position) => {
                Card cardCopy = new Card();
                targetList.Cards.Add(cardCopy);
                return cardCopy;
            });

            ListService listService = new ListService(listDaoMock.Object, null, cardServiceMock.Object);

            //When: Eine Kopie der Liste erstellt werden soll
            List copy = listService.Copy(sourceList, board, COPY_NAME, copier);

            //Then: Müssen alle Karten der Quell-Liste kopiert werden, außer den archivierten.
            copy.Cards.Should().HaveCount(2);
            cardServiceMock.Verify(s => s.Copy(card1OnSourceList, It.IsAny<List>(), card1OnSourceList.Title, copier, 0), Times.Once);
            cardServiceMock.Verify(s => s.Copy(card2OnSourceList, It.IsAny<List>(), card2OnSourceList.Title, copier, 1), Times.Once);
            cardServiceMock.Verify(s => s.Copy(archivedCardOnSourceList, It.IsAny<List>(), It.IsAny<string>(), It.IsAny<User>(), It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// Testet, dass beim Kopieren einer Liste die Reihenfolge erhalten bleibt.
        /// U.a. als Fehler im Jira gemeldet: CWETMNDS-462
        /// </summary>
        [TestMethod]
        public void TestCopyListShouldKeepOrderOfCards() {

            //Given: Eine zu kopierende Liste mit mehreren Karten
            List list = Create.List().Build();
            Card card1 = Create.Card().WithTitle("1. Karte").OnList(list);
            Card card2 = Create.Card().WithTitle("2. Karte").OnList(list);
            Card card3 = Create.Card().WithTitle("3. Karte").OnList(list);
            Card card4 = Create.Card().WithTitle("4. Karte").OnList(list);
            User copier = Create.User();

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(d => d.Save(It.IsAny<List>()));

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            // TODO: Das mit den Mocks ist irgendwie nicht schön. Sollte refactored werden.
            cardServiceMock.Setup(s => s.Copy(card1, It.IsAny<List>(), card1.Title, copier, 0)).Returns<Card, List, string, User, int>((card, cardList, title, user, position) => {
                Card card1Copy = new Card(list, title, card.Description, card.Due, card.Labels, new EntityCreatedDto(copier, DateTime.Now));
                cardList.Cards.Add(card1Copy);
                return card1Copy;
            });
            cardServiceMock.Setup(s => s.Copy(card2, It.IsAny<List>(), card2.Title, copier, 1)).Returns<Card, List, string, User, int>((card, cardList, title, user, position) => {
                Card card2Copy = new Card(list, title, card.Description, card.Due, card.Labels, new EntityCreatedDto(copier, DateTime.Now));
                cardList.Cards.Add(card2Copy);
                return card2Copy;
            });
            cardServiceMock.Setup(s => s.Copy(card3, It.IsAny<List>(), card3.Title, copier, 2)).Returns<Card, List, string, User, int>((card, cardList, title, user, position) => {
                Card card3Copy = new Card(list, title, card.Description, card.Due, card.Labels, new EntityCreatedDto(copier, DateTime.Now));
                cardList.Cards.Add(card3Copy);
                return card3Copy;
            });
            cardServiceMock.Setup(s => s.Copy(card4, It.IsAny<List>(), card4.Title, copier, 3)).Returns<Card, List, string, User, int>((card, cardList, title, user, position) => {
                Card card4Copy = new Card(list, title, card.Description, card.Due, card.Labels, new EntityCreatedDto(copier, DateTime.Now));
                cardList.Cards.Add(card4Copy);
                return card4Copy;
            });

            ListService listService = new ListService(listDaoMock.Object, null, cardServiceMock.Object);

            //When: Die Liste kopiert wird
            List copy = listService.Copy(list, list.Board, "Kopierte Liste", copier);

            //Then: Muss die Reihenfolge erhalten bleiben
            cardServiceMock.Verify(s => s.Copy(card1, It.IsAny<List>(), card1.Title, copier, 0), Times.Once);
            cardServiceMock.Verify(s => s.Copy(card2, It.IsAny<List>(), card2.Title, copier, 1), Times.Once);
            cardServiceMock.Verify(s => s.Copy(card3, It.IsAny<List>(), card3.Title, copier, 2), Times.Once);
            cardServiceMock.Verify(s => s.Copy(card4, It.IsAny<List>(), card4.Title, copier, 3), Times.Once);

        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateListAtEmptyBoard() {
            // Given: 
            Board board = Create.Board().Build();

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(x => x.Save(It.IsAny<List>()));

            // When: 
            IListService listService = new ListService(listDaoMock.Object, null, null);
            List created = listService.Create(board, "new list name");

            // Then: 
            listDaoMock.Verify(x => x.Save(It.IsAny<List>()), Times.Once);
            Assert.AreEqual(board, created.Board);
            Assert.AreEqual("new list name", created.Title);
            Assert.AreEqual(0, created.GetPositionOnBoard());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateListAtEndOfBoard() {
            // Given: 
            Board board = Create.Board().Build();
            List existingListOnBoard1 = Create.List().OnBoard(board).Build();
            List existingListOnBoard2 = Create.List().OnBoard(board).Build();

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(x => x.Save(It.IsAny<List>()));

            // When: 
            IListService listService = new ListService(listDaoMock.Object, null, null);
            List created = listService.Create(board, "new list name");

            // Then: 
            listDaoMock.Verify(x => x.Save(It.IsAny<List>()), Times.Once);
            Assert.AreEqual(board, created.Board);
            Assert.AreEqual("new list name", created.Title);
            Assert.AreEqual(2, created.GetPositionOnBoard());
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateListShouldCallDao() {
            // Given: 
            Board board = Create.Board().Build();

            Mock<IListDao> listDaoMock = new Mock<IListDao>();
            listDaoMock.Setup(x => x.Save(It.IsAny<List>()));

            // When: 
            IListService listService = new ListService(listDaoMock.Object, null, null);
            List created = listService.Create(board, "new list name");

            // Then: 
            listDaoMock.Verify(x => x.Save(It.IsAny<List>()), Times.Once);
            Assert.AreEqual(board, created.Board);
            Assert.AreEqual("new list name", created.Title);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestDoubleMoveListBehindAnotherOne() {
            // Given: 
            Board board = Create.Board().Build();
            List listToMove = Create.List().Position(0).OnBoard(board).Build();
            List anotherList = Create.List().Position(1).OnBoard(board).Build();
            List targetList = Create.List().Position(2).OnBoard(board).Build();

            ListService listService = new ListService(null, null, null);

            // When: 
            Board boardWithMovedList = listService.MoveList(listToMove, board, board.Lists.IndexOf(targetList) + 1);

            // Then: 
            board.Lists.Count.Should().Be(3);
            boardWithMovedList.Lists.Last().Should().Be(listToMove);
            listToMove.GetPositionOnBoard().Should().Be(2);

            // When:
            boardWithMovedList = listService.MoveList(anotherList, board, board.Lists.IndexOf(listToMove) + 1);
            boardWithMovedList.Lists.Last().Should().Be(anotherList);
            anotherList.GetPositionOnBoard().Should().Be(2);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestMoveListBehindAnotherOne() {
            // Given: 
            Board originBoard = Create.Board().Build();
            List listToMove = Create.List().OnBoard(originBoard).Build();
            Board targetBoard = Create.Board().Build();
            List listOnTargetBoard = Create.List().OnBoard(targetBoard).Build();

            ListService listService = new ListService(null, null, null);

            // When: 
            Board boardWithMovedList = listService.MoveList(listToMove, targetBoard, targetBoard.Lists.IndexOf(listOnTargetBoard) + 1);

            // Then: 
            originBoard.Lists.Count.Should().Be(0);
            targetBoard.Lists.Count.Should().Be(2);
            boardWithMovedList.Lists.Last().Should().Be(listToMove);
            listToMove.GetPositionOnBoard().Should().Be(1);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestMoveListRequiresBoard() {
            // Given: 
            List listToMove = Create.List().Build();
            ListService listService = new ListService(null, null, null);

            // When: 
            Action moveWithoutTargetBoard = () => listService.MoveList(listToMove, null);

            // Then: 
            moveWithoutTargetBoard.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestMoveListWithoutTarget() {
            // Given: 
            Board originBoard = Create.Board().Build();
            List listToMove = Create.List().OnBoard(originBoard).Build();
            Board targetBoard = Create.Board().Build();

            ListService listService = new ListService(null, null, null);

            // When: 
            Board boardWithMovedList = listService.MoveList(listToMove, targetBoard);

            // Then: 
            originBoard.Lists.Count.Should().Be(0);
            targetBoard.Lists.Count.Should().Be(1);
            boardWithMovedList.Lists.Single().Should().Be(listToMove);
            listToMove.GetPositionOnBoard().Should().Be(0);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestMoveWithoutCardShouldThrowException() {
            // Given: 
            // When: 
            Action moveWithoutCard = () => new CardService(null, null, null).MoveCard(null, null);

            // Then: 
            moveWithoutCard.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestMoveWithoutTargetListShouldThrowException() {
            // Given: 
            Card card = Create.Card().Build();

            // When: 
            Action moveWithoutList = () => new CardService(null, null, null).MoveCard(card, null);

            // Then: 
            moveWithoutList.ShouldThrow<ArgumentNullException>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateArchivedShouldBeTransactional() {
            Expression<Action> action = () => new ListService(null, null, null).UpdateArchived(default(List), true);
            action.ShouldHaveAttribute<TransactionAttribute>();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateArchivedShouldCallUpdateOnDomain() {
            // Given: 
            Mock<List> listMock = new Mock<List>();
            listMock.Setup(x => x.Archive(It.IsAny<DateTime>()));

            IListService listService = new ListService(null, null, null);

            // When: 
            List updatedList = listService.UpdateArchived(listMock.Object, true);

            // Then: 
            listMock.Verify(x => x.Archive(It.IsAny<DateTime>()), Times.Once);
            Assert.AreEqual(updatedList, listMock.Object);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateTitle() {
            // Given: 
            List listToUpdate = Create.List().Build();

            Mock<IListDao> listDaoMock = new Mock<IListDao>();

            IListService listService = new ListService(listDaoMock.Object, null, null);

            // When: 
            List updated = listService.Update(listToUpdate, "listenname");

            // Then: 
            Assert.AreEqual("listenname", updated.Title);
        }

        /// <summary>
        /// Testet das beim Verschieben einer Liste auf ein anderes Board die Labels entfernt werden
        /// </summary>
        [TestMethod]
        public void TestMoveListToOtherBoardShouldRemoveLabelsFromCards() {

            //Given: Eine Liste mit Labels 
            Board sourceBoard = Create.Board().Build();
            Label label1 = Create.Label().ForBoard(sourceBoard).Build();
            Label label2 = Create.Label().ForBoard(sourceBoard).Build();
            Board targetBoard = Create.Board().Build();
            List listToMove = Create.List().Build();
            Card card1OnListToMove = Create.Card().OnList(listToMove).WithLabels(label1, label2);
            Card card2OnListToMove = Create.Card().OnList(listToMove).WithLabels(label1);
            Card card3OnListToMove = Create.Card().OnList(listToMove);

            ListService listService = new ListService(null, null, null);

            //When: Die Liste auf ein anderes Board verschoben werden soll
            Board returnedTargetBoard = listService.MoveList(listToMove, targetBoard);

            //Then: Müssen die Labels von der Liste entfernt werden.
            returnedTargetBoard.Labels.Should().BeEmpty();
            sourceBoard.Labels.Should().BeEquivalentTo(new [] {label1, label2}, "Die Labels dürfen nicht vom Ausgangsboard entfernt werden.");
            listToMove.Cards.All(c => !c.Labels.Any()).Should().BeTrue("Die Labels müssen von den Karten entfernt wurden sein.");
            card1OnListToMove.Labels.Should().BeEmpty();
            card2OnListToMove.Labels.Should().BeEmpty();
            card3OnListToMove.Labels.Should().BeEmpty();
        }
        
    }
}