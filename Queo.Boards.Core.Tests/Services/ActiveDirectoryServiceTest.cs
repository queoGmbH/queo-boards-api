using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Infrastructure.ActiveDirectory;
using Queo.Boards.Core.Services.Impl;

namespace Queo.Boards.Core.Tests.Services {

    [TestClass]
    public class ActiveDirectoryServiceTest {


        /// <summary>
        /// Testet das Abrufen der möglichen Anwender, die im AD als potentielle Nutzer definiert sind.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestFindUsers() {
            //Given: Ein Name für die Gruppe die AD-Nutzer als Anwendungs-Nutzer definiert
            const string AD_ADMINISTRATORS_GROUP_NAME = "QUEO_BOARDS_ADMINS";
            const string AD_USERS_GROUP_NAME = "QUEO_BOARDS_USERS";
            Mock<IActiveDirectoryDao> activeDirectoryDaoMock = new Mock<IActiveDirectoryDao>();
            IList<string> usersList = new List<string> {"nutzer1", "nutzer2"};
            activeDirectoryDaoMock.Setup(dao => dao.FindUserNamesByGroupName(AD_USERS_GROUP_NAME)).Returns(usersList);

            ActiveDirectoryService adService = new ActiveDirectoryService(new ActiveDirectoryConfiguration(AD_ADMINISTRATORS_GROUP_NAME, AD_USERS_GROUP_NAME), activeDirectoryDaoMock.Object);

            //When: Die Nutzernamen der Nutzer abgerufen werden sollen
            IList<string> findQueoBoardsUsers = adService.FindQueoBoardsUsers();

            //Then: Muss ein entsprechender DAO-Aufruf erfolgen und die im Mock definierte Liste geliefert werden.
            activeDirectoryDaoMock.Verify(dao => dao.FindUserNamesByGroupName(AD_USERS_GROUP_NAME), Times.Once);
            findQueoBoardsUsers.Should().BeEquivalentTo(usersList);
        }

        /// <summary>
        /// Testet das Abrufen der möglichen Anwender, die im AD als potentielle Administratoren definiert sind.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestFindAdmins() {
            //Given: Ein Name für die Gruppe die AD-Nutzer als Anwendungs-Administratoren definiert
            const string AD_ADMINISTRATORS_GROUP_NAME = "QUEO_BOARDS_ADMINS";
            const string AD_USERS_GROUP_NAME = "QUEO_BOARDS_USERS";
            Mock<IActiveDirectoryDao> activeDirectoryDaoMock = new Mock<IActiveDirectoryDao>();
            IList<string> usersList = new List<string> { "nutzer1", "nutzer2" };
            activeDirectoryDaoMock.Setup(dao => dao.FindUserNamesByGroupName(AD_ADMINISTRATORS_GROUP_NAME)).Returns(usersList);

            ActiveDirectoryService adService = new ActiveDirectoryService(new ActiveDirectoryConfiguration(AD_ADMINISTRATORS_GROUP_NAME, AD_USERS_GROUP_NAME), activeDirectoryDaoMock.Object);

            //When: Die Nutzernamen der Nutzer abgerufen werden sollen
            IList<string> foundQueoBoardsAdmins = adService.FindQueoBoardsAdministrators();

            //Then: Muss ein entsprechender DAO-Aufruf erfolgen und die im Mock definierte Liste geliefert werden.
            activeDirectoryDaoMock.Verify(dao => dao.FindUserNamesByGroupName(AD_ADMINISTRATORS_GROUP_NAME), Times.Once);
            foundQueoBoardsAdmins.Should().BeEquivalentTo(usersList);
        }

        
    }
}