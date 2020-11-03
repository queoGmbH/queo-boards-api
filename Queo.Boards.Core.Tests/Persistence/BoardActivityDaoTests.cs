using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Domain.Activities;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Builders.Activities;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class BoardActivityDaoTests : PersistenceBaseTest{

        public IBoardActivityDao BoardActivityDao { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory("integration")]
        public void TestSaveAndGetBoardActivity() {

            // Given: 
            Board board = Create.Board().Build();
            BoardActivity boardActivity = ((BoardActivityBuilder)Create.BoardActivity().ForBoard(board).Creator(board.CreatedBy)).Build();

            // When: 
            BoardActivity reloaded = BoardActivityDao.Get(boardActivity.Id);

            // Then: 
            Assert.AreEqual(board, reloaded.Board);
        }
    }
}