using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class CardServiceIntegrationTest : ServiceBaseTest {
        public IBoardDao BoardDao { get; set; }

        public ICardDao CardDao { get; set; }

        public ICardService CardService { get; set; }

        public IListDao ListDao { get; set; }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestMoveCardBetweenOtherCards() {
            // Given: 
            List originList = Create.Card().WithTitle("Alte Liste der Karte").Build().List;
            Card cardToMove = Create.Card().OnList(originList).Build();
            List targetList = Create.Card().WithTitle("Neue Liste der Karte").Build().List;

            Card firstCardOnOtherList = Create.Card().OnList(targetList).Position(0).Build();
            Card lastCardOnOtherList = Create.Card().OnList(targetList).Position(1).Build();

            // When: 
            List listWithMovedCard = CardService.MoveCard(cardToMove, targetList, 1);
            ListDao.FlushAndClear();
            CardDao.FlushAndClear();

            // Then: 
            listWithMovedCard.Cards.Should().Contain(cardToMove);
            originList.Cards.Should().NotContain(cardToMove);
            firstCardOnOtherList.GetPositionInList().Should().Be(0);
            cardToMove.GetPositionInList().Should().Be(1);
            lastCardOnOtherList.GetPositionInList().Should().Be(2);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestMoveCardToListStart() {
            // Given: 
            List listWithCard = Create.Card().Build().List;
            Card cardToMove = Create.Card().OnList(listWithCard).Build();
            Checklist checklist = Create.Checklist().OnCard(cardToMove).Build();
            List targetList = Create.Card().Build().List;

            // When: 
            List listWithMovedCard = CardService.MoveCard(listWithCard.Cards.Last(), targetList, 0);
            ListDao.FlushAndClear();
            CardDao.FlushAndClear();

            // Then: 
            Assert.AreEqual(2, targetList.Cards.Count);
            Assert.AreEqual(1, listWithCard.Cards.Count);
            Card movedCard = listWithMovedCard.Cards.First();
            Card existingCard = listWithMovedCard.Cards.Last();
            Assert.AreEqual(cardToMove, movedCard);
            Assert.AreEqual(0, movedCard.GetPositionInList());
            Assert.AreEqual(1, existingCard.GetPositionInList());
        }

        /// <summary>
        ///     Testet das Verschieben einer Karte auf ein anderes Board
        /// </summary>
        [TestMethod]
        public void TestMoveCardToOtherBoard() {
            //Given: Eine Karte in einer Liste auf einem Board und eine andere Liste auf einem anderen Board
            Board sourceBoard = Create.Board().Build();
            List sourceList = Create.List().Build();
            Card card = Create.Card().OnList(sourceList).WithLabels(Create.Label().ForBoard(sourceBoard).Build(), Create.Label().ForBoard(sourceBoard).Build());

            List targetList = Create.List().Build();

            //When: Die Karte von der einen Liste / dem einen Board auf die andere Liste / das andere Board verschoben werden soll
            CardService.MoveCard(card, targetList, 0);
            CardDao.FlushAndClear();

            Board reloadedSourceBoard = BoardDao.GetByBusinessId(sourceBoard.BusinessId);
            Card reloadedMovedCard = CardDao.GetByBusinessId(card.BusinessId);

            //Then: Müssen die Label-Zuordnungen aufgehoben werden
            card.Labels.Should().BeEmpty();
            reloadedMovedCard.Labels.Should().BeEmpty();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestMoveCardWithinSameList() {
            // Given: 
            List list = Create.List().Build();
            Card card1 = Create.Card().OnList(list).Build();
            Card card2 = Create.Card().Position(1).OnList(list).Build();

            // When: 
            List listWithMovedCard = CardService.MoveCard(card1, list, 2);
            ListDao.FlushAndClear();
            CardDao.FlushAndClear();

            // Then: 
            Assert.AreEqual(0, card2.GetPositionInList());
            Assert.AreEqual(1, card1.GetPositionInList());
            Assert.AreEqual(list, listWithMovedCard);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestMoveCartToEmptyList() {
            // Given: 
            List listWithCard = Create.List().Build();
            Card cardToMove = Create.Card().OnList(listWithCard).Build();
            Checklist checklist = Create.Checklist().OnCard(cardToMove).Build();
            List emptyList = Create.List().Build();

            // When: 
            List targetList = CardService.MoveCard(cardToMove, emptyList);
            ListDao.FlushAndClear();
            CardDao.FlushAndClear();

            // Then: 
            Assert.AreEqual(1, targetList.Cards.Count);
            Assert.AreEqual(0, listWithCard.Cards.Count);
            Assert.AreEqual(cardToMove, targetList.Cards.Single());
            Assert.AreEqual(0, targetList.Cards.Single().GetPositionInList());
        }

        /// <summary>
        ///     Testet das Entfernen einen zugewiesenen Nutzers von einer Karte
        /// </summary>
        [TestMethod]
        public void TestUnassignUser() {
            //Given: Eine Karte mit zwei zugewiesenen Nutzern
            User user1 = Create.User();
            User user2 = Create.User();
            Card card = Create.Card().WithAssignedUsers(user1, user2);

            //When: Einer der beiden Nutzer von der Karte entfernt werden soll 
            IList<User> remainingAssignedUsers = CardService.UnassignUsers(card, user1);
            CardDao.FlushAndClear();

            //Then: Darf nur noch der andere Nutzer der Karte zugewiesen sein.
            remainingAssignedUsers.Should().BeEquivalentTo(user2);
            card.AssignedUsers.Should().BeEquivalentTo(user2);
        }

        /// <summary>
        /// Testet das Kopieren von Karten mit Kommentaren.
        /// CWETMNDS-514
        /// </summary>
        [TestMethod]
        public void TestCopyCardWithComments() {

            //Given: Eine Karte mit Kommentaren
            Card card = Create.Card();
            Comment comment1 = Create.Comment().OnCard(card).Build();
            Comment comment2 = Create.Comment().OnCard(card).Build();

            //When: Die Karte kopiert wird
            Card copy = CardService.Copy(card, card.List, "Kopie", Create.User());

            //Then: Müssen die Kommentare direkt an der Karte hängen und nicht erst nach dem Neuladen
            copy.Comments.Should().HaveCount(2);

        }
    }
}