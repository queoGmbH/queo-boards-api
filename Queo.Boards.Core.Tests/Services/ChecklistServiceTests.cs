using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class ChecklistServiceTests : CreateBaseTest {


        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreate() {
            // Given: 
            Card card = Create.Card().Build();

            Mock<IChecklistDao> checklistDaoMock = new Mock<IChecklistDao>();
            checklistDaoMock.Setup(x => x.Save(It.IsAny<Checklist>()));

            Mock<ITaskService> taskServiceMock = new Mock<ITaskService>();
            taskServiceMock.Setup(s => s.Create(It.IsAny<Checklist>(), It.IsAny<string>())).Returns<Checklist, string>((cl, s) => new Task(cl, s));

            IChecklistService checklistService = new ChecklistService(checklistDaoMock.Object, taskServiceMock.Object);

            string title = "Neue Checklist";

            // When: 
            Checklist created = checklistService.Create(card, title, null);

            // Then: 
            Assert.AreEqual(title, created.Title);
            Assert.AreEqual(card, created.Card);
            card.Checklists.Should().Contain(created);
            checklistDaoMock.Verify(x => x.Save(It.Is<Checklist>(c => c.Title == title && c.Card == card)), Times.Once);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateTitle() {

            // Given: 
            Checklist checklist = Create.Checklist().Title("altername").Build();
            IChecklistService service = new ChecklistService(null, null);
            string newTitle = "neuername";

            // When: 
            Checklist updated = service.UpdateTitle(checklist, newTitle);

            // Then: 
            Assert.AreEqual(updated.Title, newTitle);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateWithChecklistCopy() {

            // Given: 
            Checklist checklistToCopy = Create.Checklist().Build();
            Card card = Create.Card().Build();
            Task task1 = Create.Task().Title("task 1").OnChecklist(checklistToCopy).Build();
            Task task2 = Create.Task().Title("task 2").OnChecklist(checklistToCopy).Build();
            
            Mock<IChecklistDao> checklistDaoMock = new Mock<IChecklistDao>();
            checklistDaoMock.Setup(x => x.GetByBusinessId(checklistToCopy.BusinessId)).Returns(checklistToCopy);
            checklistDaoMock.Setup(x => x.Save(It.IsAny<Checklist>()));

            Mock<ITaskService> taskServiceMock = new Mock<ITaskService>();
            taskServiceMock.Setup(s => s.Create(It.IsAny<Checklist>(), task1.Title)).Returns<Checklist, string>((cl, s) => new Task(cl, s));
            taskServiceMock.Setup(s => s.Create(It.IsAny<Checklist>(), task2.Title)).Returns<Checklist, string>((cl, s) => new Task(cl, s));

            IChecklistService checklistService = new ChecklistService(checklistDaoMock.Object, taskServiceMock.Object);
            string title = "checklist aus kopie";

            // When: 
            Checklist created = checklistService.Create(card, title, checklistToCopy);

            // Then: 
            taskServiceMock.Verify(s => s.Create(It.IsAny<Checklist>(), task1.Title), Times.Once);
            taskServiceMock.Verify(s => s.Create(It.IsAny<Checklist>(), task2.Title), Times.Once);
            created.Title.Should().Be(title);
        }

        /// <summary>
        /// Testet das kopieren einer Checkliste mit 2 Tasks, von denen einer erledigt ist und der andere nicht
        /// </summary>
        [TestMethod]
        public void TestCopyChecklist() {

            //Given: Eine Checkliste mit einem erledigten und einem unerledigten Task
            Checklist sourceList = Create.Checklist().Build();
            Card targetCard = Create.Card().Build();

            Task undoneTask = Create.Task().OnChecklist(sourceList).Build();
            Task doneTask = Create.Task().OnChecklist(sourceList).IsDone().Build();

            Mock<IChecklistDao> checklistDaoMock = new Mock<IChecklistDao>();
            checklistDaoMock.Setup(d => d.Save(It.IsAny<Checklist>()));

            Mock<ITaskService> taskServiceMock = new Mock<ITaskService>();
            taskServiceMock.Setup(s => s.Copy(It.IsAny<Checklist>(), undoneTask)).Returns<Checklist, Task>((checklist, task) => Create.Task().OnChecklist(checklist).Title(task.Title).IsDone(task.IsDone).Build());
            taskServiceMock.Setup(s => s.Copy(It.IsAny<Checklist>(), doneTask)).Returns<Checklist, Task>((checklist, task) => Create.Task().OnChecklist(checklist).Title(task.Title).IsDone(task.IsDone).Build());

            ChecklistService checklistService = new ChecklistService(checklistDaoMock.Object, taskServiceMock.Object);

            //When: Die Checkliste kopiert werden soll
            Checklist copy = checklistService.Copy(sourceList, targetCard);

            //Then: Muss eine exakte Kopie mit ebenfalls einem erledigten und einem unerledigtem Task erstellt werden.
            copy.Title.Should().Be(sourceList.Title);
            copy.Card.Should().Be(targetCard);
            copy.Tasks.Should().HaveCount(2);

            taskServiceMock.Verify(s => s.Copy(It.IsAny<Checklist>(), doneTask), Times.Once());
            taskServiceMock.Verify(s => s.Copy(It.IsAny<Checklist>(), undoneTask), Times.Once());
        }
    }
}