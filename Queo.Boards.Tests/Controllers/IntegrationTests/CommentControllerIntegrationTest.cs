using System.Web.Http.Results;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Controllers;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.Controllers.IntegrationTests {
    [TestClass]
    public class CommentControllerIntegrationTest : ServiceBaseTest{

        /// <summary>
        /// Setzt einen <see cref="ICommentService"/>
        /// </summary>
        public ICommentService CommentService { set; private get; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestMarkACommentAsDeletedShouldNotReturnOriginalCommentTextInResult() {

            // Given: 
            Comment comment = Create.Comment().WithText("Mit nem Kommentartext").Build();

            CommentController commentController = new CommentController(CommentService);

            // When: 
            OkNegotiatedContentResult<CommentModel> commentModel = (OkNegotiatedContentResult<CommentModel>)commentController.MarkAsDeleted(comment, new BoolValueDto(true));

            // Then: 
            commentModel.Content.Text.Should().NotBe(comment.Text);
            commentModel.Content.IsDeleted.Should().BeTrue();
        }
    }
}