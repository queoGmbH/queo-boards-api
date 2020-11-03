using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Tests.Builders;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Domain {
    [TestClass]
    public class BoardTests : CreateBaseTest { 
        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("unit")]
        public void TestIsPrivateShouldBeTrueWithoutMembers() {
            // Given: 
            UserBuilder userBuilder = new UserBuilder(null);
            User user = userBuilder.Build();

            BoardBuilder boardBuilder = new BoardBuilder(null, new UserBuilder(null));
            Board board = boardBuilder.WithOwners(user).Build();

            // When: 
            bool isPrivate = board.IsPrivate;

            // Then: 
            isPrivate.Should().BeTrue();
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("unit")]
        public void TestIsPrivateShouldBeFalseWithSingleMemberNotEqualsOwner() {
            // Given: 
            UserBuilder userBuilder = new UserBuilder(null);
            User user1 = userBuilder.WithUserName("user1").Build();
            User user2 = userBuilder.WithUserName("user2").Build();

            BoardBuilder boardBuilder = new BoardBuilder(null, new UserBuilder(null));
            Board board = boardBuilder.WithOwners(user1).WithMembers(user2).Build();

            // When: 
            bool isPrivate = board.IsPrivate;

            // Then: 
            Assert.IsFalse(isPrivate);
        }

        /// <summary>
        /// </summary>
        [TestMethod]
        [TestCategory("unit")]
        public void TestIsPrivateShouldBeTrueWithSingleMemberEqualsOwner() {
            // Given: 
            UserBuilder userBuilder = new UserBuilder(null);
            User user = userBuilder.Build();

            BoardBuilder boardBuilder = new BoardBuilder(null, new UserBuilder(null));
            Board board = boardBuilder.WithOwners(user).WithMembers(user).Build();

            // When: 
            bool isPrivate = board.IsPrivate;

            // Then: 
            Assert.IsTrue(isPrivate);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.UNIT)]
        public void TestIsPrivateShouldBeFalseWithSingleMemberEqualsOwnerButPublic() {

            // Given: 
            UserBuilder userBuilder = new UserBuilder(null);
            User user = userBuilder.Build();

            BoardBuilder boardBuilder = new BoardBuilder(null, new UserBuilder(null));
            Board board = boardBuilder.WithOwners(user).WithMembers(user).Public().Build();
            
            // When: 
            bool isPrivate = board.IsPrivate;

            // Then: 
            Assert.IsFalse(isPrivate);
        }

        /// <summary>
        /// Testet das Abrufen der Board-Nutzer
        /// </summary>
        [TestMethod]
        public void TestGetBoardUsers() {

            //Given: Ein Board, dem zwei Teams mit Nutzern, mehrere Besitzer und mehrere einzelne Mitglieder zugewiesen sind.
            User owner = Create.User();
            User member = Create.User();

            User team1Member = Create.User();
            Team team1 = Create.Team().WithMembers(team1Member);

            User team2Member = Create.User();
            Team team2 = Create.Team().WithMembers(team2Member);

            Board board = Create.Board().WithOwners(owner).WithMembers(member).WithTeams(team1, team2);

            //When: Die Nutzer des Boards abgerufen wird
            IList<User> boardUsers = board.GetBoardUsers();

            //Then: Muss die Vereinigungsmenge aus Besitzern, Mitgliedern und Mitglieder der Teams abgerufen werden
            boardUsers.Should().BeEquivalentTo(owner, member, team1Member, team2Member);
        }

        /// <summary>
        /// Testet, dass in der Liste der Board-Nutzer jeder Nutzer maximal 1mal vorkommt
        /// </summary>
        [TestMethod]
        public void TestGetBoardUsersShouldBeDistinct() {

            //Given: Ein Board und ein Nutzer, der gleichzeitig Besitzer des Boards, Mitglied des Boards und Mitglied zweier dem Board zugewiesener Teams ist 
            User user = Create.User();
            Team team1 = Create.Team().WithMembers(user);
            Team team2 = Create.Team().WithMembers(user);
            Board board = Create.Board().WithOwners(user).WithMembers(user).WithTeams(team1, team2);

            //When: Die Nutzer des Boards abgerufen werden sollen
            IList<User> boardUsers = board.GetBoardUsers();

            //Then: Darf nur der eine Nutzer und auch nur einmal in der Liste enthalten sein
            boardUsers.Should().HaveCount(1);
            boardUsers.Should().OnlyContain(u => u.Equals(user));
        }
    }
}