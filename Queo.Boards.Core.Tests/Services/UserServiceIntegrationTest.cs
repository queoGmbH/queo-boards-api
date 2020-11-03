using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Exceptions;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class UserServiceIntegrationTest : ServiceBaseTest {
        public IUserService UserService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void Test_Create_new_local_user_should_also_save_can_write() {

            // Given: 
            string username = "tester";
            string hash = "abc123";
            bool canWrite = true;
            UserProfileDto profile = new UserProfileDto("mail", "first", "last", "company", "department", "phone");
            UserAdministrationDto administrationDto = new UserAdministrationDto(new List<string>(), true);

            // When: 
            User user = UserService.Create(username, hash, administrationDto, profile, canWrite);
            User reloaded = UserService.GetById(user.BusinessId);

            // Then: 
            reloaded.CanWrite.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        [ExpectedException(typeof(UserLimitReachedException))]
        public void Test_Create_new_user_when_user_limit_is_reached() {
            // Given
            UserProfileDto profile = new UserProfileDto("mail", "first", "last", "company", "department", "phone");
            UserAdministrationDto administrationDto = new UserAdministrationDto(new List<string>(), true);

            // When 
            User user1 = UserService.Create("Tester 1", "password", administrationDto, profile);
            User user2 = UserService.Create("Tester 2", "geheim", administrationDto, profile);

        }
    }
}