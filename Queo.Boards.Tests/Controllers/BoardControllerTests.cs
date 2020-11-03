using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Commands.Boards;
using Queo.Boards.Controllers.Boards;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class BoardControllerTests : CreateBaseTest {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateBoardShouldCallTheServiceAndReturnItsResult() {
            // Given: 
            Board board = Create.Board().Build();

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.Create(It.IsAny<BoardDto>(), It.IsAny<User>())).Returns(board);

            BoardController boardController = new BoardController(boardServiceMock.Object, null, null);

            // When: 
            OkNegotiatedContentResult<BoardSummaryModel> httpActionResult =
                    (OkNegotiatedContentResult<BoardSummaryModel>)boardController.Create(new BoardCreateCommand(), new User("test user", new UserAdministrationDto(), new UserProfileDto()));

            // Then: 
            Assert.AreEqual(board.BusinessId, httpActionResult.Content.BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestFindBoardsForCurrentUser() {
            // Given: 
            Board board = Create.Board().Build();
            User user = Create.User().Build();

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.FindBoardsForUser(PageRequest.All, user, null)).Returns(new Page<Board>(new List<Board> { board }, 1));

            BoardController controller = new BoardController(boardServiceMock.Object, null, null);

            // When: 
            OkNegotiatedContentResult<IList<BoardSummaryModel>> boards = (OkNegotiatedContentResult<IList<BoardSummaryModel>>)controller.FindBoardsForUser(user);

            // Then: 
            boardServiceMock.Verify(x => x.FindBoardsForUser(PageRequest.All, user, null), Times.Once);
            Assert.AreEqual(board.BusinessId, boards.Content[0].BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestFindBoardsForCurrentUserWithQuery() {
            // Given: 
            Board board = Create.Board().Build();
            User user = Create.User().Build();

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            string SEARCH_TERM = "Board";
            boardServiceMock.Setup(x => x.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM)).Returns(new Page<Board>(new List<Board> { board }, 1));

            BoardController controller = new BoardController(boardServiceMock.Object, null, null);

            // When: 
            OkNegotiatedContentResult<IList<BoardSummaryModel>> boards = (OkNegotiatedContentResult<IList<BoardSummaryModel>>)controller.FindBoardsForUser(user, SEARCH_TERM);

            // Then: 
            boardServiceMock.Verify(x => x.FindBoardsForUser(PageRequest.All, user, SEARCH_TERM), Times.Once);
            Assert.AreEqual(board.BusinessId, boards.Content[0].BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetCompleteBoardWithEmptyList() {
            // Given: 
            Board board = Create.Board().Build();
            List list1 = Create.List().OnBoard(board).Build();
            List list2 = Create.List().OnBoard(board).Build();

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(listSetup => listSetup.FindAllListsAndCardsByBoardId(It.IsAny<Guid>())).Returns(new List<List>() {list1, list2});

            BoardController controller = new BoardController(null, null, listServiceMock.Object);

            // When: 
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)controller.GetCompleteBoard(board);

            // Then: 
            Assert.AreEqual(2, result.Content.Lists.Count);
            result.Content.Lists.Select(list => list.BusinessId).Should().BeEquivalentTo(list1.BusinessId, list2.BusinessId);
            Assert.AreEqual(board.BusinessId, result.Content.Summary.BusinessId);
        }

        /// <summary>
        ///     [CWETMNDS-130]
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetCompleteBoardWithListsCardsAndLabels() {
            // Given: 
            Board board = Create.Board().Build();
            List list1 = Create.List().OnBoard(board).Build();
            List list2 = Create.List().OnBoard(board).Build();
            //board.Lists.Add(list1);
            //board.Lists.Add(list2);
            Card card1 = Create.Card().OnList(list1).Build();
            Card card2 = Create.Card().OnList(list1).Build();
            Card card3 = Create.Card().OnList(list2).Build();
            Card card4 = Create.Card().OnList(list2).Build();

            Label label1 = Create.Label().ForBoard(board).Build();
            //board.Labels.Add(label1);

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(listSetup => listSetup.FindAllListsAndCardsByBoardId(It.IsAny<Guid>())).Returns(new List<List>() { list1, list2 });

            BoardController controller = new BoardController(null, null, listServiceMock.Object);

            // When: 
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)controller.GetCompleteBoard(board);

            // Then: 
            Assert.AreEqual(2, result.Content.Lists.Count);
            Assert.AreEqual(board.BusinessId, result.Content.Summary.BusinessId);
            Assert.AreEqual(4, result.Content.Cards.Count);
            Assert.AreEqual(card1.BusinessId, result.Content.Cards.First().BusinessId);
            Assert.AreEqual(1, result.Content.Labels.Count);
            Assert.AreEqual(label1.BusinessId, board.Labels.First().BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetCompleteBoardWithoutArchivedCards() {
            // Given: 
            Board board = Create.Board().Build();
            List list1 = Create.List().Build();
            List list2 = Create.List().Build();
            Card cardOnList1 = Create.Card().ArchivedAt(DateTime.UtcNow).Build();
            list1.Cards.Add(cardOnList1);
            Card cardOnList2 = Create.Card().Build();
            list2.Cards.Add(cardOnList2);
            board.Lists.Add(list1);
            board.Lists.Add(list2);

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(listSetup => listSetup.FindAllListsAndCardsByBoardId(It.IsAny<Guid>())).Returns(new List<List>() { list1, list2 });

            BoardController controller = new BoardController(null, null, listServiceMock.Object);

            // When: 
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)controller.GetCompleteBoard(board);

            // Then: 
            Assert.AreEqual(cardOnList2.BusinessId, result.Content.Cards.Single().BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetCompleteBoardWithoutArchivedLists() {
            // Given: 
            Board board = Create.Board().Build();
            List archived = Create.List().ArchivedAt(DateTime.UtcNow).Build();
            List notArchived = Create.List().Build();
            Card cardOnArchivedList = Create.Card().Build();
            archived.Cards.Add(cardOnArchivedList);
            Card cardOnNormalList = Create.Card().Build();
            notArchived.Cards.Add(cardOnNormalList);
            board.Lists.Add(archived);
            board.Lists.Add(notArchived);

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(listSetup => listSetup.FindAllListsAndCardsByBoardId(It.IsAny<Guid>())).Returns(new List<List>() { archived, notArchived });

            BoardController controller = new BoardController(null, null, listServiceMock.Object);

            // When: 
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)controller.GetCompleteBoard(board);

            // Then: 
            Assert.AreEqual(notArchived.BusinessId, result.Content.Lists.Single().BusinessId);
            Assert.AreEqual(cardOnNormalList.BusinessId, result.Content.Cards.Single().BusinessId);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetCompleteEmptyBoard() {
            // Given: 
            Board board = Create.Board().Build();

            Mock<IListService> listServiceMock = new Mock<IListService>();
            listServiceMock.Setup(listSetup => listSetup.FindAllListsAndCardsByBoardId(It.IsAny<Guid>())).Returns(new List<List>());

            BoardController controller = new BoardController(null, null, listServiceMock.Object);

            // When: 
            OkNegotiatedContentResult<BoardModel> result = (OkNegotiatedContentResult<BoardModel>)controller.GetCompleteBoard(board);

            // Then: 
            Assert.AreEqual(board.BusinessId, result.Content.Summary.BusinessId);
        }
    }
}