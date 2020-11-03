using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Commands.Cards;
using Queo.Boards.Commands.Lists;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Services;

namespace Queo.Boards.Tests.Controllers {

    [TestClass]
    public class MoveControllerTest : ControllerBaseTest{


        /// <summary>
        /// Testet das Verschieben einer Liste innerhalb eines Boards, wenn die Liste hinter sich selbst geschoben wird.
        /// </summary>
        [TestMethod]
        public void TestMoveListToPositionAfterItself() {

            //Given: Ein Board mit mehreren Listen.
            Board board = Create.Board().Build();

            List listAtIndex0 = Create.List().OnBoard(board).Position(0).ArchivedAt(DateTime.UtcNow).Build();
            List listAtIndex1 = Create.List().OnBoard(board).Position(1).Build();
            List listAtIndex2 = Create.List().OnBoard(board).Position(2).Build();
            List listAtIndex3 = Create.List().OnBoard(board).Position(3).Build();

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(s => s.MoveList(listAtIndex1, board, 2));

            //When: Die Liste auf dem Board verschoben werden soll
            new MoveController(listServiceMock.Object, null, null).MoveList(board, new MoveListCommand() {Source = listAtIndex1, InsertAfter = listAtIndex2});

            //Then: Muss die neue Position korrekt berechnet werden, indem die zu verschiebende Liste selbst nicht berücksichtigt wird.
            listServiceMock.Verify(s => s.MoveList(listAtIndex1, board, 2), Times.Once);
        }


        /// <summary>
        /// Testet das Verschieben einer Liste innerhalb eines Boards, wenn die Liste vor sich selbst geschoben wird.
        /// </summary>
        [TestMethod]
        public void TestMoveListToPositionBeforeItself() {

            //Given: Ein Board mit mehreren Listen.
            Board board = Create.Board().Build();

            List listAtIndex0 = Create.List().OnBoard(board).Position(0).ArchivedAt(DateTime.UtcNow).Build();
            List listAtIndex1 = Create.List().OnBoard(board).Position(1).Build();
            List listAtIndex2 = Create.List().OnBoard(board).Position(2).Build();
            List listAtIndex3 = Create.List().OnBoard(board).Position(3).Build();

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(s => s.MoveList(listAtIndex2, board, 1));

            //When: Die Liste auf dem Board verschoben werden soll
            new MoveController(listServiceMock.Object, null, null).MoveList(board, new MoveListCommand() { Source = listAtIndex2, InsertAfter = listAtIndex0 });

            //Then: Muss die neue Position korrekt berechnet werden.
            listServiceMock.Verify(s => s.MoveList(listAtIndex2, board, 1), Times.Once);
        }


        /// <summary>
        /// Testet das Verschieben einer Karte innerhalb einer Liste, hinter sich selbst
        /// </summary>
        [TestMethod]
        public void TestMoveCardOnSameListToPositionBehindItself() {

            //Given: Ein Liste mit vielen Karten
            List list = Create.List().Build();

            Card cardAtIndex0 = Create.Card().OnList(list).Position(0).ArchivedAt(DateTime.UtcNow).Build();
            Card cardAtIndex1 = Create.Card().OnList(list).Position(1).Build();
            Card cardAtIndex2 = Create.Card().OnList(list).Position(2).Build();
            Card cardAtIndex3 = Create.Card().OnList(list).Position(3).Build();

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.MoveCard(cardAtIndex1, list, 2));


            //When: Die Karte auf der Liste verschoben werden soll

            new MoveController(null, null, cardServiceMock.Object).MoveCard(list, new MoveCardCommand() { Source = cardAtIndex1, InsertAfter = cardAtIndex2 });

            //Then: Muss die neue Position korrekt berechnet werden, in dem die zu verschiebende Karte bei der Positionsermittlung nicht mit berücksichtigt wird.
            cardServiceMock.Verify(s => s.MoveCard(cardAtIndex1, list, 2), Times.Once());

        }


        /// <summary>
        /// Testet das Verschieben einer Karte innerhalb einer Liste, vor sich selbst
        /// </summary>
        [TestMethod]
        public void TestMoveCardOnSameListToPositionBeforeItself() {

            //Given: Ein Liste mit vielen Karten
            List list = Create.List().Build();

            Card cardAtIndex0 = Create.Card().OnList(list).Position(0).ArchivedAt(DateTime.UtcNow).Build();
            Card cardAtIndex1 = Create.Card().OnList(list).Position(1).Build();
            Card cardAtIndex2 = Create.Card().OnList(list).Position(2).Build();
            Card cardAtIndex3 = Create.Card().OnList(list).Position(3).Build();

            Mock<ICardService> cardServiceMock = new Mock<ICardService>();
            cardServiceMock.Setup(s => s.MoveCard(cardAtIndex2, list, 1));


            //When: Die Karte auf der Liste verschoben werden soll
            new MoveController(null, null, cardServiceMock.Object).MoveCard(list, new MoveCardCommand() { Source = cardAtIndex2, InsertAfter = cardAtIndex0 });

            //Then: Muss die neue Position korrekt berechnet werden.
            cardServiceMock.Verify(s => s.MoveCard(cardAtIndex2, list, 1), Times.Once());

        }
    }
}