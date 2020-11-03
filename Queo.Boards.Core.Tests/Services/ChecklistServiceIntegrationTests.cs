using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class ChecklistServiceIntegrationTests : ServiceBaseTest {

        public IChecklistService ChecklistService { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestCreateChecklistWithCopyTasksFromExisting()
        {

            // Given: 
            Checklist checklist = Create.Checklist().Build();
            string task1Title = "task 1";
            Task task1 = Create.Task().Title(task1Title).OnChecklist(checklist).Build();
            string task2Title = "task 2";
            Task task2 = Create.Task().Title(task2Title).OnChecklist(checklist).Build();

            Card anotherCard = Create.Card().Build();

            // When: 
            Checklist checklistWithItemCopies = ChecklistService.Create(anotherCard, "kopie von checklist", checklist);

            // Then: 
            Assert.AreEqual(2, checklistWithItemCopies.Tasks.Count);
            Assert.AreEqual(task1Title, checklistWithItemCopies.Tasks.First().Title);
            Assert.AreEqual(task2Title, checklistWithItemCopies.Tasks.Last().Title);
        }
        
    }
}