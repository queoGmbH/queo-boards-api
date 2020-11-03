using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Infrastructure.Http;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Tests.WebApi {
    [TestClass]
    public class LoginIntegrationTest: ServiceBaseTest {

        private HttpServer _server;

        [TestInitialize]
        public void Setup()
        {
            TestInitialize();
            HttpConfiguration config = new HttpConfiguration();

            WebApiConfig.Register(config);
            //  config.DependencyResolver = new MyTestResolver()

            _server = new HttpServer(config);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
            TestCleanup();
        }

        [TestMethod]
        [Ignore]
        public async Task TestGetTokenForLocalUser()
        {
            HttpClient client = new HttpClient(_server);
            HttpResponseMessage httpResponseMessage = await client.GetAsync("http://localhost.com/api/user/currentUser/boardSummaries");

            Assert.AreEqual("42", await httpResponseMessage.Content.ReadAsStringAsync());
        }
    }
}