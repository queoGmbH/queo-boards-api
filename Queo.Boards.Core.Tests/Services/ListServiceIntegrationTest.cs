using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class ListServiceIntegrationTest : ServiceBaseTest{

        public IListService ListService { set; private get; }

        public IListDao ListDao { get; set; }

        public IBoardDao BoardDao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestCopyListOnSameBoard() {

            // Given: 
            List list = Create.List().Build();

            // When: 
            List copy = ListService.Copy(list, list.Board, "neuer Name", Create.User());

            // Then: 
            copy.Title.Should().Be("neuer Name");
        }


        /// <summary>
        /// Testet das Kopieren einer Karte und die anschließende Reihenfolge der Karten
        /// </summary>
        [TestMethod]
        public void TestCopyListOnSameBoardOrder() {

            //Given: Ein Board mit mehreren Listen
            Board board = Create.Board().Build();

            List listOnIndex0 = Create.List().OnBoard(board).Build();
            List listOnIndex1 = Create.List().OnBoard(board).Build();
            List listOnIndex2 = Create.List().OnBoard(board).Build();
            List listOnIndex3 = Create.List().OnBoard(board).Build();
            List listOnIndex4 = Create.List().OnBoard(board).Build();
            List listOnIndex5 = Create.List().OnBoard(board).Build();
            List listOnIndex6 = Create.List().OnBoard(board).Build();

            //When: Die Liste an Position 2 kopiert werden soll und an Position 3 eingefügt werden soll
            List copy = ListService.Copy(listOnIndex1, board, "Kopierte Liste", Create.User(), 2);
            ListDao.FlushAndClear();
            List reloadedCopy = ListDao.GetByBusinessId(copy.BusinessId);
            Board reloadedBoard = BoardDao.GetByBusinessId(board.BusinessId);

            //Then: Muss das korrekt funktionieren
            copy.GetPositionOnBoard().Should().Be(2);
            reloadedCopy.GetPositionOnBoard().Should().Be(2);

            reloadedBoard.Lists.Should().BeEquivalentTo(listOnIndex0, listOnIndex1, copy, listOnIndex2, listOnIndex3, listOnIndex4, listOnIndex5, listOnIndex6);

        }


        /// <summary>
        /// Testet das Kopieren einer Liste, auf der eine Karte mit Kommentaren enthalten ist.
        /// </summary>
        [TestMethod]
        public void TestCopyListThatHasOneCardWithComments() {

            //Given: Ein Liste mit einer Karte, zu der zwei Kommentare abgegeben wurden.
            List list = Create.List().Build();
            Card card = Create.Card().OnList(list);
            Comment comment1 = Create.Comment().OnCard(card).Build();
            Comment comment2 = Create.Comment().OnCard(card).Build();
            const string COPY_NAME = "Kopierte Liste";

            //When: Die Liste kopiert wird
            List copy = ListService.Copy(list, list.Board, COPY_NAME, Create.User(), Int32.MaxValue);

            //Then: Müssen die Kommentare an der Liste hängen.
            list.Board.Lists.Should().HaveCount(2);
            copy.Cards.Single().Comments.Should().HaveCount(2);
        }

        /// <summary>
        /// Testet, dass beim Kopieren einer Liste auf ein anderes Board, die Labels und Nutzer an einer Karte entfernt werden 
        /// </summary>
        [TestMethod]
        public void TestCopyListToOtherBoardShouldRemoveLabelsAndUsersFromCards() {

            //Given: Ein Board mit Label, Nutzern und einer Liste an der eine Karte mit Nutzern und Labels hängt
            User user1 = Create.User();
            User user2 = Create.User();
            Board sourceBoard = Create.Board().WithMembers(user1, user2);
            Label label1 = Create.Label().ForBoard(sourceBoard);
            Label label2 = Create.Label().ForBoard(sourceBoard);

            List sourceList = Create.List().OnBoard(sourceBoard);
            Card card = Create.Card().OnList(sourceList).WithLabels(label1, label2).WithAssignedUsers(user1, user2);

            //When: Die Liste auf ein anderes Board kopiert wird
            List copy = ListService.Copy(sourceList, Create.Board().Public().WithMembers(user1), "Kopie", user1);

            //Then: Müssen die Labels und Nutzer an der Karte entfernt werden 
            copy.Cards.Single().AssignedUsers.Should().BeEmpty();
            copy.Cards.Single().Labels.Should().BeEmpty();
        }

        /// <summary>
        /// Testet, dass beim Verschieben einer Liste auf ein anderes Board, die Labels und Nutzer an einer Karte entfernt werden 
        /// CWETMNDS-686
        /// </summary>
        [TestMethod]
        public void TestMoveListToOtherBoardShouldRemoveLabelsAndUsersFromCards() {

            //Given: Ein Board mit Label, Nutzern und einer Liste an der eine Karte mit Nutzern und Labeln hängt
            User user1 = Create.User();
            User user2 = Create.User();
            Board sourceBoard = Create.Board().WithMembers(user1, user2);
            Label label1 = Create.Label().ForBoard(sourceBoard);
            Label label2 = Create.Label().ForBoard(sourceBoard);

            List list = Create.List().OnBoard(sourceBoard);
            Card card = Create.Card().OnList(list).WithLabels(label1, label2).WithAssignedUsers(user1, user2);

            //When: Die Liste auf ein anderes Board verschoben wird
            ListService.MoveList(list, Create.Board().Public().WithMembers(user1));

            //Then: Müssen die Labels und Nutzer an der Karte entfernt werden 
            list.Cards.Single().AssignedUsers.Should().BeEmpty();
            list.Cards.Single().Labels.Should().BeEmpty();
            card.AssignedUsers.Should().BeEmpty();
            card.Labels.Should().BeEmpty();
        }
    }
}