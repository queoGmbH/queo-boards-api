using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.NHibernate.Persistence;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {

    /// <summary>
    /// Tests für den <see cref="UserDao"/>
    /// </summary>
    [TestClass]
    public class UserDaoTest : PersistenceBaseTest {


        public UserDao UserDao { private get; set; }

        /// <summary>
        /// Testet das Speichern und Laden eines Nutzers
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestSaveAndLoad() {

            //Given: Daten für einen Nutzer die gespeichert werden sollen
            const string USERNAME = "nutzername";
            UserAdministrationDto userAdministrationDto = new UserAdministrationDto(new List<string> {"Rolle 1", "Rolle 2"}, true);
            UserProfileDto userProfileDto = new UserProfileDto("test@nutzer.de", "Vor", "Name", "Testunternehmen", "Testabteilung", "Telefonnummer");
            User user = new User(USERNAME, userAdministrationDto, userProfileDto);

            //When: Der Nutzer gespeichert und wieder geladen werden
            UserDao.Save(user);
            UserDao.FlushAndClear();
            User reloaded = UserDao.GetByBusinessId(user.BusinessId);

            //Then: Müssen alle Daten noch korrekt vorhanden sein
            reloaded.Should().Be(user);
            reloaded.GetAdministrationDto().Should().Be(userAdministrationDto);
            reloaded.GetProfileDto().Should().Be(userProfileDto);

        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.INTEGRATION)]
        public void TestFindByUserName() {

            // Given: 
            string username = "nutzername";
            User user = Create.User().WithUserName(username).WithRoles("User").Build();

            // When: 
            UserDao.FlushAndClear();
            User reloaded = UserDao.FindByUsername(username);
            
            // Then: 
            reloaded.UserName.Should().Be(username);
        }


        /// <summary>
        /// Testet das Aktualisieren der Rollen eines Nutzers.
        /// </summary>
        [TestMethod]
        public void TestSaveRoles() {

            //Given: Ein Nutzer in der Rolle User
            User user = Create.User().WithRoles(UserRole.USER);
            user.Update(new UserAdministrationDto(new List<string> {UserRole.ADMINISTRATOR}, true), user.GetProfileDto());

            //When: Der Nutzer die Rolle Administrator bekommt, gespeichert und neu geladen wird
            UserDao.Save(user);
            UserDao.FlushAndClear();
            User reloaded = UserDao.Get(user.Id);

            //Then: Muss er die Rolle Administrator haben
            reloaded.Roles.Should().BeEquivalentTo(UserRole.ADMINISTRATOR);

        }

        /// <summary>
        /// Testet das Finden eines Nutzers in einer bestimmten Rolle
        /// </summary>
        [TestMethod]
        public void TestFindByRoleShouldFindUserWithOnlyTheRoleSearchedFor() {
            //Given: Ein Nutzer mit der Rolle nach der gesucht werden soll
            User user = Create.User().WithRoles(UserRole.ADMINISTRATOR);

            //When: Nach Nutzern mit der Rolle gesucht wird
            IPage<User> foundByRole = UserDao.FindByRole(PageRequest.All, UserRole.ADMINISTRATOR);

            //Then: Muss der Nutzer in der Ergebnisliste enthalten sein
            foundByRole.Should().Contain(user);
        }

        /// <summary>
        /// Testet das Finden eines Nutzers der neben der gesuchten Rolle auch noch anderen Rollen hat.
        /// </summary>
        [TestMethod]
        public void TestFindByRoleShouldFindUserWithTheRoleSearchedForAndOthers() {
            //Given: Ein Nutzer mit der Rolle nach der gesucht werden soll und anderen Rollen.
            User user = Create.User().WithRoles(UserRole.ADMINISTRATOR, UserRole.USER);

            //When: Nach Nutzern mit der Rolle gesucht wird
            IPage<User> foundByRole = UserDao.FindByRole(PageRequest.All, UserRole.ADMINISTRATOR);

            //Then: Muss der Nutzer in der Ergebnisliste enthalten sein
            foundByRole.Should().Contain(user);
        }

        /// <summary>
        /// Testet das Nutzer, die nicht die Rolle haben nach der gesucht wird, nicht gefunden werden.
        /// </summary>
        [TestMethod]
        public void TestFindByRoleShouldNotFindUserWithoutTheRoleSearchedFor() {
            //Given: Ein Nutzer mit einer anderen Rolle als die, nach der gesucht werden soll
            User user = Create.User().WithRoles(UserRole.ADMINISTRATOR);

            //When: Nach Nutzern mit einer anderen Rolle gesucht wird
            IPage<User> foundByRole = UserDao.FindByRole(PageRequest.All, UserRole.USER);

            //Then: Darf der Nutzer NICHT in der Ergebnisliste enthalten sein
            foundByRole.Should().NotContain(user);
        }

        /// <summary>
        /// Testet das Suchen nach mehreren Nutzern anhand einer Rolle
        /// </summary>
        [TestMethod]
        public void TestFindByRoleShouldFindMultipleusers() {
            //Given: Mehrere Nutzer
            const string ROLE_TO_SEARCH = "Coole User";
            User userWithExactlyTheRole = Create.User().WithRoles(ROLE_TO_SEARCH);
            User userWithTheRoleAndOtherRoles = Create.User().WithRoles(ROLE_TO_SEARCH, "Noch ne Rolle", "Und noch eine");
            User userWithOtherRoles = Create.User().WithRoles("Andere Rolle", "Und noch eine andere Rolle");
            User userWithoutRoles = Create.User().WithRoles();

            //When: Nach Nutzern in einer bestimmten Rolle gesucht wird
            IPage<User> foundUsers = UserDao.FindByRole(PageRequest.All, ROLE_TO_SEARCH);

            //Then: Dürfen nur die Nutzer gefunden werden die mindestens die gesuchte Rolle haben,.
            foundUsers.Should().Contain(new [] { userWithExactlyTheRole, userWithTheRoleAndOtherRoles});
            foundUsers.Should().NotContain(new[] { userWithOtherRoles, userWithoutRoles });
        }

        [TestMethod]
        public void TestCountAllActive() {
            // Given
            User user = Create.User().WithRoles(UserRole.ADMINISTRATOR);

            // When
            int userCount = UserDao.CountAllActive();

            // Then
            Assert.AreEqual(1, userCount);
        }
    }



}