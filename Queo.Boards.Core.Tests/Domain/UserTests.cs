using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Tests.Builders;

namespace Queo.Boards.Core.Tests.Domain {
    
    [TestClass]
    public class UserTests {
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void Test_Update_name_and_pwd_for_local_user_should_update_data() {

            // Given: 
            User user = new Create(null).User().AsLocalWithHash("123abc").WithUserName("testuser");

            // When: 
            user.UpdateLoginAndPasswordForLocalUser("myuser", "abc123");

            // Then: 
            user.PasswordHash.Should().Be("abc123");
            user.UserName.Should().Be("myuser");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void Test_Update_activeDirectory_user_shoud_return_unchanged_user() {

            // Given: 
            User user = new Create(null).User().WithUserName("testuser");

            // When: 
            user.UpdateLoginAndPasswordForLocalUser("myuser", "abc123");

            // Then: 
            user.PasswordHash.Should().BeNullOrEmpty();
            user.UserName.Should().Be("testuser");
        }
    }
}