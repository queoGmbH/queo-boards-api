using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Queo.Boards.Core.Domain;
using Queo.Boards.Core.Dtos;
using Queo.Boards.Core.Persistence.Impl;
using Queo.Boards.Core.Tests.Infrastructure;

namespace Queo.Boards.Core.Tests.Persistence {
    /// <summary>
    ///     Testet den <see cref="TeamDao" />.
    /// </summary>
    [TestClass]
    public class TeamDaoTests : PersistenceBaseTest {
        public TeamDao TeamDao { get; set; }

        /// <summary>
        ///     Testet das Speichern und Laden eines Teams
        /// </summary>
        [TestMethod]
        public void TestSaveAndLoadTeam() {
            //Given: Ein Team mit Mitgliedern das gespeichert werden soll
            List<User> teamMembers = new List<User>() {
                Create.User(),
                Create.User(),
                Create.User()
            };

            User creator = Create.User();
            DateTime createdAt = new DateTime(2000, 01, 01, 01, 01, 01, DateTimeKind.Utc);

            string TEAM_NAME = "Team";
            string DESCRIPTION = "Das ist ein ganz tolles Team, welches aber nur den Sinn hat, den Dao zu testen.";
            Team team = new Team(new TeamDto(TEAM_NAME, DESCRIPTION), teamMembers, new EntityCreatedDto(creator, createdAt));

            //When: Das Team gespeichert und wieder geladen wird
            TeamDao.Save(team);
            TeamDao.FlushAndClear();
            Team reloaded = TeamDao.Get(team.Id);

            //Then: Müssen alle Eigenschaften des Teams erhalten bleiben
            reloaded.ShouldBeEquivalentTo(team);
        }
    }
}