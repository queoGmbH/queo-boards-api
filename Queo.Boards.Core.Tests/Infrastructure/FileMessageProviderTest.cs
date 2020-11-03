using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Infrastructure.Templating;

namespace Queo.Boards.Core.Tests.Infrastructure {
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class FileMessageProviderTest:ServiceBaseTest {
        public IMessageProvider MailMessageProvider { get; set; }

        [TestMethod]
        public void TestLoadResource() {
            string renderedMessage = MailMessageProvider.RenderMessage("test", new ModelMap());

            Assert.IsNotNull(renderedMessage);
            renderedMessage.StartsWith("Subject: Testbetreff").Should().BeTrue();
        }

        [TestMethod]
        public void TestRenderMessageWithoutCulture() {
            string renderedMessage = MailMessageProvider.RenderMessage("TestWithoutCulture", new ModelMap());

            Assert.IsNotNull(renderedMessage);
            renderedMessage.StartsWith("Subject: TestWithoutCulture").Should().BeTrue();           
        }

        [TestMethod]
        public void TestRenderNotExistingResource() {
            new Action(() => MailMessageProvider.RenderMessage("TestNotExistingResource", new ModelMap())).ShouldThrow<FileNotFoundException>();
        }
    }
}