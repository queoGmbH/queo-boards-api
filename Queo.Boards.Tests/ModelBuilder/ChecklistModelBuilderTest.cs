using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Models.Builders;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.ModelBuilder {
    [TestClass]
    public class ChecklistModelBuilderTest : CreateBaseTest {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateModelWithoutTaskShouldResultInEmptyTaskListButNotNull() {
            // Given: 
            Checklist checklist = Create.Checklist().Build();

            // When: 
            ChecklistModel checklistModel = ChecklistModelBuilder.Build(checklist);

            // Then: 
            Assert.IsNotNull(checklistModel.Tasks);
            Assert.AreEqual(0, checklistModel.Tasks.Count);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateModelWithTaskShouldContainTaskInChecklistModel() {
            // Given: 
            Checklist checklist = Create.Checklist().Build();
            Task task = Create.Task().OnChecklist(checklist).Build();

            // When: 
            ChecklistModel checklistModel = ChecklistModelBuilder.Build(task.Checklist);

            // Then: 
            Assert.AreEqual(1, checklistModel.Tasks.Count);
        }
    }
}