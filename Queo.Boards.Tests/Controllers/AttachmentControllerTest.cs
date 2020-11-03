using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;
using Task = System.Threading.Tasks.Task;

namespace Queo.Boards.Tests.Controllers {
    [TestClass]
    public class AttachmentControllerTest : CreateBaseTest {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestDeleteAttachmentOnArchivedCardShouldResultIn404() {
            // Given: 
            Card archivedCard = Create.Card().ArchivedAt(DateTime.UtcNow).Build();
            Document document = Create.Document().Card(archivedCard).Build();

            AttachmentController controller = new AttachmentController(null);

            // When: 
            NotFoundResult nfr = (NotFoundResult)controller.DeleteDocument(document, Create.User());

            // Then: 
            nfr.Should().NotBe(null);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestGetAttachmentOnArchivedCardShouldResultIn404() {
            // Given: 
            Card archivedCard = Create.Card().ArchivedAt(DateTime.UtcNow).Build();
            Document document = Create.Document().Card(archivedCard).Build();

            User user = Create.User();

            AttachmentController controller = new AttachmentController(null);

            // When: 
            HttpResponseMessage httpResponseMessage = controller.DownloadAttachment(document, user);

            // Then: 
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public async Task TestGetAttachmentThumbnailOnArchivedCardShouldResultIn404() {
            // Given: 
            Card archivedCard = Create.Card().ArchivedAt(DateTime.UtcNow).Build();
            Document document = Create.Document().Card(archivedCard).Build();

            User user = Create.User();

            AttachmentController controller = new AttachmentController(null);

            // When: 
            HttpResponseMessage httpResponseMessage = await controller.GetAttachmentThumbnailAsync(document, user, 100, 100);

            // Then: 
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}