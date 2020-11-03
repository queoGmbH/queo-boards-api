using System.Linq;
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
    public class TaskServiceTest : CreateBaseTest {
        
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateTask() {

            // Given: 
            Checklist checklist = Create.Checklist().Build();
            string taskName = "testtask";

            Mock<ITaskDao> taskDaoMock = new Mock<ITaskDao>();
            taskDaoMock.Setup(x => x.Save(It.IsAny<Task>()));

            ITaskService taskService = new TaskService(taskDaoMock.Object);

            // When: 
            Task task = taskService.Create(checklist, taskName);

            // Then: 
            taskDaoMock.Verify(x=>x.Save(task), Times.Once);
            Assert.AreEqual(taskName, task.Title);
        }


        /// <summary>
        /// Testet das Kopieren eines Tasks innerhalb einer Checkliste
        /// </summary>
        [TestMethod]
        public void TestCopyTask() {

            //Given: Ein bestehender Task an einer Checkliste.
            Checklist checklist = Create.Checklist().Build();
            Task task = Create.Task().OnChecklist(checklist).Build();

            Mock<ITaskDao> taskDaoMock = new Mock<ITaskDao>();
            taskDaoMock.Setup(d => d.Save(It.IsAny<Task>()));
            TaskService taskService = new TaskService(taskDaoMock.Object);

            //When: Der Task kopiert werden soll
            Task copy = taskService.Copy(checklist, task);

            //Then: Muss eine exakte Kopie des Tasks am Ende der Checkliste erstellt werden
            taskDaoMock.Verify(d => d.Save(It.IsAny<Task>()), Times.Once);
            checklist.Tasks.Should().BeEquivalentTo(task, copy);
            checklist.Tasks.Last().Should().Be(copy);
            copy.Should().NotBe(task);
            copy.Title.Should().Be(task.Title);
            copy.IsDone.Should().Be(task.IsDone);
        }

        /// <summary>
        /// Testet das Kopieren eines Tasks an eine andere Checkliste
        /// </summary>
        [TestMethod]
        public void TestCopyTaskOtherChecklist() {

            //Given: Ein bestehender Task an einer Checkliste.
            Checklist checklistSource = Create.Checklist().Build();
            Checklist checklistTarget = Create.Checklist().Build();
            Task task = Create.Task().OnChecklist(checklistSource).Build();

            Mock<ITaskDao> taskDaoMock = new Mock<ITaskDao>();
            taskDaoMock.Setup(d => d.Save(It.IsAny<Task>()));
            TaskService taskService = new TaskService(taskDaoMock.Object);

            //When: Der Task kopiert und der anderen Checkliste angehängt werden soll
            Task copy = taskService.Copy(checklistTarget, task);

            //Then: Muss eine exakte Kopie des Tasks am Ende der anderen Checkliste erstellt werden
            taskDaoMock.Verify(d => d.Save(It.IsAny<Task>()), Times.Once);
            checklistTarget.Tasks.Single().Should().Be(copy);
            checklistTarget.Tasks.Should().Contain(copy);
            copy.Should().NotBe(task);
            copy.Title.Should().Be(task.Title);
            copy.IsDone.Should().Be(task.IsDone);
        }
    }
}