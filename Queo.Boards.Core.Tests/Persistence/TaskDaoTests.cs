using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    [TestClass]
    public class TaskDaoTests : PersistenceBaseTest{

        public ITaskDao TaskDao { private get; set; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveGet() {

            // Given: 
            Checklist checklist = Create.Checklist().Build();
            Task task = Create.Task().Title("meine Aufgabe").OnChecklist(checklist).Build();

            // When: 
            Task reloaded = TaskDao.Get(task.Id);

            // Then: 
            Assert.AreEqual(task, reloaded);
            Assert.AreEqual("meine Aufgabe", reloaded.Title);
            Assert.AreEqual(checklist, reloaded.Checklist);
        }
    }
}