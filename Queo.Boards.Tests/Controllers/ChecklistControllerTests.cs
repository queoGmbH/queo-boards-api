using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class ChecklistControllerTests : CreateBaseTest{
        
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateTitle() {

            // Given: 
            StringValueDto newTitle = new StringValueDto() {Value = "newTitle"};
            Checklist checklist = Create.Checklist().Title(newTitle.Value).Build();

            Mock<IChecklistService> checklistServiceMock = new Mock<IChecklistService>();
            checklistServiceMock.Setup(x => x.UpdateTitle(checklist, newTitle.Value)).Returns(checklist);

            ChecklistController controller = new ChecklistController(checklistServiceMock.Object, null);

            // When: 
            OkNegotiatedContentResult<ChecklistModel> result = (OkNegotiatedContentResult<ChecklistModel>)controller.UpdateTitle(checklist, newTitle);

            // Then: 
            checklistServiceMock.Verify(x=>x.UpdateTitle(checklist, checklist.Title));
            Assert.AreEqual(newTitle.Value, result.Content.Title);
        }
    }
}