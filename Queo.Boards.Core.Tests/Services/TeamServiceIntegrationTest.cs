using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Persistence;
using Queo.Boards.Core.Services;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Services {
    /// <summary>
    /// Integrationstests für den TeamService, der gegen eine tatsächlich existierende Datenbank testet.
    /// </summary>
    [TestClass]
    public class TeamServiceIntegrationTest : ServiceBaseTest {

        public ITeamService TeamService { get; set; }
        public ITeamDao TeamDao { get; set; }
        public IUserDao UserDao {
            get; set;
        }

        /// <summary>
        /// Testet das Hinzufügen mehrerer Mitglieder
        /// </summary>
        [TestMethod]
        public void TestAddMultipleMembers() {

            //Given: Ein Team mit einem Mitglied
            User existingMember = Create.User();
            User newMember1 = Create.User();
            User newMember2 = Create.User();
            User newMember3 = Create.User();
            Team team = Create.Team().WithMembers(existingMember);

            //When: 3 Mitglieder hinzugefügt werden.
            IList<User> members = TeamService.AddMembers(team, newMember1, newMember2, newMember3);
            TeamDao.FlushAndClear();
            Team reloadedTeam = TeamDao.Get(team.Id);

            //Then: Muss das Team 4 Mitglieder haben
            members.Should().BeEquivalentTo(existingMember, newMember1, newMember2, newMember3);
            team.Members.Should().BeEquivalentTo(existingMember, newMember1, newMember2, newMember3);
            reloadedTeam.Members.Should().BeEquivalentTo(existingMember, newMember1, newMember2, newMember3);
        }

        /// <summary>
        /// Testet das Entfernen mehrerer Mitglieder aus einem Team
        /// </summary>
        [TestMethod]
        public void TestRemoveMultipleMembers() {

            //Given: Ein Team mit mehreren Mitgliedern
            User existingMember1 = Create.User();
            User existingMember2 = Create.User();
            User existingMember3 = Create.User();
            Team team = Create.Team().WithMembers(existingMember1, existingMember2, existingMember3);

            //When: Mehrere Mitglieder aus dem Team entfernt werden soll
            IList<User> remainingMembers = TeamService.RemoveMembers(team, existingMember2, existingMember1);
            TeamDao.FlushAndClear();
            Team reloadedTeam = TeamDao.Get(team.Id);

            //Then: Müssen sie aus der Liste der Mitglieder verschwinden und die anderen weiterhin Mitglieder bleiben
            remainingMembers.Should().BeEquivalentTo(existingMember3);
            team.Members.Should().BeEquivalentTo(existingMember3);
            reloadedTeam.Members.Should().BeEquivalentTo(existingMember3);
        }


        /// <summary>
        /// Testet das Löschen eines Teams
        /// </summary>
        [TestMethod]
        public void TestDeleteTeam() {

            //Given: Ein Team mit Mitgliedern
            User teamMember1 = Create.User();
            User teamMember2 = Create.User();
            Team team = Create.Team().WithMembers(teamMember1, teamMember2);
            Action action = () => {
                TeamDao.GetByBusinessId(team.BusinessId).Should().BeNull();
            };

            //When: Das Team gelöscht wird
            TeamService.Delete(team);
            TeamDao.FlushAndClear();

            //Then: Muss es aus der DB entfernt werden
            action.ShouldThrow<ObjectNotFoundException>();

            //Then: Dürfen die Nutzer nicht gelöscht sein (kein Cascade!)
            UserDao.Get(teamMember1.Id).Should().Be(teamMember1);
            UserDao.Get(teamMember2.Id).Should().Be(teamMember2);
        }

        /// <summary>
        /// Testet das Löschen eines Team das Mitglieder hat und mehreren Boards zugewiesen ist. 
        /// </summary>
        [TestMethod]
        public void TestDeleteTeamWithMembersAndAssignedToBoards() {

            //Given: Mehrere Nutzer
            User user1 = Create.User();
            User user2 = Create.User();

            //Given: Ein Team, dass mehreren Boards zugewiesen ist
            Team team = Create.Team().WithMembers(user1, user2);
            Board board1 = Create.Board().WithTeams(team);
            Board board2 = Create.Board().WithTeams(team);

            //When: Das Team gelöscht wird
            TeamService.Delete(team);

            //Then: Muss das funktionieren und das Team von den Boards gelöscht werden
            TeamDao.FlushAndClear();
            board1.Teams.Should().NotContain(team);
            board2.Teams.Should().NotContain(team);

        }
    }
}