using System.Net;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;
using Queo.Boards.Core.Validators.Labels;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class LabelControllerTest : CreateBaseTest {

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUpdateShouldThrowErrorWhenTitleExeedsDefinedLenght() {
            // Given: 
            LabelController controller = new LabelController(null, new LabelDtoValidator());

            // When: 
            ResponseMessageResult badRequest =
                    (ResponseMessageResult)
                    controller.Update(Create.Label().Build(), new LabelDto() { Name = "suöoihf iöuh vibv aeuh febvireybhh", Color = "Blue" });

            // Then: 
            Assert.AreEqual((HttpStatusCode)422, badRequest.Response.StatusCode);
        }
    }
}