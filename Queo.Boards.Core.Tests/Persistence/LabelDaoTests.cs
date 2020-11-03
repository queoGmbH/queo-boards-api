using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class LabelDaoTests : PersistenceBaseTest {
        public ILabelDao LabelDao { private get; set; }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGet() {
            // Given: 
            Board board = Create.Board().Build();
            Label label = Create.Label().ForBoard(board).Build();

            // When: 
            Label reloaded = LabelDao.Get(label.Id);

            // Then: 
            Assert.AreEqual(reloaded, label);
        }
    }
}