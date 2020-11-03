using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests {
    [TestClass]
    public class UserBuilderTest : CreateBaseTest {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestUserBuilderUniqueness() {

            // Given: 
            IDictionary<User, object> users = new Dictionary<User, object>();
            for (int i = 0; i < 20; i++) {
                users.Add(Create.User().Build(), null);
            }

        }

    }
}