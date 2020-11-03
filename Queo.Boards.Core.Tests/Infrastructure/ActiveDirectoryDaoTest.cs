using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Infrastructure.ActiveDirectory;

namespace Queo.Boards.Core.Tests.Infrastructure {
    [TestClass]
    public class ActiveDirectoryDaoTest : PersistenceBaseTest {
        private ActiveDirectoryDao _activeDirectoryDao;

        public ActiveDirectoryAccessConfiguration ActiveDirectoryAccessConfiguration { get; set; }

        public ActiveDirectoryConfiguration ActiveDirectoryConfiguration {
            get; set;
        }

        [TestInitialize]
        public void SetUp() {
            _activeDirectoryDao = new ActiveDirectoryDao(ActiveDirectoryAccessConfiguration);
        }

        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestAuthenticate() {
            Assert.IsTrue(_activeDirectoryDao.Authenticate(ActiveDirectoryAccessConfiguration.AccessUserName, ActiveDirectoryAccessConfiguration.AccessPassword));
            Assert.IsFalse(_activeDirectoryDao.Authenticate(ActiveDirectoryAccessConfiguration.AccessUserName, "Blubb!"));
            Assert.IsFalse(_activeDirectoryDao.Authenticate("jaekel2", "XXX"));            
        }

        /// <summary>
        ///     Testet ob für einen existierenden Nutzer die Methode true liefert.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestExists() {
            string username = _activeDirectoryDao.FindUserNamesByGroupName(ActiveDirectoryConfiguration.UsersGroupName).First();
            bool actual = _activeDirectoryDao.IsExistingUsername(username);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///     Testet das Suchen nach Nutzernamen anhand eines Gruppennamens.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindByGroup() {
            //Given: Der Name einer Gruppe, deren Nutzer abgerufen werden sollen. 
            string activeDirectoryGroupName = ActiveDirectoryConfiguration.UsersGroupName;
            
            //When: Die Nutzernamen abgerufen werden sollen 
            IList<string> findUserNamesByGroupName = _activeDirectoryDao.FindUserNamesByGroupName(activeDirectoryGroupName);

            //Then: Müssen die entsprechenden Nutzernamen geliefert werden.
            findUserNamesByGroupName.Should().NotBeEmpty();
        }

        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindUser() {
            string username = _activeDirectoryDao.FindUserNamesByGroupName(ActiveDirectoryConfiguration.UsersGroupName).First();
            UserNtInformation userNtInformation = _activeDirectoryDao.FindUserInformation(username);
            Assert.IsNotNull(userNtInformation);
        }

        /// <summary>
        ///     Testet dass der DAO ein leeres Userinfo-Objekt liefert, wenn er Daten für einen Nutzernamen sucht der nicht
        ///     vorhanden ist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindUserForNonexistentUsername() {
            //ActiveDirectoryDao activeDirectoryDao = new ActiveDirectoryDao();
            UserNtInformation userNtInformation = _activeDirectoryDao.FindUserInformation("hf837hiuhfi");
            Assert.IsNull(userNtInformation);            
        }

        /// <summary>
        ///     Testet dass für einen nichtexistierenden Nutzernamen die Methode false liefert.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestIsExistingUsernameFail() {
            //ActiveDirectoryDao activeDirectoryDao = new ActiveDirectoryDao();
            bool actual = _activeDirectoryDao.IsExistingUsername("asdnu8dgh23dhn");
            Assert.AreEqual(false, actual);
        }
    }
}