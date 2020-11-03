using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Tests.Builders;
using Queo.Boards.Core.Tests.Builders.Services;

namespace Queo.Boards.Core.Tests.Infrastructure {
    [TestClass]
    public abstract class CreateBaseTest {
        protected Create Create { get; private set; }
        protected CreateService CreateService { get; private set; }

        [TestInitialize]
        public void Init() {
            Create = new Create(null);
            CreateService = new CreateService(null);
        }
    }
}