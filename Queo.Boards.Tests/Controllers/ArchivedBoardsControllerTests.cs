using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Controllers.Boards;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class ArchivedBoardsControllerTests : CreateBaseTest {

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateIsArchivedCallsService() {
            // Given: 
            Board board = Create.Board().Build();

            Mock<IBoardService> boardServiceMock = new Mock<IBoardService>();
            boardServiceMock.Setup(x => x.UpdateIsArchived(board, true)).Returns(board);

            ArchivedBoardController controller = new ArchivedBoardController(boardServiceMock.Object);

            // When: 
            OkNegotiatedContentResult<BoardSummaryModel> result = (OkNegotiatedContentResult<BoardSummaryModel>)controller.ArchiveBoard(board);

            // Then: 
            boardServiceMock.Verify(x => x.UpdateIsArchived(board, true), Times.Once);
            Assert.AreEqual(result.Content.BusinessId, board.BusinessId);
        }

    }
}