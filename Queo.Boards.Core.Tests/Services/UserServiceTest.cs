using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Infrastructure.ActiveDirectory;
using Queo.Boards.Core.Models;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Services.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    [TestClass]
    public class
        UserServiceTest : CreateBaseTest {
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void Test_Update_activeDirectory_user_should_return_unchanged() {
            // Given: 
            User user = Create.User().WithUserName("testuser");

            Mock<ISecurityService> securityServiceMock = new Mock<ISecurityService>();

            IUserService userService = CreateService.UserService().With(securityServiceMock.Object).Build();

            // When: 

            User updatedUser = userService.UpdateLocalUser(
                user,
                new UserUpdateModel(
                    "newname",
                    "newfirstname",
                    "newlastname",
                    "company",
                    "department",
                    "mail",
                    "phone",
                    new List<string>(),
                    true,
                    true));

            // Then: 
            updatedUser.UserName.Should().Be("testuser");
            securityServiceMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void Test_Update_user_should_update_user() {
            // Given: 
            Mock<ISecurityService> securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("abc123");

            IUserService userService = CreateService.UserService().With(securityServiceMock.Object).Build();

            Mock<User> userMock = new Mock<User>();
            userMock.SetupGet(x => x.UserCategory).Returns(UserCategory.Local);
            userMock.Setup(x => x.Update(It.IsAny<UserAdministrationDto>(), It.IsAny<UserProfileDto>()));
            userMock.Setup(x => x.UpdateCanWrite(It.IsAny<bool>()));
            userMock.Setup(x => x.UpdateLoginAndPasswordForLocalUser(It.IsAny<string>(), "abc123"));
            userMock.Setup(x => x.GetAdministrationDto()).Returns(new UserAdministrationDto());
            userMock.Setup(x => x.GetProfileDto()).Returns(new UserProfileDto());

            // When: 
            userService.UpdateLocalUser(
                userMock.Object,
                new UserUpdateModel(
                    "newname",
                    "newfirstname",
                    "newlastname",
                    "company",
                    "department",
                    "mail",
                    "phone",
                    new List<string>(),
                    true,
                    true));

            // Then: 
            userMock.Verify(x => x.Update(It.IsAny<UserAdministrationDto>(), It.IsAny<UserProfileDto>()), Times.Once);
            userMock.Verify(x => x.UpdateCanWrite(It.IsAny<bool>()), Times.Once);
            userMock.Verify(x => x.UpdateLoginForLocalUser(It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        ///     Testet das Erstellen eines neuen Nutzers
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestCreateUser() {
            //Given: Daten für einen neuen Nutzer
            const string USERNAME = "max.mustermann";
            IList<string> roles = new List<string> { "Rolle 1", "Rolle 2", "Rolle 3" };
            UserAdministrationDto administrationDto = new UserAdministrationDto(roles, true);
            UserProfileDto profileDto = new UserProfileDto("user@queo-boards.de", "Max", "Mustermann", "queo", "Vertrieb", "0123/456789");

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            UserService userService = CreateService.UserService().With(userDaoMock.Object);

            //When: Ein Nutzer mit diesen Daten erstellt werden sol
            User user = userService.Create(USERNAME, administrationDto, profileDto);

            //Then: Muss das korrekt funktionieren und ein Nutzer mit den Daten erstellt werden.
            user.UserName.Should().Be(USERNAME);
            user.GetAdministrationDto().Should().Be(administrationDto);
            user.GetProfileDto().Should().Be(profileDto);
        }

        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestResetPassword() {
            //Given: Daten für einen neuen Nutzer
            const string USERNAME = "max.mustermann";
            IList<string> roles = new List<string> { "Rolle 1" };
            UserAdministrationDto administrationDto = new UserAdministrationDto(roles, true);
            UserProfileDto profileDto = new UserProfileDto("user@queo-boards.de", "Max", "Mustermann", "queo", "Vertrieb", "0123/456789");

            string newPwdHash = "Banarama";
            string newPassword = "Geheim";

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            Mock<ISecurityService> securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(newPwdHash);

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(securityServiceMock.Object);
            userService.PasswordResetHours = 0.5;

            User user = userService.Create(USERNAME, "hash123", administrationDto, profileDto);

            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns(user);
            userDaoMock.Setup(dao => dao.FindByPasswordResetRequestId(It.IsAny<Guid>())).Returns(user);

            userService.PasswordResetRequest(USERNAME);

            //When: Ein neues Passwort für einen Nutzer gesetzt werden soll
            userService.ResetPassword(user.PasswordResetRequestId, newPassword);

            //Then: Muss das korrekt funktionieren und der hat ein neues Passwort/ PasswortHash.
            user.UserName.Should().Be(USERNAME);
            user.GetAdministrationDto().Should().Be(administrationDto);
            user.GetProfileDto().Should().Be(profileDto);
            Assert.AreEqual(null, user.PasswordResetRequestValidTo);
            Assert.AreEqual(null, user.PasswordResetRequestId);
            user.PasswordHash.Should().Be(newPwdHash);
        }

        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestResetPasswordUserNotFound() {
            //Given: Daten für einen neuen Nutzer
            const string USERNAME = "max.mustermann";
            IList<string> roles = new List<string> { "Rolle 1" };
            UserAdministrationDto administrationDto = new UserAdministrationDto(roles, true);
            UserProfileDto profileDto = new UserProfileDto("user@queo-boards.de", "Max", "Mustermann", "queo", "Vertrieb", "0123/456789");

            string newPassword = "Geheim";
            Guid resetGuid = Guid.NewGuid();

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            UserService userService = CreateService.UserService().With(userDaoMock.Object);
            userService.PasswordResetHours = 0.5;

            User user = userService.Create(USERNAME, "hash123", administrationDto, profileDto);

            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns(user);
            userDaoMock.Setup(dao => dao.FindByPasswordResetRequestId(Guid.NewGuid())).Returns(user);

            //When: Ein neues Passwort für einen Nutzer gesetzt werden soll
            userService.ResetPassword(resetGuid, newPassword);

            //Then: soll nix passieren und alles bleibt wie es ist.
            user.UserName.Should().Be(USERNAME);
            user.GetAdministrationDto().Should().Be(administrationDto);
            user.GetProfileDto().Should().Be(profileDto);
            Assert.AreEqual(null, user.PasswordResetRequestValidTo);
            Assert.AreEqual(null, user.PasswordResetRequestId);
        }

        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestResetPasswordWithOutPasswordResetGuid() {
            //Given: Daten für einen neuen Nutzer
            string newPassword = "geheim";

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            UserService userService = CreateService.UserService().With(userDaoMock.Object);
            
            //When: Ein Nutzer mit diesen Daten erstellt werden sol
            Action resetPasswordAction = () => userService.ResetPassword(null, newPassword);

            //Then: Muss eine ArgumentOutOfRangeException fliegen.
            resetPasswordAction.ShouldThrow<ArgumentException>();
        }

        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestResetPasswordWithInvalideTime() {
            //Given: Daten für einen neuen Nutzer
            const string USERNAME = "max.mustermann";
            IList<string> roles = new List<string> { "Rolle 1" };
            UserAdministrationDto administrationDto = new UserAdministrationDto(roles, true);
            UserProfileDto profileDto = new UserProfileDto("user@queo-boards.de", "Max", "Mustermann", "queo", "Vertrieb", "0123/456789");

            string newPwdHash = "Banarama";
            string newPassword = "Geheim";

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            Mock<ISecurityService> securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(newPwdHash);

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(securityServiceMock.Object);
            userService.PasswordResetHours = -0.5;

            User user = userService.Create(USERNAME, "hash123", administrationDto, profileDto);

            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns(user);
            userDaoMock.Setup(dao => dao.FindByPasswordResetRequestId(It.IsAny<Guid>())).Returns(user);

            userService.PasswordResetRequest(USERNAME);

            //When: Ein Nutzer mit diesen Daten erstellt werden sol
            Action resetPasswordAction = () => userService.ResetPassword(user.PasswordResetRequestId, newPassword);

            //Then: Muss eine ArgumentOutOfRangeException fliegen.
            resetPasswordAction.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestPasswordResetRequest() {
            //Given: Daten für einen neuen Nutzer
            const string USERNAME = "max.mustermann";
            IList<string> roles = new List<string> { "Rolle 1", "Rolle 2", "Rolle 3" };
            UserAdministrationDto administrationDto = new UserAdministrationDto(roles, true);
            UserProfileDto profileDto = new UserProfileDto("user@queo-boards.de", "Max", "Mustermann", "queo", "Vertrieb", "0123/456789");

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            UserService userService = CreateService.UserService().With(userDaoMock.Object);

            User user = userService.Create(USERNAME, "hash123", administrationDto, profileDto);
            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns(user);

            //When: Ein Passwort-Reset-Request gesetzt gerden soll
            userService.PasswordResetRequest(USERNAME);

            //Then: Muss das korrekt funktionieren und der Nutzer bekommt eine PasswordResetRequestId und ein PasswordResetRequestValidTo Datum.
            user.UserName.Should().Be(USERNAME);
            user.GetAdministrationDto().Should().Be(administrationDto);
            user.GetProfileDto().Should().Be(profileDto);
            Assert.AreNotEqual(null, user.PasswordResetRequestValidTo);
            Assert.AreNotEqual(null, user.PasswordResetRequestId);
            user.PasswordResetRequestId.Should().NotBe(Guid.Empty, "Keine Valide Guid angelegt.");
            user.PasswordResetRequestValidTo.Should().NotBe(DateTime.MinValue, "Datum ist ungültig");
        }

        /// <summary>
        ///     Testet das Erstellen eines neuen Administrators beim Synchronisieren mit dem AD
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUsersCreateAdmin() {
            //Given: Daten für einen neuen Nutzer der bisher nicht in der DB ist.
            string USERNAME = "Neuer Admin";

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns((User)null);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User>());
            UserNtInformation userNtInformation = new UserNtInformation(
                "Ad",
                "Ministrator",
                "queo",
                "queo-boards",
                "ad.ministrator@queo-group.com",
                "0123/456789");
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string>());
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string> { USERNAME });
            adServiceMock.Setup(ad => ad.FindUserInformation(USERNAME)).Returns(userNtInformation);

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisation mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Muss ein Nutzer mit den Informationen aus dem AD erstellt werden, der nur Administrator ist.
            summary.DeletedUsers.Should().BeEmpty();
            summary.CreatedUsers.Should().ContainSingle(user => user.UserName == USERNAME);
            summary.UpdatedUsers.Should().BeEmpty();
            userDaoMock.Verify(dao => dao.Save(It.IsAny<User>()), Times.Once);

            User createdUser = summary.CreatedUsers.Single();
            createdUser.GetAdministrationDto().Should().Be(new UserAdministrationDto(new List<string> { UserRole.ADMINISTRATOR }, true));
            createdUser.GetProfileDto().Should().Be(UserService.CreateProfileForActiveDirectoryInformation(userNtInformation));
        }

        /// <summary>
        ///     Testet das Erstellen eines neuen Administrators beim Synchronisieren mit dem AD
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUsersCreateUser() {
            //Given: Daten für einen neuen Nutzer der bisher nicht in der DB ist.
            string USERNAME = "Neuer Nutzer";

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns((User)null);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User>());
            UserNtInformation userNtInformation = new UserNtInformation(
                "Ju",
                "Sehr",
                "queo",
                "queo-boards",
                "ad.ministrator@queo-group.com",
                "0123/456789");
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string> { USERNAME });
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string>());

            adServiceMock.Setup(ad => ad.FindUserInformation(USERNAME)).Returns(userNtInformation);

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisation mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Muss ein Nutzer mit den Informationen aus dem AD erstellt werden, der nur Nutzer ist.
            summary.DeletedUsers.Should().BeEmpty();
            summary.CreatedUsers.Should().ContainSingle(user => user.UserName == USERNAME);
            summary.UpdatedUsers.Should().BeEmpty();
            userDaoMock.Verify(dao => dao.Save(It.IsAny<User>()), Times.Once);

            User createdUser = summary.CreatedUsers.Single();
            createdUser.GetAdministrationDto().Should().Be(new UserAdministrationDto(new List<string> { UserRole.USER }, true));
            createdUser.GetProfileDto().Should().Be(UserService.CreateProfileForActiveDirectoryInformation(userNtInformation));
        }

        /// <summary>
        ///     Testet das Erstellen eines neuen Nutzers der gleichzeitig Admin ist beim Synchronisieren mit dem AD
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUsersCreateUserWhoIsAlsoAdmin() {
            //Given: Daten für einen neuen Nutzer der bisher nicht in der DB ist.
            string USERNAME = "Neuer Admin und Nutzer";

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns((User)null);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User>());
            UserNtInformation userNtInformation = new UserNtInformation(
                "Ju",
                "Sehr und Admin",
                "queo",
                "queo-boards",
                "ad.ministrator@queo-group.com",
                "0123/456789");
            userDaoMock.Setup(dao => dao.Save(It.IsAny<User>()));

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string> { USERNAME });
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string> { USERNAME });
            adServiceMock.Setup(ad => ad.FindUserInformation(USERNAME)).Returns(userNtInformation);

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisation mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Muss ein Nutzer mit den Informationen aus dem AD erstellt werden, der Admin und Nutzer gleichzeitig ist.
            summary.DeletedUsers.Should().BeEmpty();
            summary.CreatedUsers.Should().ContainSingle(user => user.UserName == USERNAME);
            summary.UpdatedUsers.Should().BeEmpty();
            userDaoMock.Verify(dao => dao.Save(It.IsAny<User>()), Times.Once);

            User createdUser = summary.CreatedUsers.Single();
            createdUser.GetAdministrationDto().Should()
                .Be(new UserAdministrationDto(new List<string> { UserRole.ADMINISTRATOR, UserRole.USER }, true));
            createdUser.GetProfileDto().Should().Be(UserService.CreateProfileForActiveDirectoryInformation(userNtInformation));
        }

        /// <summary>
        ///     Testet das Synchronisieren von Nutzern mit dem AD
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUsersDisableUser() {
            //Given: Ein in der Datenbank vorhandener Nutzer dessen Name nicht mehr im AD vorhanden ist
            string USERNAME = "vorhanden";
            User existingUser = new User(USERNAME, new UserAdministrationDto(), new UserProfileDto());

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(USERNAME)).Returns(existingUser);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User> { existingUser });

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string>());
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string>());

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisation mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Muss er in der DB ebenfalls deaktiviert werden.
            summary.DeletedUsers.Should().Contain(existingUser);
            summary.CreatedUsers.Should().BeEmpty();
            summary.UpdatedUsers.Should().BeEmpty();
        }

        /// <summary>
        ///     Testet das Aktualisieren der Daten eines Nutzers, beim synchronisieren mit dem AD
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUserUpdateInformation() {
            //Given: Ein Nutzer, dessen Daten im AD geändert wurden. 
            User existingUser = Create.User().WithRoles(UserRole.USER);
            UserNtInformation newAdInformation = new UserNtInformation(
                "Neuer Vorname",
                "Neuer Nachname",
                "Neues Unternehmen",
                "Neue Abteilung",
                "neue@email.de",
                "neue Nummer");

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string> { existingUser.UserName });
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string>());
            adServiceMock.Setup(ad => ad.FindUserInformation(existingUser.UserName)).Returns(newAdInformation);

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(existingUser.UserName)).Returns(existingUser);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User> { existingUser });

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisierung mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Müssen seine Daten geändert werden und der Nutzer in der Liste der geänderten Nutzer auftauchen
            summary.DeletedUsers.Should().BeEmpty();
            summary.CreatedUsers.Should().BeEmpty();
            summary.UpdatedUsers.Should().ContainSingle(user => user.Equals(existingUser));

            existingUser.Roles.Should().BeEquivalentTo(UserRole.USER);
            existingUser.GetAdministrationDto().Should().Be(new UserAdministrationDto(new List<string> { UserRole.USER }, true));
            existingUser.GetProfileDto().Should().Be(
                new UserProfileDto(
                    newAdInformation.Email,
                    newAdInformation.FirstName,
                    newAdInformation.LastName,
                    newAdInformation.Company,
                    newAdInformation.Department,
                    newAdInformation.Phone));
        }

        /// <summary>
        ///     Testet das Aktualisieren der Daten eines Nutzers, beim synchronisieren mit dem AD, wenn keine Daten geändert
        ///     wurden.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUserUpdateNoChanges() {
            //Given: Ein Nutzer, dessen Daten im AD NICHT geändert wurden. 
            User existingUser = Create.User().WithRoles(UserRole.USER);
            UserNtInformation unchangedAdInformation = new UserNtInformation(
                existingUser.Firstname,
                existingUser.Lastname,
                existingUser.Company,
                existingUser.Department,
                existingUser.Email,
                existingUser.Phone);

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string> { existingUser.UserName });
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string>());
            adServiceMock.Setup(ad => ad.FindUserInformation(existingUser.UserName)).Returns(unchangedAdInformation);

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(existingUser.UserName)).Returns(existingUser);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User> { existingUser });

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisierung mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Müssen seine Daten unverändert sein und er darf in keiner der Listen der Zusammenfassung auftauchen.
            summary.DeletedUsers.Should().BeEmpty();
            summary.CreatedUsers.Should().BeEmpty();
            summary.UpdatedUsers.Should().BeEmpty();

            existingUser.Roles.Should().BeEquivalentTo(UserRole.USER);
            existingUser.GetAdministrationDto().Should().Be(new UserAdministrationDto(new List<string> { UserRole.USER }, true));
            existingUser.GetProfileDto().Should().Be(
                new UserProfileDto(
                    unchangedAdInformation.Email,
                    unchangedAdInformation.FirstName,
                    unchangedAdInformation.LastName,
                    unchangedAdInformation.Company,
                    unchangedAdInformation.Department,
                    unchangedAdInformation.Phone));
        }

        /// <summary>
        ///     Testet das Ändern einer Rolle, beim synchronisieren mit dem AD
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestSynchronizeUserUpdateRole() {
            //Given: Ein Nutzer, der bisher nur die Rolle Administrator hat und jetzt im AD der Gruppe für Nutzer zugeordnet ist. 
            User adminUser = Create.User().WithRoles(UserRole.ADMINISTRATOR);

            Mock<IActiveDirectoryService> adServiceMock = new Mock<IActiveDirectoryService>();
            adServiceMock.Setup(ad => ad.FindQueoBoardsUsers()).Returns(new List<string> { adminUser.UserName });
            adServiceMock.Setup(ad => ad.FindQueoBoardsAdministrators()).Returns(new List<string>());
            adServiceMock.Setup(ad => ad.FindUserInformation(adminUser.UserName)).Returns(
                new UserNtInformation(
                    adminUser.Firstname,
                    adminUser.Lastname,
                    adminUser.Company,
                    adminUser.Department,
                    adminUser.Email,
                    adminUser.Phone));

            Mock<IUserDao> userDaoMock = new Mock<IUserDao>();
            userDaoMock.Setup(dao => dao.FindByUsername(adminUser.UserName)).Returns(adminUser);
            userDaoMock.Setup(dao => dao.GetAll()).Returns(new List<User> { adminUser });

            UserService userService = CreateService.UserService().With(userDaoMock.Object).With(adServiceMock.Object);

            //When: Die Synchronisierung mit dem AD erfolgt
            ActiveDirectorySynchronisationSummary summary = userService.SynchronizeWithActiveDirectory();

            //Then: Müssen seine Rolle angepasst werden
            summary.DeletedUsers.Should().BeEmpty();
            summary.CreatedUsers.Should().BeEmpty();
            summary.UpdatedUsers.Should().ContainSingle(user => user.Equals(adminUser));

            adminUser.Roles.Should().BeEquivalentTo(UserRole.USER);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestValidateCredentialsWhenExistsInAd() {
            // Given: 
            Mock<IActiveDirectoryService> activeDirectoryDaoMock = new Mock<IActiveDirectoryService>();
            activeDirectoryDaoMock.Setup(x => x.Authenticate("user", "pwd")).Returns(true);

            IUserService userService = CreateService.UserService().With(activeDirectoryDaoMock.Object).Build();

            // When: 
            bool isCredentialsValid = userService.ValidateCredentials("user", "pwd");

            // Then: 
            isCredentialsValid.Should().BeTrue();
            activeDirectoryDaoMock.Verify(x => x.Authenticate("user", "pwd"), Times.Once);
        }
    }
}